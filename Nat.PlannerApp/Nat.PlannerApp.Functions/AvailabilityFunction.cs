using System; 
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
using Nat.PlannerApp.Functions.ViewModels;
using Nat.PlannerApp.Services;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.KendoX.Extension;
using Microsoft.Extensions.Logging;
using TLX.CloudCore.KendoX.UI;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.Exception;
using Nat.Core.Validations;

namespace Nat.PlannerApp.Functions
{
    public class AvailabilityFunction
    {

        #region  Methods for Availability  

        /// <summary>
        /// Get list of all Availability
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllAvailabilityFunction")]
        public static HttpResponseMessage GetAllAvailabilityFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Availability")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Availability"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    respVM.data = new AvailabilityViewModel().FromServiceModelList(service.GetAllAvailability());
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
                    return FunctionLayerExceptionHandler.Handle(ex,logger);
                }
            }
        }

        //GET Availability by ID Method
        [FunctionName("GetAvailabilityByIDFunction")]
        public async static Task<HttpResponseMessage> GetAvailabilityByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Availability/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Availability by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<AvailabilityViewModel> respVM = new ResponseViewModel<AvailabilityViewModel>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    respVM.data = new AvailabilityViewModel().FromServiceModel(await service.GetByIdAvailabilityAsync(id));
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

        //Add one more day slot
        //  public async Task AdditionalSlot([TimerTrigger("1 * * * * *")]TimerInfo myTimer, [Logger] NatLogger logger)
        [FunctionName("AdditionalSlot")]
        public async static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, [Logger] NatLogger logger)
        {

            using (logger.BeginFunctionScope("Update Availability"))
            {
                try
                {
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    await service.AddAvailabilityDay();
                }
                catch (ServiceLayerException ex)
                {
                }
                catch (Exception ex)
                {
                }
            }
        }

        [FunctionName("GetAvailabilityByPlannerIDFunction")]
        public static HttpResponseMessage GetAvailabilityByPlannerIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Availability/Planner/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Availability by Planner id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    respVM.data = new AvailabilityViewModel().FromServiceModelList(service.GetByPlannerIdAvailability(id));
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

        [FunctionName("SaveAvailabilityFunction")]
        public static async Task<HttpResponseMessage> SaveAvailability_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Availability")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Availability"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<AvailabilityViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Availability = body.Value;
                        AvailabilityService service = AvailabilityService.GetInstance(logger);
                        respVM.data = await service.CreateAvailabilityAsync(new AvailabilityViewModel() { }.ToServiceModelList(Availability));
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

        // UPDATE Availability Information Method
        [FunctionName("UpdateAvailabilityFunction")]
        public static async Task<HttpResponseMessage> UpdateAvailability_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put",  Route = "Availability")]HttpRequestMessage req, [Logger] NatLogger logger)
        {

            using (logger.BeginFunctionScope("Update Availability"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Availability = await req.Content.ReadAsAsync<IEnumerable<AvailabilityViewModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    respVM.data = await service.UpdateAvailabilityAsync(new AvailabilityViewModel().ToServiceModelList(Availability));
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

        // UPDATE Availability Information Method
        [FunctionName("BulkUpdateAvailabilityFunction")]
        public static async Task<HttpResponseMessage> BulkUpdateAvailability_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkAvailability")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Availability"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Availability = await req.Content.ReadAsAsync<IEnumerable<AvailabilityViewModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    respVM.data = await service.UpdateEntityAvailabilityAsync(new AvailabilityViewModel().ToServiceModelList(Availability));
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
