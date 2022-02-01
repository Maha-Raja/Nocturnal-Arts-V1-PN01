 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;

using TLX.CloudCore.Patterns.Repository.Infrastructure;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;

using Nat.LocationApp.Services.ServiceModels;
using Nat.LocationApp.Models.EFModel;
using Nat.LocationApp.Models.Repositories;
using Nat.Core.Authentication;
using Nat.Core.ServiceClient;
using Nat.Common.Constants;
using Nat.Core.Caching;
using System.Data.Entity;

namespace Nat.LocationApp.Services
{
    public class LocationService : BaseService<LocationModel, NAT_LS_Location>
    {
        private static LocationService _service;
        private const string LOCATION_CACHE_NAME = "active_locations_grid_view";
        private const int CACHE_TIME_IN_MIN = 300;
        public static LocationService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            //{
            //    _service = new LocationService();
            //}
            _service = new LocationService();
            _service.SetLogger(logger);
            return _service;
        }

        private LocationService() : base()
        {

        }

        /// <summary>
        /// This method return list of all locations
        /// </summary>
        /// <returns>Collection of location service model<returns>
        public async Task<IEnumerable<LocationModel>> GetAllLocation()
        {
            using (logger.BeginServiceScope("Get All Locations"))
            {
                try
                {
                    IEnumerable<NAT_LS_Location> sortedLocations;
                    IEnumerable<LocationModel> data = null;
                    logger.LogInformation("Fetch all locations from repo");
                    IEnumerable<NAT_LS_Location> locationModel = await uow.RepositoryAsync<NAT_LS_Location>().GetAllLocations();
                    if (locationModel != null)
                    {

                        sortedLocations = locationModel.OrderBy(x => x.Location_Name);
                        data = new LocationModel().FromDataModelList(sortedLocations);


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

        public async Task<DataSourceResult> GetlistLocation(DataSourceRequest request, Auth.UserModel userModel)
        {
            using (logger.BeginServiceScope("Get All Locations"))
            {
                try
                {
                    DataSourceResult data;
                    data = uow.RepositoryAsync<NAT_LS_LOCATION_GRID_VW>().Queryable().OrderBy(x => x.City_Name).ToDataSourceResult<NAT_LS_LOCATION_GRID_VW, LocationGridVWModel>(request);
                    return data;
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<bool> ActivateLocationAsync(string Id)
        {
            try
            {
                NAT_LS_Location LocationtEf = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByIdAsync(Convert.ToInt32(Id));
                if (LocationtEf != null)
                {
                    LocationtEf.Active_Flag = true;
                    uow.RepositoryAsync<NAT_LS_Location>().Update(LocationtEf);
                    uow.SaveChanges();
                    Caching.ClearCache(LOCATION_CACHE_NAME);
                    return true;
                }
                else
                    throw new ApplicationException("Location doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<bool> BulkActivateLocationAsync(IEnumerable<LocationModel> servicemodel)
        {
            try
            {
                foreach(LocationModel loc in servicemodel)
                {
                    await ActivateLocationAsync(loc.LocationId.ToString());
                }
                Caching.ClearCache(LOCATION_CACHE_NAME);
                return true;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<bool> BulkDeactivateLocationAsync(IEnumerable<LocationModel> servicemodel)
        {
            try
            {
                foreach (LocationModel loc in servicemodel)
                {
                    await DeactivateLocationAsync(loc.LocationId.ToString());
                }
                Caching.ClearCache(LOCATION_CACHE_NAME);
                return true;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<bool> DeactivateLocationAsync(string Id)
        {
            try
            {
                NAT_LS_Location LocationtEf = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByIdAsync(Convert.ToInt32(Id));
                if (LocationtEf != null)
                {
                    var eventResp = await NatClient.ReadAsync<IEnumerable<EventModel>>(NatClient.Method.GET, NatClient.Service.EventService, "EventsbyLocation/" + LocationtEf.Location_Short_Code);

                    if (eventResp != null && eventResp.status.IsSuccessStatusCode)
                    {
                        if(eventResp.data != null && eventResp.data.Count() > 0 )
                        {
                            throw new Exception("Events Are Scheduled for this market in Future");
                        }

                        
                    }
                    else
                    {
                        throw new Exception("Event Service Failed");
                    }
                    IEnumerable<LocationModel> childrenItemCategories = new List<LocationModel>();
                    childrenItemCategories = await GetAllActiveChildrenLocation(LocationtEf.Location_Short_Code,true);
                    int childrenActiveCount = childrenItemCategories.AsQueryable().Where(x => x.ActiveFlag == true).Count();
                    if (childrenActiveCount > 0)
                    {
                        throw new ApplicationException("Please deactivate child markets first");
                    }

                    LocationtEf.Active_Flag = false;
                    uow.RepositoryAsync<NAT_LS_Location>().Update(LocationtEf);
                    uow.SaveChanges();
                    Caching.ClearCache(LOCATION_CACHE_NAME);
                    return true;
                }
                else
                    throw new ApplicationException("Locationt doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of location
        /// </summary>
        /// <param name="servicemodel">Service Location Model</param>
        /// <returns>Location ID generated for location</returns>
        public async Task<string> CreateLocationAsync(LocationModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {

                    if (servicemodel.LocationTypeLKP == "CITY")
                    {
                        var state = this.uow.RepositoryAsync<NAT_LS_Location>().Queryable()
                        .Where(x => x.Location_Short_Code == servicemodel.ParentLocationCode)
                        .FirstOrDefault();
                        var country = this.uow.RepositoryAsync<NAT_LS_Location>().Queryable()
                        .Where(x => x.Location_Short_Code == state.Parent_Location_Code)
                        .FirstOrDefault();
                        servicemodel.LocationShortCode = country.Location_Short_Code + "-" + state.Location_Short_Code + "-" + servicemodel.AirportCode;
                        //city market id
                        var locname = country.Location_Short_Code;
                        locname = locname.ToUpper();
                        char[] charArr = locname.ToCharArray();
                        int countryid = 0;

                        for (int z = 0; z < charArr.Length; z++)
                        {
                            countryid = countryid + Constants.MarkeId[charArr[0].ToString()];
                        }

                        var locname1 = state.Location_Short_Code;
                        locname1 = locname1.ToUpper();
                        string Stateid = "";
                        char[] charArr1 = locname1.ToCharArray();
                        for (int i = 0; i < charArr1.Length; i++)
                        {
                            if (Constants.MarkeId[charArr1[i].ToString()] < 10)
                                Stateid = Stateid + "0" + Constants.MarkeId[charArr1[i].ToString()].ToString();
                            else
                                Stateid = Stateid + Constants.MarkeId[charArr1[i].ToString()].ToString();
                        }

                        var locname2 = servicemodel.AirportCode;
                        locname2 = locname2.ToUpper();
                        string Cityid = "";
                        char[] charArr2 = locname2.ToCharArray();
                        for (int y = 0; y < charArr2.Length; y++)
                        {
                            if(Constants.MarkeId[charArr2[y].ToString()] < 10)
                            Cityid = Cityid + "0" + Constants.MarkeId[charArr2[y].ToString()].ToString();
                            else
                            Cityid = Cityid + Constants.MarkeId[charArr2[y].ToString()].ToString();
                        }
                        servicemodel.MarketId = countryid.ToString() + Stateid + Cityid;

                    }
                    else
                    {


                        //for country and province
                        var locname = servicemodel.LocationName;
                        locname = locname.ToUpper();
                        char[] charArr = locname.ToCharArray();
                        char[] CodeArr = new char[] { charArr[0], charArr[1], charArr[2] };

                        string CodeStr = new string(CodeArr);
                        if (await CheckForDuplicateLocationShortCode(CodeStr))
                        {
                            List<string> allcodes = printAllKLengthRec(charArr, "", charArr.Length, 3);
                            foreach (string shortcodestr in allcodes)
                            {
                                if (!(await CheckForDuplicateLocationShortCode(shortcodestr)))
                                {
                                    servicemodel.LocationShortCode = shortcodestr;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            servicemodel.LocationShortCode = CodeStr;
                        }

                        if (servicemodel.LocationTypeLKP == "COUNTRY")
                        {
                            var locname1 = servicemodel.LocationShortCode;
                            locname1 = locname1.ToUpper();
                            char[] charArr1 = locname1.ToCharArray();
                            int countryid = 0;

                            for (int z = 0; z < charArr1.Length; z++)
                            {
                                countryid = countryid + Constants.MarkeId[charArr1[0].ToString()];
                            }
                            servicemodel.MarketId = countryid.ToString();
                        }
                        else if (servicemodel.LocationTypeLKP == "STATE")
                        {
                            var locname2 = servicemodel.LocationShortCode;
                            locname2 = locname2.ToUpper();
                            string Stateid = "";
                            char[] charArr2 = locname2.ToCharArray();
                            for (int i = 0; i < charArr2.Length; i++)
                            {
                                if (Constants.MarkeId[charArr2[i].ToString()] < 10)
                                    Stateid = Stateid + "0" + Constants.MarkeId[charArr2[i].ToString()].ToString();
                                else
                                    Stateid = Stateid + Constants.MarkeId[charArr2[i].ToString()].ToString();
                            }

                            servicemodel.MarketId = Stateid;
                        }
                    }





                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    await uow.SaveChangesAsync();
                    return Convert.ToString(Get().LocationId);
                }
                else
                {
                    throw new Exception("Service Model cannot be Null");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public List<string> printAllKLengthRec(char[] set, String prefix, int n, int k)
        {
            List<string> final = new List<string>();


            // Base case: k is 0, 
            // print prefix 
            if (k == 0)
            {
                Console.WriteLine(prefix);
                List<string> small = new List<string>();
                small.Add(prefix);
                return small;
            }

            // One by one add all characters  
            // from set and recursively  
            // call for k equals to k-1 
            for (int i = 0; i < n; ++i)
            {

                // Next character of input added 
                String newPrefix = prefix + set[i];

                // k is decreased, because  
                // we have added a new character 
                var temp = printAllKLengthRec(set, newPrefix,
                                        n, k - 1);

                foreach (string s in temp)
                {
                    final.Add(s);
                }

            }
            return final;
        }

        /// <summary>
        /// This method returns location with a given Id
        /// </summary>
        /// <param name="Id">Id of location</param>
        /// <returns>Location service model</returns>
        public async Task<LocationModel> GetLocationByIdAsync(int Id)
        {
            try
            {
                LocationModel data = null;
                NAT_LS_Location LocationModel = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByIdAsync(Id);
                if (LocationModel != null)
                {
                    data = new LocationModel().FromDataModel(LocationModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Location record
        /// </summary>
        /// <param name="servicemodel">Service LocationModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<LocationModel> UpdateLocationAsync(LocationModel servicemodel)
        {
            try
            {
                if (servicemodel.LocationId != 0 || servicemodel.LocationId > 0)
                {
                    if (servicemodel.NewUserCheck == true)
                    {
                        Auth.UserModel usermodel = new Auth.UserModel();
                        usermodel.FirstName = servicemodel.FirstName;
                        usermodel.LastName = servicemodel.LastName;
                        usermodel.Password = servicemodel.Password;
                        usermodel.UserName = servicemodel.Email;
                        usermodel.Email = servicemodel.Email;
                        usermodel.PhoneNumber = servicemodel.PhoneNumber;
                        usermodel.ReferenceTypeLKP = "admin";
                        usermodel.NatAusUserLocationMapping = new List<Auth.UserLocationMappingViewModel>();
                        Auth.UserLocationMappingViewModel loca = new Auth.UserLocationMappingViewModel();
                        loca.LocationCode = servicemodel.LocationShortCode;
                        loca.LocationName = servicemodel.LocationName;
                        usermodel.NatAusUserLocationMapping.Add(loca);

                        usermodel.ActiveFlag = true;
                        usermodel.RoleCode = "MARKET_MANAGER";
                        var userResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Register", requestBody: usermodel);
                        if (userResp.status.IsSuccessStatusCode == true)
                        {
                            servicemodel.ManagerId = long.Parse(userResp.data);
                        }
                        else
                        {
                            throw new Exception("error while creating user");
                        }
                    }
         
                    // NAT_LS_Location: stripe id  (validations code level)
                    servicemodel.ObjectState = ObjectState.Modified;
                    servicemodel.LastUpdatedDate = System.DateTime.UtcNow;
                    base.Update(servicemodel);
                    int updatedRows = uow.SaveChanges();

                    if (updatedRows == 0)
                    {
                        throw new Exception("Market Update Failed");
                    }
                    return servicemodel;
                }
                else
                {
                    throw new Exception("Id cannot be 0 or less than 0");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns parent location with a given child Id
        /// </summary>
        /// <param name="Id">Child Id of location</param>
        /// <returns>Location service model</returns>
        public async Task<LocationModel> GetParentLocationAsync(int Id)
        {
            try
            {
                LocationModel data = null;
                NAT_LS_Location LocationModel = await uow.RepositoryAsync<NAT_LS_Location>().GetParentLocationByChildIdAsync(Id);
                if (LocationModel != null)
                {
                    data = new LocationModel().FromDataModel(LocationModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns list of all immediate active children for a given Id
        /// </summary>
        /// <returns>Collection of location service model<returns>
        public async Task<IEnumerable<LocationModel>> GetImmediateActiveChildrenLocation(string code)
        {
            using (logger.BeginServiceScope("Get All Immediate Active Children Locations for a Given ID"))
            {
                try
                {
                    IEnumerable<LocationModel> data = null;
                    IEnumerable<NAT_LS_Location> locationModel = await uow.RepositoryAsync<NAT_LS_Location>().GetImmediateActiveChildrenLocation(code);
                    if (locationModel != null)
                    {
                        data = new LocationModel().FromDataModelList(locationModel);
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
        /// This method returns list of all active children for a given Id
        /// </summary>
        /// <returns>Collection of location service model<returns>
        public async Task<IEnumerable<LocationModel>> GetAllActiveChildrenLocation(string code, bool activeFlag = true)
        {
            using (logger.BeginServiceScope("Get All Active Children Locations for a Given ID"))
            {
                try
                {
                    IEnumerable<LocationModel> data = null;
                    IEnumerable<NAT_LS_Location> locationModel = uow.RepositoryAsync<NAT_LS_Location>().GetAllActiveChildrenLocation(code, activeFlag);
                    if (locationModel != null)
                    {
                        data = new LocationModel().FromDataModelList(locationModel);
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
        /// This method returns list of all location for a given Type
        /// </summary>
        /// <returns>Collection of location service model<returns>
        public async Task<IEnumerable<LocationGridVWModel>> GetLocationByType(string type, string userType = "admin", long? referenceId = null)
        {
            using (logger.BeginServiceScope("Get All Location for a Given Type"))
            {
                try
                {
                    IEnumerable<NAT_LS_LOCATION_GRID_VW> sortedLocations;
                    if ("artist".Equals(userType))
                    {
                        var artistResp = await NatClient.ReadAsync<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + referenceId.Value);

                        if (artistResp != null && artistResp.status.IsSuccessStatusCode && artistResp.data != null)
                        {
                            IEnumerable<LocationGridVWModel> data = null;
                            var locationCode = artistResp.data.LocationCode;

                            var locations = await GetActiveLocationsAsync();
                            return locations.Where(x => x.LocationShortCode == locationCode);
                        }

                        return null;

                    }
                    else
                    {
                        return await GetActiveLocationsAsync();                        
                    }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        private async Task<IEnumerable<LocationGridVWModel>> GetActiveLocationsAsync()
        {
            async Task<IEnumerable<LocationGridVWModel>> GetActiveLocationsFromDB()
            {                         
                IEnumerable<NAT_LS_LOCATION_GRID_VW> locationModel = await uow.RepositoryAsync<NAT_LS_LOCATION_GRID_VW>()
                    .Queryable()
                    .Where(x => x.Active).OrderBy(x=>x.Country_Name).ThenBy(x=>x.Province_Name).ThenBy(x=>x.City_Short_Name)
                        .ToListAsync();
                if (locationModel != null)
                {
                    
                    return new LocationGridVWModel().FromDataModelList(locationModel);
                    
                }
                throw new ApplicationException("Data cannot be fetched");
            }
            return await Caching.GetObjectFromCacheAsync(LOCATION_CACHE_NAME, CACHE_TIME_IN_MIN, GetActiveLocationsFromDB);
        }

        public async Task<Boolean> CheckForDuplicateLocationShortCode(string code)
        {
            NAT_LS_Location Location = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByShortCode(code);
            return Location != null ? true : false;
        }

        public async Task<Boolean> CheckForDuplicateLocationAirportCode(string code)
        {
            NAT_LS_Location Location = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByAirportCode(code);
            return Location != null ? true : false;
        }

        public async Task<Boolean> CheckForDuplicateLocationName(string code)
        {
            NAT_LS_Location Location = await uow.RepositoryAsync<NAT_LS_Location>().GetLocationByName(code);
            return Location != null ? true : false;
        }



        public LocationModel GetLocationByCode(string code)
        {
            var location = this.uow.RepositoryAsync<NAT_LS_Location>()
                .Queryable()
                .Where(x => x.Location_Short_Code == code)
                .FirstOrDefault();


            return new LocationModel().FromDataModel(location);
        }

        public LocationModel GetLocationByCodeForTax(string code)
        {
            var location = this.uow.RepositoryAsync<NAT_LS_Location>()
                .Queryable()
                .Where(x => x.Location_Short_Code == code)
                .FirstOrDefault();
            var Parentlocation = this.uow.RepositoryAsync<NAT_LS_Location>().Queryable()
                .Where(x => x.Location_Short_Code == location.Parent_Location_Code).FirstOrDefault();

            if (location.Tax_Rate == null || location.Tax_Rate.Value == 0)
            {
                location.Tax_Rate = Parentlocation.Tax_Rate;
            }
            if (location.State_Tax == null || location.State_Tax.Value == 0)
            {
                location.State_Tax = Parentlocation.State_Tax;
            }
            if (location.Tax_3 == null || location.Tax_3.Value == 0)
            {
                location.Tax_3 = Parentlocation.Tax_3;
            }
            if (location.Tax_4 == null || location.Tax_4.Value == 0)
            {
                location.Tax_4 = Parentlocation.Tax_4;
            }
            if (location.Tax_5 == null || location.Tax_5.Value == 0)
            {
                location.Tax_5 = Parentlocation.Tax_5;
            }
            if (location.Supplies_Tax == null || location.Supplies_Tax.Value == 0)
            {
                location.Supplies_Tax = Parentlocation.Supplies_Tax;
            }


            return new LocationModel().FromDataModel(location);
        }




        public LOCATIONVWModel GetLocationViewByCode(string code)
        {

            var location = this.uow.RepositoryAsync<NAT_LS_LOCATION_VW>().Queryable().Where(x => x.Location_Short_Code == code).FirstOrDefault();
            return new LOCATIONVWModel().FromDataModel(location);
        }

        /// <summary>
        /// This method returns parent location with a given child code and type
        /// </summary>
        /// <param name="code">Child Code of address geography</param>
        /// <param name="type">Type of location</param>
        /// <returns>Address Geography service model</returns>
        public async Task<LocationModel> GetParentLocationByTypeFunction(string code, string type)
        {
            try
            {
                LocationModel data = null;
                NAT_LS_Location locationModel = await uow.RepositoryAsync<NAT_LS_Location>().GetParentLocationByTypeFunction(code, type);
                if (locationModel != null)
                {
                    data = new LocationModel().FromDataModel(locationModel);
                    return data;
                }
                throw new Exception("Location not found");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }

}

        




//Check: Insert data only once in person table if a person wants to sign up for multiple roles with same email id. (for Create)
//bool found = uow.RepositoryAsync<NAT_AS_Artist>().GetPersonByEmail(servicemodel.NatAsPerson.PersonEmail);
//if (!found)
//{
//    servicemodel.NatAsPerson.ObjectState = ObjectState.Added;
//}