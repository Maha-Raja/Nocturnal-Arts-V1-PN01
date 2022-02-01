using Facebook;
using Google.Apis.Auth;
//using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Nat.AuthApp.Models.EFModel;
using Nat.AuthApp.Models.Repositories;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Common.Constants;
using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.Lookup;
using Nat.Core.Notification;
using Nat.Core.Notification.EmailTemplateModels;
using Nat.Core.QueueMessage;
using Nat.Core.ServiceClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using TLX.CloudCore.KendoX.Extensions;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.AuthApp.Services.Services
{
    public class UserService : BaseService<UserModel, NAT_AUS_User>
    {
        private static UserService _service;
        private const int HASH_ITERATIONS = 10000;
        //public static UserService GetInstance(NatLogger logger)
        //{
        //    _service = new UserService();
        //    _service.SetLogger(logger);
        //    return _service;
        //}


        public static UserService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new UserService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private UserService() : base()
        {

        }

        public async Task<UserModel> UpdateReference(UserReferenceModel userReferenceModel)
        {
            using (logger.BeginServiceScope("Update user reference id and reference type"))
            {
                try
                {
                    var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync(userReferenceModel.UserId);
                    user.Reference_Type_LKP = userReferenceModel.ReferenceTypeLkp;
                    user.Reference_ID = userReferenceModel.ReferenceId;
                    uow.RepositoryAsync<NAT_AUS_User>().Update(user);
                    await uow.SaveChangesAsync();
                    return new UserModel().FromDataModel(user);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<TokenModel> GoogleLogin(TokenModel Token)
        {
            using (logger.BeginServiceScope("Google login User"))
            {
                try
                {
                    GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(Token.Token);

                    UserModel userModel = null;
                    string UserName = payload.Email;
                    var googleinfo = JsonConvert.SerializeObject(payload);

                    if (googleinfo != null)
                    {
                        //Check if user with this email already exists
                        var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(UserName);
                        userModel = new UserModel().FromDataModel(user);

                        if (user != null)
                        {
                            //Check if this user already linked google account
                            var external_user_identity = await uow.RepositoryAsync<NAT_AUS_External_Identity>().GetExternalIdentityUserAsync(UserName, "3");
                            if (external_user_identity == null)
                            {
                                List<NAT_AUS_External_Identity> externaluseridentity = new List<NAT_AUS_External_Identity>
                                {
                                     new NAT_AUS_External_Identity
                                     {
                                      Identity_Provider_LKP = "3",
                                      Account_ID = UserName,
                                      Account_Object = googleinfo,
                                      Status_LKP = "1",
                                      Active_Flag = true,
                                      ObjectState = ObjectState.Added
                                     },
                                };
                                user.NAT_AUS_External_Identity = externaluseridentity;
                                uow.Repository<NAT_AUS_User>().Update(user);
                                int updatedRows = uow.SaveChanges();
                                Token.User = userModel;
                                Token.UserFound = false;
                            }
                            else
                            {
                                Token.User = userModel;
                                Token.UserFound = false;
                                // throw new Exception("External Identity Infomation alredy exists");
                            }
                            Token.NewUserCreated = false;

                            var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping);
                            Token.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;

                        }
                        else
                        {
                            //Create new user
                            List<ExternalIdentityModel> external_user_identity = new List<ExternalIdentityModel>
                             {
                                 new ExternalIdentityModel
                                 {
                                     IdentityProviderLKP = "3",
                                     AccountId = UserName,
                                     AccountObject = googleinfo,
                                     StatusLKP = "1",
                                     ActiveFlag = true,
                                     ObjectState = ObjectState.Added
                                 },
                             };

                            userModel = new UserModel();
                            userModel.UserName = UserName;
                            userModel.Email = UserName;
                            userModel.EmailVerified = true;
                            userModel.FirstName = payload.Name;
                            userModel.RoleCode = "CUSTOMER";
                            userModel.UserImageURL = payload.Picture;
                            userModel.ActiveFlag = true;

                            userModel.NatAusExternalIdentity = external_user_identity;
                            var roleID = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(userModel.RoleCode);
                            if (roleID != null)
                            {
                                logger.LogInformation("Role id: " + roleID);
                                userModel.NatAusUserRoleMapping = new List<UserRoleMappingModel>();
                                userModel.NatAusUserRoleMapping.Add(new UserRoleMappingModel()
                                {
                                    RoleId = (long)roleID,
                                    ActiveFlag = true,
                                    ObjectState = ObjectState.Added
                                });
                                Insert(userModel);
                                Token.User = userModel;
                                Token.UserFound = true;
                                Token.NewUserCreated = true;
                            }
                            await uow.SaveChangesAsync();

                            var tempUser = new UserModel().FromDataModel(await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(userModel.UserName));
                            Token.User.NatAusUserRoleMapping = tempUser.NatAusUserRoleMapping;
                            Token.User.NatAusPrivilege = tempUser.NatAusPrivilege;
                            Token.User.UserId = Get().UserId;
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;
                        }

                    }
                    else
                    {
                        throw new Exception("Error in getting user's google details");
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<string> AddNotificationPreferences(IEnumerable<NotificationPreferenceModel> servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    var prefs = new List<NotificationPreferenceModel>();
                    foreach (NotificationPreferenceModel pref in servicemodel)
                    {
                        NotificationPreferenceModel preferencesModel = new NotificationPreferenceModel();
                        preferencesModel.ChannelLKPID = pref.ChannelLKPID;
                        preferencesModel.FrequencyLKPID = pref.FrequencyLKPID;
                        NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetActiveUserByRefID(pref.UserID);
                        if (User != null)
                        {
                            preferencesModel.UserID = User.User_ID;
                        }
                        var prefTypeLookup = await LookupClient.ReadAsync<LookupViewModel>("NOTIFICATION_PREFERENCE");
                        var freqTypeLookup = await LookupClient.ReadAsync<LookupViewModel>("NOTIFICATION_FREQUENCY");
                        preferencesModel.ChannelLKPValue = prefTypeLookup[preferencesModel.ChannelLKPID].VisibleValue;
                        preferencesModel.FrequencyLKPValue = freqTypeLookup[preferencesModel.FrequencyLKPID].VisibleValue;
                        preferencesModel.ActiveFlag = true;
                        preferencesModel.ObjectState = ObjectState.Added;
                        preferencesModel.CreatedBy = "";
                        prefs.Add(preferencesModel);
                    }
                    foreach (NotificationPreferenceModel pref in prefs)
                    {
                        uow.RepositoryAsync<NAT_AUS_Notification_Preference>().Insert(pref.ToDataModel(pref));
                    }
                    await uow.SaveChangesAsync();
                    return "Preferences Saved";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<TokenModel> InstagramLogin(string code)
        {
            using (logger.BeginServiceScope("Instagram login User"))
            {
                try
                {
                    TokenModel Token = new TokenModel();
                    UserModel userModel = null;

                    //getting instagram user info
                    var client = new HttpClient();
                    var values = new Dictionary<string, string>
                    {
                         { "client_id",Environment.GetEnvironmentVariable("instagram_client_id") },
                         { "client_secret",Environment.GetEnvironmentVariable("instagram_client_secret") },
                         { "grant_type", "authorization_code" },
                         { "redirect_uri", Environment.GetEnvironmentVariable("frontend_uri")},
                         { "code", code }
                    };

                    client.Timeout = TimeSpan.FromMinutes(100);
                    var content = new FormUrlEncodedContent(values);
                    var Instagram = await client.PostAsync("https://api.instagram.com/oauth/access_token", content);
                    dynamic instagraminfo = await Instagram.Content.ReadAsStringAsync();
                    var _instagraminfo = JsonConvert.DeserializeObject(instagraminfo);
                    string[] userlist = instagraminfo.Split(',');
                    string UserName = userlist[2].Substring(14, 14);
                    string FullName = userlist[4].Substring(15, 15);
                    /////

                    //for generating auth token 
                    //var secret = Environment.GetEnvironmentVariable("TokenSecret");
                    //SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                    //{ };
                    //JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                    //Token.Token = jwtSecurityTokenHandler.CreateJwtSecurityToken().RawData;
                    ////

                    if (instagraminfo != null)
                    {
                        var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(UserName);
                        userModel = new UserModel().FromDataModel(user);
                        if (user != null)
                        {
                            var external_user_identity = await uow.RepositoryAsync<NAT_AUS_External_Identity>().GetExternalIdentityUserAsync(UserName, "2");
                            if (external_user_identity == null)
                            {
                                List<NAT_AUS_External_Identity> externaluseridentity = new List<NAT_AUS_External_Identity>
                                {
                                     new NAT_AUS_External_Identity
                                     {
                                      Identity_Provider_LKP = "2",
                                      Account_ID = UserName,
                                      Account_Object = instagraminfo,
                                      Status_LKP = "1",
                                      Active_Flag = true,
                                      ObjectState = ObjectState.Added
                                     },
                                };
                                user.NAT_AUS_External_Identity = externaluseridentity;
                                uow.Repository<NAT_AUS_User>().Update(user);
                                int updatedRows = uow.SaveChanges();
                                Token.User = userModel;
                                Token.UserFound = false;
                            }
                            else
                            {
                                Token.User = userModel;
                                Token.UserFound = false;
                                //throw new Exception("External Identity Infomation alredy exists");
                            }
                            Token.NewUserCreated = false;

                            var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping);
                            Token.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;
                        }
                        else
                        {


                            //List<ExternalIdentityModel> external_user_identity = new List<ExternalIdentityModel>();
                            //ExternalIdentityModel external_user = new ExternalIdentityModel();
                            //external_user.IdentityProviderLKP = "2";
                            //external_user.AccountId = UserName;
                            //external_user.AccountObject = instagraminfo;
                            //external_user.StatusLKP = "1";
                            //external_user.ActiveFlag = true;
                            //external_user.ObjectState = ObjectState.Added;
                            //external_user_identity.Add(external_user);

                            List<ExternalIdentityModel> external_user_identity = new List<ExternalIdentityModel>()
                            {
                                new ExternalIdentityModel
                                {
                                    IdentityProviderLKP = "2",
                                    AccountId = UserName,
                                    AccountObject = instagraminfo,
                                    StatusLKP = "1",
                                    ActiveFlag = true,
                                    ObjectState = ObjectState.Added
                                },
                            };


                            userModel = new UserModel();
                            userModel.UserName = UserName;
                            userModel.FirstName = FullName;
                            userModel.RoleCode = "CUSTOMER";
                            userModel.ActiveFlag = true;
                            userModel.NatAusExternalIdentity = external_user_identity;
                            var roleID = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(userModel.RoleCode);
                            if (roleID != null)
                            {
                                logger.LogInformation("Role id: " + roleID);
                                userModel.NatAusUserRoleMapping = new List<UserRoleMappingModel>();
                                userModel.NatAusUserRoleMapping.Add(new UserRoleMappingModel()
                                {
                                    RoleId = (long)roleID,
                                    ActiveFlag = true,
                                    ObjectState = ObjectState.Added
                                });
                                Insert(userModel);
                                Token.User = userModel;
                                Token.UserFound = true;
                                Token.NewUserCreated = true;
                            }
                            await uow.SaveChangesAsync();

                            var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync((new UserModel().ToDataModel(userModel)).NAT_AUS_User_Role_Mapping);
                            Token.User.NatAusPrivilege = userModel.NatAusPrivilege;
                            Token.User.UserId = Get().UserId;
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;
                        }
                    }
                    else
                    {
                        throw new Exception("Error in getting user's instagram details");
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async void FacebookPost(FacebookPostModel facebookpost)
        {
            using (logger.BeginServiceScope("Facebook Post User"))
            {
                try
                {
                    string PageAcessToken = Environment.GetEnvironmentVariable("PageAccessTokenSecret");
                    FacebookClient fb = new FacebookClient();
                    var facebookAuthAppSecret = Environment.GetEnvironmentVariable("facebook_authsecret");
                    var facebookAppSecretProof = GenerateFacebookSecretProof(PageAcessToken, facebookAuthAppSecret);
                    if (facebookpost.ImageUrl != null || facebookpost.ImageUrl != "")
                    {
                        var ObjImage = new
                        {
                            url = facebookpost.ImageUrl,
                            caption = facebookpost.Message,
                            access_token = PageAcessToken
                        };
                        fb.Post("/me/photos?access_token=" + PageAcessToken + "&appsecret_proof=" + facebookAppSecretProof, ObjImage);
                    }
                    else
                    {
                        fb.Post("me/feed?access_token=" + PageAcessToken + "&appsecret_proof=" + facebookAppSecretProof, new { message = facebookpost.Message });
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }

        }

        public async Task<TokenModel> FacebookLogin(TokenModel Token, string facebookauthsecret)
        {
            using (logger.BeginServiceScope("Facebook login User"))
            {
                try
                {
                    UserModel userModel = null;
                    var facebookAccessToken = Token.Token;
                    var fb = new FacebookClient(facebookAccessToken);

                    var facebookAuthAppSecret = facebookauthsecret;
                    var facebookAppSecretProof = GenerateFacebookSecretProof(facebookAccessToken, facebookAuthAppSecret);

                    //getting facebook user info
                    dynamic facebookInfo = fb.Get(string.Format("/me?appsecret_proof={0}&fields=name,email,birthday,gender,picture,cover", facebookAppSecretProof));
                    string UserName = facebookInfo.email;
                    string Picture = facebookInfo.picture.data.url;
                    string _facebookinfo = JsonConvert.SerializeObject(facebookInfo);


                    //for generating auth token 
                    //var secret = Environment.GetEnvironmentVariable("TokenSecret");
                    //SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                    //{ };
                    //JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                    //Token.Token = jwtSecurityTokenHandler.CreateJwtSecurityToken().RawData;

                    if (facebookInfo != null)
                    {
                        var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(UserName);
                        userModel = new UserModel().FromDataModel(user);
                        if (user != null)
                        {
                            var external_user_identity = await uow.RepositoryAsync<NAT_AUS_External_Identity>().GetExternalIdentityUserAsync(UserName, "1");
                            if (external_user_identity == null)
                            {
                                List<NAT_AUS_External_Identity> externaluseridentity = new List<NAT_AUS_External_Identity>
                                {
                                     new NAT_AUS_External_Identity
                                     {
                                      Identity_Provider_LKP = "1",
                                      Account_ID = UserName,
                                      Account_Object = _facebookinfo,
                                      Status_LKP = "1",
                                      Active_Flag = true,
                                      ObjectState = ObjectState.Added
                                     },
                                };
                                user.NAT_AUS_External_Identity = externaluseridentity;
                                uow.Repository<NAT_AUS_User>().Update(user);
                                int updatedRows = uow.SaveChanges();
                                Token.User = userModel;
                                Token.UserFound = false;
                            }
                            else
                            {
                                Token.User = userModel;
                                Token.UserFound = false;
                                //throw new Exception("External Identity Infomation alredy exists"); 
                            }
                            Token.NewUserCreated = false;

                            var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping);
                            Token.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;
                        }
                        else
                        {
                            List<ExternalIdentityModel> external_user_identity = new List<ExternalIdentityModel>
                             {
                                 new ExternalIdentityModel
                                 {
                                     IdentityProviderLKP = "1",
                                     AccountId = UserName,
                                     AccountObject = _facebookinfo,
                                     StatusLKP = "1",
                                     ActiveFlag = true,
                                     ObjectState = ObjectState.Added
                                 },
                             };

                            userModel = new UserModel();
                            userModel.UserName = UserName;
                            userModel.Email = UserName;
                            userModel.EmailVerified = true;
                            userModel.FirstName = facebookInfo.name;
                            userModel.RoleCode = "CUSTOMER";
                            userModel.ActiveFlag = true;
                            userModel.UserImageURL = Picture;
                            userModel.NatAusExternalIdentity = external_user_identity;
                            var roleID = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(userModel.RoleCode);
                            if (roleID != null)
                            {
                                logger.LogInformation("Role id: " + roleID);
                                userModel.NatAusUserRoleMapping = new List<UserRoleMappingModel>();
                                userModel.NatAusUserRoleMapping.Add(new UserRoleMappingModel()
                                {
                                    RoleId = (long)roleID,
                                    ActiveFlag = true,
                                    ObjectState = ObjectState.Added
                                });
                                Insert(userModel);
                                Token.User = userModel;
                                Token.UserFound = true;
                                Token.NewUserCreated = true;
                            }
                            await uow.SaveChangesAsync();

                            var tempUser = new UserModel().FromDataModel(await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(userModel.UserName));
                            Token.User.NatAusUserRoleMapping = tempUser.NatAusUserRoleMapping;
                            Token.User.NatAusPrivilege = tempUser.NatAusPrivilege;
                            Token.User.UserId = Get().UserId;
                            Token.Token = GenerateToken(Token.User, Environment.GetEnvironmentVariable("TokenSecret"));
                            return Token;
                        }
                    }
                    else
                    {
                        throw new Exception("Error in getting user's facebook details");
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }





        /// <summary>
        /// Generate a facebook secret proof (works with facebook APIs v2.4)
        /// </summary>
        /// <param name="facebookAccessToken"></param>
        /// <param name="facebookAuthAppSecret"></param>
        /// <returns></returns>
        public static string GenerateFacebookSecretProof(string facebookAccessToken, string facebookAuthAppSecret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(facebookAuthAppSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(facebookAccessToken);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] hash = hmacsha256.ComputeHash(messageBytes);
            StringBuilder sbHash = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sbHash.Append(hash[i].ToString("x2"));
            }

            return sbHash.ToString();
        }

        public async Task<UserModel> GetUserByIdAsync(int Id)
        {
            try
            {
                UserModel data = null;
                NAT_AUS_User userModel = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync(Id);
                if (userModel != null)
                {
                    data = new UserModel().FromDataModel(userModel);
                    var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(userModel.NAT_AUS_User_Role_Mapping);
                    data.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                    return data;
                }
                throw new Exception("User with such Id not Found!");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<UserModel> GetArtistManagerByLocation(string location)
        {
            try
            {
                UserModel artistman = null;

                long? ManagerRoleId = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(Constants.ARTIST_MANAGER_ROLE_CODE);
                if (ManagerRoleId != null)
                {
                    var AllManagers =  await uow.RepositoryAsync<NAT_AUS_User_Role_Mapping>().GetRoleusersAsync(Convert.ToInt32(ManagerRoleId));
                    if(AllManagers!=null && AllManagers.Count() > 0)
                    {
                        foreach (NAT_AUS_User_Role_Mapping Mapping in AllManagers)
                        {
                            foreach (NAT_AUS_User_Location_Mapping loc in Mapping.NAT_AUS_User.NAT_AUS_User_Location_Mapping)
                            {
                                if (loc.Location_Code == location)
                                {
                                    artistman = new UserModel().FromDataModel(Mapping.NAT_AUS_User);
                                    return artistman;
                                }
                            }
                        }

                        return artistman;

                    }
                    else
                    {
                        throw new Exception("No Managers Found");
                    }

                }
                else
                {
                    throw new Exception("No Role Against ARTIST_MANAGER Role");
                }
                
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<UserModel> GetArtistManagerByArtistIdAsync(int Id)
        {
            try
            {
                UserModel data = null;
                NAT_AUS_User userModel = await uow.RepositoryAsync<NAT_AUS_User>().GetArtistUserByArtistIdAsync(Id);
                if (userModel != null)
                {
                    if(userModel.Reporting_Manager != null && userModel.Reporting_Manager > 0)
                    {
                        NAT_AUS_User ManagerModel = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync(userModel.Reporting_Manager.Value);
                        data = new UserModel().FromDataModel(ManagerModel);
                        return data;
                    }
                    else
                    {
                        UserModel data1 = null;
                        return data1;
                    }

                }
                throw new Exception("No Artist User with such ArtistId");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<NotificationPreferenceModel>> GetNotificationPreferencesByUserIdAsync(int Id)
        {
            try
            {
                IEnumerable<NotificationPreferenceModel> data = null;
                var prefs = new List<NotificationPreferenceModel>();
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetActiveUserByRefID(Id);
                if (User != null)
                {
                    IEnumerable<NAT_AUS_Notification_Preference> userNotificationPrefsModel = await uow.RepositoryAsync<NAT_AUS_Notification_Preference>().GetNotificationPreferencesByUserIdAsync(User.User_ID);
                    if (userNotificationPrefsModel != null)
                    {
                        data = new NotificationPreferenceModel().FromDataModelList(userNotificationPrefsModel);
                        var prefTypeLookup = await LookupClient.ReadAsync<LookupViewModel>("NOTIFICATION_PREFERENCE");

                        var freqTypeLookup = await LookupClient.ReadAsync<LookupViewModel>("NOTIFICATION_FREQUENCY");
                        foreach (NotificationPreferenceModel pref in data)
                        {
                            pref.ChannelLKPValue = prefTypeLookup[pref.ChannelLKPID].VisibleValue;
                            pref.FrequencyLKPValue = freqTypeLookup[pref.FrequencyLKPID].VisibleValue;
                            prefs.Add(pref);
                        }
                        return prefs;
                    }
                }
                throw new Exception("Notification Preferences with such User Id not Found!");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<TokenModel> GetUserToken(string username, string password, string loginType)
        {
            var tokenModel = new TokenModel()
            {
                LoginSuccess = false,
                UserFound = false,
                Reason = ""
            };
            var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(username);

            //check if user is found or not else return error
            if (user == null) { tokenModel.Reason = Constants.LOGIN_ERROR_MESSAGE; return tokenModel; }

            //verify Password
            if (!VerifyPassword(password, user.Password_Hash)) { tokenModel.Reason = Constants.LOGIN_ERROR_MESSAGE; return tokenModel; }

            //check user role with login type
            if (ValidateUserRoleByLoginType(user, loginType) == false) { tokenModel.Reason = Constants.LOGIN_USER_TYPE_ERROR_MESSAGE; return tokenModel; }

            //check if user has logged in via phone number so check if phone number is verified else return error
            if (username == user.Phone_Number && user.Phone_Verified == false) { tokenModel.User = new UserModel().FromDataModel(user); tokenModel.Reason = Constants.LOGIN_UNVERIFIED_PHONE_MESSAGE; return tokenModel; }

            //check if user has logged in via email so check if email is verified else return error
            if (username == user.Email && user.Email_Verified == false) { tokenModel.User = new UserModel().FromDataModel(user); tokenModel.Reason = Constants.LOGIN_UNVERIFIED_EMAIL_MESSAGE; return tokenModel; }

            int? plannerId = null;
            // If the user reference is either vcp or artist then send the planner Id as well
            if (user.Reference_Type_LKP == "artist" && user.Reference_ID.HasValue)
            {
                var referenceId = user.Reference_ID.Value;
                var artistResp = await NatClient.ReadAsync<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + referenceId.ToString());
                if (artistResp.status.IsSuccessStatusCode && artistResp.data != null)
                {
                    plannerId = artistResp.data.PlannerId;
                }
            }


            //check if user has entered correct password else return error
            if (VerifyPassword(password, user.Password_Hash))
            {
                tokenModel.LoginSuccess = true;
                tokenModel.User = new UserModel().FromDataModel(user);
                tokenModel.User.PlannerId = plannerId;
                var userprivilege = (await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping)).GroupBy(x => x.Privilege_ID).Select(y => y.First());
                tokenModel.Token = GenerateToken(tokenModel.User, Environment.GetEnvironmentVariable("TokenSecret"));
                tokenModel.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                tokenModel.RefreshToken = GenerateToken(tokenModel.User.UserName, Environment.GetEnvironmentVariable("RefreshTokenSecret"), DateTime.UtcNow.AddMonths(1));
                return tokenModel;
            }
            else
            {
                tokenModel.Reason = Constants.LOGIN_ERROR_MESSAGE;
                return tokenModel;
            }
        }

        public async Task<TokenModel> RefreshToken(string RefreshToken)
        {
            var tokenModel = new TokenModel()
            {
                LoginSuccess = false,
                UserFound = false
            };
            var username = await ValidateToken<string>(RefreshToken, Environment.GetEnvironmentVariable("RefreshTokenSecret"));
            if (username != null)
            {
                var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(username);
                if (user != null)
                {
                    tokenModel.UserFound = true;
                    tokenModel.LoginSuccess = true;
                    tokenModel.User = new UserModel().FromDataModel(user);
                    //var userprivilege = await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping);
                    //tokenModel.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                    tokenModel.Token = GenerateToken(tokenModel.User, Environment.GetEnvironmentVariable("TokenSecret"));
                    tokenModel.RefreshToken = RefreshToken;
                }
            }
            return tokenModel;
        }

        public static string GenerateToken<T>(T User, string secret)
        {
            return GenerateToken<T>(User, secret, DateTime.UtcNow.AddDays(1));
        }

        public static string GenerateToken<T>(T User, string secret, DateTime expiryDate)
        {
            string _User = JsonConvert.SerializeObject(User);
            byte[] key = Convert.FromBase64String(secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim("User", _User)}),
                Expires = expiryDate,
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token, string secret)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<T> ValidateToken<T>(string token, string secret)
        {
            ClaimsPrincipal principal = GetPrincipal(token, secret);
            if (principal == null)
                return (T)Convert.ChangeType(null, typeof(T));
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return (T)Convert.ChangeType(null, typeof(T));
            }
            Claim usernameClaim = identity.Claims.FirstOrDefault();
            return JsonConvert.DeserializeObject<T>(usernameClaim.Value);
        }

        public async Task<Boolean> ForgotPassword(string Username)
        {
            try
            {
                UserModel Usermodel = null;
                Dictionary<string, string> _ValueObject = new Dictionary<string, string>();

                var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(Username);

                if (user == null) { return false; }

                Usermodel = new UserModel().FromDataModel(user);

                if (Usermodel.Email == Username)
                {
                    string emailToken = "";
                    string dateToken = "";
                    emailToken = GenerateToken(Username, Environment.GetEnvironmentVariable("TokenSecret"));
                    dateToken = GenerateToken((DateTime.Now).AddHours(24), Usermodel.PasswordHash + Environment.GetEnvironmentVariable("TokenSecret"));
                    var verifyUrl = Environment.GetEnvironmentVariable("frontend_uri") + "/home/User/NewPassword?t1=" + emailToken + "&t2=" + dateToken;
                    NotificationQueueMessage.Receiver receiver = new NotificationQueueMessage.Receiver();
                    receiver.ReceiverID = Usermodel.UserName;
                    receiver.ReceiverName = Usermodel.FirstName + " " + Usermodel.LastName;

                    var dynamicTemplateData = new ForgotPasswordTemplate
                    {
                        Name = Usermodel.FirstName + " " + Usermodel.LastName,
                        VerifyLink = verifyUrl

                    };
                    receiver.ValueObject = dynamicTemplateData;
                    NotificationQueueMessage message = new NotificationQueueMessage("AuthService", NotificationType.Email, "ForgetPassword", 0, DateTime.Now, receiver);
                    message.UserId = Usermodel.UserId.ToString();
                    await new Notification().SendEmail(message);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<bool> VerifyTokenVerifyEmail(string tokenLink)
        {
            try
            {
                VerifyEmailModel tokenModel = await ValidateToken<VerifyEmailModel>(tokenLink, Environment.GetEnvironmentVariable("TokenSecret"));
                if (tokenModel.ValidTime > DateTime.Now)
                {
                    NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(tokenModel.UserName);

                    if (User.Email_Verified == false)
                    {
                        User.Email_Verified = true;

                        User.ObjectState = ObjectState.Modified;

                        int updatedRows = await uow.SaveChangesAsync();

                        if (updatedRows != 0)
                        {
                            return true;
                        }
                        else
                        {
                            throw new Exception("Error occur while updating the User.");
                        }
                    }
                    else
                    {
                        throw new Exception("User is already verified.");
                    }
                }
                else
                {
                    throw new Exception("Link is no more Valid.");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<TokenModel> VerifyPhoneNumber(String phoneNumber, String verificationCode, Boolean isLogin)
        {
            try
            {
                //create a verification model with register user phone number
                var verificationModel = new PhoneVerificationModel();
                verificationModel.PhoneNumber = phoneNumber;
                verificationModel.VerificationCode = verificationCode;

                //call verification service to verify code with phone number
                var verificationServiceResponse = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.NotificationService, "VerifyPhoneNumber", requestBody: verificationModel);

                //check verification service is successfull or not else throw exception
                if (verificationServiceResponse.status.IsSuccessStatusCode != true || verificationServiceResponse.data == null || verificationServiceResponse.data != "approved") throw new Exception("Unable to Verify phone number with Code.");

                //fetch user from DB via phone number
                NAT_AUS_User user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(phoneNumber);

                //check  if user phone number is already verified
                if (user.Phone_Verified == true) { throw new Exception("User is already verified."); }

                //update user properties and save changes
                user.Phone_Verified = true;
                user.ObjectState = ObjectState.Modified;
                int updatedRows = await uow.SaveChangesAsync();

                //check if the user is updated successfully else throw exception
                if (updatedRows == 0) { throw new Exception("Error occur while updating the User."); }

                var tokenModel = new TokenModel()
                {
                    LoginSuccess = false,
                    UserFound = false,
                    Reason = ""
                };

                if (isLogin == false) { return tokenModel; }

                tokenModel.LoginSuccess = true;
                tokenModel.User = new UserModel().FromDataModel(user);

                var userprivilege = (await uow.RepositoryAsync<NAT_AUS_Role_Privilege_Mapping>().GetUserPrivilegesAsync(user.NAT_AUS_User_Role_Mapping)).GroupBy(x => x.Privilege_ID).Select(y => y.First());

                tokenModel.Token = GenerateToken(tokenModel.User, Environment.GetEnvironmentVariable("TokenSecret"));
                tokenModel.User.NatAusPrivilege = new PrivilegeModel().FromDataModelList(userprivilege.Select(y => y.NAT_AUS_Privilege)).ToList();
                tokenModel.RefreshToken = GenerateToken(tokenModel.User.UserName, Environment.GetEnvironmentVariable("RefreshTokenSecret"), DateTime.UtcNow.AddMonths(1));
                return tokenModel;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<string> VerifyTokenForgotPassword(string UsernameLink, string Token)
        {
            try
            {

                String[] forgottenPasswordLinkSplit = UsernameLink.Split('/');
                String encryptedUserName = forgottenPasswordLinkSplit[0];
                String encryptedTimeStamp = forgottenPasswordLinkSplit[1];
                string userName = await ValidateToken<string>(encryptedUserName, Environment.GetEnvironmentVariable("TokenSecret"));
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(userName);
                if (User != null)
                {
                    DateTime TimeStamp = await ValidateToken<DateTime>(encryptedTimeStamp, User.Password_Hash + Environment.GetEnvironmentVariable("TokenSecret"));
                    if (TimeStamp > DateTime.Now)
                    {
                        return userName;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<Boolean> ChangeForgottenPassword(ChangePasswordModel ForgottenPasswordModel)
        {
            try
            {
                if (ForgottenPasswordModel.VerificationCode != null && ForgottenPasswordModel.VerificationCode != "")
                {
                    //create a verification model with register user phone number
                    var verificationModel = new PhoneVerificationModel();
                    verificationModel.PhoneNumber = ForgottenPasswordModel.Username;
                    verificationModel.VerificationCode = ForgottenPasswordModel.VerificationCode;

                    //call verification service to verify code with phone number
                    var verificationServiceResponse = await NatClient.ReadAsync<String>(NatClient.Method.POST, NatClient.Service.NotificationService, "VerifyPhoneNumber", requestBody: verificationModel);

                    //check verification service is successfull or not else throw exception
                    if (verificationServiceResponse.status.IsSuccessStatusCode != true || verificationServiceResponse.data == null || verificationServiceResponse.data != "approved") throw new Exception("Unable to Verify phone number with Code.");

                    return await ChangePassword(ForgottenPasswordModel);
                }

                string username = await VerifyTokenForgotPassword(ForgottenPasswordModel.Username, null);
                if (username == null) { return false; }

                ForgottenPasswordModel.Username = username;
                return await ChangePassword(ForgottenPasswordModel);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }

        }

        //public async Task<Boolean> VerifyTokenChangePassword(string Username, string NewPassword, string ConfirmPassword, string Token)
        //{
        //    try
        //    {
        //        if (NewPassword == ConfirmPassword)
        //        {
        //            UserModel Usermodel = null;
        //            var user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(Username);
        //            if (user != null)
        //            {
        //                Usermodel = await ValidateToken<UserModel>(Token, user.Password_Hash);
        //                if (Usermodel != null)
        //                {

        //                    var salt = GetSalt(16);
        //                    logger.LogInformation("Salt Generated: " + salt);
        //                    byte[] hash = Hash(NewPassword, salt);
        //                    byte[] hashBytes = new byte[36];
        //                    Array.Copy(salt, 0, hashBytes, 0, 16);
        //                    Array.Copy(hash, 0, hashBytes, 16, 20);

        //                    user.Password_Hash = Convert.ToBase64String(hashBytes);
        //                    user.ObjectState = ObjectState.Modified;
        //                    int updatedRows = await uow.SaveChangesAsync();
        //                    if (updatedRows == 0)
        //                    {
        //                        return false;
        //                    }
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("New Password and Confirm Password are not same");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ServiceLayerExceptionHandler.Handle(ex, logger);
        //    }
        //}


        public async Task<Boolean> EmailVerification(string UserName, string Email, string FirstName, string LastName)
        {
            string Token = "";
            var tokenModel = new VerifyEmailModel
            {
                UserName = Email,
                ValidTime = DateTime.Now.AddHours(24)
            };

            Token = GenerateToken(tokenModel, Environment.GetEnvironmentVariable("TokenSecret"));
            var verifyUrl = Environment.GetEnvironmentVariable("frontend_uri") + "/home/User/VerifyEmail?t1=" + Token;

            NotificationQueueMessage.Receiver receiver = new NotificationQueueMessage.Receiver();
            receiver.ReceiverID = UserName;
            receiver.ReceiverName = FirstName + " " + LastName;

            var dynamicTemplateData = new VerifyEmailTemplate
            {
                Name = FirstName + " " + LastName,
                VerifyLink = verifyUrl
            };
            receiver.ValueObject = dynamicTemplateData;
            NotificationQueueMessage message = new NotificationQueueMessage("AuthService", NotificationType.Email, "VerifyEmail", 0, DateTime.Now, receiver);
            message.UserId = Email;
            string emailmessage = await new Notification().SendEmail(message);
            return emailmessage != null ? true : false;
        }



        public async Task<string> Register(UserModel userModel)
        {
            bool checkEmailExist = false;
            bool checkPhoneExist = false;

            using (logger.BeginServiceScope("Register User"))
            {

                try
                {
                    if (userModel.ReferenceTypeLKP == "customer")
                    {
                        if (userModel.Email != null && userModel.Email.Length != 0)
                        {
                            checkEmailExist = await this.checkForDuplicateEmail(userModel.Email);
                            if (checkEmailExist == true) { throw new Exception("Email Address Already exist."); }
                        }

                        //if (userModel.PhoneNumber != null && userModel.PhoneNumber.Length != 0)
                        //{
                        //    checkPhoneExist = await this.checkForDuplicatePhoneNumber(userModel.PhoneNumber);
                        //    if (checkPhoneExist == true) { throw new Exception("Phone number Already exist."); }
                        //}


                        userModel.PasswordHash = GeneratePasswordHash(userModel.Password);
                        logger.LogInformation("Password Hashed: " + userModel.PasswordHash);

                        userModel.Verified = false;
                        userModel.PhoneVerified = false;
                        userModel.EmailVerified = false;

                        logger.LogInformation("Fetch role id for: " + userModel.RoleCode);
                        var roleID = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(userModel.RoleCode.Trim());
                        if (roleID != null)
                        {
                            logger.LogInformation("Role id: " + roleID);
                            userModel.NatAusUserRoleMapping = new List<UserRoleMappingModel>();
                            userModel.NatAusUserRoleMapping.Add(new UserRoleMappingModel()
                            {
                                RoleId = (long)roleID,
                                ActiveFlag = true,
                                ObjectState = ObjectState.Added,
                                CreatedBy = ""
                            });
    //                        userModel.CreatedBy = "";
                            Insert(userModel);
                            int updatedRows = await uow.SaveChangesAsync();

                            //check if no rows are updated so return
                            if (updatedRows == 0) { return Convert.ToString(Get().UserId); }

                            //else check if user has registered via email address so send verification email
                            if (userModel.Email != null)
                            {
                                string Token = "";
                                var tokenModel = new VerifyEmailModel
                                {
                                    UserName = userModel.Email,
                                    ValidTime = DateTime.Now.AddHours(24)
                                };

                                Token = GenerateToken(tokenModel, Environment.GetEnvironmentVariable("TokenSecret"));
                                var verifyUrl = Environment.GetEnvironmentVariable("admin_uri") + "/home/User/VerifyEmail?t1=" + Token;

                                NotificationQueueMessage.Receiver receiver = new NotificationQueueMessage.Receiver();
                                receiver.ReceiverID = userModel.UserName;
                                receiver.ReceiverName = userModel.FirstName + " " + userModel.LastName;

                                var dynamicTemplateData = new VerifyEmailTemplate
                                {
                                    Name = userModel.FirstName + " " + userModel.LastName,
                                    VerifyLink = verifyUrl
                                };
                                receiver.ValueObject = dynamicTemplateData;
                                NotificationQueueMessage message = new NotificationQueueMessage("AuthService", NotificationType.Email, "VerifyEmail", 0, DateTime.Now, receiver);
                                message.UserId = userModel.UserName;
                                await new Notification().SendEmail(message);
                            }

                            //else check if user has registered via phone number so send verification code on phone number
                            if (userModel.PhoneNumber != null)
                            {
                                //create a verification model with register user phone number
                                var verificationModel = new PhoneVerificationModel();
                                verificationModel.PhoneNumber = userModel.PhoneNumber;

                                //call customer booked events service to fetch customer events
                                var verificationServiceResponse = await NatClient.ReadAsync<String>(NatClient.Method.POST, NatClient.Service.NotificationService, "SendVerificationCode", requestBody: verificationModel);

                                //check customer booked events service is successfull or not else throw exception      
                                if (verificationServiceResponse.status.IsSuccessStatusCode != true || verificationServiceResponse.data == null || verificationServiceResponse.data != "pending") { logger.LogError(new Exception()); };
                            }

                            return Convert.ToString(Get().UserId);
                        }
                        else
                        {
                            throw new Exception("Role not found");
                        }
                    }
                    else
                    {
                        if (userModel.Email != null && userModel.Email.Length != 0)
                        {
                            checkEmailExist = await this.checkForDuplicateEmail(userModel.Email);
                            if (checkEmailExist == true) { throw new Exception("Email Address Already exist."); }
                        }

                        //if (userModel.PhoneNumber != null && userModel.PhoneNumber.Length != 0)
                        //{
                        //    checkPhoneExist = await this.checkForDuplicatePhoneNumber(userModel.PhoneNumber);
                        //    if (checkPhoneExist == true) { throw new Exception("Phone number Already exist."); }
                        //}
                        if(userModel.NatAusUserLocationMapping != null)
                        {
                            foreach (UserLocationMappingModel loc in userModel.NatAusUserLocationMapping)
                            {
                                loc.ActiveFlag = true;
                                loc.ObjectState = ObjectState.Added;
                                loc.CreatedBy = "";
                            }

                        }


                        userModel.PasswordHash = GeneratePasswordHash(userModel.Password);
                        logger.LogInformation("Password Hashed: " + userModel.PasswordHash);

                        userModel.Verified = true;
                        userModel.PhoneVerified = true;
                        userModel.EmailVerified = true;
                        userModel.ReferenceTypeLKP = userModel.RoleCode.ToLower();
                        logger.LogInformation("Fetch role id for: " + userModel.RoleCode);
                        var roleID = await uow.RepositoryAsync<NAT_AUS_Role>().GetRoleIDByRoleCode(userModel.RoleCode);
                        if (roleID != null)
                        {
                            logger.LogInformation("Role id: " + roleID);
                            userModel.NatAusUserRoleMapping = new List<UserRoleMappingModel>();
                            userModel.NatAusUserRoleMapping.Add(new UserRoleMappingModel()
                            {
                                RoleId = (long)roleID,
                                ActiveFlag = true,
                                ObjectState = ObjectState.Added,
                                CreatedBy = "",
                            });
              //              userModel.CreatedBy = "";
                            Insert(userModel);
                            int updatedRows = await uow.SaveChangesAsync();

                            //check if no rows are updated so return
                            if (updatedRows == 0) { return Convert.ToString(Get().UserId); }

                            //else check if user has registered via email address so send verification email
                            if (userModel.Email != null)
                            {
                                //string Token = "";
                                //var tokenModel = new PasswordRegistrationTemplate
                                //{
                                //    UserName = userModel.Email,
                                //    ValidTime = DateTime.Now.AddHours(24)
                                //};

                                //Token = GenerateToken(tokenModel, Environment.GetEnvironmentVariable("TokenSecret"));
                                //var verifyUrl = Environment.GetEnvironmentVariable("frontend_uri") + "/home/User/VerifyEmail?t1=" + Token;

                                NotificationQueueMessage.Receiver receiver = new NotificationQueueMessage.Receiver();
                                receiver.ReceiverID = userModel.UserName;
                                receiver.ReceiverName = userModel.FirstName + " " + userModel.LastName;

                                var dynamicTemplateData = new PasswordRegistrationTemplate
                                {
                                    Name = userModel.FirstName + " " + userModel.LastName,
                                    Email = userModel.Email,
                                    Password = userModel.Password,
                                    VerifyLink = Environment.GetEnvironmentVariable("admin_portal_uri"),
                                };
                                receiver.ValueObject = dynamicTemplateData;
                                NotificationQueueMessage message = new NotificationQueueMessage("AuthService", NotificationType.Email, "PasswordRegistration", 0, DateTime.Now, receiver);
                                message.UserId = userModel.UserName;
                                await new Notification().SendEmail(message);
                            }

                            ////else check if user has registered via phone number so send verification code on phone number
                            //if (userModel.PhoneNumber != null)
                            //{
                            //    //create a verification model with register user phone number
                            //    var verificationModel = new PhoneVerificationModel();
                            //    verificationModel.PhoneNumber = userModel.PhoneNumber;

                            //    //call customer booked events service to fetch customer events
                            //    var verificationServiceResponse = await NatClient.ReadAsync<String>(NatClient.Method.POST, NatClient.Service.NotificationService, "SendVerificationCode", requestBody: verificationModel);

                            //    //check customer booked events service is successfull or not else throw exception      
                            //    if (verificationServiceResponse.status.IsSuccessStatusCode != true || verificationServiceResponse.data == null || verificationServiceResponse.data != "pending") { logger.LogError(new Exception()); };
                            //}

                            return Convert.ToString(Get().UserId);
                        }
                        else
                        {
                            throw new Exception("Role not found");
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<Boolean> ChangePassword(ChangePasswordModel obj)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(obj.Username);
            if (User != null)
            {
                User.Password_Hash = GeneratePasswordHash(obj.NewPassword);

                User.ObjectState = ObjectState.Modified;

                int updatedRows = await uow.SaveChangesAsync();
                if (updatedRows == 0)
                {
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("User not found");
            }

        }
        public async Task<Boolean> ChangeEmail(ChangeEmailModel obj)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(obj.OldEmail);
            if (User != null)
            {
                User.Email = obj.NewEmail;

                User.ObjectState = ObjectState.Modified;

                int updatedRows = await uow.SaveChangesAsync();
                if (updatedRows == 0)
                {
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("User not found");
            }

        }


        public async Task<Boolean> UpdateUser(UserModel obj)
        {
            try
            {

                if (obj != null)
                {



                    NAT_AUS_User userModel = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsername(obj.UserName);
                    if (obj.PasswordChanged == true)
                    {
                        obj.PasswordHash = GeneratePasswordHash(obj.Password);

                    }
                    else
                    {
                        obj.PasswordHash = userModel.Password_Hash;
                    }

                    if (obj.NatAusUserLocationMapping != null)
                    {
                        foreach (UserLocationMappingModel UserLocationMapping in obj.NatAusUserLocationMapping)
                        {
                            if (UserLocationMapping.UserLocationMappingId > 0)
                                UserLocationMapping.ObjectState = ObjectState.Modified;

                            else if (UserLocationMapping.UserLocationMappingId < 0)
                            {
                                UserLocationMapping.UserLocationMappingId *= -1;
                                UserLocationMapping.ObjectState = ObjectState.Deleted;
                            }
                            else if (UserLocationMapping.UserLocationMappingId == 0)
                            {
                                UserLocationMapping.ActiveFlag = true;
                                UserLocationMapping.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    UserModel user = new UserModel().FromDataModel(userModel);
                    List<UserRoleMappingModel> newrolelist = new List<UserRoleMappingModel>();

                    int i = 0;

                    foreach (RoleModel role in obj.userroles)
                    {
                        UserRoleMappingModel newrole = user.NatAusUserRoleMapping.ElementAt(0);
                        newrole.RoleId = role.RoleId;
                        newrole.NatAusRole = role;
                        newrole.ObjectState = ObjectState.Modified;
                        newrolelist.Add(newrole);
                        i++;
                    }

                    obj.NatAusUserRoleMapping = newrolelist;
                    obj.ObjectState = ObjectState.Modified;
                    obj.LastUpdatedDate = System.DateTime.UtcNow;
                    base.Update(obj);

                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows != 0)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("nothing to update");
                    }
                }
                else
                {
                    throw new Exception("user model null");

                }

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }


        }

        public async Task<Boolean> BulkChangePassword(IEnumerable<ChangePasswordModel> servicemodel)
        {
            foreach (var data in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(data.Username);
                if (User != null)
                {
                    User.Password_Hash = GeneratePasswordHash(data.NewPassword);
                    User.ObjectState = ObjectState.Modified;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }



        public async Task<Boolean> UpdateUserEmailOrPhoneNumber(UpdateUserModel obj)
        {
            NAT_AUS_User user = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(obj.UserName);

            if (obj.Email != null && obj.Email != "" && obj.Email != user.Email)
            {
                //update email
                var checkEmailExist = await this.checkForDuplicateEmail(obj.Email);
                if (checkEmailExist == true) { throw new Exception("Email Address Already exist."); }

                string Token = "";
                var tokenModel = new VerifyEmailModel
                {
                    UserName = obj.Email,
                    ValidTime = DateTime.Now.AddHours(24)
                };

                Token = GenerateToken(tokenModel, Environment.GetEnvironmentVariable("TokenSecret"));
                var verifyUrl = Environment.GetEnvironmentVariable("frontend_uri") + "/home/User/VerifyEmail?t1=" + Token;

                NotificationQueueMessage.Receiver receiver = new NotificationQueueMessage.Receiver();
                receiver.ReceiverID = obj.Email;
                receiver.ReceiverName = user.First_Name + " " + user.Last_Name;

                var dynamicTemplateData = new VerifyEmailTemplate
                {
                    Name = user.First_Name + " " + user.Last_Name,
                    VerifyLink = verifyUrl
                };
                receiver.ValueObject = dynamicTemplateData;
                NotificationQueueMessage message = new NotificationQueueMessage("AuthService", NotificationType.Email, "VerifyEmail", 0, DateTime.Now, receiver);
                message.UserId = user.User_ID.ToString();
                await new Notification().SendEmail(message);
                return true;
            }
            else if (obj.PhoneNumber != null && obj.PhoneNumber != "" && obj.PhoneNumber != user.Phone_Number)
            {
                //update phone
                var checkPhoneExist = await this.checkForDuplicatePhoneNumber(obj.PhoneNumber);
                if (checkPhoneExist == true) { throw new Exception("phoneNumber Already exist."); }


                //create a verification model with user phone number and verification code
                var verificationModel = new PhoneVerificationModel();
                verificationModel.PhoneNumber = obj.PhoneNumber;
                verificationModel.VerificationCode = obj.VerificationCode;

                //call verification service to verify code with phone number
                var verificationServiceResponse = await NatClient.ReadAsync<String>(NatClient.Method.POST, NatClient.Service.NotificationService, "VerifyPhoneNumber", requestBody: verificationModel);

                //check verification service is successfull or not else throw exception
                if (verificationServiceResponse.status.IsSuccessStatusCode != true || verificationServiceResponse.data == null || verificationServiceResponse.data != "approved") throw new Exception("Unable to Verify phone number with Code.");

                user.Phone_Number = obj.PhoneNumber;
                user.Phone_Verified = true;

                user.ObjectState = ObjectState.Modified;

                int updatedRows = await uow.SaveChangesAsync();
                return updatedRows == 0 ? false : true;
            }

            return false;
        }

        public async Task<Boolean> BulkActivateUserByReference(IEnumerable<UpdateUserModel> servicemodel)
        {
            foreach (var data in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByRef(data.ReferenceTypeLKP, data.ReferenceId);
                if (User == null) { return false; }
                User.Active_Flag = true;
                User.ObjectState = ObjectState.Modified;
            }

            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> BulkActivateUser(IEnumerable<UserActivation> servicemodel)
        {
            foreach (var data in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync((long)data.UserId);
                if (User == null) { return false; }
                User.Active_Flag = true;
                User.ObjectState = ObjectState.Modified;
            }

            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> ActivateUserByReference(string ReferenceTypeLKP, long ReferenceId)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetInActiveUserByRef(ReferenceTypeLKP, ReferenceId);
            if (User == null) { return false; }
            User.Active_Flag = true;
            User.ObjectState = ObjectState.Modified;
            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> ActivateUser(int UserId)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync(UserId);
            if (User == null) { return false; }
            User.Active_Flag = true;
            User.ObjectState = ObjectState.Modified;
            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DeactivateUser(int UserId)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync(UserId);
            if (User == null) { return false; }
            User.Active_Flag = false;
            User.ObjectState = ObjectState.Modified;
            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> BulkDeactivateUserByReference(IEnumerable<UpdateUserModel> servicemodel)
        {
            foreach (var data in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByRef(data.ReferenceTypeLKP, data.ReferenceId);
                if (User == null) { return false; }
                User.Active_Flag = false;
                User.ObjectState = ObjectState.Modified;
            }

            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<Boolean> BulkDeactivateUser(IEnumerable<UserActivation> servicemodel)
        {
            foreach (var data in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByIdAsync((long)data.UserId);
                if (User == null) { return false; }
                User.Active_Flag = false;
                User.ObjectState = ObjectState.Modified;
            }

            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DeactivateUserByReference(string ReferenceTypeLKP, long ReferenceId)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetActiveUserByRef(ReferenceTypeLKP, ReferenceId);
            if (User == null) { return false; }
            User.Active_Flag = false;
            User.ObjectState = ObjectState.Modified;
            int updatedRows = await uow.SaveChangesAsync();
            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> UpdateUserByReference(UpdateUserModel obj)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetActiveUserByRef(obj.ReferenceTypeLKP, obj.ReferenceId);

            User.User_Name = obj.UserName;
            User.First_Name = obj.FirstName;
            User.Last_Name = obj.LastName;
            User.User_Image_URL = obj.UserImageURL;
            User.Phone_Number = obj.PhoneNumber;

            User.ObjectState = ObjectState.Modified;

            int updatedRows = await uow.SaveChangesAsync();

            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<Boolean> UpdateNotificationPreferenceByUserIDAsync(IEnumerable<NotificationPreferenceModel> servicemodel)
        {
            //NAT_AUS_Notification_Preference Pref = await uow.RepositoryAsync<NAT_AUS_Notification_Preference>().GetActiveUserPreferencesByUserID(obj.UserID);

            foreach (NotificationPreferenceModel pref in servicemodel)
            {
                NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetActiveUserByRefID(pref.UserID);
                if (User != null)
                {
                    pref.UserID = User.User_ID;
                    if (pref.NotificationPreferenceID > 0)
                    {
                        pref.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<NAT_AUS_Notification_Preference>().Update(pref.ToDataModel(pref));
                    }

                    else if (pref.NotificationPreferenceID < 0)
                    {
                        pref.NotificationPreferenceID *= -1;
                        pref.ObjectState = ObjectState.Deleted;
                        uow.RepositoryAsync<NAT_AUS_Notification_Preference>().Update(pref.ToDataModel(pref));

                    }
                    else if (pref.NotificationPreferenceID == 0)
                    {
                        pref.ActiveFlag = true;
                        pref.ObjectState = ObjectState.Added;
                        uow.RepositoryAsync<NAT_AUS_Notification_Preference>().Insert(pref.ToDataModel(pref));

                    }
                }

            }

            //base.Update(servicemodel);
            int updatedRows = await uow.SaveChangesAsync();

            if (updatedRows == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> checkForDuplicateEmail(string email)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().getDuplicateEmail(email);
            return User != null ? true : false;
        }

        public async Task<Boolean> checkForDuplicatePhoneNumber(string phoneNumber)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().getDuplicatePhoneNumber(phoneNumber);
            return User != null ? true : false;
        }

        public async Task<Boolean> checkForDuplicateEmailOrPhone(string username)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(username);
            return User != null ? true : false;
        }


        public async Task<ChangePasswordResultModel> ChangePasswordbyUser(ChangePasswordModel obj)
        {
            NAT_AUS_User User = await uow.RepositoryAsync<NAT_AUS_User>().GetUserByUsernameAsync(obj.Username);
            ChangePasswordResultModel ob = new ChangePasswordResultModel();
            bool UserExists = VerifyPassword(obj.OldPassword, User.Password_Hash);
            bool SameNewPassword = VerifyPassword(obj.NewPassword, User.Password_Hash);
            if (UserExists == true && SameNewPassword == false)
            {
                User.Password_Hash = GeneratePasswordHash(obj.NewPassword);
                User.ObjectState = ObjectState.Modified;
                int updatedRows = await uow.SaveChangesAsync();
                if (updatedRows == 0)
                {
                    ob.Success = false;
                    ob.Reason = Constants.PASSWORD_CHANGE_ERROR;
                    return ob;
                }
                ob.Success = true;
                return ob;
            }
            else
            {
                if (UserExists != true)
                {
                    ob.Success = false;
                    ob.Reason = Constants.INVALID_OLD_PASSWORD;
                    return ob;
                }
                else
                {
                    ob.Success = false;
                    ob.Reason = Constants.INVALID_NEW_PASSWORD;
                    return ob;
                }
            }

        }

        private static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }

        private static string GeneratePasswordHash(string password)
        {
            var salt = GetSalt(16);
            byte[] hash = Hash(password, salt);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        private static byte[] Hash(string password, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, HASH_ITERATIONS);
            return pbkdf2.GetBytes(20);
        }

        private static bool VerifyPassword(string password, string savedPasswordHash)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, HASH_ITERATIONS);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        private static bool ValidateUserRoleByLoginType(NAT_AUS_User user, string loginType)
        {
            //validate user with respect to login type
            //Where login type is the source of the call
            //if login type is public portal so allow customers to bypass
            //if login type is admin portal so allow admin, artist, vcp to bypass
            //if login type is mobile so allow customers, artist, vcp to bypass

            if ((user.NAT_AUS_User_Role_Mapping.Where(x => x.NAT_AUS_Role.Role_Code == "CUSTOMER" ||
                                                          x.NAT_AUS_Role.Role_Code == "ARTIST" ||
                                                          x.NAT_AUS_Role.Role_Code == "VCP").Count() > 0) &&
                                                          (loginType.ToUpper() == "MOBILE"))
            {
                return true;
            }
            else if ((user.NAT_AUS_User_Role_Mapping.Where(x => x.NAT_AUS_Role.Role_Code == "CUSTOMER").Count() > 0) &&
                                                          (loginType.ToUpper() == "PUBLIC-PORTAL"))
            {
                return true;
            }
            else if ((user.Reference_Type_LKP == "admin" || user.Reference_Type_LKP == "artist" ||
                                                            user.Reference_Type_LKP == "vcp") &&
                                                            (loginType.ToUpper() == "ADMIN-PORTAL"))
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<UserModel>> GetRoleUsers(int roleId)
        {
            using (logger.BeginServiceScope("Get all role users"))
            {
                try
                {
                    var newlist = new List<NAT_AUS_User>();
                    var usersDataModel = await uow.RepositoryAsync<NAT_AUS_User_Role_Mapping>().GetRoleusersAsync(roleId);


                    foreach (NAT_AUS_User_Role_Mapping obj in usersDataModel)
                    {
                        newlist.Add(obj.NAT_AUS_User);
                    }

                    return new UserModel().FromDataModelList(newlist);




                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }
        public async Task<IEnumerable<UserModel>> GetUsersByManageId(int ManagerID)
        {
            using (logger.BeginServiceScope("Get all role users"))
            {
                try
                {
                    var usersDataModel = await uow.RepositoryAsync<NAT_AUS_User>().GetusersbyManagerIdAsync(ManagerID);

                    return new UserModel().FromDataModelList(usersDataModel);




                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method returns all users with the specified reference type 
        /// </summary>
        /// <param name="referenceType">reference type of users</param>
        /// <returns>List of users</returns>
        public async Task<IEnumerable<UserModel>> GetAllUsersByReferenceType(String refType)
        {
            return new UserModel().FromDataModelList(await uow.RepositoryAsync<NAT_AUS_User>().GetAllUsersByReferenceType(refType.ToLower()));
        }

        public async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            return new UserModel().FromDataModelList(await uow.RepositoryAsync<NAT_AUS_User>().GetAllUsers());
        }

        public async Task<DataSourceResult> GetAllUserListAsync(DataSourceRequest request)
        {
            try
            {
                var result = uow.RepositoryAsync<NAT_USERS_VW>().Queryable().ToDataSourceResult<NAT_USERS_VW, UsersVWModel>(request);
                return result;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}
