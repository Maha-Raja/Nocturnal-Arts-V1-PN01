using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
using Nat.VenueApp.Functions.ViewModels;
using Nat.VenueApp.Services;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.KendoX.Extension;
using Microsoft.Extensions.Logging;
using TLX.CloudCore.KendoX.UI;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.Exception;
using Nat.Core.Validations;
using Nat.VenueApp.Functions.ViewModel.VenueRequest;
using Nat.Core.Http.Extension;
using Nat.VenueApp.Functions.ViewModel;
using Nat.Core.Authentication;
using Nat.Common.Constants;

namespace Nat.VenueApp.Functions
{
    public class VenueFunction
    {

        #region  Methods for Venue  

        [FunctionName("GetAllVenueFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "venues")]HttpRequestMessage req, [Logger] NatLogger logger)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all venue"))
            {
                try
                {
                    logger.LogRequest(req);
                   // logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = new VenueViewModel().FromServiceModelList(service.GetAllVenue());
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

        //GET Artist by ID Method
        [FunctionName("GetVenueLovFunction")]
        public async static Task<HttpResponseMessage> GetVenueLovFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venuelov")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Venue Lov"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        UserModel = null;
                    }
                    respVM.data = new VenueLovViewModel().FromServiceModelList( await service.GetVenueLov(UserModel) );
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

        //GET Venue by ID Method
        [FunctionName("GetVenueByIDFunction")]
        public static async Task<HttpResponseMessage> GetVenueByID_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "venues/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get venue by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<VenueViewModel> respVM = new ResponseViewModel<VenueViewModel>();
                    VenueService service = VenueService.GetInstance(logger);

                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        UserModel = null;
                    }
                    var customerId = UserModel != null && UserModel.ReferenceTypeLKP == Constants.UserReferenceType["CUSTOMER"] ? UserModel.ReferenceId : null;

                    respVM.data =  new VenueViewModel().FromServiceModel(await service.GetByIdVenue(id,customerId));
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


