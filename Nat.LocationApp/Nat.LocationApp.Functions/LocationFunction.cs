using System; 
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
using Nat.LocationApp.Functions.ViewModels;
using Nat.LocationApp.Services;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.KendoX.Extension;
using Microsoft.Extensions.Logging;
using TLX.CloudCore.KendoX.UI;
using Nat.LocationApp.Services.ServiceModels;
using Nat.Core.Exception;
using Nat.Core.Validations;
using Nat.Core.Authentication;
using Nat.Common.Constants;

namespace Nat.LocationApp.Functions
{
    public class LocationFunction
    {

        #region  Methods for Location  

        /// <summary>
        /// GET list of all Location
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllLocationFunction")]
        public static async Task<HttpResponseMessage> GetAllLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Location"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModelList(await service.GetAllLocation());
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
                    return FunctionLayerExceptionHandler.Handle(ex,logger);
                }
            }
        }

        [FunctionName("GetListLocationFunction")]
        public static async Task<HttpResponseMessage> GetListLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Locations/Search")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all locations"))
            {
                try
                {
                    Auth.UserModel UserModel = new Auth.UserModel();
                    //UserModel = await Auth.Validate(req);

                    var location = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    LocationService service = LocationService.GetInstance(logger);
                    DataSourceResult result = await service.GetlistLocation(location, UserModel);
                    result.Data = new LocationGridVWViewModel().FromServiceModelList((IEnumerable<LocationGridVWModel>)result.Data);
                    respVM.data = result;
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);

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

        // INSERT Location Information Method
        [FunctionName("CreateLocationFunction")]
        public static async Task<HttpResponseMessage> CreateLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Location")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Create Location"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<LocationViewModel>();
                    Auth.UserModel userModel = await ValidateUser(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Location = body.Value;
                        Location.CreatedBy = userModel.UserName;
                        Location.LastUpdatedBy = Location.CreatedBy;
                        LocationService service = LocationService.GetInstance(logger);
                        respVM.data = service.CreateLocationAsync(new LocationViewModel() { }.ToServiceModel(Location));
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

        //Activate Location Information Method
        [FunctionName("ActivateLocationFunction")]
        public static async Task<HttpResponseMessage> ActivateLocation([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Locations/Activate/{id}")] HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Artist"))
            {
                try
                {

                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = await service.ActivateLocationAsync(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);

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

        //Activate Bulk Artist Information Method
        [FunctionName("BulkActivateMarketFunction")]
        public static async Task<HttpResponseMessage> BulkActivateMarketFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Market/BulkActivate")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate Bulk Artist"))
            {
                try
                {

                    var body = await req.GetBodyAsync<IEnumerable<LocationViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        LocationService service = LocationService.GetInstance(logger);
                        respVM.data = await service.BulkActivateLocationAsync(new LocationViewModel() { }.ToServiceModelList(body.Value));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);

                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);

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

        //Deactivate Bulk Market Information Method
        [FunctionName("BulkDeactivateMarketFunction")]
        public static async Task<HttpResponseMessage> BulkDeactivateMarketFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Market/BulkDeactivate")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Deactivate Bulk Artist"))
            {
                try
                {

                    var body = await req.GetBodyAsync<IEnumerable<LocationViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        LocationService service = LocationService.GetInstance(logger);
                        respVM.data = await service.BulkDeactivateLocationAsync(new LocationViewModel() { }.ToServiceModelList(body.Value));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);

                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                        respVM.data = body.ValidationResults;
                        respVM.status.message = Constants.INVALID_REQUEST_BODY;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);

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


        //Deactivate Location Information Method
        [FunctionName("DeactivateLocationFunction")]
        public static async Task<HttpResponseMessage> DeactivateLocation([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Locations/Deactivate/{id}")] HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Deactivate Artist"))
            {
                try
                {

                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = await service.DeactivateLocationAsync(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);

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
        //GET Location by ID Method
        [FunctionName("GetLocationByIdFunction")]
        public async static Task<HttpResponseMessage> GetLocationByIdFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Location/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Location By Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModel(await service.GetLocationByIdAsync(id));
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

        // UPDATE Location Information Method
        [FunctionName("UpdateLocationFunction")]
        public static async Task<HttpResponseMessage> UpdateLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Location")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Location"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Location = await req.Content.ReadAsAsync<LocationViewModel>();
                    Auth.UserModel userModel = await ValidateUser(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    Location.LastUpdatedBy = userModel.UserName;
                    respVM.data = new LocationViewModel().FromServiceModel(await service.UpdateLocationAsync(new LocationViewModel().ToServiceModel(Location)));
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

        //GET ParentLocation by Child ID Method
        [FunctionName("GetParentLocationFunction")]
        public async static Task<HttpResponseMessage> GetParentLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Location/Parent/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Parent Location By Child Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModel(await service.GetParentLocationAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        /// GET List of All Immediate Active Children for a Given Id
        [FunctionName("GetImmediateActiveChildrenLocationFunction")]
        public static async Task<HttpResponseMessage> GetImmediateActiveChildrenLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Location/ActiveChildren/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get All Active Immediate Children Location By Given Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModelList(await service.GetImmediateActiveChildrenLocation(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        /// GET List of All Active Children for a Given Id
        [FunctionName("GetAllActiveChildrenLocationFunction")]
        public static async Task<HttpResponseMessage> GetAllActiveChildrenLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/AllActiveChildren/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Active Children Location by Given Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModelList(await service.GetAllActiveChildrenLocation(code, true));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        /// GET list of all location of a given type
        [FunctionName("GetLocationByTypeFunction")]
        public static async Task<HttpResponseMessage> GetLocationByType([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/Type/{type}")]HttpRequestMessage req, [Logger] NatLogger logger, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All List Of Location By Given Type"))
            {
                try
                {
                    
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LocationService service = LocationService.GetInstance(logger);

                    Auth.UserModel userModel = null ;

                    try
                    {
                        userModel = await Auth.Validate(req);
                    } 
                    catch(AuthenticationException ex) { }
                    
                    IEnumerable<LocationGridVWViewModel> locations;
                    
                    List<LocationGridVWViewModel> populatedLocations = new List<LocationGridVWViewModel>();
                    
                    if (userModel != null)
                        locations = new LocationGridVWViewModel().FromServiceModelList(await service.GetLocationByType(type, userModel.ReferenceTypeLKP, userModel.ReferenceId));
                    else
                        locations = new LocationGridVWViewModel().FromServiceModelList(await service.GetLocationByType(type));
                    
                    foreach (LocationGridVWViewModel loc in locations)
                    {
                        loc.LocationName = loc.CountryName + "-" + loc.ProvinceName + "-" + loc.CityShortName;
                        populatedLocations.Add(loc);        
                    }
                    respVM.data = populatedLocations;
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

        [FunctionName("CheckForDuplicateLocationShortCode")]
        public static async Task<HttpResponseMessage> CheckForDuplicateLocationShortCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/CodeExist/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check for duplicate location short code"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to check for duplicate location short code");
                    LocationService service = LocationService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = await service.CheckForDuplicateLocationShortCode(code);
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

        [FunctionName("CheckForDuplicateLocationAirportCode")]
        public static async Task<HttpResponseMessage> CheckForDuplicateLocationAirportCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/AirportCodeExist/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check for duplicate location short code"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to check for duplicate location short code");
                    LocationService service = LocationService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = await service.CheckForDuplicateLocationAirportCode(code);
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

        [FunctionName("CheckForDuplicateLocationName")]
        public static async Task<HttpResponseMessage> CheckForDuplicateLocationName([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/LocationNameExist/{name}")] HttpRequestMessage req, [Logger] NatLogger logger, string name)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check for duplicate location short code"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Call Service Function to check for duplicate location short code");
                    LocationService service = LocationService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = await service.CheckForDuplicateLocationName(name);
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

        #region  Methods for AddressGeography

        /// <summary>
        /// GET List of All Address Geography
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllAddressGeographyFunction")]
        public static async Task<HttpResponseMessage> GetAllAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AddressGeography")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Address Geography"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModelList(await service.GetAllAddressGeography());
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        // INSERT AddressGeography Information Method
        [FunctionName("CreateAddressGeographyFunction")]
        public static async Task<HttpResponseMessage> CreateAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AddressGeography")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Create Address Geography"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<AddressGeographyViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var AddressGeography = body.Value;
                        AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                        respVM.data = await service.CreateAddressGeographyAsync(new AddressGeographyViewModel() { }.ToServiceModel(AddressGeography));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
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

        //GET AddressGeography by ID Method
        [FunctionName("GetAddressGeographyByIDFunction")]
        public async static Task<HttpResponseMessage> GetAddressGeographyByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AddressGeography/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Address Geography By Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<AddressGeographyViewModel> respVM = new ResponseViewModel<AddressGeographyViewModel>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModel(await service.GetAddressGeographyByIDAsync(id));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        // UPDATE AddressGeography Information Method
        [FunctionName("UpdateAddressGeographyFunction")]
        public static async Task<HttpResponseMessage> UpdateAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "AddressGeography")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Address Geography"))
            {
                try
                {
                    logger.LogRequest(req);
                    var AddressGeography = await req.Content.ReadAsAsync<AddressGeographyViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = await service.UpdateAddressGeographyAsync(new AddressGeographyViewModel().ToServiceModel(AddressGeography));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        //GET ParentAddressGeography by Child ID Method
        [FunctionName("GetParentAddressGeographyFunction")]
        public async static Task<HttpResponseMessage> GetParentAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AddressGeography/Parent/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get address geography location by child id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<AddressGeographyViewModel> respVM = new ResponseViewModel<AddressGeographyViewModel>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModel(await service.GetParentAddressGeographyAsync(id));
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

        //GET ParentAddressGeography by Child ID and Type Method
        [FunctionName("GetParentAddressGeographyByTypeFunction")]
        public async static Task<HttpResponseMessage> GetParentAddressGeographyByTypeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AddressGeography/ParentByType/{code}/{type}")]HttpRequestMessage req, [Logger] NatLogger logger, string code, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get address geography location by child code and given type"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<AddressGeographyViewModel> respVM = new ResponseViewModel<AddressGeographyViewModel>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModel(await service.GetParentAddressGeographyByTypeFunction(code, type));
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

        /// GET list of all immediate active children for a given Id
        [FunctionName("GetImmediateActiveChildrenAddressGeographyFunction")]
        public static async Task<HttpResponseMessage> GetImmediateActiveChildrenAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AddressGeography/ActiveChildren/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get All Active Immediate Children Address Geography By Given Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModelList(await service.GetImmediateActiveChildrenAddressGeography(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        /// GET list of all active children for a given Id
        [FunctionName("GetAllActiveChildrenAddressGeographyFunction")]
        public static async Task<HttpResponseMessage> GetAllActiveChildrenAddressGeographyFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AddressGeography/AllActiveChildren/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get All Active Children Address Geography By Given Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModelList(await service.GetAllActiveChildrenAddressGeography(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        /// GET list of all address geography of a given type
        [FunctionName("GetAddressGeographyByTypeFunction")]
        public static async Task<HttpResponseMessage> GetAddressGeographyByType([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options",Route = "AddressGeography/Type/{type}")]HttpRequestMessage req, [Logger] NatLogger logger, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All List Of Address Geography By Given Type"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModelList(await service.GetAddressGeographyByType(type));
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

        /// GET list of all address geography of a given type
        [FunctionName("GetAddressGeographyChildrenByTypeFunction")]
        public static async Task<HttpResponseMessage> GetAddressGeographyChildrenByTypeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AddressGeography/ChildrenByType/{code}/{type}")]HttpRequestMessage req, [Logger] NatLogger logger, string code, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All List Of Address Geography Children By Given Type"))
            {
                try
                {
                    
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                    respVM.data = new AddressGeographyViewModel().FromServiceModelList(await service.GetAddressGeographyChildrenByType(code,type));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
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

        // INSERT Location Information Method
        [FunctionName("CreateAddressGeographyHierarchy")]
        public static async Task<HttpResponseMessage> CreateAddressGeographyHierarchy([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AddressGeography/Hierarchy")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Create Address Geography"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<AddressGeographyViewModel>();
                    Auth.UserModel userModel = await ValidateUser(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var geography = body.Value;
                        AddressGeographyService service = AddressGeographyService.GetInstance(logger);
                        await service.CreateHierarchy(new AddressGeographyViewModel() { }.ToServiceModel(geography));
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

        #endregion

        [FunctionName("GetLocationByCodeFunction")]
        public async static Task<HttpResponseMessage> GetLocationByCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetLocationByCode/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get Parent Location By Child Id"))
            {
                if (req.Method == HttpMethod.Options)
                {
                    var resp = req.CreateResponse();
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                    resp.Headers.Add("Access-Control-Allow-Headers", "*");
                    return resp;
                }
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModel(service.GetLocationByCode(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    logger.LogResponse(resp);
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

        [FunctionName("GetLocationByCodeForTaxFunction")]
        public async static Task<HttpResponseMessage> GetLocationByCodeForTaxFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetLocationByCodeForTax/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get Parent Location By Child Id"))
            {
                if (req.Method == HttpMethod.Options)
                {
                    var resp = req.CreateResponse();
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                    resp.Headers.Add("Access-Control-Allow-Headers", "*");
                    return resp;
                }
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModel(service.GetLocationByCodeForTax(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    logger.LogResponse(resp);
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

        [FunctionName("GetLocationViewByCodeFunction")]
        public async static Task<HttpResponseMessage> GetLocationViewByCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetLocationViewByCode/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get Parent Location By Child Id"))
            {
                if (req.Method == HttpMethod.Options)
                {
                    var resp = req.CreateResponse();
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                    resp.Headers.Add("Access-Control-Allow-Headers", "*");
                    return resp;
                }
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LOCATIONVWViewModel> respVM = new ResponseViewModel<LOCATIONVWViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LOCATIONVWViewModel().FromServiceModel(service.GetLocationViewByCode(code));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    logger.LogResponse(resp);
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

        //GET ParentAddressGeography by Child ID and Type Method
        [FunctionName("GetParentLocationByTypeFunction")]
        public async static Task<HttpResponseMessage> GetParentLocationByTypeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Location/ParentByType/{code}/{type}")] HttpRequestMessage req, [Logger] NatLogger logger, string code, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get location by child code and given type"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<LocationViewModel> respVM = new ResponseViewModel<LocationViewModel>();
                    LocationService service = LocationService.GetInstance(logger);
                    respVM.data = new LocationViewModel().FromServiceModel(await service.GetParentLocationByTypeFunction(code, type));
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

        private static async Task<Auth.UserModel> ValidateUser(HttpRequestMessage req)
        {
            Auth.UserModel UserModel = new Auth.UserModel();
            UserModel = await Auth.Validate(req);
            if (UserModel == null)
                throw new Exception("Not Authenticated");
            return UserModel;
        }
    }
}
