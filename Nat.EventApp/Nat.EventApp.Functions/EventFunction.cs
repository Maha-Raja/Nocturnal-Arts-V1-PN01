using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Linq.Expressions;
using Nat.Core.BaseModelClass;
using Nat.EventApp.Services;
using Nat.Core.Exception;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.KendoX.Extension;
using TLX.CloudCore.KendoX.UI;
using Nat.Core.Validations;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.EventApp.Functions.ViewModels;
using Nat.Core.Authentication;
using Nat.EventApp.Functions.ViewModel;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Common.Constants;
using Nat.Core.Zoom;

namespace Nat.EventApp.Functions
{
    public class EventFunction
    {

        [FunctionName("GetAllEventFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Events")]HttpRequestMessage req, [Logger] NatLogger logger)
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


        [FunctionName("GetSuggestedEventFunction")]
        public static async Task<HttpResponseMessage> GetSuggestedEventFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "SuggestedEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetSuggestedEventAsync(customerId));
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


        [FunctionName("GetAllEventListItemsFunction")]
        public static async Task<HttpResponseMessage> GetAllEventListItemsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventsListItems")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetAllEventListItems());
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


        [FunctionName("GetEventLovFunction")]
        public async static Task<HttpResponseMessage> GetEventLovFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Eventlov")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventLovViewModel().FromServiceModelList(await service.GetEventLov());
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


