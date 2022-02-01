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
    public class PlannerFunction
    {

        #region  Methods for Planner  

        /// <summary>
        /// Get list of all Planner
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetAllPlannerFunction")]
        public static HttpResponseMessage GetAllPlannerFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Planners")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Planner"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    PlannerService service = PlannerService.GetInstance(logger);
                    respVM.data = new PlannerViewModel().FromServiceModelList(service.GetAllPlanner());
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

        //GET Planner by ID Method
        [FunctionName("GetPlannerByIDFunction")]
        public async static Task<HttpResponseMessage> GetPlannerByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Planner/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Planner by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<PlannerViewModel> respVM = new ResponseViewModel<PlannerViewModel>();
                    PlannerService service = PlannerService.GetInstance(logger);
                    respVM.data = new PlannerViewModel().FromServiceModel(await service.GetByIdPlannerAsync(id));
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

        [FunctionName("SavePlannerFunction")]
        public static async Task<HttpResponseMessage> SavePlanner_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Planner")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Planner"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PlannerViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Planner = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.CreatePlannerAsync(new PlannerViewModel() { }.ToServiceModel(Planner));
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

        //update planner type
        [FunctionName("UpdatePlannertypeFunction")]
        public static async Task<HttpResponseMessage> Updateplannertype([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "UpdatePlannerType/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("update planner type"))
            {
                try
                {
                    logger.LogRequest(req);
                    
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();

                       
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.plannertypeUpdateAsync(id);
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

        // UPDATE Planner Information Method
        [FunctionName("UpdatePlannerFunction")]
        public static async Task<HttpResponseMessage> UpdatePlanner_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Planner")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Planner"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Planner = await req.Content.ReadAsAsync<PlannerViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    PlannerService service = PlannerService.GetInstance(logger);
                    respVM.data = await service.UpdatePlannerAsync(new PlannerViewModel().ToServiceModel(Planner));
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

        //DELETE Planner Information Method
        [FunctionName("DeletePlannerFunction")]
        public static async Task<HttpResponseMessage> DeletePlanner_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Planner/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Delete Planner"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    PlannerService service = PlannerService.GetInstance(logger);
                    await service.DeactivatePlannerAsync(id);
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
        /// Get list of all Slots
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListSlotFunction")]
        public static async Task<HttpResponseMessage> GetListSlotFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Slots/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all slots"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Slot = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    AvailabilityService service = AvailabilityService.GetInstance(logger);
                    DataSourceResult result = service.GetAllSlotList(Slot);
                    result.Data = new SlotViewModel().FromServiceModelList((IEnumerable<SlotModel>)result.Data);
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



        // INSERT Artist Information Method
        [FunctionName("SearchPlannerByTypeAndAvailability")]
        public static async Task<HttpResponseMessage> SearchPlannerByTypeAndAvailability([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "planner/search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PlannerCustomSearchViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PlannerSearch = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = service.CustomSearchPlanner(new PlannerCustomSearchViewModel() { }.ToServiceModel(PlannerSearch));
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


        // INSERT Artist Information Method
        [FunctionName("SearchPlannerByTypeAndAvailabilityChange")]
        public static async Task<HttpResponseMessage> SearchPlannerByTypeAndAvailabilityChange([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "planner/searchchange")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PlannerCustomSearchViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PlannerSearch = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.CustomSearchPlannerAsyncchange(new PlannerCustomSearchViewModel() { }.ToServiceModel(PlannerSearch));
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

        [FunctionName("GetPlannerCollidingEvent")]
        public static async Task<HttpResponseMessage> GetPlannerCollidingEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "planner/collidingevent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Return all Planner that are colliding"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<PlannerCustomSearchViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PlannerSearch = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.GetPlannerCollidingEvent(new PlannerCustomSearchViewModel() { }.ToServiceModel(PlannerSearch));
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



        [FunctionName("WarningByPlannerId")]
        public static async Task<HttpResponseMessage> WarningByPlannerId([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Planner/warning")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Slots by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    // ResponseViewModel<IEnumerable <SlotViewModel>> respVM = new ResponseViewModel<IEnumerable <SlotViewModel>>();
                    var body = await req.GetBodyAsync<SlotViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PlannerSearch = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);



                        respVM.data = new SlotWarningViewModel().FromServiceModel(await service.WarningByPlannerIdAsync(new SlotViewModel() { }.ToServiceModel(PlannerSearch)));
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

        [FunctionName("GetSlotsByPlannerIdFunction")]
        public static async Task<HttpResponseMessage> GetSlotsByPlannerId([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SearchSlots")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Slots by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<SlotSearchModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var PlannerSearch = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        var plannerIds = PlannerSearch.PlannerId != null ? new List<int>() { PlannerSearch.PlannerId ?? default } : PlannerSearch.PlannerIds;
                        respVM.data = new SlotViewModel().FromServiceModelList(await service.SearchSlotInPlanner(plannerIds, Convert.ToDateTime(PlannerSearch.StartTime), Convert.ToDateTime(PlannerSearch.EndTime)));
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


        [FunctionName("AddSlot")]
        public static async Task<HttpResponseMessage> AddSlot([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "AddSlot")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Planner"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<SlotViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var slot = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.CreateSlotAsync(new SlotViewModel() { }.ToServiceModel(slot));
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

        [FunctionName("UpdateSlots")]

        public static async Task<HttpResponseMessage> UpdateSlots([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "UpdateSlots")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Slots"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<List<SlotViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var slot = body.Value;
                        PlannerService service = PlannerService.GetInstance(logger);
                        respVM.data = await service.UpdateSlotsAsync(new SlotViewModel() { }.ToServiceModelList(slot));
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


        //DELETE Planner Information Method
        [FunctionName("DeleteSlotFunction")]
        public static async Task<HttpResponseMessage> DeleteSlotFunction([HttpTrigger(AuthorizationLevel.Anonymous, "delete", "options",  Route = "Slot/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Delete slot"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<SlotModel> respVM = new ResponseViewModel<SlotModel>();
                    PlannerService service = PlannerService.GetInstance(logger);
                    respVM.data =  await service.deleteslotbyid(id);
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


        #endregion
    }
}
