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

using Nat.PlannerApp.Services.ServiceModels;
using Nat.PlannerApp.Models.EFModel;
using Nat.PlannerApp.Models.Repositories;
using Nat.PlannerApp.Functions.ViewModels;
using Nat.Core.ServiceClient;

namespace Nat.PlannerApp.Services
{
    public class EventService : BaseService<EventModel, NAT_PLS_Event>
    {
        private static EventService _service;
        public static EventService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new EventService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private EventService() : base()
        {

        }

        /// <summary>
        /// This method return list of all events
        /// </summary>
        /// <returns>Collection of event service model<returns>
        public IEnumerable<EventModel> GetAllEvent()
        {
            using (logger.BeginServiceScope("Get All Event"))
            {
                try
                {
                    IEnumerable<EventModel> data = null;
                    logger.LogInformation("Fetch all Event from repo");
                    IEnumerable<NAT_PLS_Event> eventModel = uow.RepositoryAsync<NAT_PLS_Event>().GetAllEvent();
                    if (eventModel != null)
                    {
                        data = new EventModel().FromDataModelList(eventModel);
                        return data;
                    }
                    throw new ApplicationException("No Event Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method gets all colliding events
        /// </summary>
        /// <returns>Event Colliding Model<returns>
        public async Task<EventCollidingModel> GetCollidingEvents(EventCollidingModel eventCollidingObj)
        {
            using (logger.BeginServiceScope("Get Colliding Events"))
            {
                try
                {
                    logger.LogInformation("Fetch all Event from repo with planner ID");

                    var allCollidingEventsByPlannerIdWithRange = new List<NAT_PLS_Event>();

                    //loop to get event models of all colliding events
                    foreach (EventDurationModel obj in eventCollidingObj.SelectedEvents)
                    {

                        obj.CollidingEventCodes = new List<string>();

                        IEnumerable<NAT_PLS_Event> data = uow.RepositoryAsync<NAT_PLS_Event>().GetAllCollidingSlotbytime(obj.PlannerId, obj.StartTime, obj.EndTime, obj.ReferenceId);
                        if (data != null)
                        {
                            foreach (NAT_PLS_Event obj1 in data)
                            {
                                allCollidingEventsByPlannerIdWithRange.Add(obj1);
                                obj.CollidingEventCodes.Add(obj1.Reference_ID);
                            }


                        }



                    }

                    //check if colliding events count is zero so return 
                    if (allCollidingEventsByPlannerIdWithRange.Count() == 0) { return eventCollidingObj; }

                    //else add the all event codes of colliding event objects to be returned

                    eventCollidingObj.CollidingEvents = new List<EventModel>();

                    foreach (NAT_PLS_Event obj1 in allCollidingEventsByPlannerIdWithRange)
                    {
                        //if (!eventCollidingObj.CollidingEventCodes.Contains(obj1.Reference_ID))
                        //{
                        eventCollidingObj.CollidingEvents.Add(new EventModel().FromDataModel(obj1));
                        //}
                    }

                    //eventCollidingObj.CollidingEventCodes.AddRange(allCollidingEventsByPlannerIdWithRange.Select(x => x.Reference_ID));


                    //set the colliding flag to true of those selected events objects which collide
                    foreach (EventDurationModel obj in eventCollidingObj.SelectedEvents)
                    {
                        var collidingEvents = allCollidingEventsByPlannerIdWithRange.Where(x => obj.StartTime < x.End_Time && x.Start_Time < obj.EndTime);
                        if (collidingEvents.Count() > 0)
                        {
                            obj.IsColliding = true;
                        }
                    }

                    return eventCollidingObj;
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        /// <summary>
        /// Create: Method for creation of Event
        /// </summary>
        /// <param name="servicemodel">Service Event Model</param>
        /// <returns>Event ID generated for Event</returns>
        public async Task<EventModel> CreateEvent(EventModel servicemodel)
        {
            try
            {
                servicemodel.Description = null;
                //Normal process for booking the slot for artist or venue for an event 
                if (servicemodel != null)
                {
                    SlotModel SlotModeldata = null;
                    logger.LogInformation("Fetch all Event from repo");
                    NAT_PLS_Slot SlotModel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllAvaliableSlotByPlannerId(servicemodel.PlannerId, servicemodel.StartTime, servicemodel.EndTime);

                    if (SlotModel != null)
                    {
                        SlotModel newSlotModel1 = new SlotModel();
                        newSlotModel1.PlannerId = SlotModel.Planner_ID;
                        newSlotModel1.StatusTypeLKPId = 1;
                        newSlotModel1.SlotTypeLKPId = 1;
                        newSlotModel1.ObjectState = ObjectState.Added;

                        if (SlotModel.Start_Time == servicemodel.StartTime && SlotModel.End_Time == servicemodel.EndTime)
                        {

                        }
                        else if (SlotModel.Start_Time == servicemodel.StartTime)
                        {
                            newSlotModel1.StartTime = servicemodel.EndTime;
                            newSlotModel1.EndTime = SlotModel.End_Time;
                            SlotModel.End_Time = servicemodel.EndTime;

                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(newSlotModel1.ToDataModel(newSlotModel1));

                        }
                        else if (SlotModel.End_Time == servicemodel.EndTime)
                        {
                            newSlotModel1.StartTime = SlotModel.Start_Time;
                            newSlotModel1.EndTime = servicemodel.StartTime;
                            SlotModel.Start_Time = servicemodel.StartTime;
                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(newSlotModel1.ToDataModel(newSlotModel1));
                        }
                        else
                        {
                            newSlotModel1.StartTime = SlotModel.Start_Time;
                            newSlotModel1.EndTime = servicemodel.StartTime;

                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(newSlotModel1.ToDataModel(newSlotModel1));

                            SlotModel newSlotModel2 = new SlotModel();
                            newSlotModel2.PlannerId = SlotModel.Planner_ID;
                            newSlotModel2.StatusTypeLKPId = 1;
                            newSlotModel2.SlotTypeLKPId = 1;
                            newSlotModel2.ObjectState = ObjectState.Added;

                            newSlotModel2.StartTime = servicemodel.EndTime;
                            newSlotModel2.EndTime = SlotModel.End_Time;

                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(newSlotModel2.ToDataModel(newSlotModel2));

                            SlotModel.Start_Time = servicemodel.StartTime;
                            SlotModel.End_Time = servicemodel.EndTime;

                        }

                        //If its an online event create an event on google calendar and generate a conference link (Google Hangout)
                        if (servicemodel.Online == true)
                        {
                            var planner = await uow.RepositoryAsync<NAT_PLS_Planner>().GetPlannerByIdAsync(servicemodel.PlannerId);
                            if (planner.Google_Calendar_ID == null)
                            {
                                var gCalendar = await GoogleCalendarApi.CreateCalendarAsync("Calendar Summary");
                                planner.Google_Calendar_ID = gCalendar.Id;
                                planner.ObjectState = ObjectState.Modified;
                                uow.RepositoryAsync<NAT_PLS_Planner>().Update(planner);
                            }
                            var gEvent = await GoogleCalendarApi.CreateEventAsync(planner.Google_Calendar_ID, servicemodel.Title, servicemodel.StartTime, servicemodel.EndTime);
                            servicemodel.GoogleEventId = gEvent.Id;
                            servicemodel.GoogleHangoutUrl = gEvent.ConferenceData.EntryPoints[0].Uri;
                        }

                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;
                        Insert(servicemodel);

                        SlotModel.Event_ID = servicemodel.EventId;
                        SlotModel.Event_Name = servicemodel.Title;
                        SlotModel.Description = servicemodel.Description;
                        SlotModel.Status_Type_LKP_ID = 2;
                        SlotModel.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<NAT_PLS_Slot>().Update(SlotModel);

                        uow.SaveChanges();
                        return Get();
                    }

                    else
                    {
                        //create a forced slot with respect to planner ID
                        //this forced slot is created in case the artist or venue is unavailable
                        //setting the slot type Lookup Id "2" indicates the forced slot
                        if (servicemodel.Forced == true)
                        {


                            IEnumerable<NAT_PLS_Slot> CollidingSlots = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllCollidingSlot(servicemodel.PlannerId, servicemodel.StartTime, servicemodel.EndTime);

                            if (CollidingSlots != null)
                            {

                                foreach (var slot1 in CollidingSlots)
                                {
                                    if (slot1.Event_ID == null)
                                    {
                                        await uow.RepositoryAsync<NAT_PLS_Slot>().DeleteSlotByslotidAsync(slot1.Slot_ID);                                        
                                    }
                                    else
                                    {
                                        if (slot1.NAT_PLS_Event.Reference_ID.IsCaseInsensitiveEqual(servicemodel.ReferenceId))
                                        {
                                            var eventId = slot1.NAT_PLS_Event.Event_ID;                                            
                                            await uow.RepositoryAsync<NAT_PLS_Slot>().DeleteSlotByslotidAsync(slot1.Slot_ID);
                                            await uow.RepositoryAsync<NAT_PLS_Event>().DeleteEventByeventidAsync(eventId);
                                        }
                                        else
                                            throw new Exception("Event already exist at this slot. Event Name: " + slot1.Event_Name + " " + slot1.Start_Time.ToShortTimeString() + " - " + slot1.End_Time.ToShortTimeString()); 

                                    }

                                }


                            } 

                            if (servicemodel.Online == true)
                            {
                                var planner = await uow.RepositoryAsync<NAT_PLS_Planner>().GetPlannerByIdAsync(servicemodel.PlannerId);
                                if (planner.Google_Calendar_ID == null)
                                {
                                    var gCalendar = await GoogleCalendarApi.CreateCalendarAsync("Calendar Summary");
                                    planner.Google_Calendar_ID = gCalendar.Id;
                                    planner.ObjectState = ObjectState.Modified;
                                    uow.RepositoryAsync<NAT_PLS_Planner>().Update(planner);
                                }
                                var gEvent = await GoogleCalendarApi.CreateEventAsync(planner.Google_Calendar_ID, servicemodel.Title, servicemodel.StartTime, servicemodel.EndTime);
                                servicemodel.GoogleEventId = gEvent.Id;
                                servicemodel.GoogleHangoutUrl = gEvent.ConferenceData.EntryPoints[0].Uri;
                            }

                            servicemodel.ActiveFlag = true;

                            SlotModel newSlotModel = new SlotModel();
                            newSlotModel.PlannerId = servicemodel.PlannerId;
                            newSlotModel.StatusTypeLKPId = 2;
                            newSlotModel.SlotTypeLKPId = 2;
                            newSlotModel.StartTime = servicemodel.StartTime;
                            newSlotModel.EndTime = servicemodel.EndTime;
                            newSlotModel.EventId = servicemodel.EventId;
                            newSlotModel.EventName = servicemodel.Title;
                            newSlotModel.TimingLKP = servicemodel.SlotTiming;
                            newSlotModel.ActiveFlag = true;
                            newSlotModel.ObjectState = ObjectState.Added;


                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(newSlotModel.ToDataModel(newSlotModel));
                            Insert(servicemodel);

                            uow.SaveChanges();
                            return Get();
                        }
                        throw new Exception("Unable to book the Slot");

                    }

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
        /// Create: Method for Cancallation of Event
        /// </summary>
        /// <param name="servicemodel">Service Event Model</param>
        /// <returns>Event ID generated for Event</returns>
        public async Task<Boolean> CancelEvent(int plannerid, string eventcode)
        {
            try
            {
                if (plannerid != 0 && eventcode != null)
                {
                    EventModel eventcanceldata = null;
                    SlotModel SlotModeldata = null;
                    logger.LogInformation("Fetch Event ");
                    NAT_PLS_Event EventCancelModel = await uow.RepositoryAsync<NAT_PLS_Event>().GetEventCancelAsync(plannerid, eventcode);
                    if (EventCancelModel != null)
                    {
                        EventCancelModel.Status_LKP_ID = 3;
                        EventCancelModel.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<NAT_PLS_Event>().Update(EventCancelModel);

                        eventcanceldata = new EventModel().FromDataModel(EventCancelModel);
                        //eventcanceldata.StatusLKPId = 3;
                        //eventcanceldata.ObjectState = ObjectState.Modified;
                        //base.Update(eventcanceldata);

                        NAT_PLS_Slot SlotModel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllScheduledSlotByPlannerId(eventcanceldata.PlannerId, eventcanceldata.StartTime, eventcanceldata.EndTime);
                        NAT_PLS_Slot tempSlot;

                        while ((tempSlot = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAvaliableSlotByEndTime(eventcanceldata.PlannerId, SlotModel.Start_Time)) != null)
                        {
                            SlotModel.Start_Time = tempSlot.Start_Time;
                            tempSlot.ObjectState = ObjectState.Deleted;
                        }
                        while ((tempSlot = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAvaliableSlotByStartTime(eventcanceldata.PlannerId, SlotModel.End_Time)) != null)
                        {
                            SlotModel.End_Time = tempSlot.End_Time;
                            tempSlot.ObjectState = ObjectState.Deleted;
                        }


                        if (SlotModel != null)
                        {
                            SlotModel.Event_ID = null;
                            SlotModel.Event_Name = null;
                            SlotModel.Description = null;
                            SlotModel.Status_Type_LKP_ID = 1;
                            SlotModel.ObjectState = ObjectState.Modified;
                            uow.RepositoryAsync<NAT_PLS_Slot>().Update(SlotModel);
                        }

                        int updatedRows = await uow.SaveChangesAsync();
                        if (updatedRows == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        /// <summary>
        /// This method returns Event with a given Id
        /// </summary>
        /// <param name="Id">Id of Event</param>
        /// <returns>Event service model</returns>
        public async Task<EventModel> GetByIdEventAsync(int Id)
        {
            try
            {
                EventModel data = null;
                NAT_PLS_Event eventModel = await uow.RepositoryAsync<NAT_PLS_Event>().GetEventByIdAsync(Id);
                if (eventModel != null)
                {
                    data = new EventModel().FromDataModel(eventModel);
                    return data;
                }
                throw new Exception("No Event Model Found By Given ID");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Event record
        /// </summary>
        /// <param name="servicemodel">Service EventModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateEventAsync(EventModel servicemodel)
        {
            try
            {
                if (servicemodel.EventId != 0 || servicemodel.EventId > 0)
                {
                    servicemodel.ObjectState = ObjectState.Modified;
                    base.Update(servicemodel);
                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        return false;
                    }
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

        public DataSourceResult GetAllEventList(DataSourceRequest request)
        {
            try
            {
                return uow.RepositoryAsync<NAT_PLS_Event>().Queryable().ToDataSourceResult<NAT_PLS_Event, EventModel>(request);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}