        [FunctionName("GetAllEventFilterFunction")]
        public static async Task<HttpResponseMessage> GetAllEventsFilterFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "EventsFilter")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Events by filter"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var EventSearch = body.Value;
                        EventService service = EventService.GetInstance(logger);
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
                        respVM.data = new EventViewModel().FromServiceModelList(await service.GetAllEventFilters(customerId, EventSearch.EventName, EventSearch.StartDate, EventSearch.EndDate, EventSearch.VenueCityCode, EventSearch.Artistid, EventSearch.PaintingId, EventSearch.Categoryid, EventSearch.MinTicketPrice, EventSearch.MaxTicketPrice, EventSearch.Sortid, EventSearch.SortAsc, EventSearch.ArtistRatingFilter, EventSearch.TicketPrice));
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



        [FunctionName("GetAllFeaturedEventsFunction")]
        public static async Task<HttpResponseMessage> GetAllFeaturedEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "FeaturedEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Faetured Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
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
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetAllFeaturedEventFilters(customerId));
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






        //[FunctionName("GetEventDetailFunction")]
        //public static async Task<HttpResponseMessage> GetEventDetailFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventsFilter")]HttpRequestMessage req, [Logger] NatLogger logger)
        //{
        //    if (req.Method == HttpMethod.Options)
        //    {
        //        var resp = req.CreateResponse();
        //        resp.Headers.Add("Access-Control-Allow-Origin", "*");
        //        resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
        //        resp.Headers.Add("Access-Control-Allow-Headers", "*");
        //        return resp;
        //    }
        //    using (logger.BeginFunctionScope("Get Slots by id"))
        //    {
        //        try
        //        {
        //            logger.LogRequest(req);
        //            //ResponseViewModel<IEnumerable <SlotViewModel>> respVM = new ResponseViewModel<IEnumerable <SlotViewModel>>();
        //            var body = await req.GetBodyAsync<EventViewModel>();
        //            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
        //            if (body.IsValid)
        //            {
        //                var EventSearch = body.Value;
        //                EventService service = EventService.GetInstance(logger);
        //                respVM.data = new EventViewModel().FromServiceModelList(await service.GetEventDetail(EventSearch.EventId));
        //                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
        //                logger.LogResponse(resp);
        //                resp.Headers.Add("Access-Control-Allow-Origin", "*");
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

        //GET Event by ID Method
        [FunctionName("GetEventDetailFunction")]
        public static async Task<HttpResponseMessage> GetEventDetailFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventDetail/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
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
                    respVM.data = new EventViewModel().FromServiceModel(await service.GetEventDetail(id, customerId));
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

        //GET Event by ID Method
        [FunctionName("GetEventByIDFunction")]
        public static HttpResponseMessage GetEventByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Events/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModel(service.GetByIdEvent(id));
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
        [FunctionName("GetfutureEventsByLocationFunction")]
        public static async Task<HttpResponseMessage> GetfutureEventsByLocationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "EventsbyLocation/{code}")] HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            using (logger.BeginFunctionScope("Get Event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    var respVM = new ResponseViewModel<IEnumerable<EventViewModel>>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetfutureEventsbylocation(code));
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
        [FunctionName("GetZoomMeetingSignature")]
        public static async Task<HttpResponseMessage> GetZoomMeetingSignature([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetZoomMeetingSignature/{hash}")]HttpRequestMessage req, [Logger] NatLogger logger, string hash)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Zoom Meeting signature"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.GetZoomMeetingSignatureAsync(hash);
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

        //GET Today Events by ArtistId and Venue Id Method
        [FunctionName("GetTodayEvent")]
        public static async Task<HttpResponseMessage> GetTodayEventAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "TodayEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    var queryParameters = req.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                    int? artistId = null;
                    int? venueId = null;
                    foreach (KeyValuePair<string, string> entry in queryParameters)
                    {
                        if (entry.Key == "ArtistId") { artistId = Convert.ToInt32(entry.Value); }
                        else if (entry.Key == "VenueId") { venueId = Convert.ToInt32(entry.Value); }
                    }
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetTodayEventWithArtistIdAndVenueId(artistId, venueId));
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

        //GET upcoming Events by ArtistId
        [FunctionName("GetUpcomingEventByArtistId")]
        public static async Task<HttpResponseMessage> GetUpcomingEventsByArtistIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "UpcomingEventsByArtistId/{artistId:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int artistId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get upcoming Event by artist id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        //UserModel = null;
                        UserModel.ReferenceId = artistId;
                    }
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetUpcomingEventsByArtistID(UserModel));
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

        //GET upcoming Events by ArtistId
        [FunctionName("GetUpcomingEventByVenueId")]
        public static async Task<HttpResponseMessage> GetUpcomingEventsByVenueIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "UpcomingEventsByVenueId/{venueId:int}")] HttpRequestMessage req, [Logger] NatLogger logger, int venueId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get upcoming Event by venue id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    Auth.UserModel userModel = new Auth.UserModel();
                    try
                    {
                        userModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        //UserModel = null;
                        userModel.ReferenceId = venueId;
                    }
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetUpcomingEventsByVenueID(userModel));
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


        //GET All Events by ArtistId
        [FunctionName("GetAllEventByArtistId")]
        public static async Task<HttpResponseMessage> GetAllEventsByArtistIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AllEventsByArtistId/{artistId:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int artistId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get upcoming Event by artist id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    Auth.UserModel userModel = new Auth.UserModel();
                    try
                    {
                        userModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        //UserModel = null;
                        userModel.ReferenceId = artistId;
                    }
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetAllEventsByArtistID(userModel));
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

        //GET All Events by ArtistId
        [FunctionName("GetAllEventsByVenueId")]
        public static async Task<HttpResponseMessage> GetAllEventsByVenueIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AllEventsByVenueId/{venueId:int}")] HttpRequestMessage req, [Logger] NatLogger logger, int venueId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get upcoming Event by venue id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    Auth.UserModel userModel = new Auth.UserModel();
                    try
                    {
                        userModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        //UserModel = null;
                        userModel.ReferenceId = venueId;
                    }
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetAllEventsByVenueID(userModel));
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

        //GET Happening Now Events by ArtistId
        [FunctionName("GetEventHappeningNowByArtistId")]
        public static async Task<HttpResponseMessage> GetEventHappeningNowByArtistIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventHappeningNowByArtistId/{artistId:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int artistId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event Happening now by artist id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModel(await service.GetEventsHappeningNowByArtistId(artistId));
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


        //GET Customer Booked Events and their details by customer Id Method
        [FunctionName("GetCustomerBookedEventsWithDetails")]
        public static async Task<HttpResponseMessage> GetCustomerBookedEventsWithDetails([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CustomerBookedEvents/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Customer Booked Events by Customer id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetCustomerEventsWithDetials(id));
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

        //GET Customer Liked Events and their details by customer Id Method
        [FunctionName("GetCustomerLikedEventsWithDetails")]
        public static async Task<HttpResponseMessage> GetCustomerLikedEventsWithDetails([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CustomerLikedEvents/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            using (logger.BeginFunctionScope("Get Customer Liked Events by Customer id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventViewModel().FromServiceModelList(await service.GetCustomerLikedEventsWithDetials(id));
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

        //GET Event by Code Method
        [FunctionName("GetEventByCodeFunction")]
        public static async Task<HttpResponseMessage> GetEventByCodeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Event/{code}")]HttpRequestMessage req, [Logger] NatLogger logger, string code)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event by code"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
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
                    respVM.data = new EventViewModel().FromServiceModel(await service.GetByCodeEvent(code, customerId));
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

        //Function to return all events details with respect to given codes
        [FunctionName("GetAllEventsWithCodes")]
        public static async Task<HttpResponseMessage> GetAllEventsWithCodes([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "EventWithCodes")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Events by filter"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventCodesViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var EventSearch = body.Value;
                        EventService service = EventService.GetInstance(logger);
                        respVM.data = new EventViewModel().FromServiceModelList(await service.GetEventsByCodes(body.Value.EventCodes));
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


        //GET Event SeatingPlan by ID Method
        [FunctionName("GetEventSeatingPlanFunction")]
        public static async Task<HttpResponseMessage> GetEventSeatingPlanFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Eventseating/{id:int}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event SeatingPlan by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventSeatingPlanViewModel> respVM = new ResponseViewModel<EventSeatingPlanViewModel>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = new EventSeatingPlanViewModel().FromServiceModel(await service.GetEventSeatingPlanByIdAsync(id));
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

        //Update Event Seat Status to booked
        [FunctionName("BookEventSeatsFunction")]
        public static async Task<HttpResponseMessage> BookEventSeatsFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "bookSeats")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Book Event Seats"))
            {
                try
                {
                    logger.LogRequest(req);
                    IEnumerable<SeatBookingViewModel> bookingInfo = await req.Content.ReadAsAsync<IEnumerable<SeatBookingViewModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.BookEventSeatsAsync(new SeatBookingViewModel() { }.ToServiceModelList(bookingInfo));
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
            try
            {
                using (logger.BeginFunctionScope("Get all Event"))
                {
                    Auth.UserModel UserModel = new Auth.UserModel();
                    UserModel = await Auth.Validate(req);
                    logger.LogRequest(req);
                    var Event = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    EventService service = EventService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllEventListAsync(Event, UserModel);
                    if (!result.Total.Equals(0))
                    {
                        result.Data = new EventViewModel().FromServiceModelList((IEnumerable<EventModel>)result.Data);
                    }
                    respVM.data = result;
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
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

        // Schedule Event in bulk
        [FunctionName("ScheduleEventsFunction")]
        public static async Task<HttpResponseMessage> ScheduleEvents([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "ScheduleEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Schedule Events in bulk"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<IEnumerable<EventViewModel>>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Event = body.Value;
                        EventService service = EventService.GetInstance(logger);
                        var serviceModelList = new EventViewModel() { }.ToServiceModelList(Event);
                        respVM.data = await service.ScheduleEvents(serviceModelList);
                        Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
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

        // CANCEL Event Information Method
        [FunctionName("CancelEventFunction")]
        public static async Task<HttpResponseMessage> CancelEventFunction_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "CancelEvent/{eventcode}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Cancel Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    respVM.data = await service.CancelEvent(eventcode);
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


        // INSERT Event Information Method
        [FunctionName("SaveEventSeatingPlanFunction")]
        public static async Task<HttpResponseMessage> SaveEventSeatingPlanFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EventSeating")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Save Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventSeatingPlanViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Seating = body.Value;
                        EventSeatingPlanService service = EventSeatingPlanService.GetInstance(logger);
                        respVM.data = service.CreateEventSeatingPlan(new EventSeatingPlanViewModel() { }.ToServiceModel(Seating));
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



        // UPDATE Event Information Method
        [FunctionName("UpdateEventFunction")]
        public static async Task<HttpResponseMessage> UpdateEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Events")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<EventViewModel>();
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
                    Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    Auth.UserModel UserModel = new Auth.UserModel();
                    try
                    {
                        UserModel = await Auth.Validate(req);

                    }
                    catch
                    {
                        UserModel = null;
                    }
                    respVM.data = new EventViewModel().FromServiceModel(await service.UpdateEventAsync(new EventViewModel().ToServiceModel(Event), UserModel));
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

        //Change Event Painting Information Method
        [FunctionName("MarkForPaintingChange")]
        public static async Task<HttpResponseMessage> MarkForPaintingChange([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Events/PaintingChange/{id}")] HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Activate Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    EventService service = EventService.GetInstance(logger);
                    await service.MarkForPaintingChange(id);
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


        //GET Event Seat Price by Event Code, Row Number, Seat Number Method
        [FunctionName("GetEventSeatPriceFunction")]
        public static async Task<HttpResponseMessage> GetEventSeatPriceFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetEventSeatPriceFunction")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("GET Event Seat Price by Event Code, Row Number, Seat Number"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventTicketViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var bodyValue = body.Value;
                        EventService service = EventService.GetInstance(logger);
                        respVM.data = await service.GetEventSeatPriceFunction(new EventTicketViewModel() { }.ToServiceModel(bodyValue));
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

        [FunctionName("SendEventReminderEmails")]
        public static async Task SendEventReminderEmails([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, [Logger] NatLogger logger)
        {
            EventService service = EventService.GetInstance(logger);                
            await service.SendReminderEmailsForUpcomingEventsAsync();
           
        }

        //[FunctionName("ChangePaintingOfMarkedEvents")]
        //public static async Task ChangePaintingOfMarkedEvents([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, [Logger] NatLogger logger)
        //{
        //    try
        //    {
        //        EventService service = EventService.GetInstance(logger);
        //        await service.UpdatePaintingOfMarkedEvents();
        //    }
        //    catch (Exception ex)
        //    {
        //        FunctionLayerExceptionHandler.Handle(ex, logger);
        //    }
        //}

        [FunctionName("CancelEventsWithIssues")]
        public static async Task CancelEventsWithIssues([TimerTrigger("0 0 */1 * * *")] TimerInfo myTimer, [Logger] NatLogger logger)
        {
            try
            {
                EventService service = EventService.GetInstance(logger);
                await service.CancelEventsWithIssues();
            }
            catch (Exception ex)
            {
                FunctionLayerExceptionHandler.Handle(ex, logger);
            }
        }


        // Event Feedback by an Artist
        [FunctionName("EventFeedback")]
        public static async Task<HttpResponseMessage> EventFeedback([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/Feedback")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Feedback for an Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<EventFeedbackViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Event = body.Value;
                        EventService service = EventService.GetInstance(logger);
                        //change service name
                        respVM.data = await service.EventFeedback(new EventFeedbackViewModel() { }.ToServiceModel(Event));
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


        // UPDATE Event Information Method
        [FunctionName("FeaturedEvent")]
        public static async Task<HttpResponseMessage> FeaturedEvent_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "FeaturedEvent/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<EventViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = service.FeaturedEvent(new EventViewModel().ToServiceModel(Event));
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



        // Bulk Update
        [FunctionName("BulkUpdate")]
        public static async Task<HttpResponseMessage> BulkUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkUpdate")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Feedback for an Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<IEnumerable<EventViewModel>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.BulkUpdate(new EventViewModel() { }.ToServiceModelList(Event));
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

        // Bulk Mark Artist Unavailable
        [FunctionName("BulkMarkArtistUnavailableByEventCode")]
        public static async Task<HttpResponseMessage> BulkMarkArtistUnavailableByEventCode([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkMarkArtistUnavailableByEventCode")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update artist of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var EventCodes = await req.Content.ReadAsAsync<List<String>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkMarkArtistUnavailableByEventCode(EventCodes);
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

        // Bulk Mark Artist Unavailable
        [FunctionName("BulkMarkArtistUnavailableByArtistID")]
        public static async Task<HttpResponseMessage> BulkMarkArtistUnavailableByArtistID([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkMarkArtistUnavailableByArtistID")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update artist of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var ArtistID = await req.Content.ReadAsAsync<Int32>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkMarkArtistUnavailableByArtistID(ArtistID);
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


        // Bulk Mark Venue Unavailable
        [FunctionName("BulkMarkVenueUnavailableByEventCodeFunction")]
        public static async Task<HttpResponseMessage> BulkMarkVenueUnavailableByEventCodeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkMarkVenueUnavailableByEventCodeFunction")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update artist of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var EventCodes = await req.Content.ReadAsAsync<List<String>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkMarkVenueUnavailableByEventCodeFunction(EventCodes);
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

        [FunctionName("BulkMarkVenueUnavailableByVenueID")]
        public static async Task<HttpResponseMessage> BulkMarkVenueUnavailableByVenueID([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkMarkVenueUnavailableByVenueID")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update artist of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var VenueID = await req.Content.ReadAsAsync<Int32>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkMarkVenueUnavailableByVenueID(VenueID);
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

        // Bulk Mark for painting change
        [FunctionName("BulkMarkForPaintingChange")]
        public static async Task<HttpResponseMessage> BulkMarkForPaintingChange([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkMarkForPaintingChange")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Mark For Painting Change"))
            {
                try
                {
                    logger.LogRequest(req);
                    var EventIds = await req.Content.ReadAsAsync<List<String>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkMarkForPaintingChange(EventIds);
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

        // Bulk Update Artist
        [FunctionName("BulkUpdateArtist")]
        public static async Task<HttpResponseMessage> BulkUpdateArtist([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkUpdateArtist")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update artist of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<IEnumerable<EventViewModel>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkUpdateArtist(new EventViewModel() { }.ToServiceModelList(Event));
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

        // Bulk Update Artist
        [FunctionName("BulkUpdateVenue")]
        public static async Task<HttpResponseMessage> BulkUpdateVenue([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "BulkUpdateVenue")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Bulk Update venue of Events"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Event = await req.Content.ReadAsAsync<IEnumerable<EventViewModel>>();

                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.BulkUpdateVenue(new EventViewModel() { }.ToServiceModelList(Event));
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

        // Remove artist association with an Event and mark event status to pending
        [FunctionName("RemoveArtistIDFromEventAndUpdateEventStatus")]
        public static async Task<HttpResponseMessage> RemoveArtistIDFromEventAndUpdateEventStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Event/{eventCode}/RemoveArtist")]HttpRequestMessage req, [Logger] NatLogger logger, String eventCode)
        {
            using (logger.BeginFunctionScope("Removing artist Id for a given Event Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.RemoveArtistIDFromEventAndUpdateEventStatus(eventCode);
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

        // Add artist to an Event and mark event status to confirmed
        [FunctionName("AddArtistForEventAndUpdateEventStatus")]
        public static async Task<HttpResponseMessage> AddArtistForEventAndUpdateEventStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Event/{eventCode}/AddArtist/{artistId}")]HttpRequestMessage req, [Logger] NatLogger logger, String eventCode, Int32 artistId)
        {
            using (logger.BeginFunctionScope("Adding artist Id for a given Event Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.AddArtistForEventAndUpdateEventStatus(eventCode, artistId);
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


        // Start Venue unavailability flow by inserting msg to queue
        [FunctionName("MarkVenueUnavailable")]
        public static async Task<HttpResponseMessage> MarkVenueUnavailable([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "MarkVenueUnavailable/{eventHash}")]HttpRequestMessage req, [Logger] NatLogger logger, String eventHash)
        {
            using (logger.BeginFunctionScope("Adding artist Id for a given Event Id"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.MarkVenueUnavailable(eventHash);
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

        [FunctionName("GetEventByHashFunction")]
        public static async Task<HttpResponseMessage> GetEventByHashFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventByHash/{hash}")]HttpRequestMessage req, [Logger] NatLogger logger, string hash)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Event by hash"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<EventViewModel> respVM = new ResponseViewModel<EventViewModel>();
                    EventService service = EventService.GetInstance(logger);
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
                    respVM.data = new EventViewModel().FromServiceModel(await service.GetByHashEvent(hash, customerId));
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

        [FunctionName("VenueUnavailable")]
        public async static void VenueUnavailable([QueueTrigger("startvenueunavailabilityflow", Connection = "AzureWebJobsStorage")]string queueMsg, [Logger] NatLogger logger)
        {

            using (logger.BeginFunctionScope("Update Availability"))
            {
                try
                {
                    EventService service = EventService.GetInstance(logger);
                    await service.VenueUnavailable(queueMsg);
                }
                catch (ServiceLayerException ex)
                {
                    ServiceLayerExceptionHandler.Handle(ex, logger);
                }
                catch (Exception ex)
                {
                    FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        // CANCEL Event Information Method
        [FunctionName("UpdateEventVenueFunction")]
        public async static Task<HttpResponseMessage> UpdateEventVenueFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "UpdateEventVenue/{eventcode}/{venueId}/{venuename}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode, int venueId, String venueName)
        {
            using (logger.BeginFunctionScope("Cancel Event"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    if(req.Headers.Authorization != null)
                        Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    respVM.data = await service.UpdateEventVenue(eventcode, venueId, venueName);
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

        // Start Artist unavailability flow by inserting msg to queue
        [FunctionName("MarkArtistUnavailable")]
        public static async Task<HttpResponseMessage> MarkArtistUnavailable([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "MarkArtistUnavailable/{eventHash}")]HttpRequestMessage req, [Logger] NatLogger logger, String eventHash)
        {
            using (logger.BeginFunctionScope("Adding artist Id for a given Event Id"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    if(req.Headers.Authorization != null)
                        Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);

                    await service.MarkArtistUnavailable(eventHash);
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

        [FunctionName("MarkArtistUnavailableByEventCodeFunction")]
        public static async Task<HttpResponseMessage> MarkArtistUnavailableByEventCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "MarkArtistUnavailableByEventCode/{eventCode}")]HttpRequestMessage req, [Logger] NatLogger logger, String eventCode)
        {
            using (logger.BeginFunctionScope("Adding artist Id for a given Event Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.MarkArtistUnavailableByEventCode(eventCode);
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

        [FunctionName("MarkVenueUnavailableByEventCodeFunction")]
        public static async Task<HttpResponseMessage> MarkVenueUnavailableByEventCodeFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "MarkVenueUnavailableByEventCodeFunction/{eventCode}")] HttpRequestMessage req, [Logger] NatLogger logger, String eventCode)
        {
            using (logger.BeginFunctionScope("Adding artist Id for a given Event Id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    await service.MarkVenueUnavailableByEventCodeFunction(eventCode);
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

        [FunctionName("ArtistUnavailable")]
        public async static void ArtistUnavailable([QueueTrigger("startartistunavailabilityflow", Connection = "AzureWebJobsStorage")]string queueMsg, [Logger] NatLogger logger)
        {

            using (logger.BeginFunctionScope("Update Availability"))
            {
                try
                {
                    EventService service = EventService.GetInstance(logger);
                    await service.ArtistUnavailable(queueMsg);
                }
                catch (ServiceLayerException ex)
                {
                    ServiceLayerExceptionHandler.Handle(ex, logger);
                }
                catch (Exception ex)
                {
                    FunctionLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        // CANCEL Event Information Method
        //[FunctionName("UpdateEventArtistFunction")]
        //public async static Task<HttpResponseMessage> UpdateEventArtist([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "UpdateEventArtist/{eventcode}/{artistId}/{venuename}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventcode, int artistId, String venueName)
        //{
        //    using (logger.BeginFunctionScope("Cancel Event"))
        //    {
        //        try
        //        {
        //            logger.LogRequest(req);

        //            ResponseViewModel<object> respVM = new ResponseViewModel<object>();
        //            EventService service = EventService.GetInstance(logger);
        //            respVM.data = await service.UpdateEventArtist(eventcode, artistId, venueName);
        //            var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
        //            resp.Headers.Add("Access-Control-Allow-Origin", "*");
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


        [FunctionName("UpdateEventArtistFunction")]
        public static async Task<HttpResponseMessage> UpdateEventArtist([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "UpdateEventArtist/{eventcode}/{artistId}/{venuename}")] HttpRequestMessage req, [Logger] NatLogger logger, string eventcode, int artistId, String venueName)
        {
            using (logger.BeginFunctionScope("get events of past x days"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    if(req.Headers.Authorization != null)
                        Nat.Core.ServiceClient.NatClient.SetToken(req.Headers.Authorization.Scheme);
                    respVM.data = await service.UpdateEventArtist(eventcode, artistId, venueName);
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

        [FunctionName("GetEventsForOrderCreationFunction")]
        public static async Task<HttpResponseMessage> GetEventsForOrderCreationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetEventsForOrderCreation")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<List<EventModel>> respVM = new ResponseViewModel<List<EventModel>>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.GetEventsForOrderCreation();
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

        [FunctionName("FetchCompletedEventsFunction")]
        public static async Task<HttpResponseMessage> FetchCompletedEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "FetchCompletedEvents")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("Update Event"))
            {

                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<List<EventModel>> respVM = new ResponseViewModel<List<EventModel>>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.FetchCompletedEvents();
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

        [FunctionName("GetPastXDaysEvent")]
        public static async Task<HttpResponseMessage> GetPastXDaysEvent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Events/PastDays/{days}")]HttpRequestMessage req, [Logger] NatLogger logger, int days)
        {
            using (logger.BeginFunctionScope("get events of past x days"))
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
                    ResponseViewModel<List<EventModel>> respVM = new ResponseViewModel<List<EventModel>>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.GetPastXDaysEvent(days);
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

        [FunctionName("UpdateEventPainting")]
        public static async Task<HttpResponseMessage> UpdateEventPainting([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "UpdateEventPainting/{eventId}/{paintingId}")]HttpRequestMessage req, [Logger] NatLogger logger, int eventId, int paintingId)
        {
            using (logger.BeginFunctionScope("update event painting"))
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
                    ResponseViewModel<List<EventModel>> respVM = new ResponseViewModel<List<EventModel>>();
                    EventService service = EventService.GetInstance(logger);
                    await service.UpdateEventPainting(eventId, paintingId);
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

        [FunctionName("SaveEventGalleryPictures")]
        public static async Task<HttpResponseMessage> SaveEventGalleryPictures([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SaveEventGalleryPictures")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("update event painting"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    var eventImages = await req.Content.ReadAsAsync<EventGalleryPicturesViewModel>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.SaveEventGalleryPictures(new EventGalleryPicturesViewModel().ToServiceModel(eventImages));
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

        [FunctionName("GetUpcomingEventByArtistIdFunction")]
        public static async Task<HttpResponseMessage> GetUpcomingEventsOfArtistByArtistId([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetUpcomingEventsOfArtistByArtistId/{artistId}")]HttpRequestMessage req, [Logger] NatLogger logger, int artistId)
        {
            using (logger.BeginFunctionScope("in GetUpcomingEventByArtistIdFunction"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    
                    respVM.data = await service.GetUpcomingEventByArtistId(artistId);
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

        [FunctionName("GetEventsOfArtistByArtistIdFunction")]
        public static async Task<HttpResponseMessage> GetEventsOfArtistByArtistId([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetEventsOfArtistByArtistId/{artistId}")]HttpRequestMessage req, [Logger] NatLogger logger, int artistId)
        {
            using (logger.BeginFunctionScope("in GetEventsOfArtistByArtistIdFunction"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = await service.GetEventsOfArtistByArtistId(artistId);
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

        [FunctionName("GetEventPicturesFunction")]
        public static async Task<HttpResponseMessage> GetEventPictures([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetEventPictures/{eventCode}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventCode)
        {
            using (logger.BeginFunctionScope("get event pictures"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    EventService service = EventService.GetInstance(logger);
                    respVM.data = service.GetEventPictures(eventCode);
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


        [FunctionName("ZoomEvent")]
        public static async Task<HttpResponseMessage> ZoomEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "ZoomEvent")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            using (logger.BeginFunctionScope("hanlde zoom event"))
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
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    var zoomEvent = await req.Content.ReadAsAsync<ZoomEvent>();
                    EventService service = EventService.GetInstance(logger);
                    await service.HandleZoomEvent(zoomEvent);
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

        [FunctionName("GetAllBookedTicketFunction")]
        public static async Task<HttpResponseMessage> GetAllBookedTicketFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "BookedTicket")] HttpRequestMessage req, [Logger] NatLogger logger)
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
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    EventService service = EventService.GetInstance(logger);
                    var result = service.GetAllBookedTickets(req.ToDataSourceRequest());
                    if(result.Total > 0)                    
                    result.Data = new BookedTicketViewModel().FromServiceModelList((IEnumerable<BookedTicketModel>)result.Data);
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



    }
}