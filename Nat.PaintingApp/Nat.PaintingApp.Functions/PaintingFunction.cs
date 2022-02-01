using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using Nat.Core.BaseModelClass;
using Nat.PaintingApp.Services;
using Nat.Core.Exception;
using Nat.PaintingApp.Functions.ViewModels;
using Nat.Core.KendoX.Extension;
using Nat.Core.Validations;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.KendoX.UI;
using System.Collections.Generic;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.Core.Http.Extension;
using Nat.PaintingApp.Functions.ViewModel;
using Nat.Core.Authentication;

namespace Nat.PaintingApp.Functions
{
    public static class PaintingFunction
    {


        [FunctionName("GetPaintingLovFunction")]
        public async static Task<HttpResponseMessage> GetPaintingsLovFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Paintinglov")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Painting Lov"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = await service.GetPaintingLov();
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

        [FunctionName("CheckDuplicateName")]
        public async static Task<HttpResponseMessage> CheckDuplicateName([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Painting/alreadyused/{code}/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string code, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Check Duplicate Name"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = await service.checkForDuplicateName(code, id);
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



        [FunctionName("GetAllPaintingFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Paintings")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = new PaintingViewModel().FromServiceModelList(service.GetAllPainting());
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
        //GET Painting by ID Method
        [FunctionName("GetPaintingByIDFunction")]
        public static async Task<HttpResponseMessage> GetPaintingByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Paintings/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Painting by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<PaintingViewModel> respVM = new ResponseViewModel<PaintingViewModel>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = new PaintingViewModel().FromServiceModel(await service.GetByIdPainting(int.Parse(id)));
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

        //GET Painting by Artist ID Method
        [FunctionName("GetPaintingByArtistIDFunction")]
        public static async Task<HttpResponseMessage> GetPaintingByArtistIDFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Paintings/Artist/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Painting by artist id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = new PaintingViewModel().FromServiceModelList(await service.GetPaintingByArtistIdAsync((int.Parse(id))));
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


        [FunctionName("FindPaintingsForEvent")]
        public static async Task<HttpResponseMessage> FindPaintingsForEvent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Paintings/sorted/{sort}")]HttpRequestMessage req, [Logger] NatLogger logger, string sort)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Painting by sort order"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = new PaintingViewModel().FromServiceModelList(await service.FindPaintingsForEvent(sort));
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
        /// Get list of all Painting
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListPaintingFunction")]
        public static async Task<HttpResponseMessage> GetListPaintingFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Paintings/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Painting"))
            {
                try
                {
                    Auth.UserModel UserModel = new Auth.UserModel();
                    UserModel = await Auth.Validate(req);
                    logger.LogRequest(req);
                    var Painting = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllPaintingList(Painting,UserModel);
                    result.Data = new PaintingViewModel().FromServiceModelList((IEnumerable<PaintingModel>)result.Data);
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


        // INSERT Painting Information Method
        [FunctionName("SavePaintingFunction")]
        public static async Task<HttpResponseMessage> SavePainting_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Paintings")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PaintingViewModel>();
                    Auth.UserModel UserModel = await ValidateUser(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Painting = body.Value;
                        Painting.CreatedBy = UserModel.UserName;
                        Painting.LastUpdatedBy = Painting.CreatedBy;
                        PaintingService service = PaintingService.GetInstance(logger);
                        respVM.data = service.CreatePainting(new PaintingViewModel() { }.ToServiceModel(Painting));
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

        // UPDATE Painting Information Method
        [FunctionName("UpdatePaintingFunction")]
        public static async Task<HttpResponseMessage> UpdatePainting_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Paintings/Edit/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {

            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    PaintingViewModel Painting = await req.Content.ReadAsAsync<PaintingViewModel>();
                    Auth.UserModel UserModel = await ValidateUser(req);
                    ResponseViewModel<PaintingViewModel> respVM = new ResponseViewModel<PaintingViewModel>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    Painting.LastUpdatedBy = UserModel.UserName;
                    respVM.data = new PaintingViewModel().FromServiceModel(await service.UpdatePainting(new PaintingViewModel().ToServiceModel(Painting)));
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

        //DELETE Painting Information Method
        [FunctionName("DeletePaintingFunction")]
        public static async Task<HttpResponseMessage> DeletePainting_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Paintings/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //Activate Painting Information Method
        [FunctionName("ActivatePaintingFunction")]
        public static async Task<HttpResponseMessage> ActivatePainting_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Paintings/Activate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Activate Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel userModel = await ValidateUser(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    service.ActivatePainting(id, userModel.UserName);
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


        //Deactivate Painting Information Method
        [FunctionName("DeactivatePaintingFunction")]
        public static async Task<HttpResponseMessage> DeactivatePainting_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Paintings/Deactivate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Deactivate Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    Auth.UserModel userModel = await ValidateUser(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    service.DeactivatePainting(id, userModel.UserName);
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


        //blobstorage testing
        [FunctionName("TestingBlobStorage")]
        public static async Task<HttpResponseMessage> TestingBlobStorage([HttpTrigger(AuthorizationLevel.Anonymous, "post","options", Route = "Paintings/test")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    var bfile = await req.ReadFileAsync();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = await service.UploadImage(bfile.Content);
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



        //blobstorage video upload
        [FunctionName("UploadVideoFunction")]
        public static async Task<HttpResponseMessage> UploadVideoFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Paintings/videoupload")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Store video on Blob"))
            {
                try
                {
                    logger.LogRequest(req);
                    var bfile = await req.ReadFileAsync();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = await service.UploadVideo(bfile.Content);
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

        [FunctionName("UploadAttachment")]
        public static async Task<HttpResponseMessage> UploadAttachment([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Paintings/UploadAttachment")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Upload attachment on Blob"))
            {
                try
                {
                    logger.LogRequest(req);
                    var bfile = await req.ReadFileAsync();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = await service.UploadAttachment(bfile.Content, bfile.Extension);
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

        [FunctionName("SubmitPaintingForApproval")]
        public static async Task<HttpResponseMessage> SubmitPaintingForApproval([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Paintings/submit")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Submit Painting For Approval"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PaintingRequestsViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var painting = body.Value;
                        
                        PaintingService service = PaintingService.GetInstance(logger);
                        var appservice = new PaintingRequestsViewModel() { }.ToServiceModel(painting);
                        respVM.data = await service.SubmitPaintingForApproval(appservice);
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

        //retrieve pending paintings from table storage...................
        //[FunctionName("RetrievePendingPainting")]
        //public static async Task<HttpResponseMessage> RetrievePendingPainting([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Painting")]HttpRequestMessage req, [Logger] NatLogger logger)
        //{
        //    if (req.Method == HttpMethod.Options)
        //    {
        //        var resp = req.CreateResponse();
        //        resp.Headers.Add("Access-Control-Allow-Origin", "*");
        //        resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
        //        resp.Headers.Add("Access-Control-Allow-Headers", "*");
        //        return resp;
        //    }
        //    using (logger.BeginFunctionScope("Get all pending paintings request"))
        //    {
        //        try
        //        {
        //            logger.LogRequest(req);
        //            logger.LogInformation("Test message");
        //            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
        //            PaintingService service = PaintingService.GetInstance(logger);
        //            respVM.data = await service.GetPendingPaintingRequestAsync();
        //            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
        //            logger.LogResponse(resp);
        //            return resp;
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
        //approve pending painting from table storage.............
        //[FunctionName("ApprovePaintingRequest")]
        //public static async Task<HttpResponseMessage> ApprovePaintingRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Painting/approve")]HttpRequestMessage req, [Logger] NatLogger logger)
        //{
        //    using (logger.BeginFunctionScope("Approve painting"))
        //    {
        //        try
        //        {
        //            logger.LogRequest(req);
        //            var body = await req.GetBodyAsync<PaintingForApprovalViewModel>();
        //            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
        //            if (body.IsValid)
        //            {
        //                var painting = body.Value;
        //                PaintingService service = PaintingService.GetInstance(logger);
        //                respVM.data = await service.ApprovePendingPaintingRequestAsync(new PaintingForApprovalViewModel() { }.ToServiceModel(painting));
        //                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
        //                logger.LogResponse(resp);
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

        //returns all the paintings according to the status and artist id
        [FunctionName("GetPaintingByTypeFunction")]
        public static async Task<HttpResponseMessage> GetPaintingByTypeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "PaintingRequest/{type}/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string type, int id)
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
                    var Painting = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    DataSourceResult result = await service.GetPaintingByType(Painting, type, id);
                    result.Data = new PaintingRequestsViewModel().FromServiceModelList((IEnumerable<PaintingRequestsModel>)result.Data);

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





        //returns all the previously approved and rejected paintings of a specific artist
        [FunctionName("GetPreviousPaintingRequests")]
        public static async Task<HttpResponseMessage> GetPreviousPaintingRequests([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "PreviousPaintingRequest")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("all the previously approved and rejected paintings of a specific artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Painting = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    DataSourceResult result = await service.GetPreviousPaintingsByArtist(Painting);
                    result.Data = new PaintingRequestsViewModel().FromServiceModelList((IEnumerable<PaintingRequestsModel>)result.Data);

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




        // UPDATE Painting Status Method
        [FunctionName("UpdatePaintingStatus")]
        public static async Task<HttpResponseMessage> UpdatePaintingStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Paintings/status")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Painting Status"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Painting = await req.Content.ReadAsAsync<PaintingRequestsViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    Auth.UserModel UserModel = await ValidateUser(req);
                    Painting.LastUpdatedBy = UserModel.UserName;
                    respVM.data = await service.UpdatePaintingStatusAsync(new PaintingRequestsViewModel().ToServiceModel(Painting));
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

        [FunctionName("UpdateStatusAsync")]
        public static async Task<HttpResponseMessage> UpdateStatusAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Paintings/UpdateStatusAsync")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Painting Status"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Painting = await req.Content.ReadAsAsync<PaintingViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    Auth.UserModel userModel = await ValidateUser(req);
                    Painting.LastUpdatedBy = userModel.UserName;
                    respVM.data = await service.UpdateStatusAsync(new PaintingViewModel().ToServiceModel(Painting));
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


        [FunctionName("CreatePainting")]
        public static async Task<HttpResponseMessage> CreatePainting([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Paintings/create")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Create Painting"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PaintingRequestsViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var painting = body.Value;

                        PaintingService service = PaintingService.GetInstance(logger);
                        var appservice = new PaintingRequestsViewModel() { }.ToServiceModel(painting);
                        appservice.JsonData = JsonConvert.SerializeObject(painting);
                        if (painting.StatusLkp != null)
                            appservice.Status = painting.StatusLkp;

                        Auth.UserModel UserModel = await ValidateUser(req);
                        appservice.LastUpdatedBy = UserModel.UserName;
                        appservice.CreatedBy = UserModel.UserName;
                        respVM.data = new PaintingViewModel().FromServiceModel(await service.CreatePaintingasync(appservice));
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


        [FunctionName("BookPaintingForEvent")]
        public static async Task<HttpResponseMessage> BookPaintingForEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BookPaintingEvent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Book Painting For a Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PaintingEventViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PaintingEvent = body.Value;
                      PaintingService service = PaintingService.GetInstance(logger);
                        respVM.data = await service.BookPaintingEvent(new PaintingEventViewModel() { }.ToServiceModel(PaintingEvent)); 
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




        [FunctionName("CancelPaintingBookingForEvent")]
        public static async Task<HttpResponseMessage> CancelPaintingBookingForEvent([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "CancelPaintingEvent/{eventcode}/{paintingId}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode, int paintingId)
        {
            using (logger.BeginFunctionScope("Cancel Painting Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PaintingService service = PaintingService.GetInstance(logger);
                    respVM.data = service.CancelPaintingForEvent(eventcode,paintingId);
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
