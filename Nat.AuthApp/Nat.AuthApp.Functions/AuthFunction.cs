using System; 
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
//using Nat.PlannerApp.Functions.ViewModels;
using Nat.AuthApp.Services;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.KendoX.Extension;
using Microsoft.Extensions.Logging;
using TLX.CloudCore.KendoX.UI;
//using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.Exception;
using Nat.Core.Validations;
using Nat.AuthApp.Services.Services;
using Nat.AuthApp.Functions.ViewModels;
using Nat.Common.Constants;
using Newtonsoft.Json;
using Nat.Core.Authentication;
using Nat.AuthApp.Services.ServiceModels;

namespace Nat.AuthApp.Functions
{
    public class LocationFunction
    {

        #region  Methods for User  

        //GET User by ID Method
        [FunctionName("GetUserByIDFunction")]
        public async static Task<HttpResponseMessage> GetUserByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "User/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get User By Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<UserViewModel> respVM = new ResponseViewModel<UserViewModel>();
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = new UserViewModel().FromServiceModel(await service.GetUserByIdAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        //GET User by ID Method
        [FunctionName("GetArtistManagerByArtistIDFunction")]
        public async static Task<HttpResponseMessage> GetArtistManagerByArtistIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "ArtistManagerByArtist/{id:int}")] HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get User By Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<UserViewModel> respVM = new ResponseViewModel<UserViewModel>();
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = new UserViewModel().FromServiceModel(await service.GetArtistManagerByArtistIdAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        //GET Users by ArtistManagerID Method
        [FunctionName("GetUsersByArtistManagerIDFunction")]
        public static async Task<HttpResponseMessage> GetUsersByArtistManagerIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "users/{ManagerID:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int ManagerID)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Privileges");
                    UserService service = UserService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new UserViewModel().FromServiceModelList(await service.GetUsersByManageId(ManagerID));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        //GET Users by ArtistManagerID Method
        [FunctionName("GetMangerbyLocationCode")]
        public static async Task<HttpResponseMessage> GetMangerbyLocationCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetMangerbyLocationCode/{Code}")] HttpRequestMessage req, [Logger] NatLogger logger, string Code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Privileges");
                    UserService service = UserService.GetInstance(logger);

                    ResponseViewModel<UserViewModel> respVM = new ResponseViewModel<UserViewModel>();
                    respVM.data = new UserViewModel().FromServiceModel(await service.GetArtistManagerByLocation(Code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("GetListUserFunction")]
        public static async Task<HttpResponseMessage> GetListUserFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Users/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Users"))
            {
                try
                {
                    logger.LogRequest(req);
                    var User = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    UserService service = UserService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllUserListAsync(User);
                    if(result.Total> 0)
                        result.Data = new UsersVWViewModel().FromServiceModelList((IEnumerable<UsersVWModel>)result.Data);
                    respVM.data = result;
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("GetListRoleFunction")]
        public static async Task<HttpResponseMessage> GetListRoleFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Role/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Users"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Role = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    RoleService service = RoleService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllRoleListAsync(Role);
                    result.Data = new RoleViewModel().FromServiceModelList((IEnumerable<RoleModel>)result.Data);
                    respVM.data = result;
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        #region  Methods for User  

        //GET User by ID Method
        [FunctionName("GetNotificationPreferencesByUserIDFunction")]
        public async static Task<HttpResponseMessage> GetNotificationPreferencesByUserIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "getPref/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get User Preferences By User Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<NotificationPreferenceViewModel>> respVM = new ResponseViewModel<IEnumerable<NotificationPreferenceViewModel> >();
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = new NotificationPreferenceViewModel().FromServiceModelList(await service.GetNotificationPreferencesByUserIdAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("AddNotificationPreferencesFunction")]
        public static async Task<HttpResponseMessage> AddNotificationPreferencesFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "addPref")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Add Notification Preferences"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<NotificationPreferenceViewModel> >();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.AddNotificationPreferences(new NotificationPreferenceViewModel() { }.ToServiceModelList(body.Value));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("UpdateNotificationPreferenceByUserIDFunction")]
        public static async Task<HttpResponseMessage> UpdateNotificationPreferenceByUserIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "updatePref")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Notification Preference By UserID"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Prefs = await req.Content.ReadAsAsync<IEnumerable<NotificationPreferenceViewModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = await service.UpdateNotificationPreferenceByUserIDAsync(new NotificationPreferenceViewModel().ToServiceModelList(Prefs));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("InstagramLogin")]
        public static async Task<HttpResponseMessage> InstagramLogin([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "InstagramLogin")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Instagram Login"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<InstagramViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Code = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = new TokenViewModel().FromServiceModel(await service.InstagramLogin(Code.Token));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("FacebookLogin")]
        public static async Task<HttpResponseMessage> FacebookLogin([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "FacebookLogin")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Facebook Login"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Token = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = new TokenViewModel().FromServiceModel(await service.FacebookLogin(new TokenViewModel() { }.ToServiceModel(Token), Environment.GetEnvironmentVariable("facebook_authsecret")));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("FacebookPost")]
        public static async Task<HttpResponseMessage> FacebookPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "FacebookPost")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Facebook Post"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<FacebookPostViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Message = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        service.FacebookPost(new FacebookPostViewModel() { }.ToServiceModel(Message));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("GoogleLogin")]
        public static async Task<HttpResponseMessage> GoogleLogin([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "GoogleLogin")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Google Login"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Token = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = new TokenViewModel().FromServiceModel(await service.GoogleLogin(new TokenViewModel() { }.ToServiceModel(Token)));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("ValidateUser")]
        public static async Task<HttpResponseMessage> ValidateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "ValidateUser")]HttpRequestMessage req, [Logger] NatLogger logger)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Validate User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Token = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.ValidateToken<UserViewModel>(Token.Token, Environment.GetEnvironmentVariable("TokenSecret"));
                        if (respVM.data != null)
                        {

                            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                            logger.LogResponse(resp);
                            resp.Headers.Add("Access-Control-Allow-Origin", "*");
                            return resp;
                        }
                        else
                        {
                            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                            respVM.data = body.ValidationResults;
                            return resp;
                        }
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("ForgotPassword")]
        public static async Task<HttpResponseMessage> ForgotPassword([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "ForgotPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Forgot Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ForgetPasswordViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var User = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = service.ForgotPassword(User.Username);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("VerifyTokenForgotPassword")]
        public static async Task<HttpResponseMessage> VerifyTokenForgotPassword([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "VerifyTokenForgotPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("VerifyToken for Forgot Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ForgetPasswordViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var UserForgetPassword = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.VerifyTokenForgotPassword(UserForgetPassword.Username, UserForgetPassword.Token);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("VerifyTokenVerifyEmail")]
        public static async Task<HttpResponseMessage> VerifyTokenVerifyEmail([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "VerifyTokenVerifyEmail")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("VerifyToken for Verification of Email"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ForgetPasswordViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var verifyEmail = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.VerifyTokenVerifyEmail(verifyEmail.Username);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// verify phone number of User
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("VerifyPhoneNumber")]
        public static async Task<HttpResponseMessage> VerifyPhoneNumber([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "VerifyPhoneNumber")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Verification of Phone Number"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PhoneVerificationViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = new TokenViewModel().FromServiceModel(await service.VerifyPhoneNumber(body.Value.PhoneNumber,body.Value.VerificationCode,body.Value.IsLogin));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        //[FunctionName("VerifyTokenChangePassword")]
        //public static async Task<HttpResponseMessage> VerifyTokenChangePassword([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "VerifyTokenChangePassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        //{
        //    if (req.Method == HttpMethod.Options)
        //    {
        //        var resp = req.CreateResponse();
        //        resp.Headers.Add("Access-Control-Allow-Origin", "*");
        //        resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
        //        resp.Headers.Add("Access-Control-Allow-Headers", "*");
        //        return resp;
        //    }
        //    using (logger.BeginFunctionScope("Verify Token Change Password"))
        //    {
        //        try
        //        {
        //            logger.LogRequest(req);
        //            var body = await req.GetBodyAsync<ForgetPasswordViewModel>();
        //            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
        //            if (body.IsValid)
        //            {
        //                var UserForgetPassword = body.Value;
        //                UserService service = UserService.GetInstance(logger);
        //                respVM.data = await service.VerifyTokenChangePassword(UserForgetPassword.Username, UserForgetPassword.NewPassword, UserForgetPassword.ConfirmPassword, UserForgetPassword.Token);
        //                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
        //                logger.LogResponse(resp);
        //                resp.Headers.Add("Access-Control-Allow-Origin", "*");
        //                return resp;
        //            }
        //            else
        //            {
        //                respVM.data = body.ValidationResults;
        //                return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
        //            }
        //        }
        //        catch (ServiceLayerException ex)
        //        {
        //            return ServiceLayerExceptionHandler.CreateResponse(ex);
        //        }
        //        catch (Exception ex)
        //        {
        //            return FunctionLayerExceptionHandler.Handle(ex, logger);
        //        }
        //    }
        //}

        /// <summary>
        /// Get Jwt token for a user
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetToken")]
        public static async Task<HttpResponseMessage> GetTokenAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Token")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Token"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UserLoginViewModel>();
                    HttpResponseMessage resp;
                    if (body.IsValid)
                    {
                        var user = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        var tokenModel = await service.GetUserToken(user.Username, user.Password, user.LoginType);                        
                        var tokenVM = new TokenViewModel().FromServiceModel(tokenModel);
                        var respVM = new ResponseViewModel<TokenViewModel>();
                        respVM.data = tokenVM;                                             
                        
                        if (tokenModel.LoginSuccess)
                        {                            
                            respVM.status.message = Constants.LOGIN_SUCCESS_MESSAGE;
                            resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        }
                        else
                        {
                            respVM.status.message = tokenModel.Reason;
                            resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);                            
                        }                        
                    }
                    else
                    {
                        var respVM = new ResponseViewModel<object>();                        
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);                        
                    }
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// Get Jwt token against a refresh token
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetRefreshToken")]
        public static async Task<HttpResponseMessage> GetRefreshTokenAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "RefreshToken")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Access Token from Refresh Token"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    if (body.IsValid)
                    {
                        var refreshTokenVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        var tokenModel = await service.RefreshToken(refreshTokenVM.RefreshToken);
                        if (tokenModel.LoginSuccess)
                        {
                            ResponseViewModel<TokenViewModel> respVM = new ResponseViewModel<TokenViewModel>();
                            var tokenVM = new TokenViewModel().FromServiceModel(tokenModel);
                            respVM.data = tokenVM;
                            respVM.status.message = Constants.LOGIN_SUCCESS_MESSAGE;
                            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                            logger.LogResponse(resp);
                            resp.Headers.Add("Access-Control-Allow-Origin", "*");
                            return resp;
                        }
                        else
                        {
                            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                            respVM.status.message = Constants.LOGIN_ERROR_MESSAGE;
                            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                            logger.LogResponse(resp);
                            resp.Headers.Add("Access-Control-Allow-Origin", "*");
                            return resp;
                        }
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("RegisterUser")]
        public static async Task<HttpResponseMessage> RegisterUserAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "User/Register")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Register New User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UserViewModel>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        try
                        {
                            Auth.UserModel UserModel = new Auth.UserModel();
                            UserModel = await Auth.Validate(req);
                            userVM.CreatedBy = UserModel.Email;
                            userVM.LastUpdatedBy = UserModel.Email;
                            respVM.data = await service.Register(new UserViewModel().ToServiceModel(userVM));
                        }
                        catch
                        {
                            userVM.CreatedBy = userVM.Email;
                            userVM.LastUpdatedBy = userVM.Email;
                            respVM.data = await service.Register(new UserViewModel().ToServiceModel(userVM));
                        }
                            
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// This function will update user ReferenceID and ReferenceTypeLKP
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("UpdateUserReference")]
        public static async Task<HttpResponseMessage> UpdateUserReference([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "User/Reference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update User Reference"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UserReferenceViewModel>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Update User Reference Type and Reference ID");
                        UserService service = UserService.GetInstance(logger);
                        ResponseViewModel<UserViewModel> respVM = new ResponseViewModel<UserViewModel>();
                        respVM.data = new UserViewModel().FromServiceModel(await service.UpdateReference(new UserReferenceViewModel().ToServiceModel(userVM)));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Update User By Reference
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("ActivateUserByReference")]
        public static async Task<HttpResponseMessage> ActivateUserByReference([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/ActivateUserByReference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UpdateUserViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.ActivateUserByReference(userVM.ReferenceTypeLKP, userVM.ReferenceId);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        [FunctionName("DeActivateUserByReference")]
        public static async Task<HttpResponseMessage> DeActivateUserByReference([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/DeActivateUserByReference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UpdateUserViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.DeactivateUserByReference(userVM.ReferenceTypeLKP, userVM.ReferenceId);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Update User By Reference
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("ActivateUser")]
        public static async Task<HttpResponseMessage> ActivateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/ActivateUser/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate User"))
            {
                try
                {
                    logger.LogRequest(req);                    

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();                      
                    logger.LogInformation("Call Service Function to Get Token");
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = await service.ActivateUser(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;

                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        [FunctionName("DeActivateUser")]
        public static async Task<HttpResponseMessage> DeActivateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/DeActivateUser/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate User"))
            {
                try
                {
                    logger.LogRequest(req);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    logger.LogInformation("Call Service Function to Get Token");
                    UserService service = UserService.GetInstance(logger);
                    respVM.data = await service.DeactivateUser(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("BulkActivateUser")]
        public static async Task<HttpResponseMessage> BulkActivateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/BulkActivateUser")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("DeActivate User In bulk"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<UserActivationViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.BulkActivateUser(new UserActivationViewModel() { }.ToServiceModelList(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        [FunctionName("BulkDeactivateUser")]
        public static async Task<HttpResponseMessage> BulkDeActivateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/BulkDeactivateUser")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Deactivate User In bulk"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<UserActivationViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.BulkDeactivateUser(new UserActivationViewModel() { }.ToServiceModelList(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("BulkActivateUserByReference")]
        public static async Task<HttpResponseMessage> BulkActivateUserByReference([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/BulkActivateUserByReference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("DeActivate User In bulk"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<UpdateUserViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.BulkActivateUserByReference(new UpdateUserViewModel() { }.ToServiceModelList(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        [FunctionName("BulkDeActivateUserByReference")]
        public static async Task<HttpResponseMessage> BulkDeActivateUserByReference([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/BulkDeActivateUserByReference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("DeActivate User In bulk"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<UpdateUserViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.BulkDeactivateUserByReference(new UpdateUserViewModel() { }.ToServiceModelList(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        // UPDATE Artist Information Method
        [FunctionName("UpdateUserFunction")]
        public static async Task<HttpResponseMessage> UpdateUserFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/update")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update user"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UserViewModel>();
                    var user = body.Value;
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    UserService service = UserService.GetInstance(logger);

                    try
                    {
                        Auth.UserModel UserModel = new Auth.UserModel();
                        UserModel = await Auth.Validate(req);
                        user.LastUpdatedBy = UserModel.UserName;
                    }
                    catch
                    {
                        //user.LastUpdatedBy = user.Email;
                    }
                    respVM.data = await service.UpdateUser(new UserViewModel().ToServiceModel(user));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Update User By Reference
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("UpdateUserByReference")]
        public static async Task<HttpResponseMessage> UpdateUserByReference([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/UpdateUserByReference")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UpdateUserViewModel>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.UpdateUserByReference(new UpdateUserViewModel().ToServiceModel(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Update User Phone Number or Email
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("UpdateUserEmailOrPhoneNumber")]
        public static async Task<HttpResponseMessage> UpdateUserEmailOrPhoneNumber([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/UpdateUserEmailOrPhoneNumber")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<UpdateUserViewModel>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.UpdateUserEmailOrPhoneNumber(new UpdateUserViewModel().ToServiceModel(userVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// Change User Password
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("AdminChangeUserPassword")]
        public static async Task<HttpResponseMessage> AdminChangeUserPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/AdminChangeUserPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Change User Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ChangePasswordViewModel>();
                    if (body.IsValid)
                    {
                        var cpVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.ChangePassword(new ChangePasswordViewModel().ToServiceModel(cpVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        //admin change user email

        [FunctionName("AdminChangeUserEmailAsync")]
        public static async Task<HttpResponseMessage> AdminChangeUserEmailAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/AdminChangeUserEmail")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Change User Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ChangeEmailViewModel>();
                    if (body.IsValid)
                    {
                        var cpVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.ChangeEmail(new ChangeEmailViewModel().ToServiceModel(cpVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Change User Password
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("AdminBulkChangeUserPassword")]
        public static async Task<HttpResponseMessage> AdminBulkChangeUserPassword([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/AdminBulkChangeUserPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Change User Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<ChangePasswordViewModel>>();
                    if (body.IsValid)
                    {
                        var cpVM = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        UserService service = UserService.GetInstance(logger);
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.BulkChangePassword(new ChangePasswordViewModel() { }.ToServiceModelList(body.Value));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        /// <summary>
        /// Change User Password
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("ChangeUserPassword")]
        public static async Task<HttpResponseMessage> ChangeUserPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/ChangeUserPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Change User Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ChangePasswordViewModel>();
                    if (body.IsValid)
                    {
                        var cpVM = body.Value;
                        logger.LogInformation("Call Service Function to change password");
                        UserService service = UserService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        
                        ChangePasswordResultModel blob = await service.ChangePasswordbyUser(new ChangePasswordViewModel().ToServiceModel(cpVM));
                        respVM.data = blob.Success;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);



                        if (blob.Success == false)
                        {
                            respVM.status.message = blob.Reason;
                            resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        else
                        {
                            resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        }
                       
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("CheckForDuplicateEmail")]
        public static async Task<HttpResponseMessage> CheckForDuplicateEmail([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "User/alreadyRegistered/{email}")]HttpRequestMessage req, [Logger] NatLogger logger, string email)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check for duplicate email"))
            {
                try
                {
                        logger.LogRequest(req);
                        logger.LogInformation("Call Service Function to check for duplicate email");
                        UserService service = UserService.GetInstance(logger);
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.checkForDuplicateEmail(email);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("EmailVerfication")]
        public static async Task<HttpResponseMessage> EmailVerfication([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "EmailVerfication")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Register New User"))
            {

                try
                {

                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<emailVerificationViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var userVM = body.Value;
                        UserService service = UserService.GetInstance(logger);
                        respVM.data = await service.EmailVerification(userVM.Email, userVM.Email, userVM.FirstName, userVM.LastName);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("CheckForDuplicateEmailOrPhone")]
        public static async Task<HttpResponseMessage> CheckForDuplicateEmailOrPhone([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "User/alreadyExist/{username}")]HttpRequestMessage req, [Logger] NatLogger logger, string username)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check for duplicate email or phone"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to check for duplicate email");
                    UserService service = UserService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = await service.checkForDuplicateEmailOrPhone(username);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// Register new user when queue message recieved
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("RegisterUserFromQueue")]
        public static async Task RegisterUserFromQueueAsync([QueueTrigger("ausresgisteruser", Connection = "AzureWebJobsStorage")] string myQueueItem, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Register New User"))
            {
                try
                {
                    var userVM = JsonConvert.DeserializeObject<UserViewModel>(myQueueItem);
                    logger.LogInformation("Call Service Function to Register User");
                    UserService service = UserService.GetInstance(logger);
                    userVM.CreatedBy = userVM.Email;
                    userVM.LastUpdatedBy = userVM.Email;

                    await service.Register(new UserViewModel().ToServiceModel(userVM));
                }
                catch (Exception ex)
                {
                    FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }
        [FunctionName("ChangeUserForgottenPassword")]
        public static async Task<HttpResponseMessage> ChangeForgottenPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "User/ChangeUserForgottenPassword")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Change User Forgotten Password"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<ChangePasswordViewModel>();
                    if (body.IsValid)
                    {
                        var cpVM = body.Value;
                        logger.LogInformation("Call Service Function to change password");
                        UserService service = UserService.GetInstance(logger);
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.ChangeForgottenPassword(new ChangePasswordViewModel().ToServiceModel(cpVM));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }
        #endregion

        #region Methods for Role





        //GET User by ID Method
        [FunctionName("GetRoleByIDFunction")]
        public async static Task<HttpResponseMessage> GetRoleByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Role/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get User By Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<RoleViewModel> respVM = new ResponseViewModel<RoleViewModel>();
                    RoleService service = RoleService.GetInstance(logger);
                    respVM.data = new RoleViewModel().FromServiceModel(await service.GetRoleByIdAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("CreateRole")]
        public static async Task<HttpResponseMessage> CreateRole([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Role/Add")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("add artist rating"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<RoleViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var role = body.Value;

                        RoleService service = RoleService.GetInstance(logger);
                        var appservice = new RoleViewModel() { }.ToServiceModel(role);
                        respVM.data = await service.CreateRoleAsync(appservice);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("UpdateRole")]
        public static async Task<HttpResponseMessage> UpdateRole([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Role/update")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update User"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<RoleViewModel>();
                    if (body.IsValid)
                    {
                        var role = body.Value;
                        logger.LogInformation("Call Service Function to Get Token");
                        RoleService service = RoleService.GetInstance(logger);

                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = await service.UpdateRole(new RoleViewModel().ToServiceModel(role));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetAllActiveRoles")]
        public static async Task<HttpResponseMessage> GetAllActiveRoles([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Roles")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                 //   await Auth.Validate(req);
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get roles");
                    RoleService service = RoleService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new RoleViewModel().FromServiceModelList(await service.GetAllActiveRoles());
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("GetAllPrivilegesofRole")]
        public static async Task<HttpResponseMessage> GetAllPrivilegesofRole([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Roles/{roleId}/privileges")]HttpRequestMessage req, [Logger] NatLogger logger, string roleId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Privileges");
                    RoleService service = RoleService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new RolePrivilegeMappingViewModel().FromServiceModelList(await service.GetRolePrivileges(Convert.ToInt32(roleId)));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("GetAllUsersofRole")]
        public static async Task<HttpResponseMessage> GetAllUsersofRole([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Roles/{userId}/users")]HttpRequestMessage req, [Logger] NatLogger logger, string userId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Privileges");
                    UserService service = UserService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new UserViewModel().FromServiceModelList(await service.GetRoleUsers(Convert.ToInt32(userId)));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        [FunctionName("GetAllPrivileges")]
        public static async Task<HttpResponseMessage> GetAllPrivileges([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Roles/privileges")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Roles"))
            {
                try
                {
                    logger.LogRequest(req);
                    RoleService service = RoleService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new PrivilegeViewModel().FromServiceModelList(await service.GetAllPrivileges());
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method returns all users with the specified reference type 
        /// </summary>
        /// <param name="referenceType">reference type of users</param>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns>List of users</returns>
        [FunctionName("GetAllUsersByReferenceType")]
        public static async Task<HttpResponseMessage> GetAllUsersByReferenceType([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetAllUsersByReferenceType/{referenceType}")]HttpRequestMessage req, [Logger] NatLogger logger, string referenceType)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Users By Refernce Types"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Users By Reference Types");
                    UserService service = UserService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new UserViewModel().FromServiceModelList(await service.GetAllUsersByReferenceType(referenceType));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        [FunctionName("GetAllUsers")]
        public static async Task<HttpResponseMessage> GetAllUsers([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetAllUsers")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Users By Refernce Types"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to Get Users By Reference Types");
                    UserService service = UserService.GetInstance(logger);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = new UserViewModel().FromServiceModelList(await service.GetAllUsers());
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
                catch (Exception ex)
                {
                    return FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        #endregion
    }
}
#endregion