        //GET Venue Seating Plan
        [FunctionName("GetVenueSeatingPlan")]
        public static async Task<HttpResponseMessage> GetVenueSeatingPlan([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "SeatingPlans/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get seating plan by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<VenueSeatingPlanViewModel>> respVM = new ResponseViewModel<IEnumerable<VenueSeatingPlanViewModel>>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = new VenueSeatingPlanViewModel().FromServiceModelList(await service.GetSeatingById(id));
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
        /// Get list of all venue
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListvenueFunction")]
        public static async Task<HttpResponseMessage> GetListVenueFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "venues/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Venue = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    VenueService service = VenueService.GetInstance(logger);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        UserModel = null;
                    }
                    DataSourceResult result = await service.GetAllVenueViewListAsync(Venue,UserModel);
                    if(result.Total > 0)
                        result.Data = new VenueVWViewModel().FromServiceModelList((IEnumerable<VenueVWmodel>)result.Data);
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

        //Search Venue Information Method
        [FunctionName("SearchVenueForEvent")]
        public static async Task<HttpResponseMessage> SearchVenueForEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "venue/searchforevent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Search Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    var searchObjVM = await req.Content.ReadAsAsync<VenueSearchForEventViewModel>();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    VenueService service = VenueService.GetInstance(logger);
                    var searchObj = new VenueSearchForEventViewModel().ToServiceModel(searchObjVM);
                    var dataSourceResult = await service.SearchVenueForEvent(searchObj);
                    dataSourceResult.Data = new VenueViewModel().FromServiceModelList((IEnumerable<VenueModel>)dataSourceResult.Data);
                    respVM.data = dataSourceResult;
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


        //CheckVenueandArtistPreferences
        [FunctionName("CheckVenueandArtistPreferences")]
        public static async Task<HttpResponseMessage> CheckVenueandArtistPreferences([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "venue/CheckVenueandArtistPreferences")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Search Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    var searchObjVM = await req.Content.ReadAsAsync<IEnumerable<VenueArtistPreferenceViewModel>>();
                    ResponseViewModel<IEnumerable<object>> respVM = new ResponseViewModel<IEnumerable<object>>();
                    VenueService service = VenueService.GetInstance(logger);
                    var searchObj = new VenueArtistPreferenceViewModel().ToServiceModelList(searchObjVM);
                    respVM.data = await service.CheckVenueandArtistPreferences(searchObj);
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

        //Search Venue Information Method
        [FunctionName("GetVirtualVenue")]
        public static async Task<HttpResponseMessage> GetVirtualVenue([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetVirtualVenue")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Virtual Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<VenueViewModel> respVM = new ResponseViewModel<VenueViewModel>();
                    VenueService service = VenueService.GetInstance(logger);
                    var venueModel = await service.GetVirtualVenue();
                    var venueVM = new VenueViewModel().FromServiceModel(venueModel);
                    respVM.data = venueVM;
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

        // INSERT Venue Information Method
        [FunctionName("SaveVenueFunction")]
        public static async Task<HttpResponseMessage> SaveVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "venues")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    var body = await req.GetBodyAsync<VenueViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Venue = body.Value;
                        Venue.CreatedBy = UserModel.UserName;
                        Venue.LastUpdatedBy = UserModel.UserName;
                        VenueService service = VenueService.GetInstance(logger);
                        VenueModel servicemodel = new VenueViewModel() { }.ToServiceModel(Venue);                        
                        respVM.data = service.CreateVenue(new VenueViewModel() { }.ToServiceModel(Venue));
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


        // UPDATE Venue Information Method
        [FunctionName("UpdateVenueFunction")]
        public static async Task<HttpResponseMessage> UpdateVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put","options", Route = "venues/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    var Venue = await req.Content.ReadAsAsync<VenueViewModel>();
                    Venue.LastUpdatedBy = UserModel.UserName;
                    ResponseViewModel<VenueViewModel> respVM = new ResponseViewModel<VenueViewModel>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = new VenueViewModel().FromServiceModel(await service.UpdateVenueAsync(new VenueViewModel().ToServiceModel(Venue)));
                    ResponseViewModel<VenueViewModel> respVMId = new ResponseViewModel<VenueViewModel>();
                    respVMId.data = new VenueViewModel().FromServiceModel(await service.GetByIdVenue(int.Parse(id),null));
                    var resp = respVMId.ToHttpResponseMessage(HttpStatusCode.OK);
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


        // DELETE Venue Information Method
        [FunctionName("DeleteVenueFunction")]
        public static async Task<HttpResponseMessage> Delete_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "venues/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //check planner id if its Venue
        [FunctionName("PlannerIdCheck")]
        public static async Task<HttpResponseMessage> PlannerIdCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venues/planner/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("check planner id for artist"))
            {
                try
                {
                    logger.LogRequest(req);

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.checkVenueForPlannerId(id);
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


        //Activate Venue Information Method
        [FunctionName("ActivateVenueFunction")]
        public static async Task<HttpResponseMessage> ActivateVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "venues/Activate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Activate Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.ActivateVenueAsync(id,UserModel.UserName);
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

        //Activate Bulk Venue Information Method
        [FunctionName("BulkActivateVenueFunction")]
        public static async Task<HttpResponseMessage> BulkActivateVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "venues/BulkActivate")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    var body = await req.GetBodyAsync<IEnumerable<VenueViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.BulkActivateVenueAsync(new VenueViewModel() { }.ToServiceModelList(body.Value),UserModel.UserName);
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





        //Deactivate Venue Information Method
        [FunctionName("DeactivateVenueFunction")]
        public static async Task<HttpResponseMessage> DeactivateVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "venues/Deactivate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Deactivate Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueService service = VenueService.GetInstance(logger);
                    Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    respVM.data = await service.DeactivateVenueAsync(id,UserModel.UserName);
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

        //Deactivate bulk venue Information Method
        [FunctionName("BulkDeactivateVenueFunction")]
        public static async Task<HttpResponseMessage> BulkDeactivateVenue_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "venues/BulkDeactivate")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    logger.LogRequest(req);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    var body = await req.GetBodyAsync<IEnumerable<VenueViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.BulkDeactivateVenueAsync(new VenueViewModel() { }.ToServiceModelList(body.Value),UserModel.UserName);
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


        [FunctionName("UploadImageFunction")]
        public static async Task<HttpResponseMessage> UploadImageFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "venues/UploadImage")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Store Image on Blob"))
            {
                try
                {
                    logger.LogRequest(req);
                    var file = await req.ReadFileAsync();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.UploadImage(file.Content); //<-- Just uploading single file
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


        [FunctionName("UploadVenueSpaceImagesFunction")]
        public static async Task<HttpResponseMessage> UploadVenueSpaceImagesFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "venues/UploadVenueSpaceImage")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Add venue space images in venue images table"))
            {
                try
                {
                    logger.LogRequest(req);
                    var venueImageModel = await req.Content.ReadAsAsync<VenueImageViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.UploadVenueSpaceImages(new VenueImageViewModel().ToServiceModel(venueImageModel));
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


        [FunctionName("SubmitVenueRequest")]
        public static async Task<HttpResponseMessage> SubmitVenueRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Venue/Request")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Venue Request"))
            {

                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<VenueRequestViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Venue = body.Value;
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.SubmitVenueRequestAsync(new VenueRequestViewModel() { }.ToServiceModel(Venue));
                        //respVM.data = Artist;
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

        [FunctionName("RetrieveVenueRequest")]
        public static async Task<HttpResponseMessage> RetrieveVenueRequest([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/RetrieveRequest/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Venue request"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    DataSourceRequest dataSourceRequest = req.ToDataSourceRequest();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    //String[] cities = { "001", "002" };
                    DataSourceResult result = await service.GetVenueRequestAsync(code, dataSourceRequest);
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
        [FunctionName("RetrieveVenueRequestByGuid")]
        public static async Task<HttpResponseMessage> RetrieveVenueRequestByGuid([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/RetrieveRequest/{code1}/{code2}")]HttpRequestMessage req, [Logger] NatLogger logger, string code1,string code2)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Venue request"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    //String[] cities = { "001", "002" }
                    respVM.data = await service.GetVenueRequestByGuidAsync(code1, code2);
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
        [FunctionName("ApproveVenueRequest")]
        public static async Task<HttpResponseMessage> ApproveVenueRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Venue/ApproveRequest")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Approve Artist Request"))
            {
                try
                {

                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<VenueRequestViewModel>();
                    Auth.UserModel UserModel = await ValidateUser(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Venue = body.Value;
                        Venue.CreatedBy = UserModel.UserName;
                        Venue.LastUpdatedBy = UserModel.UserName;
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.ApproveVenueRequestAsync(new VenueRequestViewModel() { }.ToServiceModel(Venue));
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


        [FunctionName("RejectVenueRequest")]
        public static async Task<HttpResponseMessage> RejectVenueRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Venue/RejectRequest")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Reject Venue Request"))
            {
                try
                {

                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<VenueRequestViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Venue = body.Value;
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.RejectVenueRequestAsync(new VenueRequestViewModel() { }.ToServiceModel(Venue));
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
        [FunctionName("UpdateAvailabilityFunction")]
        public static async Task<HttpResponseMessage> VenueUpdateAvailabilityFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Venue/Availability/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Venue Availability"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Availability = await req.Content.ReadAsAsync<IEnumerable<Services.ServiceModels.VenueRequest.VenueAvailabilityModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.VenueUpdateAvailabilityAsync(Availability, id);
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


        [FunctionName("GetVenueUpcomingEventsFunction")]
        public static async Task<HttpResponseMessage> GetVenueUpcomingEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/UpcomingEvents/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get venue all upcoming events"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = new VenueEventViewModel().FromServiceModelList(await service.GetAllVenuePendingEventsAsync(id));
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



        [FunctionName("AddVenueRatingFunction")]
        public static async Task<HttpResponseMessage> AddVenueRatingFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Venue/Rating/Add")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("add venue rating"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<VenueRatingLogViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var rating = body.Value;

                        VenueService service = VenueService.GetInstance(logger);
                        var appservice = new VenueRatingLogViewModel() { }.ToServiceModel(rating);
                        respVM.data = await service.AddVenueRatingAsync(appservice);
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


        [FunctionName("GetRatingLogFunction")]
        public static async Task<HttpResponseMessage> GetRatingLogFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/RatingLog/{id}/{requiredRecords}")]HttpRequestMessage req, [Logger] NatLogger logger, int id, int requiredRecords)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All List Of Paintings By Given Type"))
            {
                try
                {


                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<VenueRatingLogViewModel>> respVM = new ResponseViewModel<IEnumerable<VenueRatingLogViewModel>>();
                    VenueService service = VenueService.GetInstance(logger);
                    IEnumerable<VenueRatingLogModel> result = await service.GetVenueRatingLog(id, requiredRecords);
                    respVM.data = new VenueRatingLogViewModel().FromServiceModelList(result); ;
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


        [FunctionName("GetAverageRating")]
        public static async Task<HttpResponseMessage> GetAverageRating([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/AverageRating/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get average rating of a specific artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<VenueRatingViewModel> respVM = new ResponseViewModel<VenueRatingViewModel>();
                    VenueService service = VenueService.GetInstance(logger);
                    VenueRatingModel result = await service.getaveragerating(id);

                    respVM.data = new VenueRatingViewModel().FromServiceModel(result); ;
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




        #region  Methods for Venue  Hall

        //GET Venue Halls by Venue ID Method
        [FunctionName("GetVenueHallsByIDFunction")]
        public static async Task<HttpResponseMessage> GetVenueHallsByIDFunction_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "venues/VenueHalls/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Get Venue Halls by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueHallService service = VenueHallService.GetInstance(logger);
                    respVM.data = new VenueHallViewModel() { }.FromServiceModelList(service.GetByIdVenueHall(int.Parse(id)));
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

        //INSERT Venue Halls  Method
        [FunctionName("SaveVenueHallFunction")]
        public static async Task<HttpResponseMessage> SaveVenueHall_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "venues/VenueHalls")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Venue Halls"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueHall = await req.Content.ReadAsAsync<VenueHallViewModel>();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    VenueHallService service = VenueHallService.GetInstance(logger);
                    respVM.data = service.CreateVenueHalls(new VenueHallViewModel() { }.ToServiceModel(VenueHall));
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


        // UPDATE Venue Hall Information Method
        [FunctionName("UpdateVenueHallFunction")]
        public static async Task<HttpResponseMessage> UpdateVenueHall_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VenueHalls/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Update Venue Halls"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueHall = await req.Content.ReadAsAsync<VenueHallViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueHallService service = VenueHallService.GetInstance(logger);
                    respVM.data = service.UpdateVenueHalls(new VenueHallViewModel() { }.ToServiceModel(VenueHall));
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


        //DELETE Venue Hall Method
        [FunctionName("DeleteVenueHallFunction")]
        public static async Task<HttpResponseMessage> DeleteVenueHall_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "venues/VenueHalls/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //Activate Venue Hall Method
        [FunctionName("ActivateVenueHallFunction")]
        public static async Task<HttpResponseMessage> ActivateVenueHall_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VenueHalls/activate/{VenueHallId}")]HttpRequestMessage req, [Logger] NatLogger logger, string VenueHallId)
        {
            using (logger.BeginFunctionScope("Activate Venue Hall "))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    VenueHallService service = VenueHallService.GetInstance(logger);
                    service.ActivateVenueHall(VenueHallId);
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


        //Deactivate Venue Hall  Method
        [FunctionName("DeactivateVenueHallFunction")]
        public static async Task<HttpResponseMessage> DeactivateVenueHall_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VenueHalls/deactivate/{VenueHallId}")]HttpRequestMessage req, [Logger] NatLogger logger, string VenueHallId)
        {
            using (logger.BeginFunctionScope("Deactivate Venue Hall"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    VenueHallService service = VenueHallService.GetInstance(logger);
                    service.DeactivateVenueHall(VenueHallId);
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

        #endregion




        #region  Methods for Venue Facility

        //GET Venue Halls by Venue ID Method
        [FunctionName("GetVenueFacilityByIDFunction")]
        public static async Task<HttpResponseMessage> GetVenueFacilityByIDFunction_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "venues/VenueFacility/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Get Venue Facility by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueFacilityService service = VenueFacilityService.GetInstance(logger);
                    respVM.data = new VenueFacilityViewModel() { }.FromServiceModelList(service.GetByIdVenueFacility(int.Parse(id)));
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


        //INSERT Venue Halls  Method
        [FunctionName("SaveVenueFacilityFunction")]
        public static async Task<HttpResponseMessage> SaveVenueFacility_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "venues/VenueFacility")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Venue Facility"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueFacility = await req.Content.ReadAsAsync<VenueFacilityViewModel>();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    VenueFacilityService service = VenueFacilityService.GetInstance(logger);
                    respVM.data = service.CreateVenueFacility(new VenueFacilityViewModel() { }.ToServiceModel(VenueFacility));
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

        // UPDATE Venue Facility Information Method
        [FunctionName("UpdateVenueFacilityFunction")]
        public static async Task<HttpResponseMessage> UpdateVenueFacility_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VenueFacility/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Update Venue Facility"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueFacility = await req.Content.ReadAsAsync<VenueFacilityViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueFacilityService service = VenueFacilityService.GetInstance(logger);
                    respVM.data = service.UpdateVenueFacility(new VenueFacilityViewModel() { }.ToServiceModel(VenueFacility));
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


        //DELETE Venue Facility Method
        [FunctionName("DeleteVenueFacilityFunction")]
        public static async Task<HttpResponseMessage> DeleteVenueFacility_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "venues/VenueFacility/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Delete Venue Facility"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueFacility = await req.Content.ReadAsAsync<VenueFacilityViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueFacilityService service = VenueFacilityService.GetInstance(logger);
                    respVM.data = service.DeleteVenueFacility(new VenueFacilityViewModel() { }.ToServiceModel(VenueFacility));
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

        #endregion


        #region  Methods for Venue Contact Person

        //GET all Active Venue Contact person by ID Method
        [FunctionName("GetAllVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> GetAllActiveVCP_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "venues/VCP")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Get all active venue contact person"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    respVM.data = new VenueContactPersonViewModel().FromServiceModelList(service.GetAllActiveVenueContactPerson());
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


        //GET Venue Contact person by ID Method
        [FunctionName("GetVenueContactPersonByIDFunction")]
        public static async Task<HttpResponseMessage> GetVenueContactPersonByID_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "venues/VCP/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Get Venue contact person by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    respVM.data = new VenueContactPersonViewModel() { }.FromServiceModelList(service.GetByIdVenueContactPerson(int.Parse(id)));
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

        // INSERT Venue Contact Person Information Method
        [FunctionName("SaveVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> SaveVenueContactPerson_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "venues/VCP")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Venue Contact Person"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueContactPerson = await req.Content.ReadAsAsync<VenueContactPersonViewModel>();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    respVM.data = service.CreateVenueContactPerson(new VenueContactPersonViewModel() { }.ToServiceModel(VenueContactPerson));
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


        // UPDATE Venue Contact Person Information Method
        [FunctionName("UpdateVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> UpdateVenueContactPersonFunction_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VCP/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Update Venue Contact Person"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueContactPerson = await req.Content.ReadAsAsync<VenueContactPersonViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    respVM.data = service.UpdateVenueContactPerson(new VenueContactPersonViewModel() { }.ToServiceModel(VenueContactPerson));
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


        // DELETE Venue Contact Person Information Method
        [FunctionName("DeleteVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> DeleteVenueContactPerson_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "venues/VCP/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //Activate Venue Contact Person Method
        [FunctionName("ActivateVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> ActivateVenueContactPerson_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VCP/activate/{VenueContactPersonId}")]HttpRequestMessage req, [Logger] NatLogger logger, string VenueContactPersonId)
        {
            using (logger.BeginFunctionScope("Activate Venue Contact Person "))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    service.ActivateVenueContactPerson(VenueContactPersonId);
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


        //Deactivate Venue Contact Person  Method
        [FunctionName("DeactivateVenueContactPersonFunction")]
        public static async Task<HttpResponseMessage> DeactivateVenueContactPerson_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "venues/VCP/deactivate/{VenueContactPersonId}")]HttpRequestMessage req, [Logger] NatLogger logger, string VenueContactPersonId)
        {
            using (logger.BeginFunctionScope("Deactivate Venue Hall"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    VenueContactPersonService service = VenueContactPersonService.GetInstance(logger);
                    service.DeactivateVenueContactPerson(VenueContactPersonId);
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

        #endregion

        #region  Methods for Venue Event

        [FunctionName("BookVenueForEvent")]
        public static async Task<HttpResponseMessage> BookVenueForEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BookVenueEvent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Book Venue Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<VenueEventViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var VenueEvent = body.Value;
                        VenueService service = VenueService.GetInstance(logger);
                        respVM.data = await service.BookVenueForEvent(new VenueEventViewModel() { }.ToServiceModel(VenueEvent));
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

        [FunctionName("CancelVenueForEvent")]
        public static async Task<HttpResponseMessage> CancelVenueForEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "CancelVenueEvent/{eventcode}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode)
        {
            using (logger.BeginFunctionScope("Cancel Venue Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    VenueService service = VenueService.GetInstance(logger);
                    respVM.data = await service.CancelVenueForEvent(eventcode);
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

        [FunctionName("FindReplacementVenue")]
        public static async Task<HttpResponseMessage> FindReplacementVenue([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FindReplacementVenue")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Cancel Venue Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.Content.ReadAsAsync<FindReplacementVenueQueryViewModel>();
                    ResponseViewModel<IEnumerable<VenueViewModel>> respVM = new ResponseViewModel<IEnumerable<VenueViewModel>>();
                    VenueService service = VenueService.GetInstance(logger);
                    var respModel = await service.FindReplacementVenue(new FindReplacementVenueQueryViewModel().ToServiceModel(body));
                    respVM.data = new VenueViewModel().FromServiceModelList(respModel);
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

        #endregion

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
