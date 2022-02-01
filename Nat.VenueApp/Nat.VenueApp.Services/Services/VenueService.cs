using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Nat.VenueApp.Services.ServiceModels;
using Nat.VenueApp.Models.EFModel;
using Nat.VenueApp.Models.Repositories;

using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Common.Constants;

using TLX.CloudCore.Patterns.Repository.Infrastructure;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;
using Nat.Core.ServiceClient;
using Nat.Core.Lookup;
using Nat.VenueApp.Services.ServiceModels.VenueRequest;
using Newtonsoft.Json;
using Nat.Core.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nat.VenueApp.Services.ViewModels;
using System.Data.Entity;
using CommonMethods;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.Authentication;
using Nat.VenueApp.Services.ServiceModels.ViewModels;
using TLX.CloudCore.KendoX;

namespace Nat.VenueApp.Services
{
    public class VenueService : BaseService<VenueModel, NAT_VS_Venue>
    {
        private static VenueService _service;
        public static VenueService GetInstance(NatLogger logger)
        {
            // if (_service == null)
            //   {
            _service = new VenueService();
            //  }
            _service.SetLogger(logger);
            return _service;
        }

        private VenueService() : base()
        {

        }

        /// <summary>
        /// This method return list of all Venues
        /// </summary>
        /// <returns>Collection of Venue service model<returns>
        public IEnumerable<VenueModel> GetAllVenue()
        {
            using (logger.BeginServiceScope("Get All Venue"))
            {
                try
                {
                    IEnumerable<VenueModel> data = null;
                    IEnumerable<NAT_VS_Venue> VenueModel = uow.RepositoryAsync<NAT_VS_Venue>().GetAllVenue();
                    if (VenueModel != null)
                    {
                        data = new VenueModel().FromDataModelList(VenueModel);
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

        /// <summary>
        /// This method return list of all Venues for Kendo grid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DataSourceResult> GetAllVenueListAsync(DataSourceRequest request,Auth.UserModel userModel)
        {
            try
            {
                DataSourceResult data;
                var rolechecker = 0;
                if (userModel != null)
                    foreach (Nat.Core.Authentication.Auth.RoleModel Roles in userModel.Roles)
                    {
                        if (Roles.RoleCode == "VCP")
                        {
                            rolechecker = 3;
                        }
                    }
                IEnumerable<VenueModel> venuelistModel1 = null;
                List<VenueModel> venuelistModel = new List<VenueModel>(); ;
                
                if (rolechecker == 3)
                {
                    data = uow.RepositoryAsync<NAT_VS_Venue>().Queryable()
                    .Include(x => x.NAT_VS_Venue_Image)
                    .Include(x => x.NAT_VS_Venue_Contact_Person)
                    .Include(x => x.NAT_VS_Venue_Address)
                    .Include(x => x.NAT_VS_Venue_Facility)
                    .Include(x => x.NAT_VS_Venue_Rating)
                    .Include(x => x.NAT_VS_Venue_Hall).Where(x=>x.Venue_ID == userModel.ReferenceId).ToDataSourceResult<NAT_VS_Venue, VenueModel>(request);

                    venuelistModel = ((IEnumerable<VenueModel>)data.Data).ToList();

                }
                else
                {
                     data = uow.RepositoryAsync<NAT_VS_Venue>().Queryable()
                    .Include(x => x.NAT_VS_Venue_Image)
                    .Include(x => x.NAT_VS_Venue_Contact_Person)
                    .Include(x => x.NAT_VS_Venue_Address)
                    .Include(x => x.NAT_VS_Venue_Facility)
                    .Include(x => x.NAT_VS_Venue_Rating)
                    .Include(x => x.NAT_VS_Venue_Hall).ToDataSourceResult<NAT_VS_Venue,VenueModel>(request);

                    venuelistModel = ((IEnumerable<VenueModel>)data.Data).ToList();
                }
                var locationList = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
                var marketList = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location/Type/CITY");
                if (marketList.status.IsSuccessStatusCode)
                {

                    foreach (VenueModel venue in venuelistModel)
                    {
                        var marketchecker = false;
                        marketchecker = marketList.data.Where(x => x.LocationShortCode == venue.LocationCode).FirstOrDefault() != null ? true : false;
                        if (marketchecker)
                        {
                            Dictionary<string, LocationViewModel> locationDictionary = marketList.data.ToDictionary(item => item.LocationShortCode, item => item);

                            venue.ParentLocationCode = venue.LocationCode;
                            venue.ParentLocationName = venue.LocationCode != null ? locationDictionary[venue.ParentLocationCode].LocationName : null;
                           // venue.LocationCode = null;
                        }
                        else
                        {
                            if (locationList.status.IsSuccessStatusCode)
                            {
                                Dictionary<string, LocationViewModel> locationDictionary = locationList.data.ToDictionary(item => item.LocationShortCode, item => item);

                                venue.LocationName = venue.LocationCode != null ? locationDictionary[venue.LocationCode].LocationName : null;
                                venue.ParentLocationCode = venue.LocationCode != null ? locationDictionary[venue.LocationCode].ParentLocationCode : null;
                                venue.ParentLocationName = venue.LocationCode != null ? locationDictionary[venue.ParentLocationCode].LocationName : null;
                            }

                        }

                        //venuelistModel.Add(venue);
                    }
                }
                List<long> venueIdList = new List<long>();
                foreach (VenueModel item in venuelistModel)
                    venueIdList.Add(item.VenueId);
               // var result = venuelistModel.ToDataSourceResult(request);
                var venueeventdata = uow.RepositoryAsync<NAT_VS_Venue_Event>().Queryable()
                    .Where(x=> venueIdList.Contains(x.Venue_ID))
                                        .GroupBy(i => i.Venue_ID)                                        
                                        .Select(g => new
                                        {
                                            VenueId = g.Max(x => x.Venue_ID),
                                            Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0,
                                            LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time),
                                            UpcommingEvents = (int?)g.Count(x => x.Start_Time > System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0
                                        })
                                        .ToDictionary(item => item.VenueId, item => item);
                if (venueeventdata != null)
                {
                    var vdata = (venuelistModel);
                    foreach(var item in vdata)
                    {
                        item.Eventsheld = venueeventdata.ContainsKey(item.VenueId) ? venueeventdata[item.VenueId].Eventsheld : 0;
                        item.LastEventDate = venueeventdata.ContainsKey(item.VenueId) ? venueeventdata[item.VenueId].LastEventDate : null;
                        item.UpcommingEvents = venueeventdata.ContainsKey(item.VenueId) ? venueeventdata[item.VenueId].UpcommingEvents : 0;
                    }

                
                    data.Data = vdata;
                }

                var venueCategoryLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_CATEGORY");
                foreach (VenueModel venue in data.Data)
                {
                    venue.VenueCategoryLKPValue = (venue.VenueCategoryLKPId != null && venueeventdata.ContainsKey(venue.VenueId))? venueCategoryLookup[venue.VenueCategoryLKPId.ToString()].VisibleValue:null;
                    
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<DataSourceResult> GetAllVenueViewListAsync(DataSourceRequest request, Auth.UserModel userModel)
        {
            try
            {
                int i = 0;



                foreach (var filter in request.Filters)
                {
                    var descriptor = filter as FilterDescriptor;
                    if (descriptor != null && descriptor.Member == "ParentLocationCode")
                    {
                        CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
                        compositeFilter.FilterDescriptors.Add(new FilterDescriptor("ParentLocationCode", FilterOperator.StartsWith, descriptor.Value));
                        compositeFilter.FilterDescriptors.Add(new FilterDescriptor("LocationCode", FilterOperator.StartsWith, descriptor.Value));
                        compositeFilter.LogicalOperator = FilterCompositionLogicalOperator.Or;

                        request.Filters[i] = compositeFilter;
                        break;
                    }
                    i++;
                }
                DataSourceResult data;
                var rolechecker = 0;
                if (userModel != null)
                    foreach (Nat.Core.Authentication.Auth.RoleModel Roles in userModel.Roles)
                    {
                        if (Roles.RoleCode == "VCP")
                        {
                            rolechecker = 3;
                        }
                    }

                if (rolechecker == 3)
                {
                    data = uow.RepositoryAsync<NAT_Venue_VW>().Queryable().Where(x => x.Venue_ID == userModel.ReferenceId).ToDataSourceResult<NAT_Venue_VW, VenueVWmodel>(request);

                }
                else
                {
                    data = uow.RepositoryAsync<NAT_Venue_VW>().Queryable().ToDataSourceResult<NAT_Venue_VW, VenueVWmodel>(request);
                }

                return data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// This method returns all Venue list 
        /// </summary>
        /// <returns>Venue service object</returns>
        public async Task<IEnumerable<VenueLovModel>> GetVenueLov(Auth.UserModel userModel)
        {
            using (logger.BeginServiceScope("Get Venue Lov"))
            {
                try
                {
                    var rolechecker = 0;
                    if (userModel != null)
                        foreach (Nat.Core.Authentication.Auth.RoleModel Roles in userModel.Roles)
                        {
                            if (Roles.RoleCode == "VCP")
                            {
                                rolechecker = 3;
                            }
                        }
                    if (rolechecker == 3)
                    {
                        Dictionary<int, string> metrocitydictionary = new Dictionary<int, string>();
                        IEnumerable<NAT_VS_Venue> venue = await uow.RepositoryAsync<NAT_VS_Venue>().GetVenueLovForVCP(userModel.ReferenceId);
                        var metroLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_METRO_CITY_AREA");
                        foreach (NAT_VS_Venue v in venue)
                        {
                            string[] metros = { };
                            foreach (NAT_VS_Venue_Metro_City_Mapping m in v.NAT_VS_Venue_Metro_City_Mapping)
                            {
                                metros.Append(metroLookup[m.Metro_City_LKP_ID.ToString()].VisibleValue);
                            }
                            var str = String.Join(",", metros);
                            metrocitydictionary.Add(v.Venue_ID, str);
                        }

                        var locationList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                        Dictionary<string, AddressGeographyViewModel> locationDictionary = locationList.data.ToDictionary(item => item.GeographyShortCode, item => item);

                        List<VenueLovModel> venueLov = venue.Select(x => new VenueLovModel
                        {
                            VenueId = x.Venue_ID,
                            AddressId = x.Address_ID.Value,
                            AddressLine1 = x.NAT_VS_Venue_Address.Address_Line_1,
                            AddressLine2 = x.NAT_VS_Venue_Address.Address_Line_2,
                            PostalCode = x.NAT_VS_Venue_Address.Postal_Zip_Code,
                            Address = x.NAT_VS_Venue_Address.Postal_Zip_Code + ", " + x.NAT_VS_Venue_Address.Address_Line_1 + ", " + x.NAT_VS_Venue_Address.Address_Line_2 + ", " + (x.NAT_VS_Venue_Address.City_Name != null && locationDictionary.Keys.Contains(x.NAT_VS_Venue_Address.City_Name) ? locationDictionary[x.NAT_VS_Venue_Address.City_Name].GeographyDescription : ""),
                            Name = x.Venue_Name,
                            PlannerId = x.Planner_ID,
                            City = (x.NAT_VS_Venue_Address.City_Name != null && locationDictionary.Keys.Contains(x.NAT_VS_Venue_Address.City_Name) ? locationDictionary[x.NAT_VS_Venue_Address.City_Name].GeographyName : ""),
                            Rating = x.NAT_VS_Venue_Rating.Select(y => y.Average_Rating_Value).FirstOrDefault(),
                            VCPContactNumber = x.NAT_VS_Venue_Contact_Person.Count == 0 ? "" : x.NAT_VS_Venue_Contact_Person.FirstOrDefault().Contact_Number,
                            MetroCityArea = metrocitydictionary.Keys.Contains(x.Venue_ID) ? metrocitydictionary[x.Venue_ID] : ""
                        })
                        .OrderBy(x => x.Name)
                        .ToList();

                        return venueLov;
                    }
                    else
                    {
                        //async Task<IEnumerable<VenueLovModel>> GetVenueLovFromDB()
                        //{
                            logger.LogInformation("Fetch id and address of venue");
                            IEnumerable<NAT_VS_Venue> venue = await uow.RepositoryAsync<NAT_VS_Venue>().GetVenueLov();

                            //get metro cities
                            Dictionary<int, string> metrocitydictionary = new Dictionary<int, string>();
                            var metroLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_METRO_CITY_AREA");
                            foreach (NAT_VS_Venue v in venue)
                            {
                                List<string> metros = new List<string>();
                                foreach (NAT_VS_Venue_Metro_City_Mapping m in v.NAT_VS_Venue_Metro_City_Mapping)
                                {
                                    metros.Add(metroLookup[m.Metro_City_LKP_ID.ToString()].VisibleValue);
                                }
                                var str = String.Join(",", metros);
                                metrocitydictionary.Add(v.Venue_ID, str);
                            }

                            var locationList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                            Dictionary<string, AddressGeographyViewModel> locationDictionary = locationList.data.ToDictionary(item => item.GeographyShortCode, item => item);

                            List<VenueLovModel> venueLov = venue.Select(x => new VenueLovModel
                            {
                                VenueId = x.Venue_ID,
                                AddressId = x.Address_ID.Value,
                                AddressLine1 = x.NAT_VS_Venue_Address.Address_Line_1,
                                AddressLine2 = x.NAT_VS_Venue_Address.Address_Line_2,
                                PostalCode = x.NAT_VS_Venue_Address.Postal_Zip_Code,
                                Address = x.NAT_VS_Venue_Address.Postal_Zip_Code + ", " + x.NAT_VS_Venue_Address.Address_Line_1 + ", " + x.NAT_VS_Venue_Address.Address_Line_2 + ", " + (x.NAT_VS_Venue_Address.City_Name != null && locationDictionary.Keys.Contains(x.NAT_VS_Venue_Address.City_Name) ? locationDictionary[x.NAT_VS_Venue_Address.City_Name].GeographyDescription : ""),
                                Name = x.Venue_Name,
                                PlannerId = x.Planner_ID,
                                City = (x.NAT_VS_Venue_Address.City_Name != null && locationDictionary.Keys.Contains(x.NAT_VS_Venue_Address.City_Name) ? locationDictionary[x.NAT_VS_Venue_Address.City_Name].GeographyName : ""),
                                Rating = x.NAT_VS_Venue_Rating.Select(y => y.Average_Rating_Value).FirstOrDefault(),
                                VCPContactNumber = x.NAT_VS_Venue_Contact_Person.Count == 0 ? "" : x.NAT_VS_Venue_Contact_Person.FirstOrDefault().Contact_Number,
                                MetroCityArea = metrocitydictionary.Keys.Contains(x.Venue_ID) ? metrocitydictionary[x.Venue_ID] : ""
                            })
                            .OrderBy(x => x.Name)
                            .ToList();

                            return venueLov;
                        //}

                        //return await Caching.GetObjectFromCacheAsync<IEnumerable<VenueLovModel>>("Venuelov", 5, GetVenueLovFromDB);
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        //CheckVenueandArtistPreferences
        public async Task<List<object>> CheckVenueandArtistPreferences(IEnumerable<VenueArtistPreferenceModel> searchmodel)
        {
            List<int> venueids = new List<int>();
            int artistid = 0;
            List<object> obj = new List<object>();
            foreach (VenueArtistPreferenceModel pref in searchmodel)
            {
                venueids.Add(pref.VenueId);
                artistid = pref.ArtistId;
                
            }
            if(artistid > 0)
            {

                var ArtistData = await NatClient.ReadAsync<ArtistViewModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + artistid);
                if (ArtistData.status.IsSuccessStatusCode && ArtistData.data!=null)
                {
                    List<NAT_VS_Venue> venues = uow.RepositoryAsync<NAT_VS_Venue>().Queryable().Include(x => x.NAT_VS_Venue_Artist_Preference)
                        .Where(x => venueids.Contains(x.Venue_ID)).ToList();

                    foreach (NAT_VS_Venue v in venues)
                    {
                        var temp = new
                        {
                            ArtistId = 0,
                            VenueId = 0,
                            VenuePreference = false,
                            ArtistPreference = false

                        };
                        foreach (NAT_VS_Venue_Artist_Preference pr in v.NAT_VS_Venue_Artist_Preference)
                        {
                            if (pr.Artist_ID == artistid)
                            {
                                temp = new
                                {
                                    ArtistId = pr.Artist_ID,
                                    VenueId = v.Venue_ID,
                                    VenuePreference = true,
                                    ArtistPreference = ArtistData.data.NatAsArtistVenuePreference != null ? ArtistData.data.NatAsArtistVenuePreference.Any(x => x.VenueId == v.Venue_ID) : false

                                };
                            }
                        }
                        if (temp.ArtistId > 0)
                        {

                            var temp1 = new
                            {
                                ArtistId = temp.ArtistId,
                                VenueId = temp.VenueId,
                                VenuePreference = temp.VenuePreference,
                                ArtistPreference = ArtistData.data.NatAsArtistVenuePreference != null? ArtistData.data.NatAsArtistVenuePreference.Any(x => x.VenueId == v.Venue_ID): false
                            };
                            
                            obj.Add(temp1);
                        }
                        else
                        {
                            if(ArtistData.data.NatAsArtistVenuePreference != null && ArtistData.data.NatAsArtistVenuePreference.Any(x => x.VenueId == v.Venue_ID))
                            {
                                var temp1 = new
                                {
                                    ArtistId = artistid,
                                    VenueId = v.Venue_ID,
                                    VenuePreference = false,
                                    ArtistPreference = ArtistData.data.NatAsArtistVenuePreference != null ? ArtistData.data.NatAsArtistVenuePreference.Any(x => x.VenueId == v.Venue_ID) : false
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

        /// <summary>
        /// This method returns searched Venues list 
        /// </summary>
        /// <param name="search">VenueSearchForEventModel Model</param>
        /// <returns></returns>
        public async Task<DataSourceResult> SearchVenueForEvent(VenueSearchForEventModel search)
        {
            IEnumerable<NAT_VS_Venue> venues;

            //getting areas against city
            var locationarea = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location/AllActiveChildren/" + search.LocationCode);
            List<string> Location = new List<string>();
            foreach (var data in locationarea.data) { 
                Location.Add(data.LocationShortCode); 
            }
            Location.Add(search.LocationCode);

            if (search.AvailabilityStartTime != null || search.AvailabilityEndTime != null)
            {
                var plannerSearchResp = await NatClient.ReadAsync<IEnumerable<ViewModels.PlannerViewModel>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/search", requestBody: new
                {
                    StartTime = search.AvailabilityStartTime,
                    EndTime = search.AvailabilityEndTime,
                    ReferenceType = 2
                });
                var plannerIDs = plannerSearchResp.data.Select(i => i.PlannerId).ToList();
                //////////

                venues = await uow.RepositoryAsync<NAT_VS_Venue>().SearchVenueForEvent(plannerIDs, search.MinRating, search.MaxRating, search.SkillLKPId, search.SearchText, Location);

            } else {
                
                venues = await uow.RepositoryAsync<NAT_VS_Venue>().GetVenueByLocations(Location);
                
            }
            

            var venuesSM = new VenueModel().FromDataModelList(venues.Where(ven => ven.Active_Flag).ToList()).ToList();
            var total = venuesSM.Count;
            var paginatedData = venuesSM.AsQueryable().Skip((search.skip != null ? (int) search.skip:0)).Take((search.take != null ? (int)search.take : total)).ToList();

            //populate Event Details
            object venueeventModel = await uow.RepositoryAsync<NAT_VS_Venue_Event>().GetAllEventCalculatedDetails();
            var venueevent = JsonConvert.SerializeObject(venueeventModel);
            IEnumerable<VenueEventViewModel> VenueEventData = JsonConvert.DeserializeObject<IEnumerable<VenueEventViewModel>>(venueevent);

            if (VenueEventData != null)
            {
                Dictionary<int, VenueEventViewModel> venueeventDictionary = VenueEventData.ToDictionary(item => item.VenueId, item => item);

                paginatedData = paginatedData.Select((x) =>
                {
                    x.Eventsheld = venueeventDictionary.ContainsKey(x.VenueId) ? venueeventDictionary[x.VenueId].Eventsheld : 0;
                    x.LastEventDate = venueeventDictionary.ContainsKey(x.VenueId) ? venueeventDictionary[x.VenueId].LastEventDate : null;
                    x.UpcommingEvents = venueeventDictionary.ContainsKey(x.VenueId) ? venueeventDictionary[x.VenueId].UpcommingEvents : 0;

                    return x;
                }).ToList();
            }

            //Adding city name
            var addressList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
            Dictionary<string, AddressGeographyViewModel> locDictionary = addressList.data.ToDictionary(item => item.GeographyShortCode, item => item);
            foreach(var venue in venuesSM)
                venue.NatVsVenueAddress.Address = venue.NatVsVenueAddress.PostalZipCode + ", " + venue.NatVsVenueAddress.AddressLine1 + ", " + venue.NatVsVenueAddress.AddressLine2 + ", " + (locDictionary.Keys.Contains(venue.NatVsVenueAddress.CityName) ? locDictionary[venue.NatVsVenueAddress.CityName].GeographyName : "");

            var locationList = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
            if (locationList.status.IsSuccessStatusCode)
            {
                Dictionary<string, LocationViewModel> locationDictionary = locationList.data.ToDictionary(item => item.LocationShortCode, item => item);
                paginatedData = paginatedData.Select((x) => {
                    x.LocationName = x.LocationCode != null && locationDictionary.ContainsKey(x.LocationCode) ? locationDictionary[x.LocationCode].LocationName : null;
                    return x;
                }).ToList();
            }
            var venueCategoryLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_CATEGORY");
            foreach (VenueModel venue in paginatedData)
            {
                venue.VenueCategoryLKPValue = venue.VenueCategoryLKPId == null ? null : venueCategoryLookup[venue.VenueCategoryLKPId.ToString()].VisibleValue;
                
                //seat.SeatAllocationTypeLKPValue = seatTypeLookup[seat.SeatAllocationTypeLKPId.ToString()].VisibleValue;
                //seat.ObjectState = ObjectState.Modified;
            }

            return new DataSourceResult() { Data = paginatedData.OrderBy(x => x.VenueName), Total = total };
        }

        //check planner id for venue
        public async Task<Boolean> checkVenueForPlannerId(int id)
        {
            using (logger.BeginServiceScope("check Artist for planner id"))
            {
                try
                {
                    NAT_VS_Venue VenueModel = await uow.RepositoryAsync<NAT_VS_Venue>().GetVenueByPlannerIdAsync(id);
                    if (VenueModel != null)
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



        /// <summary>
        /// This method returns Venue with a given Id
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        /// <returns>Venue service model</returns>
        public async Task<VenueModel>  GetByIdVenue(int Id, long? customerId)

        {
            try
            {
                int CusId;
                VenueModel data = null;
                NAT_VS_Venue VenueModel = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Id);
                if (VenueModel != null)
                {
                    data = new VenueModel().FromDataModel(VenueModel);
                    var lookupList = await NatClient.ReadAsync<IEnumerable<ViewModels.CustomerLovViewModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customerslov");
                    if (lookupList.status.IsSuccessStatusCode)
                    {
                        var customerlov = lookupList.data.ToDictionary(item => item.Id, item => item.Value);
                        
                        Parallel.ForEach(data.NatVsVenueRatingLog, (item) =>
                        {
                            item.CustomerName = customerlov[CusId = item.CustomerId ?? default(int)];
                        });

                    }

                    var venueeventModel = await uow.RepositoryAsync<NAT_VS_Venue_Event>().GetAllEventCalculatedDetailsByVenueId(Id);
                    var venueevent = JsonConvert.SerializeObject(venueeventModel);
                    IEnumerable<VenueEventViewModel> VenueEventData = JsonConvert.DeserializeObject<IEnumerable<VenueEventViewModel>>(venueevent);

                    if (VenueEventData != null)
                    {
                        Dictionary<int, VenueEventViewModel> venueeventDictionary = VenueEventData.ToDictionary(item => item.VenueId, item => item);
                        data.Eventsheld = venueeventDictionary.ContainsKey(data.VenueId) ? venueeventDictionary[data.VenueId].Eventsheld : 0;
                        data.LastEventDate = venueeventDictionary.ContainsKey(data.VenueId) ? venueeventDictionary[data.VenueId].LastEventDate : null;
                        data.UpcommingEvents = venueeventDictionary.ContainsKey(data.VenueId) ? venueeventDictionary[data.VenueId].UpcommingEvents : 0;

                    }



                    var plannerResp = await NatClient.ReadAsync<IEnumerable<ViewModels.AvailabilityViewModel>>(NatClient.Method.GET, NatClient.Service.PlannerService, "Availability/Planner/" + VenueModel.Planner_ID);
                    if (plannerResp != null)
                    {
                        if (plannerResp.status.IsSuccessStatusCode)
                        {
                            List<VenueAvailabilityModel> mainAvailability = new List<VenueAvailabilityModel>();
                            foreach (ViewModels.AvailabilityViewModel venueAvailabilityModel in plannerResp.data)
                            {
                                var availability = new VenueAvailabilityModel();
                                availability.DayOfWeekLKPId = venueAvailabilityModel.DayOfWeekLKPId;
                                availability.PlannerId = venueAvailabilityModel.PlannerId;
                                availability.AvailabilityId = venueAvailabilityModel.AvailabilityId;
                                availability.AvailabilitySlot = new List<VenueAvailabilitySlotModel>();
                                foreach (AvailabilitySlotViewModel venueAvailabilitySlotModel in venueAvailabilityModel.NatPlsAvailabilitySlot)
                                {
                                    var availabilitySlot = new VenueAvailabilitySlotModel();
                                    availabilitySlot.StartTime = venueAvailabilitySlotModel.StartTime;
                                    availabilitySlot.EndTime = venueAvailabilitySlotModel.EndTime;
                                    availability.AvailabilitySlot.Add(availabilitySlot);
                                }

                                //insert into whole availability then run post
                                mainAvailability.Add(availability);
                            }

                            data.Availability = mainAvailability;
                        }
                    }
                    var preferencesResp = await NatClient.ReadAsync<List<NotificationPreferenceModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "getpref/"+ Id);
                    if (preferencesResp !=null && preferencesResp.status.IsSuccessStatusCode)
                    {
                        data.NotificationPreferences = preferencesResp.data;
                    }
                   

                    //Adding city name
                    var locationList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                    var locationlist2 = locationList.data.Where(x => x.GeographyShortCode != null).ToList();
                    Dictionary<string, AddressGeographyViewModel> locationDictionary = locationlist2.ToDictionary(item => item.GeographyShortCode, item => item);
                    data.NatVsVenueAddress.Address = data.NatVsVenueAddress.PostalZipCode + ", " + data.NatVsVenueAddress.AddressLine1 + ", " + data.NatVsVenueAddress.AddressLine2 + ", " + (locationDictionary.Keys.Contains(data.NatVsVenueAddress.CityName) ? locationDictionary[data.NatVsVenueAddress.CityName].GeographyName : "");

                    //Total Number Of Events   
                    data.EventHosted = uow.Repository<NAT_VS_Venue_Event>().GetTotalEvents(Id);

                    //Category Type Of Venue

                    //var skillLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_CATEGORY");
                    //data.VenueCategoryLKPValue = skillLookup.Keys.Contains(data.VenueCategoryLKPId) ? skillLookup[data.VenueCategoryLKPId].VisibleValue : "";


                    if (customerId == null)
                    {
                        data.FollowStatus = false;
                        return data;
                    }
                    else
                    {
                        data.FollowStatus = false;

                        var FollowingVenues = await NatClient.ReadAsync<IEnumerable<CustomerFollowingModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customer/" + Convert.ToString(customerId) + "/" + Constants.ReferenceType["VENUE"] + "/following");
               
                        if (FollowingVenues != null)
                        {
                            foreach (CustomerFollowingModel followedvenue in FollowingVenues.data)
                            {
                                if (data.VenueId == followedvenue.ReferenceId)
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
        /// This method returns Seating Plan with a given Id
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        /// <returns>Venue service model</returns>
        public async Task<List<VenueSeatingPlanModel>> GetSeatingById(int venueHallId)

        {
            try
            {
                List<VenueSeatingPlanModel> seatingPlans = null;
                List<NAT_VS_Venue_Seating_Plan> VenueSeatingModel = await uow.RepositoryAsync<NAT_VS_Venue_Seating_Plan>().GetSeatingPlans(venueHallId);
                if (VenueSeatingModel != null)
                {
                    seatingPlans = new VenueSeatingPlanModel().FromDataModelList(VenueSeatingModel).ToList();
                    var seatTypeLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("SEAT_TYPE");
                    foreach(VenueSeatingPlanModel seatingPlan in seatingPlans)
                    foreach (VenueSeatModel seat in seatingPlan.NatVsVenueSeat)
                    {
                        seat.SeatTypeLKPValue = seatTypeLookup.ContainsKey(seat.SeatTypeLKPId.ToString()) ? seatTypeLookup[seat.SeatTypeLKPId.ToString()].VisibleValue : null;
                    }
                    return seatingPlans;
                }
                else
                {
                    throw new Exception("Seating Plan Not found!");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of Venue
        /// </summary>
        /// <param name="servicemodel">Service Venue Model</param>
        /// <returns>Venue ID generated for Venue</returns>
        public async Task<string> CreateVenue(VenueModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    //Venue Contact Person Info
                    if (servicemodel.NatVsVenueContactPerson != null)
                    {
                        foreach (ServiceModels.VenueContactPersonModel VenueContactPerson in servicemodel.NatVsVenueContactPerson)
                        {
                            VenueContactPerson.CreatedBy = servicemodel.CreatedBy;
                            VenueContactPerson.ActiveFlag = true;
                            VenueContactPerson.ObjectState = ObjectState.Added;
                        }
                    }
                    ////Venue Facility Info
                    if (servicemodel.NatVsVenueFacility != null)
                    {
                        foreach (VenueFacilityModel VenueFacility in servicemodel.NatVsVenueFacility)
                        {
                            VenueFacility.CreatedBy = servicemodel.CreatedBy;
                            VenueFacility.ObjectState = ObjectState.Added;
                        }
                    }
                    //////Venue Hall Info
                    if (servicemodel.NatVsVenueHall != null)
                    {
                    foreach (VenueHallModel VenueHall in servicemodel.NatVsVenueHall)
                    {
                            VenueHall.CreatedBy = servicemodel.CreatedBy;
                            VenueHall.ActiveFlag = true;
                            VenueHall.ObjectState = ObjectState.Added;
                        }
                    }
                    if (servicemodel.NotificationPreferences != null)
                    {
                        var preferencesResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "addpref", requestBody: servicemodel.NotificationPreferences);

                        if (!preferencesResp.status.IsSuccessStatusCode)
                        {
                            throw new Exception("Preferences could not be created");
                        }
                    }
                   

                    ////Venue Image Info
                    if (servicemodel.NatVsVenueImage != null)
                    {
                        foreach (VenueImageModel VenueImage in servicemodel.NatVsVenueImage)
                        {
                            VenueImage.CreatedBy = servicemodel.CreatedBy;
                            VenueImage.ObjectState = ObjectState.Added;
                        }
                    }
                    //// NAT_VS_Venue_Rating
                    if (servicemodel.NatVsVenueRating != null)
                    {
                        foreach (VenueRatingModel VenueRating in servicemodel.NatVsVenueRating)
                        {
                            VenueRating.AverageRatingValue = 0;
                            VenueRating.NumberOfRatings = 0;
                            VenueRating.ActiveFlag = true;
                            VenueRating.CreatedBy = servicemodel.CreatedBy;
                            VenueRating.ObjectState = ObjectState.Added;
                        }
                    }
                    //Saving Venue Information
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().VenueId);
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
        /// Update: Method for Updation of Venue record
        /// </summary>
        /// <param name="servicemodel">Service VenueModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<VenueModel> UpdateVenueAsync(VenueModel servicemodel)
        {
            try
            {
                if (servicemodel.VenueId != 0 || servicemodel.VenueId > 0)
                {
                    //Update Venue Information
                    servicemodel.ObjectState = ObjectState.Modified;
                    //servicemodel.ActiveFlag = true;

                    //Venue Contact Person Info
                    if (servicemodel.NatVsVenueContactPerson != null)
                    {
                        foreach (ServiceModels.VenueContactPersonModel VenueContactPerson in servicemodel.NatVsVenueContactPerson)
                        {
                            if (VenueContactPerson.VenueContactPersonId > 0)
                            {
                                VenueContactPerson.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                VenueContactPerson.ObjectState = ObjectState.Modified;

                                if (VenueContactPerson.PrimaryVCP == true)
                                {
                                  if (servicemodel.PasswordChanged == true)
                                    {
                                   
                                    var changePasswordModel = new ViewModels.ChangePasswordViewModel()
                                    {
                                        Username = VenueContactPerson.Email,
                                        NewPassword = servicemodel.Password,
                                    };

                                    var userResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/AdminChangeUserPassword", requestBody: changePasswordModel);
                                    if (!userResp.status.IsSuccessStatusCode && userResp.data == null) { throw new Exception("Error in Updating password"); }
                                  }
                                }
                            }
                            else if (VenueContactPerson.VenueContactPersonId < 0)
                            {
                                VenueContactPerson.VenueContactPersonId *= -1;
                                VenueContactPerson.ObjectState = ObjectState.Deleted;
                            }
                            else if (VenueContactPerson.VenueContactPersonId == 0)
                            {
                                VenueContactPerson.ActiveFlag = true;
                                VenueContactPerson.CreatedBy = servicemodel.LastUpdatedBy;
                                VenueContactPerson.ObjectState = ObjectState.Added;
                            }
                        }
                    }


                    //Venue Artist Preference Info
                    if (servicemodel.NatVsVenueArtistPreference != null)
                    {
                        foreach (ServiceModels.VenueArtistPreferenceModel VenueContactPerson in servicemodel.NatVsVenueArtistPreference)
                        {
                            if (VenueContactPerson.ArtistPreferenceId > 0)
                            {
                                VenueContactPerson.ObjectState = ObjectState.Modified;
                            }
                            else if (VenueContactPerson.ArtistPreferenceId < 0)
                            {
                                VenueContactPerson.ArtistPreferenceId *= -1;
                                VenueContactPerson.ObjectState = ObjectState.Deleted;
                            }
                            else if (VenueContactPerson.ArtistPreferenceId == 0)
                            {
                                VenueContactPerson.ActiveFlag = true;
                                VenueContactPerson.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    //Venue Metro City Area Info
                    if (servicemodel.NatVsVenueMetroCityMapping != null)
                    {
                        foreach (ServiceModels.VenueMetroCityMappingModel MetroCity in servicemodel.NatVsVenueMetroCityMapping)
                        {
                            if (MetroCity.VenueMetroCityMappingId > 0)
                            {
                                MetroCity.ObjectState = ObjectState.Modified;
                            }
                            else if (MetroCity.VenueMetroCityMappingId < 0)
                            {
                                MetroCity.VenueMetroCityMappingId *= -1;
                                MetroCity.ObjectState = ObjectState.Deleted;
                            }
                            else if (MetroCity.VenueMetroCityMappingId == 0)
                            {
                                MetroCity.ObjectState = ObjectState.Added;
                            }
                        }
                    }
                    if (servicemodel.NATVSVenueBankAccount != null)
                    {
                        foreach (VenueBankAccountModel venueBankAcc in servicemodel.NATVSVenueBankAccount)
                        {
                            if (venueBankAcc.VenueBankAccountID > 0)
                            {
                                venueBankAcc.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                venueBankAcc.ObjectState = ObjectState.Modified;
                            }

                            else if (venueBankAcc.VenueBankAccountID < 0)
                            {
                                venueBankAcc.VenueBankAccountID *= -1;
                                venueBankAcc.ObjectState = ObjectState.Deleted;
                            }
                            else if (venueBankAcc.VenueBankAccountID == 0)
                            {
                                venueBankAcc.ActiveFlag = true;
                                venueBankAcc.CreatedBy = servicemodel.LastUpdatedBy;
                                venueBankAcc.ObjectState = ObjectState.Added;
                            }
                            // Check if the bank account has address specified
                            var branchAddressExists = this.uow.Repository<NAT_VS_Venue_Bank_Account>()
                                .Queryable()
                                .Where(x => x.Venue_ID == servicemodel.VenueId && x.Address_ID != null)
                                .Any();
                            if (branchAddressExists)
                            {
                                if (venueBankAcc.NatVsVenueAddress != null)
                                {
                                    venueBankAcc.NatVsVenueAddress.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                    venueBankAcc.NatVsVenueAddress.ObjectState = ObjectState.Modified;
                                }
                            }
                            else
                            {
                                if (venueBankAcc.NatVsVenueAddress != null)
                                {
                                    venueBankAcc.NatVsVenueAddress.CreatedBy = servicemodel.LastUpdatedBy;
                                    venueBankAcc.NatVsVenueAddress.ObjectState = ObjectState.Added;
                                }
                            }
                        }
                    }

                    IDictionary<int, string> dict = new Dictionary<int, string>();
                    //Venue Facility Info
                    if (servicemodel.NatVsVenueFacility != null)
                    {
                        foreach (VenueFacilityModel VenueFacility in servicemodel.NatVsVenueFacility)
                        {                            
                            if (VenueFacility.VenueFacilityId > 0)
                            {
                                VenueFacility.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                VenueFacility.ObjectState = ObjectState.Modified;
                            }

                            else if (VenueFacility.VenueFacilityId < 0)
                            {
                                VenueFacility.VenueFacilityId *= -1;
                                VenueFacility.ObjectState = ObjectState.Deleted;
                            }
                            else if (VenueFacility.VenueFacilityId == 0)
                            {
                                VenueFacility.CreatedBy = servicemodel.LastUpdatedBy;
                                VenueFacility.ObjectState = ObjectState.Added;
                            }
                        }
                    }


                    //Venue Hall Info
                    if (servicemodel.NatVsVenueHall != null)
                    {
                        foreach (VenueHallModel VenueHall in servicemodel.NatVsVenueHall)
                        {
                            if (VenueHall.VenueHallId > 0)
                            {
                                if (VenueHall.NatVsVenueSeatingPlan != null)
                                {
                                    foreach (VenueSeatingPlanModel seatingPlan in VenueHall.NatVsVenueSeatingPlan)
                                    {
                                        if (seatingPlan.SeatingPlanId == 0)
                                        {
                                            seatingPlan.CreatedBy = servicemodel.LastUpdatedBy;
                                            seatingPlan.ActiveFlag = true;
                                            seatingPlan.ObjectState = ObjectState.Added;
                                        }
                                        else if (seatingPlan.SeatingPlanId > 0)
                                        {
                                            seatingPlan.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                            seatingPlan.ObjectState = ObjectState.Modified;
                                        }
                                        else if (seatingPlan.SeatingPlanId < 0)
                                        {
                                            seatingPlan.SeatingPlanId *= -1;
                                            seatingPlan.ObjectState = ObjectState.Deleted;
                                        }

                                        foreach(VenueSeatModel seat in seatingPlan.NatVsVenueSeat)
                                        {
                                            if (seat.SeatId == 0)
                                            {
                                                seat.CreatedBy = servicemodel.LastUpdatedBy;
                                                seat.ActiveFlag = true;
                                                seat.ObjectState = ObjectState.Added;
                                            }
                                            else if (seat.SeatId > 0)
                                            {
                                                seat.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                                seat.ObjectState = ObjectState.Modified;
                                            }
                                            else if (seat.SeatId < 0)
                                            {
                                                seat.SeatId *= -1;
                                                seat.ObjectState = ObjectState.Deleted;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (VenueHall.VenueHallId < 0)
                            {
                                VenueHall.VenueHallId *= -1;
                                VenueHall.ObjectState = ObjectState.Deleted;
                            }
                            else if (VenueHall.VenueHallId == 0)
                            {
                                VenueHall.CreatedBy = servicemodel.LastUpdatedBy;
                                VenueHall.ActiveFlag = true;
                                VenueHall.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    //Venue Image Info
                    if (servicemodel.NatVsVenueImage != null)
                    {
                        foreach (VenueImageModel VenueImage in servicemodel.NatVsVenueImage)
                        {
                            if (VenueImage.VenueImageId > 0)
                            {
                                VenueImage.LastUpdatedBy = servicemodel.LastUpdatedBy;
                                VenueImage.ObjectState = ObjectState.Modified;
                                // VenueImage.CreatedDate = DateTime.Now;
                            }
                                

                            else if (VenueImage.VenueImageId < 0)
                            {
                                VenueImage.VenueImageId *= -1;
                                VenueImage.ObjectState = ObjectState.Deleted;
                            }
                            else if (VenueImage.VenueImageId == 0)
                            {
                                VenueImage.CreatedBy = servicemodel.LastUpdatedBy;
                                VenueImage.ObjectState = ObjectState.Added;
                            }
                        }
                    }
                    if (servicemodel.NatVsVenueAddress != null)
                    {

                        if (servicemodel.NatVsVenueAddress.AddressId > 0)
                        {
                            // servicemodel.NatVsVenueAddress.WKTCoordinates;
                            servicemodel.NatVsVenueAddress.CreatedDate = DateTime.UtcNow;
                            servicemodel.NatVsVenueAddress.LastUpdatedBy = servicemodel.LastUpdatedBy;
                            servicemodel.NatVsVenueAddress.ObjectState = ObjectState.Modified;
                        }
                        else if (servicemodel.NatVsVenueAddress.AddressId < 0)
                        {
                            servicemodel.NatVsVenueAddress.AddressId *= -1;
                            servicemodel.NatVsVenueAddress.ObjectState = ObjectState.Deleted;
                        }
                        else if (servicemodel.NatVsVenueAddress.AddressId == 0)
                        {
                            servicemodel.NatVsVenueAddress.CreatedBy = servicemodel.LastUpdatedBy;
                            servicemodel.NatVsVenueAddress.ObjectState = ObjectState.Added;
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

                    // NAT_VS_Venue_Rating:
                    servicemodel.NatVsVenueRating = null;


                    var obj = JsonConvert.SerializeObject(servicemodel);

                    base.Update(servicemodel);
                        int updatedRows = uow.SaveChanges();
                    var venueEvents = await uow.RepositoryAsync<NAT_VS_Venue_Event>().GetAllVenuePendingEventsAsync(servicemodel.VenueId);
                    if ((bool)(!servicemodel.ActiveFlag) && (venueEvents.ToList().Count > 0))
                    {
                        var removeVenueFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkVenueUnavailableByVenueID", requestBody: servicemodel.VenueId);
                        if (!removeVenueFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Venue Deactivated. Venue cannot be removed from Upcoming Events");
                    }
                    //servicemodel.HoursPerWeek = Math.Round(CalculateHoursPerWeek(servicemodel.Availability.ToList), 1, MidpointRounding.ToEven);
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
        /// This method activates Venue 
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        public async Task<bool> ActivateVenueAsync(string Id,string username)
        {
            try
            {
                NAT_VS_Venue VenueEf = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(Id));
                if (VenueEf != null)
                {
                    VenueEf.Last_Updated_By = username;
                    UpdateUserModel userUpdate = new UpdateUserModel();
                    userUpdate.ReferenceId = VenueEf.Venue_ID;
                    userUpdate.ReferenceTypeLKP = "vcp";
                    var userActivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/ActivateUserByReference", requestBody: userUpdate);
                    if (userActivate.status.IsSuccessStatusCode && userActivate.data != false)
                    {
                        uow.RepositoryAsync<NAT_VS_Venue>().SetActiveFlag(true, VenueEf);
                        uow.SaveChanges();
                        return true;
                    }
                    else { throw new Exception("User does not exists"); }
                }
                else
                    throw new ApplicationException("Venue does not exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method Activates venue in bulk
        /// </summary>
        /// <param >venue model list</param>
        public async Task<bool> BulkActivateVenueAsync(IEnumerable<VenueModel> servicemodel,string username)
        {
            try
            {
                List<UpdateUserModel> bulkupdateuser = new List<UpdateUserModel>();
                foreach (var data in servicemodel)
                {
                    NAT_VS_Venue VenueEf = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(data.VenueId));
                    if (VenueEf != null)
                    {
                        VenueEf.Last_Updated_By = username;
                        UpdateUserModel updateuser = new UpdateUserModel();
                        updateuser.ReferenceId = VenueEf.Venue_ID;
                        updateuser.ReferenceTypeLKP = "vcp";
                        bulkupdateuser.Add(updateuser);
                    }
                    else
                    {
                        throw new ApplicationException("Venue doesnot exists");
                    }
                }
                var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/BulkActivateUserByReference", requestBody: bulkupdateuser);
                if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                {
                    foreach (var data in servicemodel)
                    {
                        NAT_VS_Venue VenueEf =  uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(data.VenueId));
                        uow.RepositoryAsync<NAT_VS_Venue>().SetActiveFlag(true, VenueEf);
                    }
                    int updatedRows = await uow.SaveChangesAsync();

                    if (updatedRows == 0)
                    {
                        //for revsersal of user all user active flag if error occurs in artist active flag updation
                        foreach (var data in servicemodel)
                        {
                            UpdateUserModel updateuser = new UpdateUserModel();
                            updateuser.ReferenceId = data.VenueId;
                            updateuser.ReferenceTypeLKP = "vcp";
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
        /// This method deactivates Venue 
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        public async Task<bool> DeactivateVenueAsync(string Id,string username)
        {
            try
            {
                NAT_VS_Venue VenueEf = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(Id));
                if (VenueEf != null)
                {
                    VenueEf.Last_Updated_By = username;
                    UpdateUserModel userUpdate = new UpdateUserModel();
                    userUpdate.ReferenceId = VenueEf.Venue_ID;
                    userUpdate.ReferenceTypeLKP = "vcp";                    
                    var userDeactivate = await NatClient.ReadAsyncWithHeaders<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/DeActivateUserByReference", requestBody: userUpdate);
                    if (userDeactivate.status.IsSuccessStatusCode && userDeactivate.data != false)
                    {
                        uow.RepositoryAsync<NAT_VS_Venue>().SetActiveFlag(false, VenueEf);
                        uow.SaveChanges();
                        var removeVenueFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkVenueUnavailableByVenueID",requestBody:VenueEf.Venue_ID);
                        if (!removeVenueFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Venue Deactivated. Venue cannot be removed from Upcoming Events");
                        return true;
                    }
                    else { throw new Exception("User does not exists"); }
                }
                else
                    throw new ApplicationException("Venue does not exists");
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
        public async Task<bool> BulkDeactivateVenueAsync(IEnumerable<VenueModel> servicemodel,string username)
        {
            try
            {
                List<UpdateUserModel> bulkupdateuser = new List<UpdateUserModel>();
                foreach (var data in servicemodel)
                {
                    NAT_VS_Venue VenueEf =  uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(data.VenueId));
                    if (VenueEf != null)
                    {
                        VenueEf.Last_Updated_By = username;
                        UpdateUserModel updateuser = new UpdateUserModel();
                        updateuser.ReferenceId = VenueEf.Venue_ID;
                        updateuser.ReferenceTypeLKP = "vcp";
                        bulkupdateuser.Add(updateuser);

                        var removeVenueFromEventService = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.EventService, "BulkMarkVenueUnavailableByVenueID", requestBody: VenueEf.Venue_ID);
                        if (!removeVenueFromEventService.status.IsSuccessStatusCode)
                            throw new Exception("Venue Deactivated. Venue cannot be removed from Upcoming Events");
                    }
                    else
                    {
                        throw new ApplicationException("Venue doesnot exists");
                    }
                }
                var useractivate = await NatClient.ReadAsync<bool>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/BulkDeActivateUserByReference", requestBody: bulkupdateuser);
                if (useractivate.status.IsSuccessStatusCode && useractivate.data != false)
                {
                    foreach (var data in servicemodel)
                    {
                        NAT_VS_Venue VenueEf =  uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(Convert.ToInt32(data.VenueId));
                        uow.RepositoryAsync<NAT_VS_Venue>().SetActiveFlag(false, VenueEf);
                    }
                    int updatedRows = await uow.SaveChangesAsync();

                    if (updatedRows == 0)
                    {
                        //for revsersal of user all user active flag if error occurs in artist active flag updation
                        foreach (var data in servicemodel)
                        {
                            UpdateUserModel updateuser = new UpdateUserModel();
                            updateuser.ReferenceId = data.VenueId;
                            updateuser.ReferenceTypeLKP = "vcp";
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

        public async Task<VenueRequestModel> SubmitVenueRequestAsync(VenueRequestModel obj)
        {
            try
            {

                //Guid Generator for each Object
                Guid newGuid = Guid.NewGuid();

                //ArtistRequestTableEntityModel created for entry
                VenueRequestTableEntityModel ob = new VenueRequestTableEntityModel(obj.City, newGuid.ToString(), obj);

                var cityList = await NatClient.ReadAsync<IEnumerable<AddressGeographyViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography");
                if (cityList.status.IsSuccessStatusCode)
                {
                    Dictionary<string, AddressGeographyViewModel> cityDictionary = cityList.data.ToDictionary(item => item.GeographyShortCode, item => item);

                    AddressGeographyViewModel city = cityDictionary[obj.City];
                    ob.Location = city.GeographyName;
                }

                ob.HoursPerWeek = Convert.ToString(Math.Round(CalculateHoursPerWeekForEntity(obj.Availability), 1, MidpointRounding.ToEven));

                ob.VenueJsonData = JsonConvert.SerializeObject(obj);

                TableStorage ts = new TableStorage();
                await ts.InsertTableStorage("VenueRequest", ob);

                return obj;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<DataSourceResult> GetVenueRequestAsync(String code, DataSourceRequest dataSourceRequest)
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

            //    for (int i = 1; i < cities.Count; i++)
            //    {
            //        var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, cities.ElementAt(i));
            //        finalFilter = TableQuery.CombineFilters(finalFilter, TableOperators.Or, filter);
            //    }

            TableStorage ts = new TableStorage();
            IEnumerable<VenueRequestTableEntityModel> requests = await ts.RetrieveTableStorage<VenueRequestTableEntityModel>("VenueRequest");
            requests = requests.Select((x) => {
                x.VenueData = JsonConvert.DeserializeObject<VenueRequestModel>(x.VenueJsonData);
                return x;
            }).ToList();
            var dataas= requests.AsQueryable().ToDataSourceResult(dataSourceRequest);
            return requests.AsQueryable().ToDataSourceResult(dataSourceRequest);

            //}
            //else
            //{
            //    return null;
            //}

        }

        public async Task<VenueRequestTableEntityModel> GetVenueRequestByGuidAsync(String code1, String code2)
        {
           
            string finalFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, code1);

            //var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, cities.ElementAt(i));
            //finalFilter = TableQuery.CombineFilters(finalFilter, TableOperators.Or, filter);

            TableStorage ts = new TableStorage();
            IEnumerable<VenueRequestTableEntityModel> requests = await ts.RetrieveTableStorage<VenueRequestTableEntityModel>("VenueRequest", finalFilter);
            requests = requests.Select((x) => {
                x.VenueData = JsonConvert.DeserializeObject<VenueRequestModel>(x.VenueJsonData);
                return x;
            }).ToList();
            return requests.AsQueryable().Where(x => x.RowKey == code2).FirstOrDefault();


        }

        public async Task<VenueModel> ApproveVenueRequestAsync(VenueRequestModel servicemodel)
        {
            VenueModel venueModel = new VenueModel();
            venueModel.VenueName = servicemodel.VenueName;
            venueModel.VenueCategoryLKPId = servicemodel.Category;
            venueModel.MaxSeatingCapacity = servicemodel.Capacity;
            venueModel.LocationCode = servicemodel.LocationCode;
            venueModel.VenueImageUrl = servicemodel.VenueImageUrl;
            venueModel.FacebookProfileUrl = servicemodel.FacebookProfileUrl;
            venueModel.TwitterProfileUrl = servicemodel.TwitterProfileUrl;
            venueModel.InstagramProfileUrl = servicemodel.InstagramProfileUrl;
            venueModel.SpecialInstr = servicemodel.SpecialInstr;
            venueModel.Notes = servicemodel.Notes;
            venueModel.Email = servicemodel.Email;
            venueModel.ContactNumber = servicemodel.ContactNumber;
            venueModel.SIN = servicemodel.SIN;
            venueModel.TaxNumber = servicemodel.TaxNumber;
            venueModel.PaymentCycleLKPId = servicemodel.PaymentCycleLKPId;
            venueModel.ActiveFlag = servicemodel.ActiveFlag;
            venueModel.ObjectState = ObjectState.Added;
            venueModel.CompanyName = servicemodel.CompanyName;
            venueModel.LastUpdatedBy = servicemodel.LastUpdatedBy;
            venueModel.CreatedBy = servicemodel.CreatedBy;
            venueModel.VEventDescription = servicemodel.VEventDescription;
            venueModel.Onboarded = servicemodel.Onboarded;
            venueModel.GoogleMapURL = servicemodel.GoogleMapURL;
            venueModel.VenueURl = servicemodel.VenueURl;
            


            //Venue Address
            VenueAddressModel venueAddressModel = new VenueAddressModel();
            venueAddressModel.AddressLine1 = servicemodel.Address;
            venueAddressModel.AddressLine2 = servicemodel.AddressTwo;
            venueAddressModel.CityName = servicemodel.City;
            venueAddressModel.PostalZipCode = servicemodel.ZipCode;
            venueAddressModel.WKTCoordinates = servicemodel.VenueAddress.WKTCoordinates;
            venueAddressModel.AddressGeographyId = servicemodel.AddressGeographyId;
            venueAddressModel.ObjectState = ObjectState.Added;
            venueModel.NatVsVenueAddress = new VenueAddressModel();
            venueModel.NatVsVenueAddress = venueAddressModel;

            //Contact Person
            venueModel.NatVsVenueContactPerson = new List<VenueContactPersonModel>();
            foreach (VenueContactPersonRequestModel element in servicemodel.ContactPerson)
            {
                VenueContactPersonModel venueContactPersonModel = new VenueContactPersonModel();
                venueContactPersonModel.FirstName = element.FirstName;
                venueContactPersonModel.LastName = element.LastName;
                venueContactPersonModel.Email = element.Email;
                venueContactPersonModel.ProfileImageLink = element.ImagePath;
                venueContactPersonModel.ContactNumber = element.ContactNumber;
                venueContactPersonModel.Designation = element.Designation;
                venueContactPersonModel.PrimaryVCP = element.PrimaryVCP;
                venueContactPersonModel.Greeting = element.Greeting;
                venueContactPersonModel.TextFlag = element.TextFlag;
                venueContactPersonModel.Notes = element.Notes;
                venueContactPersonModel.ObjectState = ObjectState.Added;
                venueModel.NatVsVenueContactPerson.Add(venueContactPersonModel);
            }

            //Metro City Area
            venueModel.NatVsVenueMetroCityMapping = new List<VenueMetroCityMappingModel>();
            if (servicemodel.NatVsVenueMetroCityMapping != null)
            {
                foreach (VenueMetroCityMappingModel element in servicemodel.NatVsVenueMetroCityMapping)
                {
                    VenueMetroCityMappingModel VenueMetroCit = new VenueMetroCityMappingModel();
                    VenueMetroCit.MetroCityLKPId = element.MetroCityLKPId;
                    VenueMetroCit.ObjectState = ObjectState.Added;
                    venueModel.NatVsVenueMetroCityMapping.Add(VenueMetroCit);
                }
            }

            //Artist Preference
            if (servicemodel.NatVsVenueArtistPreference != null && servicemodel.NatVsVenueArtistPreference.Count() > 0 )
            {
                venueModel.NatVsVenueArtistPreference = new List<VenueArtistPreferenceModel>();
                foreach (VenueArtistPreferenceModel element in servicemodel.NatVsVenueArtistPreference)
                {
                    VenueArtistPreferenceModel venueArtistPreferenceModel = new VenueArtistPreferenceModel();
                    venueArtistPreferenceModel.ArtistId = element.ArtistId;
                    venueArtistPreferenceModel.ActiveFlag = true;
                    venueArtistPreferenceModel.ObjectState = ObjectState.Added;
                    venueModel.NatVsVenueArtistPreference.Add(venueArtistPreferenceModel);
                }
            }


            //Venue Images
            if (servicemodel.Images != null && servicemodel.Images.Count() > 0)
            {
                venueModel.NatVsVenueImage = new List<VenueImageModel>();
                foreach (VenueImageRequestModel element in servicemodel.Images)
                {
                    VenueImageModel venueImageModel = new VenueImageModel();
                    venueImageModel.ImagePath = element.ImagePath;
                    venueImageModel.ImageTypeLKPId = element.ImageTypeLKPId;
                    venueImageModel.ObjectState = ObjectState.Added;
                    venueModel.NatVsVenueImage.Add(venueImageModel);
                }
            }

            // Venue Facilities
            if (servicemodel.Facilities != null && servicemodel.Facilities.Count() > 0)
            {
                venueModel.NatVsVenueFacility = new List<VenueFacilityModel>();
                foreach (VenueFacilityRequestModel element in servicemodel.Facilities)
                {
                    VenueFacilityModel venueFacilityModel = new VenueFacilityModel();
                    venueFacilityModel.FacilityLKPId = element.FacilityLKPId;
                    venueFacilityModel.ObjectState = ObjectState.Added;
                    venueModel.NatVsVenueFacility.Add(venueFacilityModel);
                }
            }

            //// Venue Rating Simple Object Creation
            venueModel.NatVsVenueRating = new List<VenueRatingModel>();
            VenueRatingModel venueRatingModel = new VenueRatingModel();
            venueRatingModel.AverageRatingValue = 0;
            venueRatingModel.NumberOfRatings = 0;
            venueRatingModel.ObjectState = ObjectState.Added;
            venueModel.NatVsVenueRating.Add(venueRatingModel);


            //// Venue Hall added
            venueModel.NatVsVenueHall = new List<VenueHallModel>();
            VenueHallModel venueHallModel = new VenueHallModel();
            venueHallModel.NatVsVenueSeatingPlan = new List<VenueSeatingPlanModel>();
            if (servicemodel.SeatingPlans != null)
            {
                foreach (VenueSeatingPlanModel seatingPlan in servicemodel.SeatingPlans)
                {
                    seatingPlan.TotalSeats = seatingPlan.NatVsVenueSeat.Count;
                    seatingPlan.VenueHallId = venueHallModel.VenueHallId;
                    seatingPlan.ActiveFlag = true;
                    seatingPlan.ObjectState = ObjectState.Added;
                    foreach (VenueSeatModel seat in seatingPlan.NatVsVenueSeat)
                    {
                        seat.ObjectState = ObjectState.Added;
                    }
                    venueHallModel.NatVsVenueSeatingPlan.Add(seatingPlan);
                }
            }
            venueHallModel.ObjectState = ObjectState.Added;
            venueModel.NatVsVenueHall.Add(venueHallModel);

            //planner work
            venueModel.PlannerId = servicemodel.PlannerId;
            var plannerResp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.PlannerService, "UpdatePlannerType/" + servicemodel.PlannerId);

            if (servicemodel.AccountNumber != null)
            {

                venueModel.NATVSVenueBankAccount = new List<VenueBankAccountModel>();
                VenueBankAccountModel venueBankAccountModel = new VenueBankAccountModel();
                venueBankAccountModel.BankAccountNumber = servicemodel.AccountNumber;
                venueBankAccountModel.BankLKPID = servicemodel.VenueBankLKPId;
                venueBankAccountModel.BankRoutingNumber = servicemodel.BankRoutingNumber;
                venueBankAccountModel.TransitNumber = servicemodel.TransitNumber;
                venueBankAccountModel.ObjectState = ObjectState.Added;
                
                if (servicemodel.BranchZipCode != null)
                {
                    VenueAddressModel branchAddress = new VenueAddressModel();
                    branchAddress.AddressLine1 = servicemodel.BranchAddress;
                    branchAddress.AddressLine2 = servicemodel.BranchAddressTwo;
                    branchAddress.CityName = servicemodel.BranchCity;
                    branchAddress.PostalZipCode = servicemodel.BranchZipCode;
                    branchAddress.AddressGeographyId = servicemodel.BranchAddressGeographyId;
                    branchAddress.ObjectState = ObjectState.Added;

                    venueBankAccountModel.NatVsVenueAddress = branchAddress;
                }
                venueModel.NATVSVenueBankAccount.Add(venueBankAccountModel);
            }

            if (plannerResp.status.IsSuccessStatusCode)
            {

                var venueDM = venueModel.ToDataModel(venueModel);
                Insert(venueModel);

                VenueContactPersonRequestModel ContactPersonPrimary = servicemodel.ContactPerson.Where(x => x.PrimaryVCP = true).FirstOrDefault();

                var userModel = new ViewModels.UserViewModel()
                {
                    UserId = 0,
                    TenantId = 1,
                    UserName = ContactPersonPrimary.Email,
                    FirstName = ContactPersonPrimary.FirstName,
                    LastName = ContactPersonPrimary.LastName,
                    Email = ContactPersonPrimary.Email,
                    PhoneNumber = ContactPersonPrimary.ContactNumber,
                    ActiveFlag = true,
                    Password = ContactPersonPrimary.Password,
                    RoleCode = Constants.UserRoleCode["VCP"],
                    ReferenceId = 0,
                    UserImageURL = ContactPersonPrimary.ImagePath,
                    ReferenceTypeLKP = Constants.UserReferenceType["VCP"]
                };

                if (servicemodel.LocationCode != null)
                {
                    userModel.NatAusUserLocationMapping = new List<UserLocationMappingViewModel>();
                    
                    var venueLocation = new UserLocationMappingViewModel();
                    venueLocation.LocationCode = servicemodel.LocationCode;
                    venueLocation.ActiveFlag = true;
                    userModel.NatAusUserLocationMapping.Add(venueLocation);
                    
                }

                var userResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Register", requestBody: userModel);
                
                if (userResp.status.IsSuccessStatusCode)
                {
                    var changes = await uow.SaveChangesAsync();
                    venueModel.VenueId = this.Get().VenueId;
                    if (changes > 0)
                    {
                        long venueID = Convert.ToInt64(Get().VenueId);
                        var userReferenceModel = new UserReferenceViewModel()
                        {
                            ReferenceId = venueID,
                            ReferenceTypeLkp = Constants.UserReferenceType["VCP"],
                            UserId = Int64.Parse(userResp.data)
                        };

                        var updateUserRefereceResponse = await NatClient.ReadAsync<UserModel>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Reference", requestBody: userReferenceModel);

                        if (!updateUserRefereceResponse.status.IsSuccessStatusCode)
                        {
                            throw new Exception("An error occured while updating user reference");
                        }

                        if (servicemodel.NotificationPreferences != null)
                        {
                            foreach (NotificationPreferenceModel pref in servicemodel.NotificationPreferences)
                            {
                                pref.UserID = venueID;
                            }
                            var preferencesResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "addPref", requestBody: servicemodel.NotificationPreferences);

                            if (!preferencesResp.status.IsSuccessStatusCode)
                            {
                                throw new Exception("Preferences could not be created");
                            }
                        }

                        if (servicemodel.PartitionKey != null && servicemodel.RowKey != null)
                        {
                            TableStorage ts = new TableStorage();
                            var deletedEntity = await ts.DeleteTableStorage<VenueRequestTableEntityModel>("VenueRequest", servicemodel.PartitionKey, servicemodel.RowKey);
                            await ts.InsertTableStorage("ApprovedVenueRequest", deletedEntity);
                        }
                    }
                }
                else
                {
                    throw new Exception("An error occured while registering user");
                }
            }
            else
            {
                throw new Exception("An error occured while creating planner");
            }
            
            return venueModel;
        }


        public async Task<bool> RejectVenueRequestAsync(VenueRequestModel servicemodel)
        {
           
            if (servicemodel.PartitionKey != null && servicemodel.RowKey != null)
            {
                TableStorage ts = new TableStorage();
                var deletedEntity = await ts.DeleteTableStorage<VenueRequestTableEntityModel>("VenueRequest", servicemodel.PartitionKey, servicemodel.RowKey);
                deletedEntity.RejectionReason = servicemodel.RejectionReason;
                await ts.InsertTableStorage("RejectedVenueRequest", deletedEntity);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UploadVenueSpaceImages(VenueImageModel obj)
        {
            try
            {
                uow.RepositoryAsync<NAT_VS_Venue_Image>().Insert(obj.ToDataModel(obj));
                uow.SaveChanges();

                return true;
            }

            catch (Exception e)
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
                var imgname = await ts.InsertBlobStorage("VenueImagesContainerName", bfile, fileName);

                //returns the name of the img saved in blob
                return Environment.GetEnvironmentVariable("VenueImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("VenueImagesContainerName") + "/" +imgname;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> VenueUpdateAvailabilityAsync(IEnumerable<Nat.VenueApp.Services.ServiceModels.VenueRequest.VenueAvailabilityModel> availabilityModelList, int id)
        {
            var venue = await uow.RepositoryAsync<NAT_VS_Venue>().Queryable().Where(x => x.Venue_ID == id).FirstOrDefaultAsync();

            venue.Hours_Per_Week = Math.Round(CalculateHoursPerWeek(availabilityModelList.ToList()), 1, MidpointRounding.ToEven);

            foreach (var x in availabilityModelList)
            {
                x.NatPlsAvailabilitySlot = x.AvailabilitySlot;
            }

            var availabilityResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.PlannerService, "Availability", requestBody: availabilityModelList);

            if (availabilityResp.status.IsSuccessStatusCode)
            {
                uow.RepositoryAsync<NAT_VS_Venue>().Update(venue);
                uow.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<VenueModel> GetVirtualVenue()
        {
            try
            {
                var venueModel = await uow.RepositoryAsync<NAT_VS_Venue>().GetVirtualVenue();
                return new VenueModel().FromDataModel(venueModel);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// / Create: Method for creation of Venue Event
        /// </summary>
        /// <param name="servicemodel"></param>
        /// <returns></returns>
        public async Task<Boolean> BookVenueForEvent(VenueEventModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    VenueModel data = null;
                    NAT_VS_Venue venuemodel = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(servicemodel.VenueId);
                    if (venuemodel != null)
                    {
                        data = new VenueModel().FromDataModel(venuemodel);
                        servicemodel.PlannerId = Convert.ToInt32(data.PlannerId);
                        //Don't book slot for virtual venue
                        //Virtual venue can host multiple events at the same time
                        if (!venuemodel.Virtual)
                        {
                            var Eventdata = await NatClient.ReadAsync<object>(NatClient.Method.POST, NatClient.Service.PlannerService, "PlannerEvent", requestBody: servicemodel);
                            if (!Eventdata.status.IsSuccessStatusCode || Eventdata.data == null) return false;
                        }

                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;
                        NAT_VS_Venue_Event dataModel = new VenueEventModel().ToDataModel(servicemodel);
                        uow.Repository<NAT_VS_Venue_Event>().Insert(dataModel);
                        await uow.SaveChangesAsync();
                        return true;
                    }
                    else
                    { return false; }
                }
                else
                {
                    return false; }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<Boolean> CancelVenueForEvent(string eventcode)
        {
            try
            {
                if (eventcode != null)
                {
                    VenueEventModel venueeventdata = null;
                    VenueModel venuedata = null;

                    NAT_VS_Venue_Event venueeventModel = await uow.RepositoryAsync<NAT_VS_Venue_Event>().GetVenueEventByEventCodeAsync(eventcode);
                    if (venueeventModel != null)
                    {
                        venueeventdata = new VenueEventModel().FromDataModel(venueeventModel);
                        NAT_VS_Venue venueModel = uow.RepositoryAsync<NAT_VS_Venue>().GetVenueById(venueeventdata.VenueId);
                        if (venueModel != null)
                        {
                            venuedata = new VenueModel().FromDataModel(venueModel);
                            venuedata.PlannerId = venueeventModel.NAT_VS_Venue.Planner_ID;
                            var Eventdata = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.PlannerService, "CancelPlannerEvent/" + venuedata.PlannerId + "/" + eventcode);
                            if (Eventdata.status.IsSuccessStatusCode && Eventdata.data != null)
                            {
                                venueeventModel.Status_LKP_ID = 3;
                                venueeventModel.ObjectState = ObjectState.Modified;
                                //NAT_VS_Venue_Event dataModel = new VenueEventModel().ToDataModel(venueeventdata);
                                uow.Repository<NAT_VS_Venue_Event>().Update(venueeventModel);
                                int updatedRows = uow.SaveChanges();
                                if (updatedRows == 0)
                                {
                                    return false;
                                }
                                return true;
                            }
                            else
                            { return false; }
                        }
                        else
                        { return false; }
                    }
                    else
                    { return false; }
                }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        public Decimal CalculateHoursPerWeek(List<VenueAvailabilityModel> obj)
        {
            Double hours = 0;
            foreach (VenueAvailabilityModel artistAvailabilityModel in obj)
            {
                foreach (VenueAvailabilitySlotModel artistAvailabilitySlotModel in artistAvailabilityModel.NatPlsAvailabilitySlot)
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

        public Decimal CalculateHoursPerWeekForEntity(ICollection<VenueAvailabilityModel> obj)
        {
            Double hours = 0;
            foreach (VenueAvailabilityModel artistAvailabilityModel in obj)
            {
                foreach (VenueAvailabilitySlotModel artistAvailabilitySlotModel in artistAvailabilityModel.AvailabilitySlot)
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

        public async Task<IEnumerable<VenueEventModel>> GetAllVenuePendingEventsAsync(int VenueId)
        {
            try
            {
                IEnumerable<VenueEventModel> data = null;
                IEnumerable<NAT_VS_Venue_Event> venueEventModel = await uow.RepositoryAsync<NAT_VS_Venue_Event>().GetAllVenuePendingEventsAsync(VenueId);
                if (venueEventModel != null)
                {
                    data = new VenueEventModel().FromDataModelList(venueEventModel);
                    return data;
                }
                else
                    throw new ApplicationException("Returns with null object");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<VenueRatingLogModel> AddVenueRatingAsync(VenueRatingLogModel obj)
        {
            try
            {

                obj.ObjectState = ObjectState.Added;
                obj.ReviewDate = DateTime.UtcNow;
                //NAT_AS_Artist_Rating_Log rating = new ArtistRatingLogModel().ToDataModel(obj);
                uow.Repository<NAT_VS_Venue_Rating_Log>().Insert(obj.ToDataModel(obj));


                int venueid = obj.VenueId.Value;
               NAT_VS_Venue_Rating venuerModel = await uow.RepositoryAsync<NAT_VS_Venue_Rating>().GetVenueRatingRecordAsync(venueid);
                VenueRatingModel data1 = new VenueRatingModel().FromDataModel(venuerModel);
                if (data1 != null)
                {
                    AverageCalculator ab = new AverageCalculator();
                    data1.AverageRatingValue = ab.NewRating(data1.AverageRatingValue, data1.NumberOfRatings.Value, obj.RatingValue);
                    data1.NumberOfRatings = data1.NumberOfRatings + 1;
                    data1.ObjectState = ObjectState.Modified;
                    //NAT_AS_Artist_Rating rat = new ArtistRatingModel().ToDataModel(data1);
                    uow.RepositoryAsync<NAT_VS_Venue_Rating>().Update(data1.ToDataModel(data1));
                }
                else
                {
                    VenueRatingModel data2 = new VenueRatingModel();
                    data2.VenueId = venueid;
                    data2.AverageRatingValue = obj.RatingValue;
                    data2.NumberOfRatings = 1;
                    data2.ActiveFlag = true;
                    data2.ObjectState = ObjectState.Added;
                    uow.Repository<NAT_VS_Venue_Rating>().Insert(data2.ToDataModel(data2));


                }
                await uow.SaveChangesAsync();
                return obj;
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }



        }


        public async Task<IEnumerable<VenueRatingLogModel>> GetVenueRatingLog(int id, int requiredRecords)
        {
            int custId;
            IEnumerable<NAT_VS_Venue_Rating_Log> venueRatingLog = await uow.RepositoryAsync<NAT_VS_Venue_Rating_Log>().GetVenueRatingLogByVenueId(id, requiredRecords);
            IEnumerable<VenueRatingLogModel> venueRatingLogModel = new VenueRatingLogModel().FromDataModelList(venueRatingLog);
            var lookupList = await NatClient.ReadAsync<IEnumerable<ViewModels.CustomerLovViewModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customerslov");


            if (lookupList.status.IsSuccessStatusCode)
            {
                var customerlov = lookupList.data.ToDictionary(item => item.Id, item => item);

                venueRatingLogModel = venueRatingLogModel.Where(x => x.CustomerId != null).Select((item) =>
                {
                    item.CustomerName = customerlov[custId = item.CustomerId ?? default(int)].Value;
                    item.CustomerProfileImageUrl = customerlov[custId = item.CustomerId ?? default(int)].Image;
                    return item;
                }).ToList();
            }
            return venueRatingLogModel;

        }
        public async Task<VenueRatingModel> getaveragerating(int id)
        {

            NAT_VS_Venue_Rating rat = await uow.RepositoryAsync<NAT_VS_Venue_Rating>().GetVenueRatingRecordAsync(id);
            VenueRatingModel data1 = new VenueRatingModel().FromDataModel(rat);
            return data1;

        }

        public async Task<List<VenueModel>> FindReplacementVenue(FindReplacementVenueQueryModel query)
        {
            try
            {
                var replacementVenueList = new List<VenueModel>();

                //Call configuration service to get Radius to find venue with in
                var radiusConfigResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/FindReplacementVenueInRadius");
                if (!radiusConfigResponse.status.IsSuccessStatusCode || radiusConfigResponse.data == null) throw new Exception("Unable to access configuration");
                var radius = Convert.ToInt32(radiusConfigResponse.data.Value);

                //Find venue with is radius of same location
                var venueWithInRadius = await uow.RepositoryAsync<NAT_VS_Venue>().FindVenueWithInRadius(query.VenueId, radius);

                //Find venues in venueWithInRadius with capacity
                var venueWithInRadiusWithCapacity = new List<VenueModel>();

                //prepare plannerIds in advance for the next step
                var plannerIds = new List<int>();
                foreach (var venue in venueWithInRadius)
                {
                    if(venue.Max_Seating_Capacity > query.TotalTicketsSold)
                    {
                        venueWithInRadiusWithCapacity.Add(new VenueModel().FromDataModel(venue));
                        if(venue.Planner_ID != null)
                        {
                            plannerIds.Add(venue.Planner_ID ?? default);
                        }
                    }
                }

                //Find venues in venueWithInRadiusWithCapacity that are available to host the event
                if (venueWithInRadiusWithCapacity.Count > 0)
                {
                    var plannerSearchResp = await NatClient.ReadAsync<List<int>>(NatClient.Method.POST, NatClient.Service.PlannerService, "planner/searchchange", requestBody: new
                    {
                        StartTime = query.EventStartTime,
                        EndTime = query.EventEndTime,
                        ReferenceType = 2,
                        PlannerIds = plannerIds
                    });
                    if (!plannerSearchResp.status.IsSuccessStatusCode) throw new Exception("Planner search api error");
                    var filteredPlannerIds = plannerSearchResp.data;
                    foreach(int plannerId in filteredPlannerIds)
                    {
                        foreach(var venue in venueWithInRadiusWithCapacity)
                        {
                            if(venue.PlannerId == plannerId)
                            {
                                replacementVenueList.Add(venue);
                            }
                        }
                    }
                }

                return replacementVenueList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


    }
}

        




//Check: Insert data only once in person table if a person wants to sign up for multiple roles with same email id. (for Create)
//bool found = uow.RepositoryAsync<NAT_VS_Venue>().GetPersonByEmail(servicemodel.NatAsPerson.PersonEmail);
//if (!found)
//{
//    servicemodel.NatAsPerson.ObjectState = ObjectState.Added;
//}