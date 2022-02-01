using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Nat.ArtistApp.Functions.ViewModels;
using Nat.ArtistApp.Functions.ViewModels.ArtistRequest;
using Nat.ArtistApp.Services;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.ArtistApp.Services.ServiceModels.ArtistRequest;
using Nat.ArtistApp.Services.Services;
using Nat.Common.Constants;
using Nat.Core.Authentication;
using Nat.Core.BaseModelClass;
using Nat.Core.Exception;
using Nat.Core.Http.Extension;
using Nat.Core.KendoX.Extension;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.Validations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TLX.CloudCore.KendoX.UI;

namespace Nat.ArtistApp.Functions
{
    public class ArtistFunction
    {

        #region  Methods for Artist  


        //GET Artist by ID Method
        [FunctionName("GetArtistLovFunction")]
        public async static Task<HttpResponseMessage> GetArtistLovFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artistslov")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get artist Lov"))
            {
                try
                {
                    
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        UserModel = null;
                    }
                    respVM.data = await service.GetArtistLov(UserModel);
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

        /// <summary>
        /// Get list of all artist
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllArtistFunction")]
        public static async Task<HttpResponseMessage> GetAllArtistFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all artist"))
            {
                try
                {
                    
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = new ArtistViewModel().FromServiceModelList(await service.GetAllArtistAsync());
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


        //CheckVenueandArtistPreferences
        [FunctionName("CheckVenueandArtistPreferences")]
        public static async Task<HttpResponseMessage> CheckVenueandArtistPreferences([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "artist/CheckVenueandArtistPreferences")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    var searchObjVM = await req.Content.ReadAsAsync<IEnumerable<ArtistVenuePreferenceViewModel>>();
                    ResponseViewModel<IEnumerable<object>> respVM = new ResponseViewModel<IEnumerable<object>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    var searchObj = new ArtistVenuePreferenceViewModel().ToServiceModelList(searchObjVM);
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


        //check planner id if its artist
        [FunctionName("PlannerIdCheck")]
        public static async Task<HttpResponseMessage> PlannerIdCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/planner/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
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
                    

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.checkArtistForPlannerId(id);
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


        //GET Artist by ID Method
        [FunctionName("GetArtistByIDFunction")]
        public async static Task<HttpResponseMessage> GetArtistByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get artist by id"))
            {
                try
                {
                    
                    ResponseViewModel<ArtistViewModel> respVM = new ResponseViewModel<ArtistViewModel>();
                    ArtistService service = ArtistService.GetInstance(logger);
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

                    respVM.data = new ArtistViewModel().FromServiceModel(await service.GetByIdArtistAsync(id, customerId));
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

        private static async Task<Auth.UserModel> ValidateUser(HttpRequestMessage req)
        {
            Auth.UserModel UserModel = new Auth.UserModel();
            UserModel = await Auth.Validate(req);
            if (UserModel == null)
                throw new Exception("Not Authenticated");
            return UserModel;
        }

        /// <summary>
        /// Get list of all artist
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListArtistFunction")]
        public static async Task<HttpResponseMessage> GetListArtistFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all artist"))
            {
                try
                {
                    Auth.UserModel UserModel = new Auth.UserModel();
                    UserModel = await Auth.Validate(req);
                    
                    var Artist = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllArtistListAsync(Artist, UserModel);
                    result.Data = new ArtistViewModel().FromServiceModelList((IEnumerable<ArtistModel>)result.Data);
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

        /// <summary>
        /// Get Artist List From View
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetListArtistVWFunction")]
        public static async Task<HttpResponseMessage> GetListArtistVWFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/SearchVW")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all artist"))
            {
                try
                {
                    Auth.UserModel UserModel = new Auth.UserModel();
                    UserModel = await Auth.Validate(req);

                    var Artist = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllArtistViewListAsync(Artist, UserModel);
                    if(result.Total > 0)
                    result.Data = new ArtistVWViewModel().FromServiceModelList((IEnumerable<ArtistVWModel>)result.Data);
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

        /// <summary>
        /// Get Artist Disbursements From View
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetArtistDisbursementByIdVWFunction")]
        public static async Task<HttpResponseMessage> GetArtistDisbursementByIdVWFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/Disbursement/{id:int}")] HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Artist Disbursement By ID from View"))
            {
                try
                {
                    ResponseViewModel<IEnumerable<ArtistDisbursementVWViewModel>> respVM = new ResponseViewModel<IEnumerable<ArtistDisbursementVWViewModel>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    UserModel = await Auth.Validate(req);
                    respVM.data = new ArtistDisbursementVWViewModel().FromServiceModelList(await service.GetArtistDisbursementById(id));
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

        // INSERT Artist Information Method
        [FunctionName("SaveArtistFunction")]
        public static async Task<HttpResponseMessage> SaveArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Artists")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Artist"))
            {
                try
                {
                                        
                    var body = await req.GetBodyAsync<ArtistViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        Auth.UserModel UserModel = await ValidateUser(req);
                        if (UserModel == null)
                            throw new Exception("Not Authenticated");
                        var Artist = body.Value;
                        Artist.CreatedBy = UserModel.UserName;
                        Artist.LastUpdatedBy = Artist.CreatedBy;
                        ArtistService service = ArtistService.GetInstance(logger);
                        respVM.data = await service.CreateArtistAsync(new ArtistViewModel() { }.ToServiceModel(Artist));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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
        [FunctionName("UpdateArtistFunction")]
        public static async Task<HttpResponseMessage> UpdateArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
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
                    
                    Auth.UserModel UserModel = await ValidateUser(req);                    
                    var Artist = await req.Content.ReadAsAsync<ArtistViewModel>();
                    Artist.LastUpdatedBy = UserModel.UserName;                    
                    ResponseViewModel<ArtistViewModel> respVM = new ResponseViewModel<ArtistViewModel>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    respVM.data = new ArtistViewModel().FromServiceModel(await service.UpdateArtistAsync(new ArtistViewModel().ToServiceModel(Artist)));
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


        //DELETE Artist Information Method
        [FunctionName("DeleteArtistFunction")]
        public static async Task<HttpResponseMessage> DeleteArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Artists/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            throw new NotImplementedException();
        }


        //Activate Artist Information Method
        [FunctionName("ActivateArtistFunction")]
        public static async Task<HttpResponseMessage> ActivateArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/Activate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
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
                    
                    Auth.UserModel UserModel = await ValidateUser(req);                    
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.ActivateArtistAsync(id,UserModel.UserName);
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
        [FunctionName("BulkActivateArtistFunction")]
        public static async Task<HttpResponseMessage> BulkActivateArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/BulkActivate")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var body = await req.GetBodyAsync<IEnumerable<ArtistViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        ArtistService service = ArtistService.GetInstance(logger);
                        Auth.UserModel UserModel = await ValidateUser(req);
                        respVM.data = await service.BulkActivateArtistAsync(new ArtistViewModel() { }.ToServiceModelList(body.Value), UserModel.UserName);
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




        //Deactivate Artist Information Method
        [FunctionName("DeactivateArtistFunction")]
        public static async Task<HttpResponseMessage> DeactivateArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/Deactivate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
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
                    ArtistService service = ArtistService.GetInstance(logger);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    respVM.data = await service.DeactivateArtistAsync(id,UserModel.UserName);
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


        //Deactivate Artist Information Method
        [FunctionName("BulkDeactivateArtistFunction")]
        public static async Task<HttpResponseMessage> BulkDeactivateArtist_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/BulkDeactivate")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var body = await req.GetBodyAsync<IEnumerable<ArtistViewModel>>();
                    if (body.IsValid)
                    {
                        ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                        ArtistService service = ArtistService.GetInstance(logger);
                        Auth.UserModel UserModel = await ValidateUser(req);
                        respVM.data = await service.BulkDeactivateArtistAsync(new ArtistViewModel() { }.ToServiceModelList(body.Value),UserModel.UserName);
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

        //Deactivate Artist Information Method
        [FunctionName("SearchArtistForEvent")]
        public static async Task<HttpResponseMessage> SearchArtistForEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "artists/searchforevent")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var searchObjVM = await req.Content.ReadAsAsync<ArtistSearchForEventViewModel>();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    var searchObj = new ArtistSearchForEventViewModel().ToServiceModel(searchObjVM);
                    var dataSourceResult = await service.SearchArtistForEvent(searchObj);
                    dataSourceResult.Data = new ArtistViewModel().FromServiceModelList((IEnumerable<ArtistModel>)dataSourceResult.Data);
                    respVM.data = dataSourceResult;
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

        /// <summary>
        /// Get list of artist events
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetArtistUpcomingEventsFunction")]
        public static async Task<HttpResponseMessage> GetArtistUpcomingEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/UpcomingEvents/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get artist all upcoming events"))
            {
                try
                {
                    
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistEventService service = ArtistEventService.GetInstance(logger);
                    respVM.data = new ArtistEventViewModel().FromServiceModelList(await service.GetAllArtistPendingEventsAsync(id));
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

        [FunctionName("SubmitArtistRequest")]
        public static async Task<HttpResponseMessage> SubmitArtistRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artists/Request")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Artist Request"))
            {
                try
                {
                    
                    var body = await req.GetBodyAsync<ArtistRequestViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Artist = body.Value;
                        ArtistService service = ArtistService.GetInstance(logger);
                        respVM.data = await service.SubmitArtistRequestAsync(new ArtistRequestViewModel() { }.ToServiceModel(Artist));
                        //respVM.data = Artist;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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

        [FunctionName("RetrieveArtistRequest")]
        public static async Task<HttpResponseMessage> RetrieveArtistRequest([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artists/RetrieveRequest/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all artist request"))
            {
                try
                {
                    
                    DataSourceRequest dataSourceRequest = req.ToDataSourceRequest();
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.GetArtistRequestAsync(code, dataSourceRequest);
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

        [FunctionName("RetrieveArtistRequestByGuid")]
        public static async Task<HttpResponseMessage> RetrieveArtistRequestByGuid([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artist/RetrieveRequest/{city}/{guid}")]HttpRequestMessage req, [Logger] NatLogger logger, string city, string guid)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Single Artist Request"))
            {
                try
                {
                    
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.GetArtistRequestByGuidAsync(city, guid);
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

        [FunctionName("ApproveArtistRequest")]
        public static async Task<HttpResponseMessage> ApproveArtistRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artists/ApproveRequest")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Approve Artist Request"))
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
                    
                    var body = await req.GetBodyAsync<ArtistRequestViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        Auth.UserModel UserModel = new Auth.UserModel();
                        UserModel = await Auth.Validate(req);
                        var Artist = body.Value;
                        ArtistService service = ArtistService.GetInstance(logger);
                        respVM.data = await service.ApproveArtistRequestAsync((new ArtistRequestViewModel() { }.ToServiceModel(Artist)),UserModel);
                        //respVM.data = Artist;
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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

        [FunctionName("RejectArtistRequest")]
        public static async Task<HttpResponseMessage> RejectArtistRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artist/RejectRequest")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Reject Artist Request"))
            {
                try
                {

                    
                    var body = await req.GetBodyAsync<ArtistRequestViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Artist = body.Value;
                        ArtistService service = ArtistService.GetInstance(logger);
                        respVM.data = await service.RejectArtistRequestAsync(new ArtistRequestViewModel() { }.ToServiceModel(Artist));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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

        [FunctionName("UploadImageFunction")]
        public static async Task<HttpResponseMessage> UploadImageFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artists/UploadImage")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var file = await req.ReadFileAsync();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.UploadImage(file.Content); //<-- Just uploading single file
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

        [FunctionName("UpdateAvailabilityFunction")]
        public static async Task<HttpResponseMessage> ArtistUpdateAvailabilityFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Artists/Availability/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Artist Availability"))
            {
                try
                {
                    
                    var Availability = await req.Content.ReadAsAsync<IEnumerable<ArtistAvailabilityModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.ArtistUpdateAvailabilityAsync(Availability, id);
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

        [FunctionName("AddArtistRatingFunction")]
        public static async Task<HttpResponseMessage> AddArtistRatingFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artists/Rating/Add")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var body = await req.GetBodyAsync<ArtistRatingLogViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var rating = body.Value;

                        ArtistService service = ArtistService.GetInstance(logger);
                        var appservice = new ArtistRatingLogViewModel() { }.ToServiceModel(rating);
                        respVM.data = await service.AddArtistRatingAsync(appservice);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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

        //blobstorage testing
        [FunctionName("ArtistUploadDocumentsBlobStorage")]
        public static async Task<HttpResponseMessage> ArtistUploadDocumentsBlobStorage([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artists/ArtistUploadDocumentsBlobStorage")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var bfile = await req.ReadFileAsync();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    respVM.data = await service.UploadDocument(bfile.Content, bfile.Extension);
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

        [FunctionName("GetRatingLogFunction")]
        public static async Task<HttpResponseMessage> GetRatingLogFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artist/RatingLog/{id}/{requiredRecords}")]HttpRequestMessage req, [Logger] NatLogger logger, int id, int requiredRecords)
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


                    
                    ResponseViewModel<IEnumerable<ArtistRatingLogViewModel>> respVM = new ResponseViewModel<IEnumerable<ArtistRatingLogViewModel>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    IEnumerable<ArtistRatingLogModel> result = await service.GetArtistRatingLog(id, requiredRecords);
                    respVM.data = new ArtistRatingLogViewModel().FromServiceModelList(result); ;
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


        [FunctionName("GetAverageRating")]
        public static async Task<HttpResponseMessage> GetAverageRating([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artist/AverageRating/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
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
                    
                    ResponseViewModel<ArtistRatingViewModel> respVM = new ResponseViewModel<ArtistRatingViewModel>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    ArtistRatingModel result = await service.getaveragerating(id);

                    respVM.data = new ArtistRatingViewModel().FromServiceModel(result); ;
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


        #endregion


        #region  Methods for Artist Bank Account

        //GET Artist Bank Accounts by ID Method
        [FunctionName("GetArtistBankAccountsByIDFunction")]
        public static async Task<HttpResponseMessage> GetArtistBankAccountsByID_Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Artists/Account/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Get artist bank account by id"))
            {
                try
                {
                    
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistBankAccountService service = ArtistBankAccountService.GetInstance(logger);
                    respVM.data = new ArtistBankAccountViewModel() { }.FromServiceModelList(await service.GetByIdArtistBankAccountsAsync(int.Parse(id)));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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



        //INSERT Artist Bank Account  Method
        [FunctionName("SaveArtistBankAccountFunction")]
        public static async Task<HttpResponseMessage> SaveArtistBankAccount_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Artists/Accounts")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Artist Bank Account"))
            {
                try
                {
                    
                    var ArtistBankAccount = await req.Content.ReadAsAsync<ArtistBankAccountViewModel>();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    ArtistBankAccountService service = ArtistBankAccountService.GetInstance(logger);
                    respVM.data = await service.CreateArtistBankAccountsAsync(new ArtistBankAccountViewModel() { }.ToServiceModel(ArtistBankAccount));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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


        // UPDATE Artist Bank Account Method
        [FunctionName("UpdateArtistBankAccountFunction")]
        public static async Task<HttpResponseMessage> UpdateArtistBankAccount_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Artists/Accounts/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Update Artist Bank Account"))
            {
                try
                {
                    
                    var ArtistBankAccount = await req.Content.ReadAsAsync<ArtistBankAccountViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    ArtistBankAccountService service = ArtistBankAccountService.GetInstance(logger);
                    respVM.data = await service.UpdateArtistBankAccountsAsync(new ArtistBankAccountViewModel() { }.ToServiceModel(ArtistBankAccount));
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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


        //DELETE Artist Bank Account  Method
        [FunctionName("DeleteArtistBankAccountFunction")]
        public static async Task<HttpResponseMessage> DeleteBankAccount_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Artists/Accounts/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //Activate  Artist Bank Account Method
        [FunctionName("ActivateArtistBankAccountFunction")]
        public static async Task<HttpResponseMessage> ActivateArtistBankAccount_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Artists/Accounts/activate/{BankAccountId}")]HttpRequestMessage req, [Logger] NatLogger logger, string BankAccountId)
        {
            using (logger.BeginFunctionScope("Activate Artist Bank Account"))
            {
                try
                {
                    
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    ArtistBankAccountService service = ArtistBankAccountService.GetInstance(logger);
                    await service.ActivateArtistBankAccountAsync(BankAccountId);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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


        //Deactivate Artist Bank Account Method
        [FunctionName("DeactivateArtistBankAccountFunction")]
        public static async Task<HttpResponseMessage> DeactivateArtistBankAccount_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Artists/Accounts/deactivate/{BankAccountId}")]HttpRequestMessage req, [Logger] NatLogger logger, string BankAccountId)
        {
            using (logger.BeginFunctionScope("Deactivate Artist Bank Account"))
            {
                try
                {
                    
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    ArtistBankAccountService service = ArtistBankAccountService.GetInstance(logger);
                    await service.DeactivateArtistBankAccountAsync(BankAccountId);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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


        #region  Methods for Artist Event
        [FunctionName("BookArtistForEvent")]
        public static async Task<HttpResponseMessage> BookArtistForEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BookArtistEvent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Book Artist Event"))
            {
                try
                {
                    
                    var body = await req.GetBodyAsync<ArtistEventViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var ArtistEvent = body.Value;
                        ArtistService service = ArtistService.GetInstance(logger);
                        respVM.data = new ArtistEventViewModel().FromServiceModel(await service.BookArtistForEvent(new ArtistEventViewModel() { }.ToServiceModel(ArtistEvent)));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        
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

        [FunctionName("CancelArtistForEvent")]
        public static async Task<HttpResponseMessage> CancelArtistForEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "CancelArtistEvent/{eventcode}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode)
        {
            using (logger.BeginFunctionScope("Cancel Artist Event"))
            {
                try
                {
                    
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    ArtistService service = ArtistService.GetInstance(logger);

                    //Auth.UserModel UserModel = new Auth.UserModel();
                    //try
                    //{
                    //    UserModel = await Auth.Validate(req);
                    //}
                    //catch
                    //{
                    //    UserModel = null;
                    //}
                    //var artistId = UserModel != null ? UserModel.ReferenceId : null;

                    //respVM.data = await service.CancelArtistForEvent(eventcode, artistId);
                    respVM.data = await service.CancelArtistForEvent(eventcode);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
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


        [FunctionName("FindReplacementArtist")]
        public static async Task<HttpResponseMessage> FindReplacementArtist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "FindReplacementArtist")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    
                    var body = await req.Content.ReadAsAsync<FindReplacementArtistQueryViewModel>();
                    ResponseViewModel<IEnumerable<ArtistViewModel>> respVM = new ResponseViewModel<IEnumerable<ArtistViewModel>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    var respModel = await service.FindReplacementArtist(new FindReplacementArtistQueryViewModel().ToServiceModel(body));
                    respVM.data = new ArtistViewModel().FromServiceModelList(respModel);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
            }
        }


        //// UPDATE Artist Rating
        //[FunctionName("UpdateArtistRating")]
        //public static async Task<HttpResponseMessage> UpdateArtistRating_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ArtistRating/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        //{
        //    HttpResponseMessage Response;
        //    string Error_Message = "";
        //    int id_int = Convert.ToInt32(id);

        //    try
        //    {
        //        var Rating_data = await req.Content.ReadAsAsync<RatingDTO>();

        //        if (Rating_data != null)
        //        {
        //            log.Info("Updating Database");
        //            var Artist_Rating = unitOfWork.ArtistRating.Get(p => p.Artist_ID == id_int);
        //            var artistRating = Artist_Rating.FirstOrDefault();

        //            var avg = new AverageCalculator();
        //            artistRating.Average_Rating_Value = avg.NewRating(artistRating.Average_Rating_Value, Convert.ToInt32(artistRating.Number_Of_Ratings), Rating_data.RatingValue);

        //            //Adding ArtistRatingLog
        //            var RatingLog = new ArtistRatingLogDTO();
        //            var _ERatingLog = RatingLog.ToEFModel(RatingLog);
        //            _ERatingLog.Rating_Value = Rating_data.RatingValue;

        //            unitOfWork.ArtistRating.Update(artistRating);
        //            unitOfWork.ArtistRatingLog.Update(_ERatingLog);

        //            unitOfWork.Save();
        //            Response = req.CreateResponse(HttpStatusCode.OK);

        //        }
        //        else
        //        {
        //            Error_Message = "Failed to Save Artist Information";
        //            log.Error(Error_Message);
        //            Response = req.CreateErrorResponse(HttpStatusCode.BadRequest, Error_Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Error_Message = "Failed to parse employee";
        //        log.Error(ex.Message);
        //        Response = req.CreateErrorResponse(HttpStatusCode.BadRequest, Error_Message);
        //    }
        //    return Response;
        //}


        //GET Artist Bank Accounts by ID Method
        [FunctionName("GetArtistsByLocationFunction")]
        public static async Task<HttpResponseMessage> GetArtistsByLocation([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "GetArtistsByLocation")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Get artist bank account by id"))
            {
                try
                {
                    if (req.Method == HttpMethod.Options)
                    {
                        var o = req.CreateResponse();
                        o.Headers.Add("Access-Control-Allow-Origin", "*");
                        o.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                        o.Headers.Add("Access-Control-Allow-Headers", "*");
                        return o;
                    }
                    var body = await req.Content.ReadAsAsync<List<string>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    var userModel = await Auth.Validate(req);
                    respVM.data = await service.GetArtistsByLocation(body, userModel);

                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
            }
        }

        [FunctionName("CheckForDuplicateStageName")]
        public static async Task<HttpResponseMessage> CheckForDuplicateStageName([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Artist/StageNameExist")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    logger.LogInformation("Call Service Function to check for duplicate stagename");
                    var body = await req.Content.ReadAsAsync<StageNameCheckerViewModel>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    respVM.data = await service.CheckForDuplicateStageName(body.LocationList , body.StageName);
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

        [FunctionName("UpdateArtistCreditLimitFunction")]
        public static async Task<HttpResponseMessage> UpdateArtistCreditLimit([HttpTrigger(AuthorizationLevel.Anonymous, "options", "post", Route = "UpdateArtistCreditLimit")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Get artist bank account by id"))
            {
                try
                {
                    if (req.Method == HttpMethod.Options)
                    {
                        var o = req.CreateResponse();
                        o.Headers.Add("Access-Control-Allow-Origin", "*");
                        o.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                        o.Headers.Add("Access-Control-Allow-Headers", "*");
                        return o;
                    }
                    var body = await req.Content.ReadAsAsync<Dictionary<int, decimal>>();
                    ArtistService service = ArtistService.GetInstance(logger);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    service.UpdateArtistCreditLimit(body);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
                catch (ServiceLayerException ex)
                {
                    return ServiceLayerExceptionHandler.CreateResponse(ex);
                }
            }
        }
    }
}
