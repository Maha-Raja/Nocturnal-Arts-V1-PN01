using Microsoft.WindowsAzure.Storage.Table;
using Nat.ArtistApp.Models.EFModel;
using Nat.ArtistApp.Models.Repositories;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.ArtistApp.Services.ServiceModels.ArtistRequest;
using Nat.ArtistApp.Services.ViewModels;
using Nat.Common.Constants;
using Nat.Core.Caching;
using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.Lookup;
using Nat.Core.ServiceClient;
using Nat.Core.Storage;
using Nat.Core.MarketTimeZone.Model;
using Nat.Core.MarketTimeZone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using TLX.CloudCore.KendoX.Extensions;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using System.Net.Http;
using CommonMethods;
using Nat.CustomerApp.Services.ServiceModels;
using Microsoft.JScript;
using Convert = System.Convert;
using Nat.Core.Authentication;
using System.Globalization;
using Nat.ArtistApp.Services.ServiceModels.ViewModels;

namespace Nat.ArtistApp.Services
{
    public class ArtistService : BaseService<ArtistModel, NAT_AS_Artist>
    {
        private static ArtistService _service;
        public static ArtistService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new ArtistService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private ArtistService() : base()
        {

        }

        public async Task<DataSourceResult> GetAllArtistViewListAsync(DataSourceRequest request, Auth.UserModel UserModel)
        {
            try
            {
                DataSourceResult artistlist;
                var rolechecker = 0;
                foreach (Auth.RoleModel Roles in UserModel.Roles)
                {
                    if (Roles.RoleName == "Artist Manager")
                    {
                        rolechecker = 1;
                    }
                    if (Roles.RoleName == "Artist")
                    {
                        rolechecker = 2;
                    }
                }

                if (rolechecker == 1)
                {
                    var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + UserModel.UserId);
                    var ArtistList = userlist.data.Select(x => x.ReferenceId);

                    if (ArtistList != null)
                    {
                        artistlist = uow.RepositoryAsync<NAT_Artist_VW>().Queryable().Where(x => ArtistList.Contains(x.Artist_ID)).ToDataSourceResult<NAT_Artist_VW, ArtistVWModel>(request);
                    }
                    else
                    {
                        artistlist = new DataSourceResult();                    }
                }
                else if (rolechecker == 2)
                {
                    if (UserModel.ReferenceId != null)
                    {
                        artistlist = uow.RepositoryAsync<NAT_Artist_VW>().Queryable().Where(x => x.Artist_ID == UserModel.ReferenceId).ToDataSourceResult<NAT_Artist_VW, ArtistVWModel>(request);
                    }
                    else
                    {
                        artistlist = new DataSourceResult();
                    }
                }
                else
                {
                    artistlist = uow.RepositoryAsync<NAT_Artist_VW>().Queryable().ToDataSourceResult<NAT_Artist_VW, ArtistVWModel>(request);

                }

                return artistlist;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        public async Task<Object> GetArtistLov(Auth.UserModel userModel)
        {
            using (logger.BeginServiceScope("Get Artist Lov"))
            {
                try
                {
                    var rolechecker = 0;
                    if(userModel!=null)
                    foreach (Auth.RoleModel Roles in userModel.Roles)
                    {
                        if (Roles.RoleCode == "ARTIST_MANAGER")
                        {
                            rolechecker = 1;
                        }
                        else if (Roles.RoleCode == "ARTIST")
                        {
                            rolechecker = 2;
                        }
                        else if (Roles.RoleCode == "VCP")
                        {
                            rolechecker = 3;
                        }
                    }
                    Object artistLov = null;
                    if (rolechecker == 1)
                    {
                        
                            var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + userModel.UserId);
                            var artistIdList = userlist.data.Select(x => x.ReferenceId).ToList();
                            
                            if (artistIdList != null)
                            {
                                artistLov = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistLovForArtistManager(artistIdList);
                            }
                            
                        
                        return artistLov;
                    }
                    else if (rolechecker == 2)
                    {
                        
                            List<long> artistIdList = null;
                            artistLov = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistLovForArtist((long)userModel.ReferenceId);
                            
                        
                        return artistLov;
                    }
                    else
                    {
                        async Task<Object> GetArtistLovFromDB()
                        {
                            logger.LogInformation("Fetch id and name of artist");
                            artistLov = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistLov();
                            return artistLov;
                        }
                        return await Caching.GetObjectFromCacheAsync<Object>("artist/lov", 5, GetArtistLovFromDB);
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <returns>Collection of artist service model<returns>
        public async Task<IEnumerable<ArtistModel>> GetAllArtistAsync()
        {
            using (logger.BeginServiceScope("Get All Artist"))
            {
                try
                {
                    IEnumerable<ArtistModel> data = null;
                    logger.LogInformation("Fetch all artist from repo");
                    IEnumerable<NAT_AS_Artist> artistModel = await uow.RepositoryAsync<NAT_AS_Artist>().GetAllArtistAsync();
                    if (artistModel != null)
                    {
                        data = new ArtistModel().FromDataModelList(artistModel);
                        return data;
                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        public async Task<IEnumerable<ArtistDisbursementVWModel>> GetArtistDisbursementById(int id)
        {
            using (logger.BeginServiceScope("Get Artist Disbursement By ID from View"))
            {
                try
                {
                    IEnumerable<ArtistDisbursementVWModel> data = null;
                    IEnumerable<NAT_ARTIST_DISBURSEMENT_VW> disbursementList = await uow.RepositoryAsync<NAT_ARTIST_DISBURSEMENT_VW>().Queryable().Where(x => x.Artist_ID == id).Distinct().ToListAsync();
                    if(disbursementList != null)
                    {
                        data = new ArtistDisbursementVWModel().FromDataModelList(disbursementList);
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch(Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        //CheckVenueandArtistPreferences
        public async Task<List<object>> CheckVenueandArtistPreferences(IEnumerable<ArtistVenuePreferenceModel> searchmodel)
        {
            List<int> artistids = new List<int>();
            int venueid = 0;
            List<object> obj = new List<object>();
            foreach (ArtistVenuePreferenceModel pref in searchmodel)
            {
                artistids.Add(pref.ArtistId);
                venueid = pref.VenueId;

            }
            if (venueid > 0)
            {

                var VenueData = await NatClient.ReadAsync<VenueViewModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + venueid);
                if (VenueData.status.IsSuccessStatusCode && VenueData.data != null)
                {
                    List<NAT_AS_Artist> artists = uow.RepositoryAsync<NAT_AS_Artist>().Queryable().Include(x => x.NAT_AS_Artist_Venue_Preference)
                        .Where(x => artistids.Contains(x.Artist_ID)).ToList();

                    foreach (NAT_AS_Artist v in artists)
                    {
                        var temp = new
                        {
                            ArtistId = 0,
                            VenueId = 0,
                            VenuePreference = false,
                            ArtistPreference = false

                        };
                        foreach (NAT_AS_Artist_Venue_Preference pr in v.NAT_AS_Artist_Venue_Preference)
                        {
                            if (pr.Venue_ID == venueid)
                            {
                                temp = new
                                {
                                    ArtistId = v.Artist_ID,
                                    VenueId = venueid,
                                    VenuePreference = VenueData.data.NatVsVenueArtistPreference !=null? VenueData.data.NatVsVenueArtistPreference.Any(x => x.ArtistId == v.Artist_ID): false,
                                    ArtistPreference = true

                                };
                            }
                        }
                        if (temp.ArtistId > 0)
                        {

                            var temp1 = new
                            {
                                ArtistId = temp.ArtistId,
                                VenueId = temp.VenueId,
                                VenuePreference = VenueData.data.NatVsVenueArtistPreference != null ? VenueData.data.NatVsVenueArtistPreference.Any(x => x.ArtistId == v.Artist_ID) : false,
                                ArtistPreference = temp.ArtistPreference
                            };

                            obj.Add(temp1);
                        }
                        else
                        {
                            if (VenueData.data.NatVsVenueArtistPreference != null && VenueData.data.NatVsVenueArtistPreference.Any(x => x.ArtistId == v.Artist_ID))
                            {
                                var temp1 = new
                                {
                                    ArtistId = v.Artist_ID,
                                    VenueId = venueid,
                                    VenuePreference = VenueData.data.NatVsVenueArtistPreference != null ? VenueData.data.NatVsVenueArtistPreference.Any(x => x.ArtistId == v.Artist_ID) : false,
                                    ArtistPreference = false
                                };

                                obj.Add(temp1);
                            }
                        }
                    }



                    return obj;
                }
                else
                {
                    throw new Exception("Artist Service failed");
                }
            }
            else
            {
                throw new Exception("Artist Id cannot be null");
            }
        }

        public async Task<Boolean> checkArtistForPlannerId(int id)
        {
            using (logger.BeginServiceScope("check Artist for planner id"))
            {
                try
                {
                    NAT_AS_Artist artistModel = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByPlannerIdAsync(id);
                    if (artistModel != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<DataSourceResult> GetAllArtistListAsync(DataSourceRequest request, Auth.UserModel UserModel)
        {
            try
            {
                IEnumerable<NAT_AS_Artist> artistlistEF;

                //var rolechecker = 0;
                //foreach (Auth.RoleModel Roles in UserModel.Roles)
                //{
                //    if (Roles.RoleName == "Artist Manager")
                //    {
                //        rolechecker = 1;
                //    }
                //}


                var rolechecker = 0;
                foreach (Auth.RoleModel Roles in UserModel.Roles)
                {
                    if (Roles.RoleName == "Artist Manager")
                    {
                        rolechecker = 1;
                    }
                    if (Roles.RoleName == "Artist")
                    {
                        rolechecker = 2;
                    }
                }
                if (rolechecker == 1)
                {
                    var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + UserModel.UserId);
                    var ArtistList = userlist.data.Select(x => x.ReferenceId);

                    if (ArtistList != null)
                    {
                        artistlistEF = await uow.RepositoryAsync<NAT_AS_Artist>().Queryable().Where(x => ArtistList.Contains(x.Artist_ID))
                            .Include(x => x.NAT_AS_Artist_Rating)
                            .Include(x => x.NAT_AS_Artist_Skill)
                            .Include(x => x.NAT_AS_Artist_Location_Mapping)
                            .ToListAsync();
                    }
                    else
                    {
                        artistlistEF = new List<NAT_AS_Artist>();

                    }
                }
                else if (rolechecker == 2)
                {
                    if (UserModel.ReferenceId != null)
                    {
                        artistlistEF = await uow.RepositoryAsync<NAT_AS_Artist>().Queryable().Where(x => x.Artist_ID == UserModel.ReferenceId)
                            .Include(x => x.NAT_AS_Artist_Rating)
                            .Include(x => x.NAT_AS_Artist_Skill)
                            .Include(x => x.NAT_AS_Artist_Location_Mapping)
                            .ToListAsync();
                    }
                    else
                    {
                        artistlistEF = new List<NAT_AS_Artist>();
                    }
                }
                else
                {
                    artistlistEF = await uow.RepositoryAsync<NAT_AS_Artist>().Queryable()
                    .Include(x => x.NAT_AS_Artist_Rating)
                    .Include(x => x.NAT_AS_Artist_Skill)
                    .Include(x => x.NAT_AS_Artist_Location_Mapping)
                    .ToListAsync();


                }



                var artistlistModel = new ArtistModel().FromDataModelList(artistlistEF);
                foreach(ArtistModel artist in artistlistModel)
                {
                    MarketTimeZoneModel marketTimeZone = await MarketTimeZoneClient.GetMarketTimeAsync(artist.LastUpdatedDate, artist.LocationCode);
                    artist.LastUpdatedDate = marketTimeZone.MarketTime;
                    artist.ArtistTimezone = marketTimeZone.TimeZone;
                }
                var result = artistlistModel.ToDataSourceResult(request);
                //.ToDataSourceResult<NAT_AS_Artist, ArtistModel>(request);


                var artisteventModel = await uow.RepositoryAsync<NAT_AS_Artist_Event>().GetAllEventCalculatedDetails();
                var artistevent = JsonConvert.SerializeObject(artisteventModel);
                IEnumerable<ArtistEventViewModel> ArtistEventData = JsonConvert.DeserializeObject<IEnumerable<ArtistEventViewModel>>(artistevent);

                if (ArtistEventData != null)
                {
                    Dictionary<int, ArtistEventViewModel> artisteventDictionary = ArtistEventData.ToDictionary(item => item.ArtistId, item => item);

                    var data = ((IEnumerable<ArtistModel>)result.Data);

                    data = data.Select((x) =>
                    {

                        x.Eventsheld = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].Eventsheld : 0;
                        x.LastEventDate = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].LastEventDate : null;
                        x.UpcommingEvents = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].UpcommingEvents : 0;

                        return x;
                    })
                        .ToList();
                    result.Data = data;
                }

                var locationList = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
                if (locationList.status.IsSuccessStatusCode)
                {
                    var data2 = ((IEnumerable<ArtistModel>)result.Data);
                    Dictionary<string, LocationViewModel> locationDictionary = locationList.data.ToDictionary(item => item.LocationShortCode, item => item);

                    foreach (var artist in data2)
                    {
                        foreach (var location in artist.NatAsArtistLocationMapping)
                        {
                            if (locationDictionary.ContainsKey(location.LocationCode) && artist.LocationCode != null)
                            {
                                artist.LocationName = locationDictionary[location.LocationCode].LocationName;
                                location.LocationName = locationDictionary[location.LocationCode].LocationName;
                            }
                        }
                    }
                    result.Data = data2;
                }
                var skillLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("ARTIST_SKILL");
                List<ArtistModel> data3 = ((IEnumerable<ArtistModel>)result.Data).ToList();
                data3 = data3.Select((x) =>
                {
                    x.NatAsArtistSkill = x.NatAsArtistSkill.Select((y) =>
                    {
                        y.SkillLKPValue = skillLookup[y.SkillLKPId.ToString()].VisibleValue;
                        return y;
                    }).ToList();

                    return x;
                }).ToList();

                result.Data = data3;

                return result;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns artist with a given Id
        /// </summary>
        /// <param name="Id">Id of artist</param>
        /// <returns>Artist service model</returns>
        public async Task<ArtistModel> GetByIdArtistAsync(int Id, long? customerId)
        {
            try
            {
                int CusId;
                ArtistModel data = null;
                NAT_AS_Artist artistModel = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Id);
                if (artistModel != null)
                {
                    data = new ArtistModel().FromDataModel(artistModel);

                    //Fetch Gender Lookup From LookupService
                    var genderLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("GENDER");
                    data.GenderLKPValue = data.GenderLKPId != null ? genderLookup[data.GenderLKPId.ToString()].VisibleValue : null;

                    MarketTimeZoneModel marketTimeZone = await MarketTimeZoneClient.GetMarketTimeAsync(data.LastUpdatedDate,data.LocationCode);

                    data.LastUpdatedDate = marketTimeZone.MarketTime;
                    data.ArtistTimezone = marketTimeZone.TimeZone;
                    data.LocationCode = this.uow.RepositoryAsync<NAT_AS_Artist_Location_Mapping>()
                        .Queryable()
                        .Where(x => x.Artist_ID == artistModel.Artist_ID)
                        .Select(x => x.Location_Code)
                        .FirstOrDefault();

                    var lookupList = await NatClient.ReadAsync<IEnumerable<ViewModels.CustomerLovViewModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customerslov");
                    if (lookupList.status.IsSuccessStatusCode)
                    {
                        var customerlov = lookupList.data.ToDictionary(item => item.Id, item => item);

                        Parallel.ForEach(data.NatAsArtistRatingLog, (item) =>
                        {
                            item.CustomerName = customerlov[CusId = item.CustomerId ?? default(int)].Value;
                            item.CustomerProfileImageUrl = customerlov[CusId = item.CustomerId ?? default(int)].Image;
                        });

                    }
                    var skillLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("ARTIST_SKILL");
                    foreach (ArtistSkillModel skill in data.NatAsArtistSkill)
                    {
                        skill.SkillLKPValue = skillLookup[skill.SkillLKPId.ToString()].VisibleValue;
                        //seat.SeatAllocationTypeLKPValue = seatTypeLookup[seat.SeatAllocationTypeLKPId.ToString()].VisibleValue;
                        //seat.ObjectState = ObjectState.Modified;
                    }

                    var artisteventModel = await uow.RepositoryAsync<NAT_AS_Artist_Event>().GetEventCalculatedDetailsByArtistId(Id);
                    var artistevent = JsonConvert.SerializeObject(artisteventModel);
                    IEnumerable<ArtistEventViewModel> ArtistEventData = JsonConvert.DeserializeObject<IEnumerable<ArtistEventViewModel>>(artistevent);

                    if (ArtistEventData != null)
                    {
                        Dictionary<int, ArtistEventViewModel> artisteventDictionary = ArtistEventData.ToDictionary(item => item.ArtistId, item => item);
                        data.Eventsheld = artisteventDictionary.ContainsKey(data.ArtistId) ? artisteventDictionary[data.ArtistId].Eventsheld : 0;
                        data.LastEventDate = artisteventDictionary.ContainsKey(data.ArtistId) ? artisteventDictionary[data.ArtistId].LastEventDate : null;
                        data.UpcommingEvents = artisteventDictionary.ContainsKey(data.ArtistId) ? artisteventDictionary[data.ArtistId].UpcommingEvents : 0;

                    }

                    var plannerResp = await NatClient.ReadAsync<IEnumerable<ViewModels.AvailabilityViewModel>>(NatClient.Method.GET, NatClient.Service.PlannerService, "Availability/Planner/" + artistModel.Planner_ID);

                    if (plannerResp.status.IsSuccessStatusCode)
                    {
                        List<ArtistAvailabilityModel> mainAvailability = new List<ArtistAvailabilityModel>();
                        foreach (ViewModels.AvailabilityViewModel venueAvailabilityModel in plannerResp.data)
                        {
                            var availability = new ArtistAvailabilityModel();
                            availability.DayOfWeekLKPId = venueAvailabilityModel.DayOfWeekLKPId;
                            availability.PlannerId = venueAvailabilityModel.PlannerId;
                            availability.AvailabilityId = venueAvailabilityModel.AvailabilityId;
                            availability.AvailabilitySlot = new List<ArtistAvailabilitySlotModel>();
                            foreach (AvailabilitySlotViewModel venueAvailabilitySlotModel in venueAvailabilityModel.NatPlsAvailabilitySlot)
                            {
                                var availabilitySlot = new ArtistAvailabilitySlotModel();
                                availabilitySlot.StartTime = venueAvailabilitySlotModel.StartTime;
                                availabilitySlot.EndTime = venueAvailabilitySlotModel.EndTime;
                                availability.AvailabilitySlot.Add(availabilitySlot);
                            }

                            //insert into whole availability then run post
                            mainAvailability.Add(availability);
                        }

                        data.Availability = mainAvailability;
                    }

                    var preferencesResp = await NatClient.ReadAsync<List<NotificationPreferenceModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "getpref/" + Id);
                    if (preferencesResp != null && preferencesResp.status.IsSuccessStatusCode)
                    {
                        data.NotificationPreferences = preferencesResp.data;
                    }


                    //Appending Address And City In Address
                    //var locationList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                    //Dictionary<string, AddressGeographyViewModel> locationDictionary = locationList.data.ToDictionary(item => item.GeographyShortCode, item => item);
                    //data.NatAsArtistResidentialAddress.Address = data.NatAsArtistResidentialAddress.AddressLine1 + ", " + (locationDictionary.Keys.Contains(data.NatAsArtistResidentialAddress.CityName) ? locationDictionary[data.NatAsArtistResidentialAddress.CityName].GeographyName : "");

                    ////Adding CityName and No of Followers In Artist
                    //data.CityName = (data.NatAsArtistResidentialAddress.CityName != null && locationDictionary.Keys.Contains(data.NatAsArtistResidentialAddress.CityName) ? locationDictionary[data.NatAsArtistResidentialAddress.CityName].GeographyName : "");

                    //Total Number Of Events   
                    data.EventHosted = uow.Repository<NAT_AS_Artist_Event>().GetTotalEvents(Id);

                    if (customerId == null)
                    {
                        data.FollowStatus = false;
                        return data;
                    }

                    else
                    {
                        data.FollowStatus = false;
                        var FollowingArtists = await NatClient.ReadAsync<IEnumerable<CustomerFollowingModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customer/" + Convert.ToString(customerId) + "/" + Constants.ReferenceType["ARTIST"] + "/following");

                        if (FollowingArtists != null)
                        {
                            foreach (CustomerFollowingModel followedartist in FollowingArtists.data)
                            {
                                if (data.ArtistId == followedartist.ReferenceId)
                                {
                                    data.FollowStatus = true;
                                }
                            }

                        }
                        return data;
                    }

                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        
        /// <summary>
        /// Create: Method for creation of artist
        /// This is not used anywhere. ApproveRequest method is used for Artist Creation
        /// </summary>
        /// <param name="servicemodel">Service Artist Model</param>
        /// <returns>Artist ID generated for artist</returns>
        public async Task<string> CreateArtistAsync(ArtistModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    // NAT_AS_Artist: Place checks for tenant id, Artist_Status_LKP_ID (database level), stripe id  (validations code level) 
                    servicemodel.ActiveFlag = true;
                    //if (servicemodel.AvailableCredit < 0)
                        servicemodel.AvailableCredit = 9999;
                    servicemodel.ObjectState = ObjectState.Added;

                    // NAT_AS_Artist_Bank_Account: place checks for tenant id,bank id (database level), Bank Account Number,Bank Routing Number (validations code level)
                    if (servicemodel.NatAsArtistBankAccount != null)
                    {
                        foreach (ArtistBankAccountModel ArtistBankAccount in servicemodel.NatAsArtistBankAccount)
                        {
                            ArtistBankAccount.CreatedBy = servicemodel.LastUpdatedBy;
                            ArtistBankAccount.LastUpdatedBy = servicemodel.LastUpdatedBy;
                            ArtistBankAccount.ActiveFlag = true;
                            ArtistBankAccount.ObjectState = ObjectState.Added;
                        }
                    }
                    // NAT_AS_Artist_Rating: place checks for tenant id (database level)
                    if (servicemodel.NatAsArtistRating != null)
                    {
                        foreach (ArtistRatingModel ArtistRating in servicemodel.NatAsArtistRating)
                        {
                            ArtistRating.CreatedBy = servicemodel.LastUpdatedBy;
                            ArtistRating.LastUpdatedBy = servicemodel.LastUpdatedBy;
                            ArtistRating.AverageRatingValue = 0;
                            ArtistRating.NumberOfRatings = 0;
                            ArtistRating.ActiveFlag = true;
                            ArtistRating.ObjectState = ObjectState.Added;
                        }
                    }
                    // NAT_AS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                    // Person Email, Date_Of_Birth, Person_Extension (validations code level)
                    var planner = new ViewModels.PlannerViewModel()
                    {
                        PlannerId = 0,
                        PlannerTypeLKPId = 1,//Standard
                        ReferenceTypeLKPId = 1,//Artist
                        ReferenceId = null,//Artist not created yet
                        ActiveFlag = true
                    };
                    var plannerResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.PlannerService, "Planner", requestBody: planner);

                    if (plannerResp.status.IsSuccessStatusCode)
                    {
                        int plannerID = Int32.Parse(plannerResp.data);
                        servicemodel.PlannerId = plannerID;
                        Insert(servicemodel);
                        await uow.SaveChangesAsync();
                        return Convert.ToString(Get().ArtistId);
                    }
                    else
                    {
                        throw new ServiceLayerException("Planner service returned error");
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

        /// <summary>
        /// Update: Method for Updation of artist record
        /// </summary>
        /// <param name="servicemodel">Service ArtistModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<ArtistModel> UpdateArtistAsync(ArtistModel servicemodel)
        {
            try
            {
                if (servicemodel.ArtistId != 0 || servicemodel.ArtistId > 0)
                {

                    NAT_AS_Artist arti = await uow.RepositoryAsync<NAT_AS_Artist>().Queryable().Where(x => x.Artist_ID == servicemodel.ArtistId).AsNoTracking().FirstOrDefaultAsync(); 
                    // NAT_AS_Artist: stripe id  (validations code level)
                    servicemodel.ObjectState = ObjectState.Modified;


                    if(arti.Artist_Email != servicemodel.ArtistEmail)
                    {
                        var userResp = await NatClient.ReadAsync<Boolean>(NatClient.Method.GET, NatClient.Service.AuthService, "User/alreadyRegistered/" + servicemodel.ArtistEmail);
                        if (userResp.status.IsSuccessStatusCode && userResp.data == false)
                        {
                            var ChangeEmail = new ViewModels.ChangeEmailViewModel()
                            {
                                NewEmail = servicemodel.ArtistEmail,
                                OldEmail = arti.Artist_Email,
                            };
                            var userResp1 = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/AdminChangeUserEmail", requestBody: ChangeEmail);
                        }
                        else
                        {
                            throw new Exception("Artist Email Already registered");
                        }
                    }

                    if(servicemodel.ArtistDisbursementHeader != null && servicemodel.ArtistDisbursementHeader.Count() > 0)
                    {
                        var response = await NatClient.ReadAsyncWithHeaders<bool>(NatClient.Method.PUT, NatClient.Service.FinancialService, "BulkUpdateDisbursementHeader", requestBody: servicemodel.ArtistDisbursementHeader);
                        if(response.status.IsSuccessStatusCode != true || response.data == false)
                        {
                            throw new Exception("Failed to Update Disbursement Header");
                        }
                    }

                    // NAT_AS_Artist_Bank_Account: Bank Account Number,Bank Routing Number(validations code level)
                    if (servicemodel.NatAsArtistBankAccount != null)
                    {
                        foreach (ArtistBankAccountModel ArtistBankAccount in servicemodel.NatAsArtistBankAccount)
                        {
                            if (ArtistBankAccount.ArtistBankAccountId > 0)
                            {
                                ArtistBankAccount.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                ArtistBankAccount.ObjectState = ObjectState.Modified;
                            }

                            else if (ArtistBankAccount.ArtistBankAccountId < 0)
                            {
                                ArtistBankAccount.ArtistBankAccountId *= -1;
                                ArtistBankAccount.ObjectState = ObjectState.Deleted;
                            }
                            else if (ArtistBankAccount.ArtistBankAccountId == 0)
                            {
                                ArtistBankAccount.CreatedBy = servicemodel.LastUpdatedBy;
                                ArtistBankAccount.LastUpdatedBy = servicemodel.LastUpdatedBy; 
                                ArtistBankAccount.ActiveFlag = true;
                                ArtistBankAccount.ObjectState = ObjectState.Added;                                
                            }

                            // Check if the bank account has address specified
                            var branchAddressExists = this.uow.Repository<NAT_AS_Artist_Bank_Account>()
                                .Queryable()
                                .Where(x => x.Artist_ID == servicemodel.ArtistId && x.Address_ID != null)
                                .Any();
                            if (branchAddressExists)
                            {
                                if (ArtistBankAccount.NatAsArtistAddress != null)
                                {
                                    ArtistBankAccount.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                    ArtistBankAccount.NatAsArtistAddress.ObjectState = ObjectState.Modified;
                                }
                            }
                            else
                            {
                                if (ArtistBankAccount.NatAsArtistAddress != null)
                                {
                                    ArtistBankAccount.CreatedBy = servicemodel.LastUpdatedBy;
                                    ArtistBankAccount.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                    ArtistBankAccount.NatAsArtistAddress.ObjectState = ObjectState.Added;
                                }


                            }

                        }
                    }

                    // NAT_AS_Artist_Location_Mapping: Artist locatin mapping
                    if (servicemodel.NatAsArtistLocationMapping != null)
                    {
                        foreach (ArtistLocationMappingModel artistLocationMapping in servicemodel.NatAsArtistLocationMapping)
                        {
                            if (artistLocationMapping.ArtistLocationMappingId > 0)
                            {
                                artistLocationMapping.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                artistLocationMapping.ObjectState = ObjectState.Modified;
                            }

                            else if (artistLocationMapping.ArtistLocationMappingId < 0)
                            {
                                artistLocationMapping.ArtistLocationMappingId *= -1;
                                artistLocationMapping.ObjectState = ObjectState.Deleted;
                            }
                            else if (artistLocationMapping.ArtistLocationMappingId == 0)
                            {
                                artistLocationMapping.CreatedBy = servicemodel.LastUpdatedBy;
                                artistLocationMapping.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                artistLocationMapping.ActiveFlag = true;
                                artistLocationMapping.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    // artist venue preference
                    if (servicemodel.NatAsArtistVenuePreference != null)
                    {
                        foreach (ArtistVenuePreferenceModel artistLocationMapping in servicemodel.NatAsArtistVenuePreference)
                        {
                            if (artistLocationMapping.VenuePreferenceId > 0)
                            {
                                artistLocationMapping.ObjectState = ObjectState.Modified;
                            }

                            else if (artistLocationMapping.VenuePreferenceId < 0)
                            {
                                artistLocationMapping.VenuePreferenceId *= -1;
                                artistLocationMapping.ObjectState = ObjectState.Deleted;
                            }
                            else if (artistLocationMapping.VenuePreferenceId == 0)
                            {
                                artistLocationMapping.ActiveFlag = true;
                                artistLocationMapping.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    if (servicemodel.NatAsArtistResidentialAddress != null)
                    {
                        if (servicemodel.NatAsArtistResidentialAddress.AddressId > 0)
                        {
                            servicemodel.NatAsArtistResidentialAddress.LastUpdatedBy = servicemodel.LastUpdatedBy;
                            servicemodel.NatAsArtistResidentialAddress.ObjectState = ObjectState.Modified;
                        }

                        else if (servicemodel.NatAsArtistResidentialAddress.AddressId < 0)
                        {
                            servicemodel.NatAsArtistResidentialAddress.AddressId *= -1;
                            servicemodel.NatAsArtistResidentialAddress.ObjectState = ObjectState.Deleted;
                        }
                        else if (servicemodel.NatAsArtistResidentialAddress.AddressId == 0)
                        {
                            servicemodel.NatAsArtistResidentialAddress.CreatedBy = servicemodel.LastUpdatedBy;
                            servicemodel.NatAsArtistResidentialAddress.LastUpdatedBy = servicemodel.LastUpdatedBy;
                            servicemodel.NatAsArtistResidentialAddress.ObjectState = ObjectState.Added;
                        }
                    }

                    if (servicemodel.NatAsArtistSkill != null)
                    {
                        foreach (ArtistSkillModel ArtistSkill in servicemodel.NatAsArtistSkill)
                        {
                            if (ArtistSkill.ArtistSkillId > 0)
                            {
                                ArtistSkill.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                ArtistSkill.ObjectState = ObjectState.Modified;
                            }

                            else if (ArtistSkill.ArtistSkillId < 0)
                            {
                                ArtistSkill.ArtistSkillId *= -1;
                                ArtistSkill.ObjectState = ObjectState.Deleted;
                            }
                            else if (ArtistSkill.ArtistSkillId == 0)
                            {
                                ArtistSkill.CreatedBy = servicemodel.LastUpdatedBy;
                                ArtistSkill.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                ArtistSkill.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    if (servicemodel.NatAsArtistDocument != null)
                    {
                        foreach (ArtistDocumentModel ArtistDocument in servicemodel.NatAsArtistDocument)
                        {
                            if (ArtistDocument.ArtistDocumentId > 0)
                            {
                                ArtistDocument.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                ArtistDocument.ObjectState = ObjectState.Modified;
                            }

                            else if (ArtistDocument.ArtistDocumentId < 0)
                            {
                                ArtistDocument.ArtistDocumentId *= -1;
                                ArtistDocument.ObjectState = ObjectState.Deleted;
                            }
                            else if (ArtistDocument.ArtistDocumentId == 0)
                            {
                                ArtistDocument.CreatedBy = servicemodel.LastUpdatedBy;
                                ArtistDocument.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                ArtistDocument.ObjectState = ObjectState.Added;
                            }
                        }
                    }
                    if (servicemodel.NotificationPreferences != null)
                    {
                        var preferencesResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.AuthService, "updatepref", requestBody: servicemodel.NotificationPreferences);

                        if (!preferencesResp.status.IsSuccessStatusCode)
                        {
                            throw new Exception("Preferences could not be updated");
                        }
                    }

                    if (servicemodel.PasswordChanged == true)
                    {
                        var changePasswordModel = new ViewModels.ChangePasswordViewModel()
                        {
                            Username = servicemodel.ArtistEmail,
                            NewPassword = servicemodel.Password,
                        };

                        var userResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/AdminChangeUserPassword", requestBody: changePasswordModel);
                    }
                    servicemodel.LastUpdatedDate = System.DateTime.UtcNow;
                    base.Update(servicemodel);
                    int updatedRows = await uow.SaveChangesAsync();

                    var artistEvents = await uow.RepositoryAsync<NAT_AS_Artist_Event>().Queryable().Where(x => x.Artist_ID == servicemodel.ArtistId && x.Start_Time > DateTime.Now).ToListAsync();
                    if (((bool)!servicemodel.ActiveFlag) && artistEvents.Count() > 0)
                    {
                        var removeArtistFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkArtistUnavailableByArtistID", requestBody: servicemodel.ArtistId);
                        if (!removeArtistFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Artist Deactivated. Artist cannot be removed from Upcoming Events");
                    }

                    if (updatedRows == 0)
                    {
                        return servicemodel;
                    }

                    return servicemodel;                    
                }
                else
                {
                    return servicemodel;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method activates artist 
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public async Task<bool> ActivateArtistAsync(string Id,string username)
        {
            try
            {
                NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(Id));
                if (ArtistEf != null)
                {
                    ArtistEf.Last_Updated_By = username;
                    UpdateUserModel updateuser = new UpdateUserModel();
                    updateuser.ReferenceId = ArtistEf.Artist_ID;
                    updateuser.ReferenceTypeLKP = "artist";                    
                    var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/ActivateUserByReference", requestBody: updateuser);
                    if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                    {
                        uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(true, ArtistEf);
                        uow.SaveChanges();
                        return true;
                    }
                    else { throw new Exception("User doesnot exists"); }
                }
                else
                    throw new ApplicationException("Artist doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method Activates artist in bulk
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public async Task<bool> BulkActivateArtistAsync(IEnumerable<ArtistModel> servicemodel,string username)
        {
            try
            {
                List<UpdateUserModel> bulkupdateuser = new List<UpdateUserModel>();
                foreach (var data in servicemodel)
                {
                    NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(data.ArtistId));
                    if (ArtistEf != null)
                    {
                        ArtistEf.Last_Updated_By = username;
                        UpdateUserModel updateuser = new UpdateUserModel();
                        updateuser.ReferenceId = ArtistEf.Artist_ID;
                        updateuser.ReferenceTypeLKP = "artist";
                        bulkupdateuser.Add(updateuser);
                    }
                    else
                    {
                        throw new ApplicationException("Artist doesnot exists");
                    }
                }
                var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/BulkActivateUserByReference", requestBody: bulkupdateuser);
                if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                {
                    foreach (var data in servicemodel)
                    {
                        NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(data.ArtistId));
                        uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(true, ArtistEf);
                    }
                    int updatedRows = await uow.SaveChangesAsync();

                    if (updatedRows == 0)
                    {
                        //for revsersal of user all user active flag if error occurs in artist active flag updation
                        foreach (var data in servicemodel)
                        {
                            UpdateUserModel updateuser = new UpdateUserModel();
                            updateuser.ReferenceId = data.ArtistId;
                            updateuser.ReferenceTypeLKP = "artist";
                            if (data.ActiveFlag == true)
                            {
                                var reverseuserflag = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/ActivateUserByReference", requestBody: updateuser);
                            }
                            else
                            {
                                var reverseuserflag = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/DeActivateUserByReference", requestBody: updateuser);
                            }
                        }
                        return false;
                    }
                    return true;
                }
                else
                {
                    throw new Exception("User doesnot exists");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// This method deactivates artist 
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public async Task<bool> DeactivateArtistAsync(string Id,string username)
        {
            try
            {
                NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(Id));
                if (ArtistEf != null)
                {
                    ArtistEf.Last_Updated_By = username;
                    UpdateUserModel updateuser = new UpdateUserModel();
                    updateuser.ReferenceId = ArtistEf.Artist_ID;
                    updateuser.ReferenceTypeLKP = "artist";
                    var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/DeActivateUserByReference", requestBody: updateuser);
                    if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                    {
                        uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(false, ArtistEf);
                        uow.SaveChanges();
                        var removeArtistFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkArtistUnavailableByArtistID", requestBody: ArtistEf.Artist_ID);
                        if (!removeArtistFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Venue Deactivated. Venue cannot be removed from Upcoming Events");
                        return true;
                    }
                    else { throw new Exception("User doesnot exists"); }
                }
                else
                    throw new ApplicationException("Artist doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates artist in bulk
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public async Task<bool> BulkDeactivateArtistAsync(IEnumerable<ArtistModel> servicemodel,string username)
        {
            try
            {
                List<UpdateUserModel> bulkupdateuser = new List<UpdateUserModel>();
                foreach (var data in servicemodel)
                {
                    NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(data.ArtistId));
                    if (ArtistEf != null)
                    {
                        ArtistEf.Last_Updated_By = username;
                        UpdateUserModel updateuser = new UpdateUserModel();
                        updateuser.ReferenceId = ArtistEf.Artist_ID;
                        updateuser.ReferenceTypeLKP = "artist";
                        bulkupdateuser.Add(updateuser);

                        var removeArtistFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkArtistUnavailableByArtistID", requestBody: ArtistEf.Artist_ID);
                        if (!removeArtistFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Artist Deactivated. Artist cannot be removed from Upcoming Events");
                    }
                    else
                    {
                        throw new ApplicationException("Artist doesnot exists");
                    }
                }
                var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/BulkDeActivateUserByReference", requestBody: bulkupdateuser);
                if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                {
                    foreach (var data in servicemodel)
                    {
                        NAT_AS_Artist ArtistEf = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(Convert.ToInt32(data.ArtistId));
                        uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(false, ArtistEf);
                    }
                    int updatedRows = await uow.SaveChangesAsync();

                    if (updatedRows == 0)
                    {
                        //for revsersal of user all user active flag if error occurs in artist active flag updation
                        foreach (var data in servicemodel)
                        {
                            UpdateUserModel updateuser = new UpdateUserModel();
                            updateuser.ReferenceId = data.ArtistId;
                            updateuser.ReferenceTypeLKP = "artist";
                            if (data.ActiveFlag == true)
                            {
                                var reverseuserflag = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/ActivateUserByReference", requestBody: updateuser);
                            }
                            else
                            {
                                var reverseuserflag = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/DeActivateUserByReference", requestBody: updateuser);
                            }
                        }
                        return false;
                    }
                    return true;
                }
                else { throw new Exception("User doesnot exists"); }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<DataSourceResult> SearchArtistForEvent(ArtistSearchForEventModel search)
        {////////////////////


            ///////////////////
            ///
            //var plannerSearchResp = await NatClient.ReadAsync<IEnumerable<ViewModels.PlannerViewModel>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/search", requestBody: new
            //{
            //    StartTime = search.AvailabilityStartTime,
            //    EndTime = search.AvailabilityEndTime,
            //    ReferenceType = 1
            //});
            //var plannerIDs = plannerSearchResp.data.Select(i => i.PlannerId).ToList();
            //var artists = await uow.RepositoryAsync<NAT_AS_Artist>().SearchArtistForEvent(plannerIDs, search.MinRating, search.MaxRating, search.SkillLKPId, search.SearchText, search.LocationCode);
            try
            {

                var allArtistWithLocationCodes = await uow.RepositoryAsync<NAT_AS_Artist>().GetAllArtistByLocationCode(search.LocationCode);
                var artistsSM = new ArtistModel().FromDataModelList(allArtistWithLocationCodes.Where(art => art.Active_Flag).ToList()).ToList();

                //////////
                if (search.AvailabilityStartTime != null || search.AvailabilityEndTime != null)
                {
                    var plannerSearchResp = await NatClient.ReadAsync<List<int>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/searchchange", requestBody: new
                    {
                        StartTime = search.AvailabilityStartTime,
                        EndTime = search.AvailabilityEndTime,
                        ReferenceType = 1
                    });
                    var plannerIDs = plannerSearchResp.data;

                    var plannerCollisionSearchResp = await NatClient.ReadAsync<List<int>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/collidingevent", requestBody: new
                    {
                        StartTime = search.AvailabilityStartTime,
                        EndTime = search.AvailabilityEndTime,
                        ReferenceType = 1
                    });
                    var collidingPlannerIDs = plannerCollisionSearchResp.data;

                    //getting areas against city


                    var allAvailableArtists = await uow.RepositoryAsync<NAT_AS_Artist>().SearchArtistForEvent(plannerIDs, search.MinRating, search.MaxRating, search.SkillLKPId, search.SearchText, search.LocationCode);

                    foreach (var obj in allAvailableArtists)
                    {
                        var artist = artistsSM.Where(x => x.ArtistId == obj.Artist_ID).FirstOrDefault();
                        if (artist != null) artist.IsArtistAvailable = true;
                    }

                    artistsSM = artistsSM.Where(x => x.PlannerId != null && !collidingPlannerIDs.Contains(x.PlannerId ?? default)).ToList();
                }
                /////////


                //var allAvailableArtists = await uow.RepositoryAsync<NAT_AS_Artist>().SearchArtistForEvent(plannerIDs, search.MinRating, search.MaxRating, search.SkillLKPId, search.SearchText, search.LocationCode);


                if (search.VirtualEvent)
                {
                    artistsSM = artistsSM.Where(x => x.HostVirtualEvent).ToList();
                }


                var total = artistsSM.Count;
                var paginatedData = artistsSM.AsQueryable().Skip((search.skip != null ? (int)search.skip : 0)).Take((search.take != null ? (int)search.take : total)).ToList();

                var skillLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("ARTIST_SKILL");
                paginatedData = paginatedData.Select((x) =>
                {
                    x.NatAsArtistSkill = x.NatAsArtistSkill.Select((y) =>
                    {
                        y.SkillLKPValue = skillLookup[y.SkillLKPId.ToString()].VisibleValue;
                        return y;
                    }).ToList();

                    return x;
                }).ToList();

                var artisteventModel = await uow.RepositoryAsync<NAT_AS_Artist_Event>().GetAllEventCalculatedDetails();
                var artistevent = JsonConvert.SerializeObject(artisteventModel);
                IEnumerable<ArtistEventViewModel> ArtistEventData = JsonConvert.DeserializeObject<IEnumerable<ArtistEventViewModel>>(artistevent);

                if (ArtistEventData != null)
                {
                    Dictionary<int, ArtistEventViewModel> artisteventDictionary = ArtistEventData.ToDictionary(item => item.ArtistId, item => item);

                    paginatedData = paginatedData.Select((x) =>
                    {

                        x.Eventsheld = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].Eventsheld : 0;
                        x.LastEventDate = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].LastEventDate : null;
                        x.UpcommingEvents = artisteventDictionary.ContainsKey(x.ArtistId) ? artisteventDictionary[x.ArtistId].UpcommingEvents : 0;

                        return x;
                    }).ToList();
                }

                var locationList = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
                if (locationList.status.IsSuccessStatusCode)
                {
                    Dictionary<string, LocationViewModel> locationDictionary = locationList.data.ToDictionary(item => item.LocationShortCode, item => item);
                    paginatedData = paginatedData.Select((x) =>
                    {
                        x.LocationName = x.LocationCode != null ? locationDictionary[x.LocationCode].LocationName : null;
                        return x;
                    }).ToList();
                }
                return new DataSourceResult() { Data = paginatedData.OrderByDescending(x => x.IsArtistAvailable).ThenBy(x => x.ArtistFullName), Total = total };
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<String> SubmitArtistRequestAsync(ArtistRequestModel obj)
        {
            try
            {
                //Guid Generator for each Object
                Guid newGuid = Guid.NewGuid();

                //ArtistRequestTableEntityModel created for entry
                ArtistRequestTableEntityModel ob = new ArtistRequestTableEntityModel(obj.City, newGuid.ToString(), obj);

                ob.ArtistFullName = ob.FirstName + " " + ob.LastName;

                var cityList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                if (cityList.status.IsSuccessStatusCode)
                {
                    Dictionary<string, AddressGeographyViewModel> cityDictionary = cityList.data.ToDictionary(item => item.GeographyShortCode, item => item);

                    AddressGeographyViewModel city = cityDictionary[obj.City];
                    ob.CityName = city.GeographyName;
                }

                ob.HoursPerWeek = Convert.ToString(Math.Round(CalculateHoursPerWeekForEntity(obj.Availability), 1, MidpointRounding.ToEven));

                ob.ArtistJsonData = JsonConvert.SerializeObject(obj);

                TableStorage ts = new TableStorage();
                await ts.InsertTableStorage("ArtistRequest", ob);

                return "Added Artist Request Successfully";

            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        public async Task<DataSourceResult> GetArtistRequestAsync(string code, DataSourceRequest dataSourceRequest)
        {
            //var locations = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location/AllActiveChildren/" + code);

            //if (locations.status.IsSuccessStatusCode)
            //{
            //    List<String> cities = new List<String>();

            //    List<LocationViewModel> locationsList = locations.data.ToList();

            //    foreach (LocationViewModel loc in locationsList)
            //    {
            //        cities.Add(loc.LocationShortCode);
            //    }

            //    string finalFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, cities.ElementAt(0));

            //    for (int i = 1; i < cities.Count(); i++)
            //    {
            //        var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, cities.ElementAt(i));
            //        finalFilter = TableQuery.CombineFilters(finalFilter, TableOperators.Or, filter);
            //    }

            TableStorage ts = new TableStorage();
            IEnumerable<ArtistRequestTableEntityModel> requests = await ts.RetrieveTableStorage<ArtistRequestTableEntityModel>("ArtistRequest");

            requests = requests.Select((x) =>
            {
                x.ArtistData = JsonConvert.DeserializeObject<ArtistRequestModel>(x.ArtistJsonData);
                return x;
            }).ToList();

            var tempReq = requests.AsQueryable().ToDataSourceResult(dataSourceRequest);
            return requests.AsQueryable().ToDataSourceResult(dataSourceRequest);

            //}
            //else
            //{
            //    return null;
            //}
        }

        public async Task<ArtistRequestTableEntityModel> GetArtistRequestByGuidAsync(String city, String guid)
        {
            string finalFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, city);

            TableStorage ts = new TableStorage();
            IEnumerable<ArtistRequestTableEntityModel> requests = await ts.RetrieveTableStorage<ArtistRequestTableEntityModel>("ArtistRequest", finalFilter);

            requests = requests.Select((x) =>
            {
                x.ArtistData = JsonConvert.DeserializeObject<ArtistRequestModel>(x.ArtistJsonData);
                return x;
            }).ToList();

            return requests.AsQueryable().Where(x => x.RowKey == guid).FirstOrDefault();

        }

        public async Task<ArtistModel> ApproveArtistRequestAsync(ArtistRequestModel obj, Auth.UserModel UserModel)
        {
            try
            {
                //Checking if the manager created the artist

                var rolechecker = 0;
                foreach (Auth.RoleModel Roles in UserModel.Roles)
                {
                    if (Roles.RoleName == "Artist Manager")
                    {
                        rolechecker = 1;
                    }
                }


                //Artist Main Fields
                ArtistModel artistModel = new ArtistModel();
                artistModel.ArtistFirstName = obj.FirstName;
                artistModel.ArtistLastName = obj.LastName;
                artistModel.ContactNumber = obj.ContactNo;
                artistModel.EmergencyContact = obj.EmergencyContact;
                artistModel.ArtistEmail = obj.Email;
                artistModel.BusinessName = obj.StudioBusiness;
                artistModel.ArtistPortfolioUrl = obj.PortfolioUrl;
                artistModel.ArtistProfileImageLink = obj.ProfileImageUrl;
                artistModel.LocationCode = obj.LocationCode;
                artistModel.ArtistAbout = obj.ArtistAbout;
                artistModel.FacebookProfileUrl = obj.FacebookProfileUrl;
                artistModel.TwitterProfileUrl = obj.TwitterProfileUrl;
                artistModel.InstagramProfileUrl = obj.InstagramProfileUrl;
                artistModel.StageName = obj.StageName;
                artistModel.SIN = obj.SIN;
                artistModel.TaxNumber = obj.TaxNumber;
                artistModel.PaymentCycleLKPId = obj.PaymentCycleLKPId;         //Added after changes, remove this in case of issue
                artistModel.DefaultPaymentMethod = obj.DefaultPaymentMethod;
                artistModel.CompanyEmail = obj.CompanyEmail;
                artistModel.ActiveFlag = obj.ActiveFlag;
                artistModel.VisibleToPublic = obj.VisibleToPublic;
                artistModel.EventsVisibleToPublic = obj.EventsVisibleToPublic;
                artistModel.CreatedBy = UserModel.Email;
                artistModel.LastUpdatedBy = UserModel.Email;
                artistModel.AvailableCredit = 20000;

                artistModel.GoogleMapsUrl = obj.GoogleMapsUrl;
                artistModel.GoogleMapsUrlSupply = obj.GoogleMapsUrlSupply;


                artistModel.EmergencyContactEmail = obj.EmergencyContactEmail;
                artistModel.Gender = obj.Gender;
                artistModel.IdType = obj.IdType;
                artistModel.IdNumber = obj.IdNumber;
                artistModel.CompanyPhone = obj.CompanyPhone;
                artistModel.CompanyName = obj.CompanyName;
                artistModel.Onboarded = obj.Onboarded;

        artistModel.EmergencyContactName = obj.EmergencyContactName; 
                artistModel.EmergencyContactRelationship = obj.EmergencyContactRelationship; 
                artistModel.BizNumber = obj.BizNumber; 
                artistModel.Notes = obj.Notes;
                artistModel.Greeting = obj.Greeting; 
                artistModel.AvailableCredit = obj.AvailableCredit;
                artistModel.ObjectState = ObjectState.Added;

                //Artist Residential Address
                ArtistAddressModel artistAddressModel = new ArtistAddressModel();
                artistAddressModel.AddressLine1 = obj.Address;
                artistAddressModel.AddressLine2 = obj.AddressTwo;
                artistAddressModel.CityName = obj.City;
                artistAddressModel.PostalZipCode = obj.ZipCode;
                artistAddressModel.ObjectState = ObjectState.Added;
                artistAddressModel.AddressGeographyId = obj.AddressGeographyId;
                artistModel.NatAsArtistResidentialAddress = artistAddressModel;

                // Artist Skills
                artistModel.NatAsArtistSkill = new List<ArtistSkillModel>();
                foreach (ArtistSkillRequestModel element in obj.Skill)
                {
                    ArtistSkillModel artistSkillModel = new ArtistSkillModel();
                    artistSkillModel.SkillLKPId = element.SkillId;
                    artistSkillModel.SkillRating = element.Rating;
                    artistSkillModel.ObjectState = ObjectState.Added;
                    artistModel.NatAsArtistSkill.Add(artistSkillModel);
                }

                if(obj.NatAsArtistVenuePreference != null  && obj.NatAsArtistVenuePreference.Count() > 0)
                {
                    artistModel.NatAsArtistVenuePreference = new List<ArtistVenuePreferenceModel>();
                    foreach (ArtistVenuePreferenceModel element in obj.NatAsArtistVenuePreference)
                    {
                        ArtistVenuePreferenceModel artistSkillModel = new ArtistVenuePreferenceModel();
                        artistSkillModel.VenueId = element.VenueId;
                        artistSkillModel.ActiveFlag = element.ActiveFlag;
                        artistSkillModel.ObjectState = ObjectState.Added;
                        artistModel.NatAsArtistVenuePreference.Add(artistSkillModel);
                    }
                }



                // Artist Documents
                artistModel.NatAsArtistDocument = new List<ArtistDocumentModel>();
                if (obj.Document != null)
                {
                    foreach (ArtistDocumentRequestModel element in obj.Document)
                    {
                        ArtistDocumentModel artistDocumentModel = new ArtistDocumentModel();
                        artistDocumentModel.DocumentName = element.DocumentName;
                        artistDocumentModel.DocumentUrl = element.DocumentUrl;
                        artistDocumentModel.FileName = element.FileName;
                        artistDocumentModel.FileType = element.FileType;
                        artistDocumentModel.ObjectState = ObjectState.Added;
                        artistModel.NatAsArtistDocument.Add(artistDocumentModel);
                    }
                }

                // Artist Rating Simple Object Creation
                artistModel.NatAsArtistRating = new List<ArtistRatingModel>();
                ArtistRatingModel artistRatingModel = new ArtistRatingModel();
                artistRatingModel.AverageRatingValue = 0;
                artistRatingModel.NumberOfRatings = 0;
                artistRatingModel.ObjectState = ObjectState.Added;
                artistModel.NatAsArtistRating.Add(artistRatingModel);

                if (obj.AccountNumber != null)              //Added after changes, remove this in case of issue
                {
                    // Artist Banking Simple Object Creation
                    artistModel.NatAsArtistBankAccount = new List<ArtistBankAccountModel>();
                    ArtistBankAccountModel artistBankAccountModel = new ArtistBankAccountModel();
                    artistBankAccountModel.BankAccountNumber = obj.AccountNumber;
                    artistBankAccountModel.BankRoutingNumber = obj.BankRoutingNumber;
                    artistBankAccountModel.BankLKPId = obj.ArtistBankLKPId;
                    artistBankAccountModel.TransitNumber = obj.TransitNumber;
                    artistBankAccountModel.ObjectState = ObjectState.Added;

                    if (obj.BranchZipCode != null)
                    {
                        ArtistAddressModel branchAddress = new ArtistAddressModel();
                        branchAddress.AddressLine1 = obj.BranchAddress;
                        branchAddress.AddressLine2 = obj.BranchAddressTwo;
                        branchAddress.CityName = obj.BranchCity;
                        branchAddress.PostalZipCode = obj.BranchZipCode;
                        branchAddress.AddressGeographyId = obj.BranchAddressGeographyId;
                        branchAddress.ObjectState = ObjectState.Added;

                        artistBankAccountModel.NatAsArtistAddress = branchAddress;
                    }
                    artistModel.NatAsArtistBankAccount.Add(artistBankAccountModel);
                }

                if (obj.CompanyAccountNumber != null)              //company bank details
                {
                    ArtistBankAccountModel artistBankAccountModel = new ArtistBankAccountModel();
                    artistBankAccountModel.BankAccountNumber = obj.CompanyAccountNumber;
                    artistBankAccountModel.BankCode = obj.CompanyBankCode;
                    artistBankAccountModel.ObjectState = ObjectState.Added;

                    if (obj.BranchZipCode != null)
                    {
                        ArtistAddressModel branchAddress = new ArtistAddressModel();
                        branchAddress.AddressLine1 = obj.BranchAddress;
                        branchAddress.AddressLine2 = obj.BranchAddressTwo;
                        branchAddress.CityName = obj.BranchCity;
                        branchAddress.PostalZipCode = obj.BranchZipCode;
                        branchAddress.AddressGeographyId = obj.BranchAddressGeographyId;
                        branchAddress.ObjectState = ObjectState.Added;

                        artistBankAccountModel.NatAsArtistAddress = branchAddress;
                    }
                    artistModel.NatAsArtistBankAccount.Add(artistBankAccountModel);
                }

                var plannerStatusUpdateTasks = new List<Task>();

                if (obj.LocationCodesList != null && obj.LocationCodesList.Count > 0)
                {
                    artistModel.NatAsArtistLocationMapping = new List<ArtistLocationMappingModel>();
                    foreach (var location in obj.LocationCodesList)
                    {
                        var artistLocation = new ArtistLocationMappingModel();
                        artistLocation.LocationCode = location.Key;
                        artistLocation.PlannerId = location.Value;
                        artistLocation.ActiveFlag = true;
                        artistLocation.ObjectState = ObjectState.Added;
                        artistModel.NatAsArtistLocationMapping.Add(artistLocation);
                        plannerStatusUpdateTasks.Add(NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.PlannerService, "UpdatePlannerType/" + location.Value));
                    }
                }
                //Planner Adding
                artistModel.PlannerId = obj.PlannerId;

                await Task.WhenAll(plannerStatusUpdateTasks);

                this.Insert(artistModel);

                var userModel = new ViewModels.UserViewModel()
                {
                    UserId = 0,
                    TenantId = 1,
                    UserName = obj.Email,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    Email = obj.Email,
                    PhoneNumber = obj.ContactNo,
                    ActiveFlag = true,
                    Password = obj.Password,
                    RoleCode = Constants.UserRoleCode["ARTIST"],
                    ReferenceId = 0,
                    UserImageURL = obj.ProfileImageUrl,
                    ReferenceTypeLKP = Constants.UserReferenceType["ARTIST"]

                };
                if (rolechecker == 1)
                {
                    userModel.ReportingManager = UserModel.UserId;
                }

                if (obj.LocationCodesList != null && obj.LocationCodesList.Count > 0)
                {
                    userModel.NatAusUserLocationMapping = new List<UserLocationMappingViewModel>();
                    foreach (var location in obj.LocationCodesList)
                    {
                        var artistLocation = new UserLocationMappingViewModel();
                        artistLocation.LocationCode = location.Key;
                        artistLocation.ActiveFlag = true;
                        userModel.NatAusUserLocationMapping.Add(artistLocation);
                    }
                }
                var userResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Register", requestBody: userModel);

                if (userResp.status.IsSuccessStatusCode)
                {
                    int updatedRows = await uow.SaveChangesAsync();

                    if (updatedRows != 0)
                    {
                        artistModel = this.Get();

                        long artistId = Convert.ToInt64(artistModel.ArtistId);
                        var userReferenceModel = new UserReferenceViewModel()
                        {
                            ReferenceId = artistId,
                            ReferenceTypeLkp = Constants.UserReferenceType["ARTIST"],
                            UserId = Int64.Parse(userResp.data)

                        };

                        var updateUserRefereceResponse = await NatClient.ReadAsync<UserModel>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Reference", requestBody: userReferenceModel);

                        if (!updateUserRefereceResponse.status.IsSuccessStatusCode)
                        {
                            throw new Exception("An error occured while updating user reference");
                        }

                        if (obj.NotificationPreferences != null)
                        {
                            foreach (NotificationPreferenceModel pref in obj.NotificationPreferences)
                            {
                                pref.UserID = artistId;
                            }
                            var preferencesResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "addPref", requestBody: obj.NotificationPreferences);

                            if (!preferencesResp.status.IsSuccessStatusCode)
                            {
                                throw new Exception("Preferences could not be created");
                            }
                        }

                        if (obj.PartitionKey != null && obj.RowKey != null)
                        {
                            TableStorage ts = new TableStorage();
                            var deletedEntity = await ts.DeleteTableStorage<ArtistRequestTableEntityModel>("ArtistRequest", obj.PartitionKey, obj.RowKey);
                            await ts.InsertTableStorage("ApprovedArtistRequest", deletedEntity);
                        }
                    }
                }
                else
                {
                    throw new Exception("An error occured while registering user");
                }

                artistModel.NatAsArtistLocationMapping = new ArtistLocationMappingModel().FromDataModelList(await uow.RepositoryAsync<NAT_AS_Artist_Location_Mapping>().GetArtistLocationsList(Get().ArtistId)).ToList();

                return artistModel;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<bool> RejectArtistRequestAsync(ArtistRequestModel servicemodel)
        {

            if (servicemodel.PartitionKey != null && servicemodel.RowKey != null)
            {
                TableStorage ts = new TableStorage();
                var deletedEntity = await ts.DeleteTableStorage<ArtistRequestTableEntityModel>("ArtistRequest", servicemodel.PartitionKey, servicemodel.RowKey);
                deletedEntity.RejectionReason = servicemodel.RejectionReason;
                await ts.InsertTableStorage("RejectedArtistRequest", deletedEntity);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> UploadImage(byte[] bfile)
        {
            try
            {

                //guid generator for img file name
                var fileName = Guid.NewGuid() + ".jpeg";

                //using insertBlobStorage function from Nat.Core
                BlobStorage ts = new BlobStorage();
                var imgname = await ts.InsertBlobStorage("ArtistImagesContainerName", bfile, fileName);

                //returns the name of the img saved in blob
                return Environment.GetEnvironmentVariable("ArtistImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("ArtistImagesContainerName") + "/" + imgname;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ArtistDocumentModel> UploadDocument(byte[] bfile, string fileextension)
        {
            try
            {
                //guid generator for img file name
                var fileName = Guid.NewGuid() + "." + fileextension;

                //using insertBlobStorage function from Nat.Core
                BlobStorage ts = new BlobStorage();
                var documentname = await ts.InsertBlobStorage("ArtistDocumentsContainerName", bfile, fileName);

                ArtistDocumentModel documentrequestmodel = new ArtistDocumentModel();
                documentrequestmodel.FileName = fileName;
                documentrequestmodel.DocumentUrl = Environment.GetEnvironmentVariable("ArtistDocumentsContainerBaseUrl") + Environment.GetEnvironmentVariable("ArtistDocumentsContainerName") + "/" + documentname;
                documentrequestmodel.FileType = fileextension.Substring(1);
                //returns the name of the img saved in blob
                return documentrequestmodel;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> ArtistUpdateAvailabilityAsync(IEnumerable<ArtistAvailabilityModel> availabilityModelList, int id)
        {
            //ArtistModel artist = await GetByIdArtistAsync(id);
            var artist = await uow.RepositoryAsync<NAT_AS_Artist>().Queryable().Where(x => x.Artist_ID == id).FirstOrDefaultAsync();

            artist.Hours_Per_Week = Math.Round(CalculateHoursPerWeek(availabilityModelList.ToList()), 1, MidpointRounding.ToEven);

            var availabilityResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.PlannerService, "Availability", requestBody: availabilityModelList);

            if (availabilityResp.status.IsSuccessStatusCode)
            {
                uow.RepositoryAsync<NAT_AS_Artist>().Update(artist);
                uow.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// Create: Method for creation of artist event
        /// </summary>
        /// <param name="servicemodel">Service Artist Event Model</param>
        /// <returns>Event ID generated for  Artist Event</returns>
        public async Task<ArtistEventModel> BookArtistForEvent(ArtistEventModel servicemodel)
        {
            try
            {
                if (servicemodel == null) throw new Exception("Proper data not provided");

                ArtistModel data = null;
                NAT_AS_Artist artistModel = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(servicemodel.ArtistId);

                if (artistModel == null) throw new Exception("Unable to find artist");

                data = new ArtistModel().FromDataModel(artistModel);
                var plannerId = 0;
                foreach(var locationMapping in artistModel.NAT_AS_Artist_Location_Mapping)
                {
                    if(locationMapping.Location_Code == servicemodel.LocationCode)
                    {
                        plannerId = locationMapping.Planner_ID ?? 0;
                    }
                }

                if(plannerId != 0)
                {
                    servicemodel.PlannerId = plannerId;
                    var Eventdata = await NatClient.ReadAsync<ArtistEventModel>(NatClient.Method.POST, NatClient.Service.PlannerService, "PlannerEvent", requestBody: servicemodel);
                    if (Eventdata.status.IsSuccessStatusCode && Eventdata.data != null)
                    {
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;
                        NAT_AS_Artist_Event dataModel = new ArtistEventModel().ToDataModel(servicemodel);
                        uow.Repository<NAT_AS_Artist_Event>().Insert(dataModel);
                        await uow.SaveChangesAsync();
                        servicemodel.GoogleHangoutUrl = Eventdata.data.GoogleHangoutUrl;
                        return servicemodel;
                    }
                    else
                    {
                        throw new Exception("Got error from planner service: " + Eventdata.status.message);
                    }
                } 
                else
                {
                    throw new Exception("Unable to find planner in location mapping, Location Code:" + servicemodel.LocationCode);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Create: Method for Cancel of artist event
        /// </summary>
        /// <param name="servicemodel">Service Artist Event Model</param>
        /// <returns></returns>
        public async Task<Boolean> CancelArtistForEvent(string eventcode)
        {
            try
            {
                if (eventcode == null) { return false; }

                ArtistEventModel artisteventdata = null;
                ArtistModel artistdata = null;

                NAT_AS_Artist_Event artisteventModel = await uow.RepositoryAsync<NAT_AS_Artist_Event>().GetArtistEventByEventCodeAsync(eventcode);

                if (artisteventModel == null) { return false; }

                artisteventdata = new ArtistEventModel().FromDataModel(artisteventModel);
                NAT_AS_Artist artistModel = await uow.RepositoryAsync<NAT_AS_Artist>().GetArtistByIdAsync(artisteventdata.ArtistId);
                int artistId = artisteventdata.ArtistId;

                if (artistModel == null) { return false; }

                artistdata = new ArtistModel().FromDataModel(artistModel);
                foreach(var locMapping in artistdata.NatAsArtistLocationMapping)
                {
                    if (eventcode.Contains(locMapping.LocationCode))
                    {
                        artistdata.PlannerId = locMapping.PlannerId;
                    }
                }
                var Eventdata = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.PlannerService, "CancelPlannerEvent/" + artistdata.PlannerId + "/" + eventcode);

                if (Eventdata.status.IsSuccessStatusCode == false || Eventdata.data == null) { return false; }

                artisteventModel.Status_LKP_ID = 3;
                artisteventModel.ObjectState = ObjectState.Modified;
                uow.Repository<NAT_AS_Artist_Event>().Update(artisteventModel);
                int updatedRows = await uow.SaveChangesAsync();

                if (updatedRows == 0) { return false; }

                //if artist ID is null then return true
                //if (artistId == null) { return true; }

                ////if artist Id is not null so call remove artist from event service 
                //var removeArtistFromEventService = await NatClient.ReadAsync<EventViewModel>(NatClient.Method.PUT, NatClient.Service.EventService, "Event/" + eventcode + "/RemoveArtist");

                ////check response if remove artist from event service is successfull else throw exception
                //if (removeArtistFromEventService.status.IsSuccessStatusCode == false || removeArtistFromEventService.data == null) { throw new Exception("Unable to remove artist from event."); }

                ////create artist search model to pass in search artist for event method
                //var searchModel = new ArtistSearchForEventModel
                //{
                //    AvailabilityStartTime = removeArtistFromEventService.data.EventStartTime,
                //    AvailabilityEndTime = removeArtistFromEventService.data.EventEndTime,
                //    LocationCode = removeArtistFromEventService.data.LocationCode
                //};

                ////call search artist for event method to fetch all avaiable artists
                //var availableArtists = (IEnumerable<ArtistModel>)((await this.SearchArtistForEvent(searchModel)).Data);
                //var artistIds = new List<Int32>();

                ////add all artist Ids to list except the one artist that has cancelled the event
                //availableArtists.ToList().ForEach(artist =>
                //{
                //    if (artistId != artist.ArtistId)
                //    {
                //        artistIds.Add(artist.ArtistId);
                //    }
                //});

                //check if available artist count is not equal to zero
                //TODO:- if artist count is equal to zero have to notify admin that event is in pending status


                //if (artistIds.Count != 0)
                //{
                //    //call configuration service to get time added to current time to get events starting soon
                //    var lookupResponse = await NatClient.ReadAsync<LookupViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/EVENT_ATTEND_PUSH_NOTIFICATION");
                //    var notifTitle = "Event Notification";
                //    var notifBody = "Do you want to attend the event?";
                //    if (lookupResponse != null && lookupResponse.status.IsSuccessStatusCode == true) {
                //        notifTitle = lookupResponse.data.VisibleValue;
                //        notifBody = lookupResponse.data.LookupDescription.Replace(":Date:", (removeArtistFromEventService.data.EventStartTime ?? System.DateTime.Now).ToString("dd MMM, yyyy")).Replace(":VenueName:", removeArtistFromEventService.data.VenueName);
                //    }

                //    var notificationModel = new PushNotificationModel
                //    {
                //        UserIds = artistIds,
                //        NotificaitonTitle = notifTitle,
                //        NotificaitonBody = notifBody
                //    };
                //    // send push notifications to all available artists 
                //    var sendPushNotificationService = await NatClient.ReadAsync<EventViewModel>(NatClient.Method.POST, NatClient.Service.NotificationService, "SendPushNotifications", requestBody: notificationModel);
                //}

                return true;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public Decimal CalculateHoursPerWeek(List<ArtistAvailabilityModel> obj)
        {
            Double hours = 0;
            foreach (ArtistAvailabilityModel artistAvailabilityModel in obj)
            {
                foreach (ArtistAvailabilitySlotModel artistAvailabilitySlotModel in artistAvailabilityModel.NatPlsAvailabilitySlot)
                {

                    if (artistAvailabilitySlotModel.StartTime > artistAvailabilitySlotModel.EndTime)
                    {
                        artistAvailabilitySlotModel.EndTime = artistAvailabilitySlotModel.EndTime.AddDays(1);

                    }

                    TimeSpan ts = artistAvailabilitySlotModel.EndTime - artistAvailabilitySlotModel.StartTime;

                    hours = hours + ts.TotalHours;
                }
            }
            return Convert.ToDecimal(hours);
        }
        public Decimal CalculateHoursPerWeekForEntity(ICollection<ArtistAvailabilityRequestModel> obj)
        {
            Double hours = 0;
            foreach (ArtistAvailabilityRequestModel artistAvailabilityModel in obj)
            {
                foreach (ArtistAvailabilitySlotRequestModel artistAvailabilitySlotModel in artistAvailabilityModel.AvailabilitySlot)
                {

                    if (artistAvailabilitySlotModel.StartTime > artistAvailabilitySlotModel.EndTime)
                    {
                        artistAvailabilitySlotModel.EndTime = artistAvailabilitySlotModel.EndTime.AddDays(1);

                    }

                    TimeSpan ts = artistAvailabilitySlotModel.EndTime - artistAvailabilitySlotModel.StartTime;
                    hours = hours + ts.TotalHours;
                }
            }
            return Convert.ToDecimal(hours);
        }



        public async Task<ArtistRatingLogModel> AddArtistRatingAsync(ArtistRatingLogModel obj)
        {
            try
            {

                obj.ObjectState = ObjectState.Added;
                obj.ReviewDate = DateTime.UtcNow;
                //NAT_AS_Artist_Rating_Log rating = new ArtistRatingLogModel().ToDataModel(obj);
                uow.Repository<NAT_AS_Artist_Rating_Log>().Insert(obj.ToDataModel(obj));


                int artistid = obj.ArtistId.Value;
                NAT_AS_Artist_Rating artistrModel = await uow.RepositoryAsync<NAT_AS_Artist_Rating>().GetArtistRatingRecordAsync(artistid);
                ArtistRatingModel data1 = new ArtistRatingModel().FromDataModel(artistrModel);
                if (data1 != null)
                {
                    AverageCalculator ab = new AverageCalculator();
                    data1.AverageRatingValue = ab.NewRating(data1.AverageRatingValue, data1.NumberOfRatings.Value, obj.RatingValue);
                    data1.NumberOfRatings = data1.NumberOfRatings + 1;
                    data1.ObjectState = ObjectState.Modified;
                    //NAT_AS_Artist_Rating rat = new ArtistRatingModel().ToDataModel(data1);
                    uow.RepositoryAsync<NAT_AS_Artist_Rating>().Update(data1.ToDataModel(data1));
                }
                else
                {
                    ArtistRatingModel data2 = new ArtistRatingModel();
                    data2.ArtistId = artistid;
                    data2.AverageRatingValue = obj.RatingValue;
                    data2.NumberOfRatings = 1;
                    data2.ActiveFlag = true;
                    data2.ObjectState = ObjectState.Added;
                    uow.Repository<NAT_AS_Artist_Rating>().Insert(data2.ToDataModel(data2));


                }
                await uow.SaveChangesAsync();
                return obj;
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }



        }


        public async Task<IEnumerable<ArtistRatingLogModel>> GetArtistRatingLog(int id, int requiredRecords)
        {
            int custId;
            IEnumerable<NAT_AS_Artist_Rating_Log> artistRatingLog = await uow.RepositoryAsync<NAT_AS_Artist_Rating_Log>().GetArtistRatingLogByArtistId(id, requiredRecords);
            IEnumerable<ArtistRatingLogModel> artistRatingLogModel = new ArtistRatingLogModel().FromDataModelList(artistRatingLog);
            var lookupList = await NatClient.ReadAsync<IEnumerable<ViewModels.CustomerLovViewModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customerslov");


            if (lookupList.status.IsSuccessStatusCode)
            {
                var customerlov = lookupList.data.ToDictionary(item => item.Id, item => item);

                artistRatingLogModel = artistRatingLogModel.Select((item) =>
                {
                    item.CustomerName = customerlov[custId = item.CustomerId ?? default(int)].Value;
                    item.CustomerProfileImageUrl = customerlov[custId = item.CustomerId ?? default(int)].Image;
                    return item;
                }).ToList();
            }
            return artistRatingLogModel;

        }
        public async Task<ArtistRatingModel> getaveragerating(int id)
        {

            NAT_AS_Artist_Rating rat = await uow.RepositoryAsync<NAT_AS_Artist_Rating>().GetArtistRatingRecordAsync(id);
            ArtistRatingModel data1 = new ArtistRatingModel().FromDataModel(rat);
            return data1;

        }

        public async Task<List<ArtistModel>> FindReplacementArtist(FindReplacementArtistQueryModel model)
        {
            try
            {
                var replacementArtistList = new List<ArtistModel>();
                // Skipping the preferred artists

                var locationCode = this.uow.Repository<NAT_AS_Artist_Location_Mapping>()
                    .Queryable()
                    .Where(x => x.Artist_ID == model.ArtistId)
                    .Select(x => x.Location_Code)
                    .FirstOrDefault();

                // Find all artists with this location code
                var otherArtists = this.uow.RepositoryAsync<NAT_AS_Artist_Location_Mapping>()
                    .Queryable()
                    .Where(x => x.Location_Code == locationCode && x.NAT_AS_Artist.Artist_ID != model.ArtistId)
                    .Select(x => x.NAT_AS_Artist.Artist_ID)
                    .ToList();

                // Fetch the artist with the rating
                var artistThresholdResp = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/ArtistThresholdRating");

                if (!artistThresholdResp.status.IsSuccessStatusCode || artistThresholdResp.data == null)
                    throw new Exception("Unable to access configuration");

                var artistThreshold = Convert.ToInt32(artistThresholdResp.data.Value);

                // Find the market of this event

                // Artist with ThresholdRating
                List<NAT_AS_Artist> artistsWithRatingModel = this.uow.RepositoryAsync<NAT_AS_Artist_Rating>()
                    .Queryable()
                    .Where(x => x.Average_Rating_Value >= artistThreshold && otherArtists.Contains(x.NAT_AS_Artist.Artist_ID)) // And also we need to see if those artists are in the preferred venue
                    .Include(x => x.NAT_AS_Artist)
                    .Select(x => x.NAT_AS_Artist)
                    .Include(x => x.NAT_AS_Artist_Location_Mapping)
                    .ToList();

                var artistsWithRating = new ArtistModel().FromDataModelList(artistsWithRatingModel);

                //prepare plannerIds in advance for the next step
                var plannerIds = new List<int>();
                foreach (var artist in artistsWithRating)
                {
                    foreach (var locationMapping in artist.NatAsArtistLocationMapping)
                    {
                        if (locationMapping.LocationCode == model.LocationCode && locationMapping.PlannerId != null)
                        {
                            plannerIds.Add(locationMapping.PlannerId ?? default);
                        }
                    }
                }

                var plannerSearchResp = await NatClient.ReadAsync<List<int>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/searchchange", requestBody: new
                {
                    StartTime = model.EventStartTime,
                    EndTime = model.EventEndTime,
                    ReferenceType = 2,
                    PlannerIds = plannerIds
                });

                if (!plannerSearchResp.status.IsSuccessStatusCode)
                    throw new Exception("Planner search api error");

                var filteredPlannerIds = plannerSearchResp.data;

                foreach (int plannerId in filteredPlannerIds)
                {
                    foreach (var artist in artistsWithRating)
                    {
                        foreach (var locationMapping in artist.NatAsArtistLocationMapping)
                        {
                            if (locationMapping.PlannerId == plannerId)
                            {
                                replacementArtistList.Add(artist);
                            }
                        }
                    }
                }
                return replacementArtistList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<ArtistLocationCollection> GetArtistsByLocation(List<string> locationCodes, Nat.Core.Authentication.Auth.UserModel userModel)
        {
            List<dynamic> artists = new List<dynamic>();
            ArtistLocationCollection collection = new ArtistLocationCollection();
            try
            {
                if (locationCodes != null && locationCodes.Count > 0)
                {
                    foreach (var locationCode in locationCodes)
                    {
                        ArtistLocationModel model = new ArtistLocationModel();
                        if (userModel.ReferenceTypeLKP == "admin")
                        {
                            var rolechecker = 0;
                            foreach (Auth.RoleModel Roles in userModel.Roles)
                            {
                                if (Roles.RoleName == "Artist Manager")
                                {
                                    rolechecker = 1;
                                }
                            }

                            if (rolechecker == 1)
                            {
                                var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + userModel.UserId);
                                var ArtistList = userlist.data.Select(x => x.ReferenceId);
                                if (ArtistList != null)
                                {

                                    artists = this.uow.Repository<NAT_AS_Artist_Location_Mapping>()
                                          .Queryable()
                                          .Where(x => ArtistList.Contains(x.Artist_ID))
                                          .Include(x => x.NAT_AS_Artist)
                                          .Where(x => x.Location_Code == locationCode)
                                          .Select(x => new
                                          {
                                              Name = x.NAT_AS_Artist.Artist_First_Name + " " + x.NAT_AS_Artist.Artist_Last_Name,
                                              Id = x.NAT_AS_Artist.Artist_ID,
                                              LocationCode = x.NAT_AS_Artist.Location_Code
                                          })
                                          .ToList<dynamic>();
                                }
                                else
                                {
                                    artists = new List<dynamic>();

                                }

                            }
                            else
                            {
                                artists = this.uow.Repository<NAT_AS_Artist_Location_Mapping>()
                                .Queryable()
                                .Include(x => x.NAT_AS_Artist)
                                .Where(x => x.Location_Code == locationCode)
                                .Select(x => new
                                {
                                    Name = x.NAT_AS_Artist.Artist_First_Name + " " + x.NAT_AS_Artist.Artist_Last_Name,
                                    Id = x.NAT_AS_Artist.Artist_ID,
                                    LocationCode = x.NAT_AS_Artist.Location_Code
                                })
                                .ToList<dynamic>();
                            }

                        }
                        if (userModel.ReferenceTypeLKP == "artist")
                        {
                            artists = this.uow.Repository<NAT_AS_Artist_Location_Mapping>()
                                .Queryable()
                                .Include(x => x.NAT_AS_Artist)
                                .Where(x => x.Artist_ID == userModel.ReferenceId.Value)
                                .Select(x => new
                                {
                                    Name = x.NAT_AS_Artist.Artist_First_Name + " " + x.NAT_AS_Artist.Artist_Last_Name,
                                    Id = x.NAT_AS_Artist.Artist_ID,
                                    LocationCode = x.NAT_AS_Artist.Location_Code
                                })
                                .ToList<dynamic>();
                        }

                        model.LocationCode = locationCode;
                        model.Artists = artists;

                        collection.ArtistByLocation.Add(model);
                        collection.Artists.AddRange(model.Artists);
                        collection.Artists = collection.Artists.Distinct().ToList();
                        collection.ArtistsByMarket.Add(locationCode, artists);
                    }
                }
                return collection;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<Boolean> CheckForDuplicateStageName(List<string> locationCodes, string stagename)
        {
            List<dynamic> artists = new List<dynamic>();
            List<dynamic> final = new List<dynamic>();
            foreach (var locationCode in locationCodes)
            {
                artists = this.uow.Repository<NAT_AS_Artist_Location_Mapping>()
                .Queryable()
                .Include(x => x.NAT_AS_Artist)
                .Where(x => x.Location_Code == locationCode)
                .Select(x => new
                {
                    StageName = x.NAT_AS_Artist.Stage_Name,
                    Id = x.NAT_AS_Artist.Artist_ID,
                    LocationCode = x.NAT_AS_Artist.Location_Code
                })
                .ToList<dynamic>();

                final.AddRange(artists);   
            }

            var artist = final.FirstOrDefault(a => a.StageName != null && a.StageName.ToUpper() == stagename.ToUpper());
            return artist != null ? true : false;

        }
        

        public void UpdateArtistCreditLimit(Dictionary<int, decimal> artistDictionary)
        {
            try
            {
                if (artistDictionary != null && artistDictionary.Count > 0)
                {
                    foreach (var key in artistDictionary.Keys)
                    {
                        var creditLimit = artistDictionary[key];

                        var artist = uow.Repository<NAT_AS_Artist>()
                            .Queryable()
                            .Where(x => x.Artist_ID == key)
                            .FirstOrDefault();

                        artist.Available_Credit += creditLimit;

                        this.uow.Repository<NAT_AS_Artist>().Update(artist);
                    }
                    this.uow.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}