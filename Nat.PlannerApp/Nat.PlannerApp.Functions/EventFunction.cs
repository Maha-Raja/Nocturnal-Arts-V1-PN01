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
    public class EventFunction
    {

        #region  Methods for Event  
        /// <summary>
        /// Get list of all Event
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllEventFunction")]
        public static HttpResponseMessage GetAllEventFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Events")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModelList(service.GetAllEvent());
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

        //GET Event by ID Method
        [FunctionName("GetEventByIDFunction")]
        public async static Task<HttpResponseMessage> GetEventByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModel(await service.GetByIdEventAsync(id));
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

        [FunctionName("SaveEventFunction")]
        public static async Task<HttpResponseMessage> SaveEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PlannerEvent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Event = body.Value;
                        EventService service = EventService.GetInstance(logger);
                        respVM.data = await service.CreateEvent(new EventViewModel() { }.ToServiceModel(Event));
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

        [FunctionName("CancelEventFunction")]
        public static async Task<HttpResponseMessage> CancelEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "CancelPlannerEvent/{id}/{eventcode}")]HttpRequestMessage req, [Logger] NatLogger logger, int id, string eventcode)
        {
            using (logger.BeginFunctionScope("Cancel Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.CancelEvent(id, eventcode);
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


        // UPDATE Event Information Method
        [FunctionName("UpdateEventFunction")]
        public static async Task<HttpResponseMessage> UpdateEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Event")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<EventViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.UpdateEventAsync(new EventViewModel().ToServiceModel(Event));
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

        /// <summary>
        /// Get list of all Event
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListEventFunction")]
        public static async Task<HttpResponseMessage> GetListEventFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Events/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    EventService service = EventService.GetInstance(logger);
                    DataSourceResult result = service.GetAllEventList(Event);
                    result.Data = new EventViewModel().FromServiceModelList((IEnumerable<EventModel>)result.Data);
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


        /// <summary>
        /// Get List of Colliding Events according to events duration list passed as parameter
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns>Event Colliding Model</returns>
        [FunctionName("GetCollidingEvents")]
        public static async Task<HttpResponseMessage> GetCollidingEvents([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "GetCollidingEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventCollidingViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        EventService service = EventService.GetInstance(logger);
                        respVM.data = await service.GetCollidingEvents(new EventCollidingViewModel() { }.ToServiceModel(body.Value));
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

       
    }
}
