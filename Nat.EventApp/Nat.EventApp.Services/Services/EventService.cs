using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nat.EventApp.Services.ServiceModels;
using Nat.EventApp.Models.EFModel;
using Nat.Core.Exception;
using Nat.EventApp.Models.Repositories;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.KendoX;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;
using Nat.Core.ServiceClient;
using Nat.EventApp.Services.ViewModels;
using Nat.Core.Lookup;
using Nat.EventApp.Functions.ViewModels;
using Nat.EventApp.Functions.ServiceModels;
using Nat.Core.Caching;
using Nat.Core.Notification.EmailTemplateModels;
using Nat.Core.MarketTimeZone.Model;
using Nat.Core.MarketTimeZone;
using static Nat.Core.QueueMessage.NotificationQueueMessage;
using Nat.Core.QueueMessage;
using Nat.Core.Notification;
using Microsoft.WindowsAzure.Storage.Table;
using Nat.Core.Storage;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.EventApp.Services.ServiceModels.ViewModel;
using Nat.Common.Constants;
using System.Data.Entity;
using Newtonsoft.Json;
using Nat.EventApp.Functions.ViewModel;
using Nat.Core.BaseModelClass;
using Nat.Core.Authentication;
using Nat.Core.Zoom;
using System.Globalization;
using BitlyAPI;
namespace Nat.EventApp.Services
{
    public class EventService : BaseService<EventModel, NAT_ES_Event>
    {
        private static EventService _service;
        public static EventService GetInstance(NatLogger logger)
        {
            _service = new EventService();
            _service.SetLogger(logger);
            return _service;
        }

        private EventService() : base()
        {

        }

        /// <summary>
        /// This method return list of all Events
        /// </summary>
        /// <returns>Collection of Event service model<returns>
        public IEnumerable<EventModel> GetAllEvent()
        {
            using (logger.BeginServiceScope("Get All Event"))
            {
                try
                {
                    IEnumerable<EventModel> data = null;
                    logger.LogInformation("Fetch all Event from repo");
                    IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetAllEvent();
                    if (EventModel != null)
                    {
                        data = new EventModel().FromDataModelList(EventModel);
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
        /// This method return list of suggested Events
        /// </summary>
        /// <returns>Collection of Event service model<returns>
        public async Task<IEnumerable<EventModel>> GetSuggestedEventAsync(long? customerId)
        {
            using (logger.BeginServiceScope("Get All Event"))
            {
                try
                {
                    
                    logger.LogInformation("Fetch suggested Events from repo");
                    IEnumerable<string> EventCodes = uow.RepositoryAsync<NAT_ES_Event>().GetSuggestedEvents();

                    if (EventCodes != null)
                    {
                        List<EventModel> eventModels = new List<EventModel>();

                        foreach (String eventCode in EventCodes)
                        {
                            EventModel x = await GetByCodeEvent(eventCode, customerId);
                            eventModels.Add(x);
                        }



                        List<TicketSummaryModel> ticketSummaryList = new List<TicketSummaryModel>();
                        TicketSummaryModel ticketSummary = new TicketSummaryModel();
                        List<NAT_TICKET_SUMMARY_VW> ticketList = uow.Repository<NAT_TICKET_SUMMARY_VW>().Queryable().Where(x => x.Ticket_Status != Constants.TICKET_STATUS_CANCELLED && EventCodes.Contains(x.Event_Code)).ToList();
                        ticketSummaryList = ticketSummary.FromDataModelList(ticketList).ToList();
                        //add ticket summary

                        for (int i = 0; i < eventModels.Count; i++)
                        {
                            var seatingPlan = eventModels[i].NatEsEventSeatingPlan.Where(x => x.ActiveFlag == true).FirstOrDefault();
                            if (seatingPlan != null || eventModels[i].Virtual)
                            {
                                eventModels[i].TotalTickets = 0;
                                eventModels[i].BookedTickets = 0;
                                eventModels[i].TicketSummary = new List<TicketTypeSummaryModel>();
                                List<TicketSummaryModel> eventTicketList = ticketSummaryList.Where(x => x.EventCode.ToUpper().Equals(eventModels[i].EventCode.ToUpper())).ToList();
                                foreach (var seatType in eventTicketList)
                                {

                                    var ticketTypeSummary = new TicketTypeSummaryModel();
                                    ticketTypeSummary.Type = seatType.SeatType;
                                    ticketTypeSummary.Total = (int)seatType.TotalSeats;
                                    ticketTypeSummary.Sold = (int)seatType.TicketCount;
                                    eventModels[i].TotalTickets += ticketTypeSummary.Total;
                                    eventModels[i].BookedTickets += ticketTypeSummary.Sold;
                                    eventModels[i].TicketSummary.Add(ticketTypeSummary);
                                }

                            }

                        }

                        return eventModels;
                    }
                    throw new ApplicationException("Unable to fetch suggested event list");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        /// <summary>
        /// This method return list of all Events With address and painting details
        /// </summary>
        /// <returns>Collection of Event service model<returns>
        public async Task<IEnumerable<EventModel>> GetAllEventListItems()
        {
            using (logger.BeginServiceScope("Get All Event List Items"))
            {
                try
                {
                    List<EventModel> list = null;
                    int listsize = 0;
                    IEnumerable<EventModel> data = null;
                    logger.LogInformation("Fetch all Event List Items from repo");
                    IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetAllEvent();
                    if (EventModel != null)
                    {
                        data = new EventModel().FromDataModelList(EventModel);
                        list = data.ToList();
                        listsize = list.Count();
                        var lookupListPainting = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingLovViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintinglov");
                        if (lookupListPainting.status.IsSuccessStatusCode)
                        {
                            var paintinglov = lookupListPainting.data.ToDictionary(item => item.Id, item => item);

                            for (int i = 0; i < listsize; i++)
                            {
                                var dataitems = paintinglov[list[i].PaintingId];
                                list[i].PaintingName = dataitems.Name;
                                list[i].PaintingRating = dataitems.Rating;
                                list[i].PaintingImage = dataitems.Image;
                            }
                        }

                        var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                        if (lookupListAddress.status.IsSuccessStatusCode)
                        {
                            var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);
                            for (int i = 0; i < listsize; i++)
                            {
                                if (list[i].VenueId != null)
                                {
                                    var dataitems = addresslov[list[i].VenueId];
                                    list[i].VenueAddress = dataitems.Address;
                                    list[i].VenueName = dataitems.Name;
                                    list[i].VenueRating = dataitems.Rating;
                                }
                            }
                        }
                        return list;
                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<IEnumerable<EventLovModel>> GetEventLov()
        {
            using (logger.BeginServiceScope("Get Event Lov"))
            {
                try
                {
                    async Task<IEnumerable<EventLovModel>> GetEventLovFromDB()
                    {
                        logger.LogInformation("Fetch id, name and event code of event");
                        IEnumerable<NAT_ES_Event> eventss = await uow.RepositoryAsync<NAT_ES_Event>().GetEventLov();
                        List<EventLovModel> EventLov = eventss.Select(x => new EventLovModel
                        {
                            EventId = x.Event_ID,
                            EventCode = x.Event_Code,
                            EventName = x.Event_Name,
                            VenueId = x.Venue_ID,
                            ArtistId = x.Artist_ID,
                            EventStartTime = x.Event_Start_Time,
                            EventEndTime = x.Event_End_Time
                        }).
                        ToList();

                        EventLov = EventLov.OrderBy(x => x.EventName).ToList();

                        return EventLov;
                    }
                    return await Caching.GetObjectFromCacheAsync<IEnumerable<EventLovModel>>("Eventlov", 0, GetEventLovFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        public async Task<IEnumerable<EventModel>> GetAllEventFilters(long? customerId, string EventName, DateTime? StartDate, DateTime? EndDate, string VenueCityCode, ICollection<int> ArtistId, int paintingId, ICollection<int?> Category, decimal? MinPrice, decimal? MaxPrice, int Sortid, bool SortAsc, int ArtistRatingFltr, decimal? TicketPrice)
        {
            try
            {
                List<EventModel> list = new List<EventModel>();
                List<EventModel> list1 = null;

                
                string sortproperty = "";
                int listsize = 0;
                IEnumerable<EventModel> data = null;
                if (Sortid == 2 || Sortid == 3) { sortproperty = "Event_Date"; }
                else if (Sortid == 4 || Sortid == 5) { sortproperty = "Min_Ticket_Price"; }
                IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetAllEventsFilter(EventName, StartDate, EndDate, VenueCityCode, ArtistId, paintingId, Category, MinPrice, MaxPrice, SortAsc, sortproperty);
                if (EventModel != null)
                {
                    data = new EventModel().FromDataModelList(EventModel);
                    list1 = data.ToList();

                    if (TicketPrice != null)
                    {
                        foreach (EventModel eventss in list1)
                        {
                            if (eventss.ArtistId == null || eventss.VenueId == null)
                                continue;
                            if (eventss.NatEsEventTicketPrice.Count() > 0)
                            {
                                foreach (EventTicketPriceModel prices in eventss.NatEsEventTicketPrice)
                                {
                                    if (prices.Price == TicketPrice)
                                    {
                                        list.Add(eventss);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Skip event if Artist or Venue is not assigned to Event
                        foreach (EventModel eventss in list1)
                        {
                            if (eventss.ArtistId == null || eventss.VenueId == null)
                                continue;
                            list.Add(eventss);
                        }
                        //list = list1;
                    }
                    listsize = list.Count();
                    if (list.Count > 0)
                        await PopulateEventLookupValues(customerId, StartDate, EndDate, list);

                    if (ArtistRatingFltr != 0)
                    {
                        if (ArtistRatingFltr == 1) { list = list.Where(x => x.ArtistRating <= Convert.ToDouble("2.0")).ToList(); }
                        else if (ArtistRatingFltr == 2) { list = list.Where(x => x.ArtistRating >= Convert.ToDouble("3.0") && x.ArtistRating >= Convert.ToDouble("4.0")).ToList(); }
                        else if (ArtistRatingFltr == 3) { list = list.Where(x => x.ArtistRating >= Convert.ToDouble("4.0")).ToList(); }
                        else if (ArtistRatingFltr == 4) { list = list.Where(x => x.ArtistRating == Convert.ToDouble("5.0")).ToList(); }
                    }

                    var eventCodeList = new List<string>();

                    foreach (var evt in list)
                    {
                        eventCodeList.Add(evt.EventCode);
                    }

                    List<TicketSummaryModel> ticketSummaryList = new List<TicketSummaryModel>();
                    TicketSummaryModel ticketSummary = new TicketSummaryModel();
                    List<NAT_TICKET_SUMMARY_VW> ticketList = uow.Repository<NAT_TICKET_SUMMARY_VW>().Queryable().Where(x => x.Ticket_Status != Constants.TICKET_STATUS_CANCELLED && eventCodeList.Contains(x.Event_Code)).ToList();
                    ticketSummaryList = ticketSummary.FromDataModelList(ticketList).ToList();
                    //add ticket summary

                    for (int i = 0; i < list.Count; i++)
                    {
                        var seatingPlan = list[i].NatEsEventSeatingPlan.Where(x => x.ActiveFlag == true).FirstOrDefault();
                        if (seatingPlan != null || list[i].Virtual)
                        {
                            list[i].TotalTickets = list[i].Virtual ? (list[i].Capacity ?? default) : seatingPlan.NatEsEventSeat.Count;
                            list[i].BookedTickets = 0;
                            list[i].TicketSummary = new List<TicketTypeSummaryModel>();
                            List<TicketSummaryModel> eventTicketList = ticketSummaryList.Where(x => x.EventCode.ToUpper().Equals(list[i].EventCode.ToUpper())).ToList();
                            foreach (var seatType in eventTicketList)
                            {

                                var ticketTypeSummary = new TicketTypeSummaryModel();
                                ticketTypeSummary.Type = seatType.SeatType;
                                ticketTypeSummary.Total = (int)seatType.TotalSeats;
                                ticketTypeSummary.Sold = (int)seatType.TicketCount;
                                //list[i].TotalTickets += ticketTypeSummary.Total;
                                list[i].BookedTickets += ticketTypeSummary.Sold;
                                list[i].TicketSummary.Add(ticketTypeSummary);
                            }

                        }

                    }
                    EventName = EventName.ToLower();
                    if (EventName != null && EventName != "")
                    {
                        list = list.Where(x => x.VenueName.ToLower().Contains(EventName)
                            || x.PaintingName.ToLower().Contains(EventName)
                            || x.VenueCity.ToLower().Contains(EventName)
                            || x.VenueMetroAreas.ToLower().Contains(EventName)).ToList();
                        
                    }
                    return list;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        private static async Task PopulateEventLookupValues(long? customerId, DateTime? StartDate, DateTime? EndDate, List<EventModel> list)
        {
            var lookupList = await NatClient.ReadAsync<IEnumerable<ViewModels.ArtistLovViewModel>>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artistslov");
            var lookupListPainting = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingLovViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintinglov");
            var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
            var CustomerLikedEventsdata = await NatClient.ReadAsync<IEnumerable<CustomerLikedEventsModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customer/" + Convert.ToString(customerId) + "/likedevents");
            for (int i = 0; i < list.Count; i++)
            {
                #region Set Artist
                if (lookupList.status.IsSuccessStatusCode)
                {
                    var artistlov = lookupList.data.ToDictionary(item => item.Id, item => item);
                    if (list[i].ArtistId != null)
                    {
                        var dataitems = artistlov[Convert.ToInt32(list[i].ArtistId)];
                        list[i].ArtistName = dataitems.Value;
                        list[i].ArtistRating = dataitems.Rating;
                        list[i].ArtistImage = dataitems.Image;
                        list[i].ArtistFirstName = dataitems.FirstName;
                        list[i].ArtistLastName = dataitems.LastName;
                        list[i].ArtistPhone = dataitems.Phone;
                        list[i].ArtistEmail = dataitems.Email;
                        list[i].ArtistPlannerId = dataitems.PlannerId.Value;
                    }
                }
                #endregion
                #region Set Paintings
                if (lookupListPainting.status.IsSuccessStatusCode)
                {
                    var paintinglov = lookupListPainting.data.ToDictionary(item => item.Id, item => item);


                    var dataitems = paintinglov[list[i].PaintingId];
                    list[i].PaintingName = dataitems.Name;
                    list[i].PaintingRating = dataitems.Rating;
                    list[i].PaintingImage = dataitems.Image;
                    list[i].PaintingVideo = dataitems.Video;

                }
                #endregion
                #region Set Venues
                if (lookupListAddress.status.IsSuccessStatusCode)
                {
                    var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);

                    if (list[i].VenueId != null)
                    {
                        var dataitems = addresslov[list[i].VenueId];
                        list[i].VenueAddress = dataitems.Address;
                        list[i].VenueName = dataitems.Name;
                        list[i].VenueRating = dataitems.Rating;
                        list[i].VenueCity = dataitems.City;
                        list[i].VenuePlannerId = dataitems.PlannerId.Value;
                        list[i].VenueMetroAreas = dataitems.MetroCityArea;
                        list[i].VenueAddressLine1 = dataitems.AddressLine1;
                        list[i].VenueAddressLine2 = dataitems.AddressLine2;
                        list[i].VenuePostalCode = dataitems.PostalCode;
                    }

                }

                #endregion

                list[i].LikeStatus = false;
                if (customerId != null)
                {
                    foreach (CustomerLikedEventsModel likedEvent in CustomerLikedEventsdata.data)
                    {
                        if (list[i].EventCode == likedEvent.EventCode)
                        {
                            list[i].LikeStatus = true;
                        }
                    }
                }
                if (StartDate != null)
                {
                    list[i].StartDate = StartDate;
                    list[i].EndDate = EndDate;
                }

            }
        }

        public async Task<IEnumerable<EventModel>> GetAllFeaturedEventFilters(long? customerId)
        {
            try
            {
                List<EventModel> list = null;
   
                int listsize = 0;
                IEnumerable<EventModel> data = null;
                IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetAllFeaturedEvents();
                if (EventModel != null)
                {
                    data = new EventModel().FromDataModelList(EventModel);
                    list = data.ToList();
                    listsize = list.Count();
                    if (listsize > 0)
                        await PopulateEventLookupValues(customerId, null, null, list);
                    return list;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<EventModel> GetEventDetail(int EventId, long? customerId)
        {
            try
            {
                // List<EventModel> list = null;
                // int listsize = 0;
                EventModel data = null;
                List<EventModel> eventList = null;
                NAT_ES_Event EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventDetail(EventId);

                if (EventModel != null)
                {
                    data = new EventModel().FromDataModel(EventModel);
                    var facilityIconLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_FACILITY");
                    //var venuelov = lookupListVenue.data.ToDictionary(item => item.AddressId, item => item);
                    data.LikeStatus = false;

                    foreach (EventFacilityModel facility in data.NatEsEventFacility)
                    {
                        if (facilityIconLookup.ContainsKey(facility.FacilityLKPId.ToString()))
                        {
                            facility.FacilityIcon = facilityIconLookup[facility.FacilityLKPId.ToString()].LookupDescription;
                            facility.FacilityName = facilityIconLookup[facility.FacilityLKPId.ToString()].VisibleValue;
                            facility.ObjectState = ObjectState.Modified;
                        }
                    }
                    eventList = new List<EventModel>();
                    eventList.Add(data);
                    await PopulateEventLookupValues(customerId, null, null, eventList);
                    data = eventList.FirstOrDefault();

                    if (data.Virtual)
                    {
                        var bitly = new Bitly("d96fb78a2c3deddb013f143ddc88311271dce6e3");
                        string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, data.EventCode);
                        TableStorage ts = new TableStorage();
                        List<VitualEventLinkTableEntityModel> requests = (await ts.RetrieveTableStorage<VitualEventLinkTableEntityModel>("VirtualEventLink", filter)).ToList();
                        string meetingStartLink = null;
                        foreach (var virtualLink in requests)
                        {
                            if (virtualLink.ZoomStartLink != null)
                            {
                                var responseLink = await bitly.PostShorten(virtualLink.ZoomStartLink);
                                meetingStartLink = responseLink.Link;
                                
                                
                            }
                        }
                        data.MeetingLink = meetingStartLink;
                    }

                    var seatingPlan = data.NatEsEventSeatingPlan.Where(x => x.ActiveFlag == true).FirstOrDefault();
                    if (seatingPlan != null)
                    {
                        var seatTypeSummary = await uow.RepositoryAsync<NAT_ES_Event_Seat>().Queryable()
                        .Where(x => x.Seating_Plan_ID == seatingPlan.SeatingPlanId && x.Active_Flag == true)
                        .GroupBy(x => x.Seat_Type_LKP_ID)
                        .Select(x => new
                        {
                            TotalSeats = x.Count(),
                            SeatType = x.Key
                        }).ToListAsync();

                        var ticketsResp = await NatClient.ReadAsync<List<TicketModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetTicketsByEventCode/" + data.EventCode);
                        if (!ticketsResp.status.IsSuccessStatusCode) throw new Exception("Failed to get ticket info");



                        data.TotalTickets = 0;
                        data.BookedTickets = 0;
                        data.TicketSummary = new List<TicketTypeSummaryModel>();
                        foreach (var seatType in seatTypeSummary)
                        {
                            var ticketTypeSummary = new TicketTypeSummaryModel();
                            ticketTypeSummary.Type = seatType.SeatType.ToString();
                            ticketTypeSummary.Total = seatType.TotalSeats;
                            ticketTypeSummary.Sold = ticketsResp.data.Where(x => x.TicketTypeLkp == ticketTypeSummary.Type).Count();
                            data.TotalTickets += ticketTypeSummary.Total;
                            data.BookedTickets += ticketTypeSummary.Sold;
                            data.TicketSummary.Add(ticketTypeSummary);
                        }
                    }

                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        public async Task<DataSourceResult> GetAllEventListAsync(DataSourceRequest request, Auth.UserModel UserModel)
        {
            try
            {

                DataSourceResult data;

                var rolechecker = 0;
                if (UserModel.Roles != null)
                    foreach (Auth.RoleModel Roles in UserModel.Roles)
                    {
                        if (Roles.RoleCode == "ARTIST_MANAGER")
                        {
                            rolechecker = 1;
                        }
                        else if (Roles.RoleCode == "ARTIST")
                        {
                            rolechecker = 2;
                        }
                        else if (Roles.RoleCode == "VCP")
                        {
                            rolechecker = 3;
                        }
                    }

                if (rolechecker == 1)
                {
                    var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + UserModel.UserId);
                    var ArtistList = userlist.data.Select(x => x.ReferenceId);
                    if (ArtistList != null)
                    {
                        data = uow.RepositoryAsync<NAT_ES_Event>().Queryable().Where(x => ArtistList.Contains(x.Artist_ID))
                                        .Include(x => x.NAT_ES_Event_Facility)
                                        .Include(x => x.NAT_ES_Event_Ticket_Price)
                                        .Include(x => x.NAT_ES_Event_Seating_Plan)
                                        .ToDataSourceResult<NAT_ES_Event, EventModel>(request);
                    }
                    else
                    {
                        data = new DataSourceResult();
                    }

                }
                else if (rolechecker == 2)
                {
                    if (UserModel.ReferenceId != null)
                    {
                        data = uow.RepositoryAsync<NAT_ES_Event>().Queryable().Where(x => x.Artist_ID == UserModel.ReferenceId)
                                        .Include(x => x.NAT_ES_Event_Facility)
                                        .Include(x => x.NAT_ES_Event_Ticket_Price)
                                        .Include(x => x.NAT_ES_Event_Seating_Plan)
                                        .ToDataSourceResult<NAT_ES_Event, EventModel>(request);
                    }
                    else
                    {
                        data = new DataSourceResult();
                    }
                }
                else if (rolechecker == 3)
                {
                    if (UserModel.ReferenceId != null)
                    {
                        data = uow.RepositoryAsync<NAT_ES_Event>().Queryable().Where(x => x.Venue_ID == UserModel.ReferenceId)
                                        .Include(x => x.NAT_ES_Event_Facility)
                                        .Include(x => x.NAT_ES_Event_Ticket_Price)
                                        .Include(x => x.NAT_ES_Event_Seating_Plan)
                                        .ToDataSourceResult<NAT_ES_Event, EventModel>(request);
                    }
                    else
                    {
                        data = new DataSourceResult();
                    }
                }
                else
                {
                    data = uow.RepositoryAsync<NAT_ES_Event>().Queryable()
                                        .Include(x => x.NAT_ES_Event_Facility)
                                        .Include(x => x.NAT_ES_Event_Ticket_Price)
                                        .Include(x => x.NAT_ES_Event_Seating_Plan)
                                        .ToDataSourceResult<NAT_ES_Event, EventModel>(request);
                }


                ResponseViewModel<IEnumerable<Auth.UserModel>> userResp = null;
                IEnumerable<Auth.UserModel> AllArtists = null;
                IEnumerable<Auth.UserModel> AllManagers = null;

                if (rolechecker == 3)
                {
                    userResp = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsers");
                    if (userResp.status.IsSuccessStatusCode != true || userResp.data == null) throw new Exception("Unable To get Users");
                    AllArtists = userResp.data.Where(x => x.ReferenceTypeLKP == Constants.ARTIST_USER_REFERENCE_TYPE);
                    //AllManagers = userResp.data.Where(s => s.Roles.Any(x => x.RoleCode.Equals(Constants.ARTIST_MANAGER_ROLE_CODE)));
                    var AllusersWithRoles = userResp.data.Where(s => s.Roles != null);
                    AllManagers = AllusersWithRoles.Where(s => s.Roles.Any(x => x.RoleCode.Equals(Constants.ARTIST_MANAGER_ROLE_CODE)));
                }

                if (data.Total > 0)
                {
                    var list = ((IEnumerable<EventModel>)data.Data).ToList();
                    var eventCodeList = new List<string>();
                    foreach (var evt in list)
                        eventCodeList.Add(evt.EventCode);
                    List<TicketSummaryModel> ticketSummaryList = new List<TicketSummaryModel>();
                    TicketSummaryModel ticketSummary = new TicketSummaryModel();
                    List<NAT_TICKET_SUMMARY_VW> ticketList = uow.Repository<NAT_TICKET_SUMMARY_VW>().Queryable().Where(x => eventCodeList.Contains(x.Event_Code)).ToList();
                    ticketSummaryList = ticketSummary.FromDataModelList(ticketList).ToList();
                    await PopulateEventLookupValues(null, null, null, list);


                    //setting the event likes and ticket counts
                    for (int i = 0; i < list.Count; i++)
                    {
                        var seatingPlan = list[i].NatEsEventSeatingPlan.Where(x => x.ActiveFlag == true).FirstOrDefault();
                        if (seatingPlan != null || list[i].Virtual)
                        {
                            list[i].TotalTickets = 0;
                            list[i].BookedTickets = 0;
                            list[i].TicketSummary = new List<TicketTypeSummaryModel>();
                            List<TicketSummaryModel> eventTicketList = ticketSummaryList.Where(x => x.EventCode.ToUpper().Equals(list[i].EventCode.ToUpper())).ToList();
                            foreach (var seatType in eventTicketList)
                            {

                                var ticketTypeSummary = new TicketTypeSummaryModel();
                                ticketTypeSummary.Type = seatType.SeatType;
                                ticketTypeSummary.Total = (int)seatType.TotalSeats;
                                seatType.TicketStatus = seatType.TicketStatus ?? "";
                                ticketTypeSummary.Status = seatType.TicketStatus;
                                ticketTypeSummary.Sold = (seatType.TicketStatus.ToUpper().Equals(Constants.TICKET_STATUS_CANCELLED)) ? 0 : (int)seatType.TicketCount;
                                if (list[i].TicketSummary.Where(x => x.Type != null && x.Type.Equals(seatType.SeatType)).FirstOrDefault() == null)
                                {
                                    list[i].TotalTickets += ticketTypeSummary.Total;
                                }
                                list[i].BookedTickets += ticketTypeSummary.Sold;
                                list[i].TicketSummary.Add(ticketTypeSummary);
                            }

                        }

                        //var customerLikesData = await NatClient.ReadAsync<Nullable<Int32>>(NatClient.Method.GET, NatClient.Service.CustomerService, "EventsLikes/" + list[i].EventCode);
                        //if (customerLikesData.status.IsSuccessStatusCode && customerLikesData.data.HasValue)
                        //    list[i].EventLikesCount = customerLikesData.data.Value;

                        if (list[i].Virtual)
                        {
                            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, list[i].EventCode);
                            TableStorage ts = new TableStorage();
                            List<VitualEventLinkTableEntityModel> requests = (await ts.RetrieveTableStorage<VitualEventLinkTableEntityModel>("VirtualEventLink", filter)).ToList();
                            string meetingStartLink = null;
                            foreach (var virtualLink in requests)
                            {
                                if (virtualLink.ZoomStartLink != null)
                                {
                                    meetingStartLink = virtualLink.ZoomStartLink;
                                }
                            }
                            list[i].MeetingLink = meetingStartLink;
                        }

                        if (rolechecker == 3)
                        {
                            if (list[i].ArtistId != null)
                            {
                                var ArtistUser = AllArtists.Where(x => x.ReferenceId == list[i].ArtistId).FirstOrDefault();
                                if (ArtistUser.ReportingManager != null)
                                {
                                    //artist Manager
                                    var ManagerUser = AllManagers.Where(x => x.UserId == ArtistUser.ReportingManager).FirstOrDefault();

                                    if (ManagerUser != null)
                                    {
                                        list[i].ArtistName = ManagerUser.FirstName + " " + ManagerUser.LastName;
                                        list[i].ArtistPhone = ManagerUser.PhoneNumber;
                                        list[i].ArtistEmail = ManagerUser.Email;
                                    }
                                    else
                                    {
                                        //marketmanager
                                        var ManagerUse1r = AllManagers.Where(x => x.NatAusUserLocationMapping.Any(y => y.LocationCode.Equals(list[i].LocationCode))).FirstOrDefault();
                                        if (ManagerUse1r != null)
                                        {
                                            list[i].ArtistName = ManagerUse1r.FirstName + " " + ManagerUse1r.LastName;
                                            list[i].ArtistPhone = ManagerUse1r.PhoneNumber;
                                            list[i].ArtistEmail = ManagerUse1r.Email;
                                        }
                                        else
                                        {
                                            list[i].ArtistId = null;
                                            // throw new Exception("No Artist Manger found for location code : " + list[i].LocationCode);
                                        }
                                    }
                                }
                                else
                                {
                                    //marketmanager
                                    var ManagerUser = AllManagers.Where(x => x.NatAusUserLocationMapping.Any(y => y.LocationCode.Equals(list[i].LocationCode))).FirstOrDefault();
                                    if (ManagerUser != null)
                                    {
                                        list[i].ArtistName = ManagerUser.FirstName + " " + ManagerUser.LastName;
                                        list[i].ArtistPhone = ManagerUser.PhoneNumber;
                                        list[i].ArtistEmail = ManagerUser.Email;
                                    }
                                    else
                                    {
                                        list[i].ArtistId = null;
                                        // throw new Exception("No Artist Manger found for location code : " + list[i].LocationCode);
                                    }
                                }
                            }

                            else
                            {
                                //marketmanager
                                var ManagerUser = AllManagers.Where(x => x.NatAusUserLocationMapping.Any(y => y.LocationCode.Equals(list[i].LocationCode))).FirstOrDefault();
                                if (ManagerUser != null)
                                {
                                    list[i].ArtistName = ManagerUser.FirstName + " " + ManagerUser.LastName;
                                    list[i].ArtistPhone = ManagerUser.PhoneNumber;
                                    list[i].ArtistEmail = ManagerUser.Email;
                                }
                                else
                                {
                                    list[i].ArtistId = null;
                                    // throw new Exception("No Artist Manger found for location code : " + list[i].LocationCode);
                                }

                            }

                        }
                    }
                    /////  
                    data.Data = list;
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Today Events with a artist Id and Venue Id
        /// </summary>
        /// <param name="artistId">Id of artist</param>
        /// <param name="venueId">Id of venue</param>
        /// <returns>List of Event service model</returns>
        public async Task<IEnumerable<EventModel>> GetTodayEventWithArtistIdAndVenueId(int? artistId, int? venueId)
        {
            try
            {
                IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetTodayEventsWithArtistIdAndVenueId(artistId, venueId);
                var list = new EventModel().FromDataModelList(EventModel).ToList();
                if (list.Count > 0)
                    await PopulateEventLookupValues(null, null, null, list);

                var marketResp = await NatClient.ReadAsync<IEnumerable<LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");

                if (marketResp.status.IsSuccessStatusCode != true || marketResp.data == null) throw new Exception("Set Event Location service failed.");


                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].LocationCode != null)
                    {
                        marketResp.data.ToList().ForEach(location =>
                        {
                            if (location.LocationShortCode == list[i].LocationCode)
                            {
                                list[i].Market = location.LocationName;
                            }
                        });
                    }
                }

                return list;

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
        public EventModel GetByIdEvent(int Id)

        {
            try
            {
                EventModel data = null;
                NAT_ES_Event EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(Id);
                if (EventModel != null)
                {
                    data = new EventModel().FromDataModel(EventModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<EventModel>> GetfutureEventsbylocation(string code)

        {
            try
            {
               var EventModel = await uow.RepositoryAsync<NAT_ES_Event>().Queryable().Where(x => x.Location_Code == code && ((x.Event_Start_Time ?? System.DateTime.UtcNow) > System.DateTime.UtcNow) && x.Event_Status_LKP_ID != 3).ToListAsync();
                if (EventModel != null)
                {
                   var data = new EventModel().FromDataModelList(EventModel);
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns array of Events with respect to given Codes
        /// </summary>
        /// <param name="eventCodes">Codes of Events</param>
        /// <returns>List of Event service model</returns>
        public async Task<List<EventModel>> GetEventsByCodes(List<String> eventCodes)
        {
            try
            {
                var eventModelList = new List<EventModel>();
                foreach (String code in eventCodes)
                {
                    eventModelList.Add(await this.GetByCodeEvent(code, null));
                }
                return eventModelList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns array of Events with respect to artist ID
        /// </summary>
        /// <param name="artistId">Artist Id</param>
        /// <returns>List of Event service model</returns>
        public async Task<List<EventModel>> GetUpcomingEventsByArtistID(Auth.UserModel user)
        {
            try
            {
                DataSourceRequest dataSourceRequest = new DataSourceRequest();
                dataSourceRequest.Filters = new List<IFilterDescriptor>();
                var newFilter = new CompositeFilterDescriptor();
                var filterDescriptor = new FilterDescriptor("ArtistId", FilterOperator.IsEqualTo, user.ReferenceId);
                newFilter.FilterDescriptors.Add(filterDescriptor);
                dataSourceRequest.Filters.Add(newFilter);
                var dateFilterDescriptor = new FilterDescriptor("EventStartTime", FilterOperator.IsGreaterThanOrEqualTo, DateTime.UtcNow);
                dataSourceRequest.Filters.Add(dateFilterDescriptor);
                var eventModelList = await this.GetAllEventListAsync(dataSourceRequest, user);

                return (List<EventModel>)eventModelList.Data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns array of Events with respect to artist ID
        /// </summary>
        /// <param name="artistId">Artist Id</param>
        /// <returns>List of Event service model</returns>
        public async Task<List<EventModel>> GetUpcomingEventsByVenueID(Auth.UserModel user)
        {
            try
            {
                DataSourceRequest dataSourceRequest = new DataSourceRequest();
                dataSourceRequest.Filters = new List<IFilterDescriptor>();
                var newFilter = new CompositeFilterDescriptor();
                var filterDescriptor = new FilterDescriptor("VenueId", FilterOperator.IsEqualTo, user.ReferenceId);
                newFilter.FilterDescriptors.Add(filterDescriptor);
                dataSourceRequest.Filters.Add(newFilter);
                var dateFilterDescriptor = new FilterDescriptor("EventStartTime", FilterOperator.IsGreaterThanOrEqualTo, DateTime.UtcNow);
                dataSourceRequest.Filters.Add(dateFilterDescriptor);
                var eventModelList = await this.GetAllEventListAsync(dataSourceRequest, user);

                return (List<EventModel>)eventModelList.Data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns all Events with respect to artist ID. It is used from Tablet and Invoice Generation
        /// </summary>
        /// <param name="artistId">Artist Id</param>
        /// <returns>List of Event service model</returns>
        public async Task<List<EventModel>> GetAllEventsByArtistID(Auth.UserModel user)
        {
            try
            {
                DataSourceRequest dataSourceRequest = new DataSourceRequest();
                dataSourceRequest.Filters = new List<IFilterDescriptor>();
                var newFilter = new CompositeFilterDescriptor();
                var filterDescriptor = new FilterDescriptor("ArtistId", FilterOperator.IsEqualTo, user.ReferenceId);
                newFilter.FilterDescriptors.Add(filterDescriptor);
                dataSourceRequest.Filters.Add(newFilter);
                var eventModelList = await this.GetAllEventListAsync(dataSourceRequest, user);

                return (List<EventModel>)eventModelList.Data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns all Events with respect to artist ID
        /// </summary>
        /// <param name="artistId">Artist Id</param>
        /// <returns>List of Event service model</returns>
        public async Task<List<EventModel>> GetAllEventsByVenueID(Auth.UserModel user)
        {
            try
            {
                DataSourceRequest dataSourceRequest = new DataSourceRequest();
                dataSourceRequest.Filters = new List<IFilterDescriptor>();
                var newFilter = new CompositeFilterDescriptor();
                var filterDescriptor = new FilterDescriptor("VenueId", FilterOperator.IsEqualTo, user.ReferenceId);
                newFilter.FilterDescriptors.Add(filterDescriptor);
                dataSourceRequest.Filters.Add(newFilter);
                var eventModelList = await this.GetAllEventListAsync(dataSourceRequest, user);

                return (List<EventModel>)eventModelList.Data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Event Happening Now with respect to artist ID
        /// </summary>
        /// <param name="artistID">Artist Id</param>
        /// <returns>Event service model</returns>
        public async Task<EventModel> GetEventsHappeningNowByArtistId(Int32 artistId)
        {
            try
            {
                //call configuration service to get time added to current time to get events starting soon
                var configResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/EventsHappeningSoonTime");
                var timeToAdd = (configResponse != null && configResponse.status.IsSuccessStatusCode == true) ? Convert.ToDouble(configResponse.data.Value) : 0.0;

                //get events happening now
                IEnumerable<NAT_ES_Event> EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventsHappeningNowByArtistId(artistId, timeToAdd);
                var list = new EventModel().FromDataModelList(EventModel).ToList();

                if (list.Count == 0) { return null; }

                return await this.GetByCodeEvent(list.First().EventCode, null);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        /// <summary>
        /// This method returns customer booked events with details
        /// </summary>
        /// <param name="Id">Id of customer</param>
        /// <returns>List of Events</returns>
        public async Task<IEnumerable<EventModel>> GetCustomerEventsWithDetials(int Id)
        {
            try
            {
                //call customer booked events service to fetch customer events
                var customerBookedEventsResponse = await NatClient.ReadAsync<IEnumerable<BookedEventsModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetCustomerBookedEvents/" + Id);

                //check customer booked events service is successfull or not else throw exception
                if (customerBookedEventsResponse.status.IsSuccessStatusCode != true || customerBookedEventsResponse.data == null) throw new Exception("Unable to fetch customer booked events.");

                var eventModelList = new List<EventModel>();
                foreach (BookedEventsModel data in customerBookedEventsResponse.data)
                {
                    eventModelList.Add(await this.GetByCodeEvent(data.EventCode, Convert.ToInt64(Id)));
                }
                return eventModelList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns customer Liked events with details
        /// </summary>
        /// <param name="Id">Id of customer</param>
        /// <returns>List of Events</returns>
        public async Task<IEnumerable<EventModel>> GetCustomerLikedEventsWithDetials(int Id)
        {
            try
            {
                var eventModelList = new List<EventModel>(); ;
                var CustomerLikedEventsdata = await NatClient.ReadAsync<IEnumerable<CustomerLikedEventsModel>>(NatClient.Method.GET, NatClient.Service.CustomerService, "Customer/" + Convert.ToString(Id) + "/likedevents");
                foreach (CustomerLikedEventsModel data in CustomerLikedEventsdata.data)
                {
                    eventModelList.Add(await this.GetByCodeEvent(data.EventCode, Convert.ToInt64(Id)));
                }

                return eventModelList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Event with a given Code
        /// </summary>
        /// <param name="Code">Code of Event</param>
        /// <returns>Event service model</returns>
        public async Task<EventModel> GetByCodeEvent(string code, long? customerId)
        {
            try
            {
                EventModel data = null;
                List<EventModel> eventList = null;
                NAT_ES_Event EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(code);

                if (EventModel != null)
                {
                    data = new EventModel().FromDataModel(EventModel);

                    await PopulateLocationInEvent(data);

                    var facilityIconLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("VENUE_FACILITY");
                    var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((data.EventDate ?? System.DateTime.UtcNow), data.LocationCode, null);
                    var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((data.EventStartTime ?? System.DateTime.UtcNow), data.LocationCode, null);
                    var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((data.EventEndTime ?? System.DateTime.UtcNow), data.LocationCode, null);
                    //var venuelov = lookupListVenue.data.ToDictionary(item => item.AddressId, item => item);
                    data.LikeStatus = false;                    
                    data.TimeZone = markettime.TimeZone;
                    
                    foreach (EventFacilityModel facility in data.NatEsEventFacility)
                    {
                        facility.FacilityIcon = facilityIconLookup[facility.FacilityLKPId.ToString()].LookupDescription;
                        facility.ObjectState = ObjectState.Modified;
                    }
                    eventList = new List<EventModel>();
                    eventList.Add(data);
                    await PopulateEventLookupValues(customerId, null, null, eventList);
                    data = eventList.FirstOrDefault();
                    return data;
                }
                else
                {
                    throw new Exception("Event with such Event Code not Found!");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        private static async Task PopulateLocationInEvent(EventModel data)
        {
            // Get the market details
            var marketCode = data.LocationCode;

            var marketResp = await NatClient.ReadAsync<LocationViewModel>(NatClient.Method.GET, NatClient.Service.LocationService, "GetLocationByCode/" + marketCode);

            if (marketResp != null && marketResp.status.IsSuccessStatusCode)
            {
                var marketViewModel = marketResp.data;
                data.Location = marketViewModel;
            }
        }


        /// <summary>
        /// This method returns Event SeatingPlan with a given Id
        /// </summary>
        /// <param name="Id">Id of Event</param>
        /// <returns>Event service model</returns>
        public async Task<EventSeatingPlanModel> GetEventSeatingPlanByIdAsync(int Id)

        {
            try
            {
                EventSeatingPlanModel data = null;
                var eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(Id);
                var eventTicketPriceList = (await GetEventTicketByEventId(Id)).ToList();
                var seatTypeLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("SEAT_TYPE");
                var eventTicketPriceDictionary = eventTicketPriceList.ToDictionary(item => item.SeatTypeLKP, item => item);
                if (eventModel.Virtual)
                {
                    var ticketsResp = await NatClient.ReadAsync<List<TicketModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetTicketsByEventCode/" + eventModel.Event_Code);
                    if (!ticketsResp.status.IsSuccessStatusCode) throw new Exception("Unable to get booked seats");
                    Dictionary<string, TicketModel> ticketDictionary = ticketsResp.data.ToDictionary(item => item.RowNumber + item.SeatNumber, item => item);
                    data = new EventSeatingPlanModel();
                    data.NatEsEventSeat = new List<EventSeatModel>();

                    for (int i = 0; i < eventModel.Capacity; i++)
                    {
                        data.NatEsEventSeat.Add(new EventSeatModel()
                        {
                            SeatTypeLKPId = 3,
                            RowNumber = "A",
                            SeatNumber = (i + 1).ToString(),
                            StatusLKP = ticketDictionary.ContainsKey("A" + (i + 1).ToString()) && ticketDictionary["A" + (i + 1).ToString()].StatusLkp == "Confirmed" ? "3" : null
                        });
                    }
                }
                else
                {
                    NAT_ES_Event_Seating_Plan EventSeatingModel = uow.RepositoryAsync<NAT_ES_Event_Seating_Plan>().GetEventSeatingPlanById(Id);
                    data = new EventSeatingPlanModel().FromDataModel(EventSeatingModel);
                }
                if (data != null)
                {
                    if (data.NatEsEventSeat != null)
                    {
                        foreach (EventSeatModel seat in data.NatEsEventSeat)
                        {
                            seat.SeatTypeLKPValue = seatTypeLookup.ContainsKey(seat.SeatTypeLKPId.ToString()) ? seatTypeLookup[seat.SeatTypeLKPId.ToString()].VisibleValue : null;
                            seat.Price = eventTicketPriceDictionary.ContainsKey(seat.SeatTypeLKPId.ToString()) ? eventTicketPriceDictionary[seat.SeatTypeLKPId.ToString()].Price.Value : 0;
                        }
                        return data;
                    }
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<bool> BookEventSeatsAsync(IEnumerable<SeatBookingModel> bookingInfo)

        {
            try
            {
                string eventCode = null;
                foreach (SeatBookingModel booking in bookingInfo)
                {
                    eventCode = booking.EventCode;
                    NAT_ES_Event_Seat seat = await uow.RepositoryAsync<NAT_ES_Event_Seat>().GetEventSeatingPlanByEventCodeAsync(booking.EventCode, booking.RowNumber, booking.SeatNumber);
                    if (seat != null)
                    {
                        seat.Status_LKP = booking.Status;
                        seat.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<NAT_ES_Event_Seat>().Update(seat);
                    }
                }
                int updatedRows = uow.SaveChanges();

                //for virual event there is not seats to be updated
                if (!String.IsNullOrEmpty(eventCode))
                {
                    var eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);
                    if (eventModel.Virtual)
                    {
                        return true;
                    }
                }
                if (updatedRows == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        /// <summary>
        /// Create: Method for creation of Event
        /// </summary>
        /// <param name="servicemodel">Service Event Model</param>
        /// <returns>Event ID generated for Event</returns>
        public async Task<string> CreateEvent(EventModel servicemodel, PaintingViewModel paintingData, VenueModel venueModel, ArtistModel artistModel, bool BulkCreation = false)
        {
            try
            {
                if (servicemodel != null)
                {

                    var EventCodeSeq = entityContext.GetNextSequenceEventCode();
                    long? nextSequenceValue = EventCodeSeq.Single();
                    Int32 eventcode = Convert.ToInt32(nextSequenceValue.Value);
                    VenueModel Venuedata = venueModel;
                    ArtistModel artistResponse = artistModel;
                    var venueAddress = venueModel.NatVsVenueAddress.Address;

                    servicemodel.EventCode = servicemodel.LocationCode + "-" + eventcode;
                    servicemodel.VenueAddress = String.IsNullOrEmpty(servicemodel.VenueAddress) ? venueAddress : servicemodel.VenueAddress;
                    servicemodel.VenueName = venueModel.VenueName;
                    

                    //Book Artist for event
                    #region Artist
                    Core.BaseModelClass.ResponseViewModel<ArtistEventModel> BookArtistdata = null;
                    
                    if (servicemodel.ArtistId != null)
                    {
                        //artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(servicemodel.ArtistId));
                        ArtistEventModel artisteventmodel = new ArtistEventModel();
                        artisteventmodel.ArtistId = Convert.ToInt32(servicemodel.ArtistId);
                        artisteventmodel.Title = servicemodel.EventName;
                        artisteventmodel.Description = servicemodel.EventDescription;
                        artisteventmodel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                        artisteventmodel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                        artisteventmodel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                        artisteventmodel.ReferenceId = servicemodel.EventCode;
                        artisteventmodel.StatusLKPId = servicemodel.EventStatusLKPId;
                        artisteventmodel.Forced = true;
                        artisteventmodel.UDF = null;
                        artisteventmodel.LocationCode = servicemodel.LocationCode;
                        artisteventmodel.Online = false;//servicemodel.Virtual;

                        BookArtistdata = await NatClient.ReadAsync<ArtistEventModel>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artisteventmodel);
                        if (!BookArtistdata.status.IsSuccessStatusCode || BookArtistdata.data == null)
                        {
                            throw new Exception("Unable to book artist for this event");                            
                        }
                    }

                    if (servicemodel.ArtistId != null && BookArtistdata.status.IsSuccessStatusCode && BookArtistdata.data != null)
                    {
                        servicemodel.GoogleHangoutUrl = BookArtistdata.data.GoogleHangoutUrl;
                    }
                    else
                    {
                        servicemodel.EventStatusLKPId = 2; //Pending
                    }
                    #endregion
                    //Book Venue for event
                    #region Venue
                    Core.BaseModelClass.ResponseViewModel<bool> BookVenuedata = null;
                    VenueEventModel venueeventmodel = new VenueEventModel();
                    venueeventmodel.VenueId = Convert.ToInt32(servicemodel.VenueId);
                    venueeventmodel.Title = servicemodel.EventName;
                    venueeventmodel.Description = servicemodel.EventDescription;
                    venueeventmodel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                    venueeventmodel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                    venueeventmodel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                    venueeventmodel.ReferenceId = servicemodel.EventCode;
                    venueeventmodel.StatusLKPId = servicemodel.EventStatusLKPId;
                    venueeventmodel.Forced = true;
                    venueeventmodel.UDF = null;

                    BookVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.VenueService, "BookVenueEvent", requestBody: venueeventmodel);
                    if (!BookVenuedata.status.IsSuccessStatusCode || BookVenuedata.data == false)
                    {
                        throw new Exception("Unable to book venue for this event. " + BookVenuedata.status.message);
                    }
                    #endregion


                    
                    // NAT_ES_Event: Place checks for tenant id, Event_Status_LKP_ID (database level), stripe id  (validations code level) 

                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    if (servicemodel.NatEsEventSeatingPlan != null)
                    {
                        foreach (EventSeatingPlanModel eventSeatingPlan in servicemodel.NatEsEventSeatingPlan)
                        {
                            eventSeatingPlan.ActiveFlag = true;
                            eventSeatingPlan.SeatingPlanId = 0;
                            eventSeatingPlan.ObjectState = ObjectState.Added;
                            if (eventSeatingPlan.NatEsEventSeat != null)
                            {
                                foreach (EventSeatModel eventSeat in eventSeatingPlan.NatEsEventSeat)
                                {
                                    eventSeat.ActiveFlag = true;
                                    eventSeat.SeatId = 0;
                                    eventSeat.ObjectState = ObjectState.Added;
                                }
                            }
                        }
                    }

                    if (servicemodel.NatEsEventTicketPrice != null)
                    {
                        foreach (EventTicketPriceModel eventTicketPrice in servicemodel.NatEsEventTicketPrice)
                        {
                            eventTicketPrice.ActiveFlag = true;
                            eventTicketPrice.ObjectState = ObjectState.Added;
                        }
                    }

                    if (servicemodel.NatEsEventFacility != null)
                    {
                        foreach (EventFacilityModel facility in servicemodel.NatEsEventFacility)
                        {
                            facility.ObjectState = ObjectState.Added;
                        }
                    }

                    #region Painting & Kit Items
                    var painting = paintingData;

                    if (paintingData != null)
                    {
                        var paintingResp = await NatClient.ReadAsync<PaintingViewModel>(NatClient.Method.GET, NatClient.Service.PaintingService, "paintings/" + servicemodel.PaintingId);
                        if (!paintingResp.status.IsSuccessStatusCode) throw new Exception("Unable get painting. " + paintingResp.status.message);

                        painting = paintingResp.data;
                    }
                    var kitItemLovResp = await NatClient.ReadAsync<List<KitItemLovViewModel>>(NatClient.Method.GET, NatClient.Service.SuppliesService, "KitItemLov");
                    if (!kitItemLovResp.status.IsSuccessStatusCode) throw new Exception("Unable get Kit items. " + kitItemLovResp.status.message);

                    var kitItemDictionary = kitItemLovResp.data.ToDictionary(x => x.KitItemId, x => x);

                    var configResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/Event_Supply_Order_Link_Template");

                    if (configResponse.status.IsSuccessStatusCode != true || configResponse.data == null) throw new Exception("Configuration service failed. " + configResponse.status.message);

                    string supplyOrderLink = "";
                    string suppliesDescrition = "";
                    if (painting.NatPaintingKitItem != null)
                    {
                        List<string> itemIdList = new List<string>();
                        List<string> itemQtyList = new List<string>();

                        foreach (var item in painting.NatPaintingKitItem)
                        {
                            var kitItem = kitItemDictionary[item.KitItemId ?? default];
                            if (kitItem != null && kitItem.StoreId != null && kitItem.StoreId != "" && item.Quantity > 0)
                            {
                                itemIdList.Add(kitItem.StoreId);
                                itemQtyList.Add(item.Quantity.ToString());
                                suppliesDescrition += "<li>" + item.Quantity + " x " + kitItem.KitItemName + "</li>";
                            }
                        }
                        if (itemIdList.Count > 0 && itemQtyList.Count == itemIdList.Count)
                        {
                            supplyOrderLink = Environment.GetEnvironmentVariable("ArtMartBaseUrl") + "cart?additems&productIds=" + string.Join(",", itemIdList) + "&quantities=" + string.Join(",", itemQtyList);
                            suppliesDescrition = "<ul>" + suppliesDescrition + "</ul>";
                        }
                    }

                    if (supplyOrderLink != "" && suppliesDescrition != "")
                    {
                        servicemodel.EventDescription = servicemodel.EventDescription + "<br>" + String.Format(configResponse.data.Value, suppliesDescrition, supplyOrderLink);
                    }
                    #endregion

                    //convert event time and dates to market timezone
                    var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventDate ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventStartTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventEndTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    TableStorage tableStorage = new TableStorage();

                    #region Zoom Link
                    if (servicemodel.Virtual)
                    {
                        await CreateZoomLink(servicemodel, artistResponse, marketStarttime, tableStorage);
                    }
                    #endregion

                    // NAT_CS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                    // Person Email, Date_Of_Birth, Person_Extension (validations code level)
                    Insert(servicemodel);
                    await uow.SaveChangesAsync();
                    servicemodel.EventId = Get().EventId;

                    // Add this to the Azure Table Storage
                    // We use this for the cancellation flow
                    var eventCode = servicemodel.EventCode;
                    string eventWithSalt = eventCode + "paintception" + (new Random().Next().ToString());

                    System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
                    byte[] hash = hashAlgo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(eventWithSalt));

                    var rowKey = System.Convert.ToBase64String(hash);
                    rowKey = rowKey.Replace("/", "");
                    rowKey = rowKey.Replace("=", "");
                    servicemodel.EventHash = rowKey;

                    
                    EventUnavailabilityEntities eventTableEntity = new EventUnavailabilityEntities(rowKey, servicemodel);

                    eventTableEntity.ResetTableEntityForVenueCancellation();
                    eventTableEntity.ResetTableEntityForArtistCancellation();
                    eventTableEntity.CurrentArtist = servicemodel.ArtistId;
                    eventTableEntity.CurrentVenue = servicemodel.VenueId;

                    // No need to await for this execution
                    await tableStorage.InsertTableStorage("EventCancellationEntities", eventTableEntity);

                    //await PopulateLocationInEvent(servicemodel);
                    servicemodel.EventDate = markettime.MarketTime;
                    servicemodel.EventStartTime = marketStarttime.MarketTime;
                    servicemodel.EventEndTime = marketEndtime.MarketTime;
                    servicemodel.TimeZone = marketStarttime.TimeZone;

                    //Send notification to artist
                    await SendEventCreationNotificationToArtist(servicemodel, tableStorage, BulkCreation, artistResponse);

                    //Send notification to venue
                    await SendEventCreationNotificationToVenue(servicemodel, Venuedata, BulkCreation);

                    return Convert.ToString(servicemodel.EventId);
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

        private static async Task CreateZoomLink(EventModel servicemodel, ArtistModel artistResponse, MarketTimeZoneModel marketStarttime, TableStorage tableStorage)
        {
            string filter = TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, servicemodel.EventCode),
                                        TableOperators.And,
                                        TableQuery.GenerateFilterCondition("Role", QueryComparisons.Equal, "1"));

            TableStorage ts = new TableStorage();
            List<VitualEventLinkTableEntityModel> virtualLinkExists = (await ts.RetrieveTableStorage<VitualEventLinkTableEntityModel>("VirtualEventLink", filter)).ToList();
            ZoomMeeting zoomObj = null;
            if (virtualLinkExists != null && virtualLinkExists.Count > 0)
            {
                servicemodel.MeetingLink = virtualLinkExists.FirstOrDefault().ZoomStartLink;
            }
            else
            {
                string apiKey = artistResponse.VideoKey??Environment.GetEnvironmentVariable("ZoomApiKey");
                string apiSecret = artistResponse.VideoSecret??Environment.GetEnvironmentVariable("ZoomApiSecret");
                string apiUser = artistResponse.VideoUser??Environment.GetEnvironmentVariable("ZoomUserId");

                zoomObj = ZoomMeeting.CreateMeeting(apiKey, apiSecret, apiUser, servicemodel.EventName, ((DateTime?)marketStarttime.MarketTime) ?? default, 2);

                var virtualEventLink = new VitualEventLinkTableEntityModel();
                if (artistResponse != null)
                {
                    virtualEventLink.Email = artistResponse.ArtistEmail;
                    virtualEventLink.Name = artistResponse.ArtistFirstName + " " + artistResponse.ArtistLastName;
                }
                virtualEventLink.PartitionKey = servicemodel.EventCode;
                var linkGuid = Guid.NewGuid().ToString();
                virtualEventLink.Link = Environment.GetEnvironmentVariable("PublicPortalBaseUrl") + "event/join/" + linkGuid;
                virtualEventLink.RowKey = linkGuid;
                virtualEventLink.Role = "1";
                virtualEventLink.MeetingNumber = zoomObj.id.ToString();
                virtualEventLink.Password = zoomObj.password;
                virtualEventLink.ZoomStartLink = zoomObj.start_url;
                await tableStorage.InsertTableStorage("VirtualEventLink", virtualEventLink);
                servicemodel.MeetingLink = zoomObj.start_url;
            }
        }

        private async Task SendEventCreationNotificationToVenue(EventModel servicemodel, VenueModel Venuedata, bool BulkCreation)
        {
            if (!servicemodel.Virtual)
            {
                IDictionary<String, String> emailNameDict = new Dictionary<String, String>();
                IDictionary<String, String> smsContactDict = new Dictionary<String, String>();
                servicemodel.IsVenue = false;
                emailNameDict = new Dictionary<string, string>();
                foreach (VenueContactPersonModel venueContactPerson in Venuedata.NatVsVenueContactPerson)
                {
                    if (venueContactPerson.Email != null && venueContactPerson.Email != "" && !emailNameDict.ContainsKey(venueContactPerson.Email))
                    {
                        emailNameDict.Add(new KeyValuePair<String, String>(venueContactPerson.Email, venueContactPerson.FirstName + " " + venueContactPerson.LastName));
                        if (venueContactPerson.TextFlag != true)
                            smsContactDict.Add(new KeyValuePair<String, String>(venueContactPerson.Email, venueContactPerson.FirstName + " " + venueContactPerson.LastName));
                    }
                }
                await SendEmailsAsync(emailNameDict, servicemodel, "EventCreationEmailForVenue", Venuedata.VenueId.ToString(), null, BulkCreation); // sending email to venue
            }
        }

        private async Task SendEventCreationNotificationToArtist(EventModel servicemodel, TableStorage tableStorage, bool BulkCreation, ArtistModel ArtistData)
        {
            if (servicemodel.ArtistId != null)
            {
                IDictionary<String, String> emailNameDict = new Dictionary<String, String>();
                IDictionary<String, String> smsContactDict = new Dictionary<String, String>();
                //call get artist service to fetch artist details 
                var artistResponse = ArtistData;//await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(servicemodel.ArtistId));
                var artistEmailTemplate = "EventCreationEmail";
                //if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                //{                          
                artistEmailTemplate = "VirtualEventCreationEmail";
                emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.CompanyEmail, artistResponse.ArtistFirstName + " " + artistResponse.ArtistLastName));
                smsContactDict.Add(new KeyValuePair<String, String>(artistResponse.CompanyPhone, artistResponse.ArtistFirstName + " " + artistResponse.ArtistLastName));
                //}
                await SendEmailsAsync(emailNameDict, servicemodel, artistEmailTemplate, servicemodel.ArtistId.ToString(), null, BulkCreation); // sending email to artist
            }
        }

        public async Task<List<string>> ScheduleEvents(IEnumerable<EventModel> serviceModelList)
        {
            try
            {
                var virtualVenueId = Convert.ToInt32(Environment.GetEnvironmentVariable("VirtualVenueId"));
                var virutalEvents = serviceModelList.FirstOrDefault().Virtual == true;
                var eventCodeList = new List<string>();
                var eventModelList = serviceModelList.ToList();
                var paintingListResp = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintings/sorted/ticketsSold");
                if (!paintingListResp.status.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to load painting list. " + paintingListResp.status.message);
                }
                var paintingList = paintingListResp.data.ToList();
                //if (eventModelList.Count > 0 && eventModelList[0].Virtual)
                //{
                //    paintingList = paintingList.Where(x => x.Tags.ToLower().Contains("virtual")).ToList();
                //}

                var configPaintingUsedInSameVenueInLastXDaysResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingPaintingUsedInSameVenueInLastXDays");
                if (configPaintingUsedInSameVenueInLastXDaysResponse.status.IsSuccessStatusCode != true || configPaintingUsedInSameVenueInLastXDaysResponse.data == null) throw new Exception("Configuration service failed. " + configPaintingUsedInSameVenueInLastXDaysResponse.status.message);
                var AutoPaintingPaintingUsedInSameVenueInLastXDays = Convert.ToInt32(configPaintingUsedInSameVenueInLastXDaysResponse.data.Value);

                var configDaysPastAfterPaintingWasLastUsedResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingXDaysPastAfterPaintingWasLastUsed");
                if (configDaysPastAfterPaintingWasLastUsedResponse.status.IsSuccessStatusCode != true || configDaysPastAfterPaintingWasLastUsedResponse.data == null) throw new Exception("Configuration service failed. " + configDaysPastAfterPaintingWasLastUsedResponse.status.message);
                var AutoPaintingXDaysPastAfterPaintingWasLastUsed = Convert.ToInt32(configDaysPastAfterPaintingWasLastUsedResponse.data.Value);

                Dictionary<int, VenueModel> venueList = new Dictionary<int, VenueModel>();
                if (virutalEvents)
                {
                    var Venuedata = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + virtualVenueId);
                    if (Venuedata.status.IsSuccessStatusCode && Venuedata.data != null)
                    {
                        venueList.Add(Venuedata.data.VenueId, Venuedata.data);
                    }
                }
                else
                {
                    var venueIdList = serviceModelList.Select(v => v.VenueId).Distinct();
                    foreach (var data in venueIdList)
                    {
                        var Venuedata = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + Convert.ToString(data));
                        if (Venuedata.status.IsSuccessStatusCode && Venuedata.data != null)
                        {
                            venueList.Add(Venuedata.data.VenueId, Venuedata.data);
                        }
                    }
                }

                foreach (var eventModel in eventModelList)
                {
                    if (virutalEvents)
                    {
                        eventModel.VenueId = virtualVenueId;
                    }

                    //Select Artist for event
                    var artistSearchResp = await NatClient.ReadAsync<DataSourceResultModel<ArtistModel>>(NatClient.Method.POST, NatClient.Service.ArtistService, "artists/searchforevent", requestBody: new
                    {
                        AvailabilityStartTime = eventModel.EventStartTime,
                        AvailabilityEndTime = eventModel.EventEndTime,
                        LocationCode = eventModel.LocationCode,
                        VirtualEvent = eventModel.Virtual
                    });
                    if (!artistSearchResp.status.IsSuccessStatusCode)
                    {
                        throw new Exception("Error occured while finding artist." + artistSearchResp.status.message);
                    }

                    var venue = venueList[(int)eventModel.VenueId];
                    var artistList = artistSearchResp.data.Data;
                    var availableArtistList = artistList.Where(x => x.IsArtistAvailable).ToList();
                    var filteredArtist = availableArtistList.Where(x => !venue.NatVsVenueArtistPreference.Any(y => y.ArtistId == x.ArtistId)).ToList();
                    var artist = filteredArtist.FirstOrDefault();

                    if (artist != null && artist.IsArtistAvailable)
                    {
                        eventModel.ArtistId = artist.ArtistId;
                    }
                    //Select painting for event
                    PaintingViewModel selectedPainting = null;
                    foreach(var painting in paintingList)
                    {
                        if (!virutalEvents)
                        {
                            var checkVenueDate = (eventModel.EventStartTime ?? default).AddDays(-1 * AutoPaintingPaintingUsedInSameVenueInLastXDays);
                            var venueCheck = uow.RepositoryAsync<NAT_ES_Event>()
                                .Queryable()
                                .Any(x => x.Venue_ID == eventModel.VenueId && x.Painting_ID == painting.PaintingId && x.Event_Start_Time > checkVenueDate && x.Event_End_Time < eventModel.EventStartTime);

                            if (venueCheck) continue;
                        }

                        var checkPaintingDate = (eventModel.EventStartTime ?? default).AddDays(-1 * AutoPaintingXDaysPastAfterPaintingWasLastUsed);
                        var paintingMarketCheck = uow.RepositoryAsync<NAT_ES_Event>()
                            .Queryable()
                            .Any(x => x.Painting_ID == painting.PaintingId && x.Location_Code == eventModel.LocationCode && x.Event_Start_Time > checkPaintingDate && x.Event_End_Time < eventModel.EventStartTime);

                        if (paintingMarketCheck) continue;

                        selectedPainting = painting;
                        break;
                    }

                    if(selectedPainting == null)
                    {
                        var lastUsedPaintingListResp = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintings/sorted/lastUsed");
                        if (!lastUsedPaintingListResp.status.IsSuccessStatusCode)
                        {
                            throw new Exception("Failed to load painting list. " + lastUsedPaintingListResp.status.message);
                        }
                        var lastUsedPaintingList = lastUsedPaintingListResp.data.ToList();
                        selectedPainting = lastUsedPaintingList.Last();
                    }

                    eventModel.PaintingId = selectedPainting.PaintingId;
                    eventModel.EventName = selectedPainting.PaintingName;

                    //Call Create Event
                    eventCodeList.Add(await this.CreateEvent(eventModel, selectedPainting, venue, artist, true));
                    
                }
                return eventCodeList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<EventFeedbackModel> EventFeedback(EventFeedbackModel servicemodel)
        {
            try
            {
                servicemodel.ActiveFlag = true;
                servicemodel.ObjectState = ObjectState.Added;

                uow.RepositoryAsync<NAT_ES_Event_Feedback>().Insert(servicemodel.ToDataModel(servicemodel));


                VenueImageViewModel venueeventmodel = new VenueImageViewModel();


                venueeventmodel.ImagePath = servicemodel.ImagePath;
                venueeventmodel.ImageTypeLKPId = Constants.VENUE_IMAGE_POST_EVENT_LKP_HIDDEN;
                NAT_ES_Event EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(servicemodel.EventId);
                venueeventmodel.VenueId = EventModel.Venue_ID;
                var VenueimgResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.VenueService, "venues/UploadVenueSpaceImage", requestBody: venueeventmodel);
                if (VenueimgResp.status.IsSuccessStatusCode)
                {
                    await uow.SaveChangesAsync();
                    return servicemodel;
                }
                else
                {
                    throw new Exception("Unable to upload Feedback");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        //Bulk update Service
        public async Task<Boolean> BulkUpdate(IEnumerable<EventModel> servicemodel)
        {
            try
            {
                //Warning Api to check all colliding events
                EventCollidingViewModel collideModel = new EventCollidingViewModel();
                collideModel.SelectedEvents = new List<EventDurationViewModel>();
                //List<string> eventcodes = new List<string>();
                foreach (EventModel obj in servicemodel)
                {
                    //eventcodes.Add(obj.EventCode);
                    EventDurationViewModel durationmodel = new EventDurationViewModel();
                    //Artist planner id 
                    durationmodel.PlannerId = obj.ArtistPlannerId;
                    durationmodel.StartTime = obj.EventStartTime.Value;
                    durationmodel.EndTime = obj.EventEndTime.Value;
                    durationmodel.ReferenceId = obj.EventCode;
                    collideModel.SelectedEvents.Add(durationmodel);
                    //Venue planner id
                    EventDurationViewModel durationmodel1 = new EventDurationViewModel();
                    durationmodel1.PlannerId = obj.VenuePlannerId;
                    durationmodel1.StartTime = obj.EventStartTime.Value;
                    durationmodel1.EndTime = obj.EventEndTime.Value;
                    durationmodel1.ReferenceId = obj.EventCode;
                    collideModel.SelectedEvents.Add(durationmodel1);

                }

                var warningresp = await NatClient.ReadAsync<EventCollidingViewModel>(NatClient.Method.POST, NatClient.Service.PlannerService, "GetCollidingEvents", requestBody: collideModel);
                if (warningresp.status.IsSuccessStatusCode)
                {

                    EventCollidingViewModel warningmodel = warningresp.data;
                    if (warningmodel.CollidingEventCodes != null)
                    {
                        foreach (EventDurationViewModel blob in warningmodel.SelectedEvents)
                        {
                            var CheckArtist = await NatClient.ReadAsync<Boolean>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/planner/" + blob.PlannerId);
                            if (CheckArtist.status.IsSuccessStatusCode && CheckArtist.data)
                            {
                                //call cancel artist for every colliding event
                                foreach (string str in blob.CollidingEventCodes)
                                {
                                    var CancelArtistresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + str);
                                    if (!CancelArtistresp.status.IsSuccessStatusCode || !CancelArtistresp.data)
                                    {
                                        throw new Exception("Unable to cancel colliding events for artist");
                                    }
                                    else
                                    {
                                        await RemoveArtistIDFromEventAndUpdateEventStatus(str);

                                    }


                                }



                            }
                            else
                            {
                                var CheckVenue = await NatClient.ReadAsync<Boolean>(NatClient.Method.GET, NatClient.Service.VenueService, "Venues/planner/" + blob.PlannerId);
                                if (CheckVenue.status.IsSuccessStatusCode && CheckVenue.data)
                                {
                                    //call cancel venue
                                    foreach (string str in blob.CollidingEventCodes)
                                    {
                                        var CancelVenueresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + str);
                                        if (!CancelVenueresp.status.IsSuccessStatusCode || !CancelVenueresp.data)
                                        {
                                            throw new Exception("Unable to cancel colliding events for Venue");
                                        }
                                        else
                                        {
                                            await RemoveVenueIDFromEventAndUpdateEventStatus(str);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Unable to check plannerid against Artist and Venue");

                                }
                            }
                        }
                    }


                    //final update

                    foreach (EventModel obj in servicemodel)
                    {
                        NAT_ES_Event EventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(obj.EventCode);
                        if (obj.EventStartTime != EventModel.Event_Start_Time || obj.EventEndTime != EventModel.Event_End_Time)
                        {
                            var CancelArtistresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + obj.EventCode);
                            if (!CancelArtistresp.status.IsSuccessStatusCode || !CancelArtistresp.data)
                            {
                                throw new Exception("Unable to cancel old artist for event");
                            }
                            var CancelVenueresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + obj.EventCode);
                            if (!CancelVenueresp.status.IsSuccessStatusCode || !CancelVenueresp.data)
                            {
                                throw new Exception("Unable to cancel old venue for event");
                            }

                            //book artist for new dates
                            ArtistEventModel artisteventmodel = new ArtistEventModel();
                            artisteventmodel.ArtistId = Convert.ToInt32(obj.ArtistId);
                            artisteventmodel.Title = obj.EventName;
                            artisteventmodel.Description = obj.EventDescription;
                            artisteventmodel.EventTypeLKPId = obj.EventTypeLKPId;
                            artisteventmodel.StartTime = Convert.ToDateTime(obj.EventStartTime);
                            artisteventmodel.EndTime = Convert.ToDateTime(obj.EventEndTime);
                            artisteventmodel.ReferenceId = obj.EventCode;
                            artisteventmodel.StatusLKPId = obj.EventStatusLKPId;
                            artisteventmodel.Forced = true;
                            artisteventmodel.LocationCode = obj.LocationCode;
                            artisteventmodel.UDF = null;
                            ////

                            //call book artist for event service
                            var BookArtistdata = await NatClient.ReadAsync<Object>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artisteventmodel);

                            //check response for book artist event service if not successfull then throw exception
                            if (BookArtistdata.status.IsSuccessStatusCode == false || BookArtistdata.data == null) { throw new Exception("Unable to book new artist for event"); }

                            //book Venue for new dates
                            VenueEventModel venueeventmodel = new VenueEventModel();
                            venueeventmodel.VenueId = Convert.ToInt32(obj.VenueId);
                            venueeventmodel.Title = obj.EventName;
                            venueeventmodel.Description = obj.EventDescription;
                            venueeventmodel.EventTypeLKPId = obj.EventTypeLKPId;
                            venueeventmodel.StartTime = Convert.ToDateTime(obj.EventStartTime);
                            venueeventmodel.EndTime = Convert.ToDateTime(obj.EventEndTime);
                            venueeventmodel.ReferenceId = obj.EventCode;
                            venueeventmodel.StatusLKPId = obj.EventStatusLKPId;
                            venueeventmodel.Forced = true;
                            venueeventmodel.UDF = null;


                            var BookVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.VenueService, "BookVenueEvent", requestBody: venueeventmodel);

                            if (BookVenuedata.status.IsSuccessStatusCode == false || BookVenuedata.data == false) { throw new Exception("Unable to book new venue for event"); }


                            foreach (EventTicketPriceModel ticket in obj.NatEsEventTicketPrice)
                            {
                                ticket.ObjectState = ObjectState.Modified;
                            }


                            if (obj.EventId != 0 || obj.EventId > 0)
                            {
                                // NAT_ES_Event: stripe id  (validations code level)
                                obj.ObjectState = ObjectState.Modified;
                                base.Update(obj);

                            }
                        }
                        //check for artist or venue change
                        else if (obj.ArtistId != EventModel.Artist_ID || obj.VenueId != EventModel.Venue_ID)
                        {
                            ////check for artist change
                            if (obj.ArtistId != EventModel.Artist_ID)
                            {
                                var CancelArtistresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + obj.EventCode);
                                if (!CancelArtistresp.status.IsSuccessStatusCode || !CancelArtistresp.data)
                                {
                                    throw new Exception("Unable to cancel old artist for event");
                                }
                                //book artist for new dates
                                ArtistEventModel artisteventmodel = new ArtistEventModel();
                                artisteventmodel.ArtistId = Convert.ToInt32(obj.ArtistId);
                                artisteventmodel.Title = obj.EventName;
                                artisteventmodel.Description = obj.EventDescription;
                                artisteventmodel.EventTypeLKPId = obj.EventTypeLKPId;
                                artisteventmodel.StartTime = Convert.ToDateTime(obj.EventStartTime);
                                artisteventmodel.EndTime = Convert.ToDateTime(obj.EventEndTime);
                                artisteventmodel.ReferenceId = obj.EventCode;
                                artisteventmodel.StatusLKPId = obj.EventStatusLKPId;
                                artisteventmodel.Forced = true;
                                artisteventmodel.LocationCode = obj.LocationCode;
                                artisteventmodel.UDF = null;
                                ////

                                //call book artist for event service
                                var BookArtistdata = await NatClient.ReadAsync<Object>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artisteventmodel);

                                //check response for book artist event service if not successfull then throw exception
                                if (BookArtistdata.status.IsSuccessStatusCode == false || BookArtistdata.data == null) { throw new Exception("Unable to book new artist for event"); }


                            }

                            //check for venue change
                            if (obj.VenueId != EventModel.Venue_ID)
                            {
                                var CancelVenueresp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + obj.EventCode);
                                if (!CancelVenueresp.status.IsSuccessStatusCode || !CancelVenueresp.data)
                                {
                                    throw new Exception("Unable to cancel old venue for event");
                                }

                                //book Venue for new dates
                                VenueEventModel venueeventmodel = new VenueEventModel();
                                venueeventmodel.VenueId = Convert.ToInt32(obj.VenueId);
                                venueeventmodel.Title = obj.EventName;
                                venueeventmodel.Description = obj.EventDescription;
                                venueeventmodel.EventTypeLKPId = obj.EventTypeLKPId;
                                venueeventmodel.StartTime = Convert.ToDateTime(obj.EventStartTime);
                                venueeventmodel.EndTime = Convert.ToDateTime(obj.EventEndTime);
                                venueeventmodel.ReferenceId = obj.EventCode;
                                venueeventmodel.StatusLKPId = obj.EventStatusLKPId;
                                venueeventmodel.Forced = true;
                                venueeventmodel.UDF = null;


                                var BookVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.VenueService, "BookVenueEvent", requestBody: venueeventmodel);

                                if (BookVenuedata.status.IsSuccessStatusCode == false || BookVenuedata.data == false) { throw new Exception("Unable to book new venue for event"); }

                            }


                            foreach (EventTicketPriceModel ticket in obj.NatEsEventTicketPrice)
                            {
                                ticket.ObjectState = ObjectState.Modified;
                            }


                            //update event table
                            if (obj.EventId != 0 || obj.EventId > 0)
                            {
                                // NAT_ES_Event: stripe id  (validations code level)
                                obj.ObjectState = ObjectState.Modified;
                                base.Update(obj);

                            }
                        }
                        else if (obj.NatEsEventTicketPrice != EventModel.NAT_ES_Event_Ticket_Price)
                        {
                            foreach (EventTicketPriceModel ticket in obj.NatEsEventTicketPrice)
                            {
                                ticket.ObjectState = ObjectState.Modified;
                                uow.Repository<EventTicketPriceModel>().Update(ticket);
                            }
                        }
                        else
                        {
                            throw new Exception("Nothing new to update");
                        }

                    }

                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        throw new Exception("not able to update");
                    }

                    return true;

                    //var BookArtistdata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artisteventmodel);

                    ////check response for book artist event service if not successfull then throw exception
                    //if (BookArtistdata.status.IsSuccessStatusCode == false || BookArtistdata.data == false)

                }
                else
                {
                    throw new Exception("Unable to hold off colliding events");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkUpdateArtist(IEnumerable<EventModel> serviceModels)
        {
            try
            {
                var eventList = serviceModels.ToList();
                foreach (var eventServiceModel in eventList)
                {
                    var eventEFModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(eventServiceModel.EventId);
                    if (eventEFModel.Artist_ID != eventServiceModel.ArtistId)
                    {
                        var cancelArtistResp = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + eventServiceModel.EventCode);
                        if (!cancelArtistResp.status.IsSuccessStatusCode) throw new Exception("Error in cancelling artist booking: " + cancelArtistResp.status.message);
                    }
                    await UpdateEventArtist(eventServiceModel.EventCode, eventServiceModel.ArtistId ?? default, eventServiceModel.ArtistName);

                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkUpdateVenue(IEnumerable<EventModel> serviceModels)
        {
            try
            {
                var eventList = serviceModels.ToList();
                foreach (var eventServiceModel in eventList)
                {
                    var eventEFModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(eventServiceModel.EventId);
                    if (eventEFModel.Venue_ID != eventServiceModel.VenueId)
                    {
                        var cancelVenueResp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + eventServiceModel.EventCode);
                        if (!cancelVenueResp.status.IsSuccessStatusCode || !cancelVenueResp.data) throw new Exception("Error in cancelling venue booking: " + cancelVenueResp.status.message);

                        await UpdateEventVenue(eventServiceModel.EventCode, eventServiceModel.VenueId ?? default, eventServiceModel.VenueName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async void CreateOrdersForEvent()
        {
            try
            {
                await NatClient.ReadAsync<object>(NatClient.Method.GET, NatClient.Service.SuppliesService, "CreateOrdersForEvent");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Cancel: Method for cancel of Event
        /// </summary>
        /// <param name="servicemodel">Service Event Model</param>

        public async Task<Boolean> CancelEvent(string eventcode)
        {
            try
            {
                if (eventcode != null)
                {
                    EventModel eventdata = null;
                    NAT_ES_Event eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventcode);
                    if (eventModel != null)
                    {
                        eventdata = new EventModel().FromDataModel(eventModel);

                        // Because event has been cancelled
                        // Its time to send the emails to all the ticket holders

                        var Bookingdata = await NatClient.ReadAsync<Object>(NatClient.Method.POST, NatClient.Service.TicketBookingService, "CancelAllBookings", null, requestBody: new { EventCode = eventModel.Event_Code });
                        if (!Bookingdata.status.IsSuccessStatusCode)
                            throw new Exception("Event cannot be cancelled: " + Bookingdata.status.message);
                        var CancelArtistdata = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + eventcode);
                        var CancelVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + eventcode);
                        if (CancelArtistdata.status.IsSuccessStatusCode && CancelVenuedata.status.IsSuccessStatusCode)
                        {
                            eventModel.Event_Status_LKP_ID = 3;
                            eventModel.ObjectState = ObjectState.Modified;
                            //NAT_ES_Event dataModel = new EventModel().ToDataModel(eventdata);
                            uow.Repository<NAT_ES_Event>().Update(eventModel);
                            int updatedRows = await uow.SaveChangesAsync();
                            if (updatedRows == 0)
                            {
                                return false;
                            }



                            EventModel m = new EventModel();
                            m.EventCode = eventModel.Event_Code;

                            var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.Event_Date ?? System.DateTime.UtcNow), eventdata.LocationCode, null);
                            var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.Event_Start_Time ?? System.DateTime.UtcNow), eventdata.LocationCode, null);
                            var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.Event_End_Time ?? System.DateTime.UtcNow), eventdata.LocationCode, null);

                            m.EventDate = markettime.MarketTime;
                            m.EventStartTime = marketStarttime.MarketTime;
                            m.EventEndTime = marketEndtime.MarketTime;
                            m.TimeZone = marketStarttime.TimeZone;
                            m.EventName = eventModel.Event_Name;

                            IDictionary<string, string> emailNameDict = new Dictionary<string, string>();
                            IDictionary<String, String> smsContactDict = new Dictionary<String, String>();

                            // Send emails to VCP
                            if (eventModel.Venue_ID != null && eventModel.Venue_ID > 0)
                            {
                                var venueResp = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + eventModel.Venue_ID);
                                if (venueResp.status.IsSuccessStatusCode && venueResp.data != null)
                                {
                                    m.VenueName = venueResp.data.VenueName;

                                    var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                                    if (lookupListAddress.status.IsSuccessStatusCode)
                                    {
                                        var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);
                                        var dataitems = addresslov[eventModel.Venue_ID];
                                        m.VenueAddress = dataitems.Address;

                                    }

                                    var venueContactPerson = venueResp.data.NatVsVenueContactPerson;
                                    emailNameDict = new Dictionary<string, string>();


                                    foreach (var contactPerson in venueContactPerson)
                                    {
                                        var email = contactPerson.Email;
                                        var name = contactPerson.FirstName + " " + contactPerson.LastName;
                                        emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                                        if (contactPerson.TextFlag != true)
                                            smsContactDict.Add(new KeyValuePair<String, String>(contactPerson.ContactNumber, name));
                                    }

                                    await SendEmailsAsync(emailNameDict, m, "EventCancellationTemplatewithVenue", venueResp.data.VenueId.ToString());
                                    await sendSMSAsync(smsContactDict, m, "EVENT_CANCELLATION_SMS", venueResp.data.VenueId.ToString());
                                }
                            }


                            // Send emails to Artist
                            if (eventModel.Artist_ID != null && eventModel.Artist_ID > 0)
                            {
                                var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(eventModel.Artist_ID));
                                if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                                {
                                    m.Name = artistResponse.data.ArtistFirstName;
                                    emailNameDict = new Dictionary<string, string>();
                                    smsContactDict = new Dictionary<string, string>();
                                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                                    smsContactDict.Add(new KeyValuePair<String, String>(artistResponse.data.ContactNumber, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                                    await sendSMSAsync(smsContactDict, m, "EVENT_CANCELLATION_SMS", artistResponse.data.ArtistId.ToString());

                                    if (eventModel.Venue_ID != null && eventModel.Venue_ID > 0)
                                    {
                                        await SendEmailsAsync(emailNameDict, m, "EventCancellationTemplatewithVenue", artistResponse.data.ArtistId.ToString());
                                    }
                                    else
                                    {
                                        await SendEmailsAsync(emailNameDict, m, "EventCancellationTemplateWithoutVenue", artistResponse.data.ArtistId.ToString());
                                    }

                                }
                            }



                            emailNameDict = new Dictionary<string, string>();

                            // Find the admins and send them email
                            var auth = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                            var userList = auth.data;
                            if (userList != null)
                            {
                                foreach (var user in userList)
                                {
                                    emailNameDict = new Dictionary<string, string>();
                                    emailNameDict.Add(new KeyValuePair<String, String>(user.Email, user.FirstName + " " + user.LastName));

                                    if (eventModel.Venue_ID != null && eventModel.Venue_ID > 0)
                                    {
                                        await SendEmailsAsync(emailNameDict, m, "EventCancellationTemplatewithVenue", user.UserId.ToString());
                                    }
                                    else
                                    {
                                        await SendEmailsAsync(emailNameDict, m, "EventCancellationTemplateWithoutVenue", user.UserId.ToString());
                                    }
                                }

                            }
                            return true;
                        }
                        else
                        { return false; }
                    }
                    else
                    { return false; }
                }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Create: Method for removing artist Id of Event and updating event status to pending
        /// </summary>
        /// <param name="eventCode">EventCode from which artistId is removed</param>
        /// <param name="artistId">ArtistId which is removed from event</param>
        /// <returns>True if artist Id is successfully removed and event status is successfully updated</returns>
        public async Task<EventModel> RemoveArtistIDFromEventAndUpdateEventStatus(string eventCode)
        {
            try
            {
                //check if event code or artist Id is null so throw exception
                if (eventCode == null) { throw new Exception("Invalid parameter eventCode."); }

                //get event object with event code
                NAT_ES_Event eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);

                //set artist Id to null i.e dissassociate artist from Event
                eventModel.Artist_ID = null;

                //update event status to pending i.e status Lookup Id 2
                eventModel.Event_Status_LKP_ID = 2;

                //update event object and save changes
                uow.Repository<NAT_ES_Event>().Update(eventModel);
                int updatedRows = await uow.SaveChangesAsync();

                //check if successfully updated event object else throw exception
                if (updatedRows == 0) { throw new Exception("Unable to remove artist Id and update event status to pending."); }

                //return event data model
                return new EventModel().FromDataModel(eventModel);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<EventModel> RemoveVenueIDFromEventAndUpdateEventStatus(string eventCode)
        {
            try
            {
                //check if event code or artist Id is null so throw exception
                if (eventCode == null) { throw new Exception("Invalid parameter eventCode."); }

                //get event object with event code
                NAT_ES_Event eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);

                //set artist Id to null i.e dissassociate artist from Event
                eventModel.Venue_ID = null;

                //update event status to pending i.e status Lookup Id 2
                eventModel.Event_Status_LKP_ID = 2;

                //update event object and save changes
                uow.Repository<NAT_ES_Event>().Update(eventModel);
                int updatedRows = await uow.SaveChangesAsync();

                //check if successfully updated event object else throw exception
                if (updatedRows == 0) { throw new Exception("Unable to remove Venue Id and update event status to pending."); }

                //return event data model
                return new EventModel().FromDataModel(eventModel);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Create: Method for adding artist for Event and updating event status to scheduled
        /// </summary>
        /// <param name="eventCode">EventCode for which artistId is added</param>
        /// <param name="artistId">ArtistId which is added for event</param>
        /// <returns>True if artist Id is successfully Added and event status is successfully updated</returns>
        public async Task<EventModel> AddArtistForEventAndUpdateEventStatus(string eventCode, Int32? artistId)
        {
            try
            {
                //check if event code or artist Id is null so throw exception
                if (eventCode == null || artistId == null) { throw new Exception("Invalid parameters eventCode or artistId."); }

                //get event object with event code
                NAT_ES_Event eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);

                //set artist Id of event to associate artist to Event
                eventModel.Artist_ID = artistId;

                //update event status to Scheduled i.e status Lookup Id 1
                eventModel.Event_Status_LKP_ID = 1;

                //create Artist event model to book artist for event
                ArtistEventModel artisteventmodel = new ArtistEventModel();
                artisteventmodel.ArtistId = Convert.ToInt32(artistId);
                artisteventmodel.Title = eventModel.Event_Name;
                artisteventmodel.Description = eventModel.Event_Description;
                artisteventmodel.EventTypeLKPId = eventModel.Event_Type_LKP_ID;
                artisteventmodel.StartTime = Convert.ToDateTime(eventModel.Event_Start_Time);
                artisteventmodel.EndTime = Convert.ToDateTime(eventModel.Event_End_Time);
                artisteventmodel.ReferenceId = eventCode;
                artisteventmodel.StatusLKPId = eventModel.Event_Status_LKP_ID;
                artisteventmodel.LocationCode = eventModel.Location_Code;
                artisteventmodel.UDF = null;
                ////

                //call book artist for event service
                var BookArtistdata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artisteventmodel);

                //check response for book artist event service if not successfull then throw exception
                if (BookArtistdata.status.IsSuccessStatusCode == false || BookArtistdata.data == false) { throw new Exception("Unable to Book artist for event."); }

                //update event object and save changes
                uow.Repository<NAT_ES_Event>().Update(eventModel);
                int updatedRows = await uow.SaveChangesAsync();

                //check if successfully updated event object else throw exception
                if (updatedRows == 0) { throw new Exception("Unable to Add artist Id and update event status to Scheduled."); }

                //return event data model
                return new EventModel().FromDataModel(eventModel);

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
        public async Task<EventModel> UpdateEventAsync(EventModel servicemodel , Auth.UserModel UserModel)
        {
            try
            {
                if (servicemodel.EventId > 0)
                {
                    var eventEfModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventForUpdateById(servicemodel.EventId);
                    var oldeventdetails = new EventModel().FromDataModel(eventEfModel);

                    var oldeventdate = oldeventdetails.EventDate;
                    var oldeventstarttime = oldeventdetails.EventStartTime;
                    var oldeventendtime = oldeventdetails.EventEndTime;

                    bool venueUpdated = servicemodel.VenueId != eventEfModel.Venue_ID;
                    bool artistUpdated = servicemodel.ArtistId != eventEfModel.Artist_ID;
                    bool timeUpdated = servicemodel.EventStartTime != eventEfModel.Event_Start_Time || servicemodel.EventEndTime != eventEfModel.Event_End_Time;
                    bool paintingUpdated = servicemodel.PaintingId != eventEfModel.Painting_ID;
                    var seatingPlans = servicemodel.NatEsEventSeatingPlan.ToList();
                    bool seatingPlanChanged = seatingPlans.Count > 0 && seatingPlans[0].SeatingPlanId == 0;

                    if (timeUpdated)
                    {
                        eventEfModel.Event_Date = servicemodel.EventStartTime;
                        eventEfModel.Event_Start_Time = servicemodel.EventStartTime;
                        eventEfModel.Event_End_Time = servicemodel.EventEndTime;
                    }

                    if (venueUpdated || timeUpdated)
                    {
                        //cancel old booking
                        var cancelVenueResp = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + servicemodel.EventCode);
                        if (!cancelVenueResp.status.IsSuccessStatusCode || !cancelVenueResp.data) throw new Exception("Error in cancelling venue booking: " + cancelVenueResp.status.message);

                        //create new booking
                        if (servicemodel.VenueId != null)
                        {
                            VenueEventModel venueeventmodel = new VenueEventModel();
                            venueeventmodel.VenueId = servicemodel.VenueId ?? default;
                            venueeventmodel.Title = servicemodel.EventName;
                            venueeventmodel.Description = servicemodel.EventDescription;
                            venueeventmodel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                            venueeventmodel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                            venueeventmodel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                            venueeventmodel.ReferenceId = servicemodel.EventCode;
                            venueeventmodel.StatusLKPId = servicemodel.EventStatusLKPId;
                            venueeventmodel.Forced = true;
                            venueeventmodel.UDF = null;
                            var bookVenueResp = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.VenueService, "BookVenueEvent", requestBody: venueeventmodel);
                            if (!bookVenueResp.status.IsSuccessStatusCode || !bookVenueResp.data) throw new Exception("Error in booking venue: " + bookVenueResp.status.message);
                        }

                        //update event
                        if (venueUpdated)
                        {
                            eventEfModel.Venue_ID = servicemodel.VenueId;
                            eventEfModel.Venue_Hall_ID = servicemodel.VenueHallId;
                            eventEfModel.Venue_City_Code = servicemodel.VenueCityCode;
                        }
                    }

                    if (artistUpdated || timeUpdated)
                    {
                        //cancel old booking
                        var cancelArtistResp = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + servicemodel.EventCode);
                        if (!cancelArtistResp.status.IsSuccessStatusCode) throw new Exception("Error in cancelling artist booking: " + cancelArtistResp.status.message);

                        //create new booking
                        if (servicemodel.ArtistId != null)
                        {
                            ArtistEventModel artistEventModel = new ArtistEventModel();
                            artistEventModel.ArtistId = servicemodel.ArtistId ?? default;
                            artistEventModel.Title = servicemodel.EventName;
                            artistEventModel.Description = servicemodel.EventDescription;
                            artistEventModel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                            artistEventModel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                            artistEventModel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                            artistEventModel.ReferenceId = servicemodel.EventCode;
                            artistEventModel.StatusLKPId = servicemodel.EventStatusLKPId;
                            artistEventModel.Forced = true;
                            artistEventModel.SlotTiming = servicemodel.EventTiming;
                            artistEventModel.LocationCode = servicemodel.LocationCode;
                            artistEventModel.UDF = null;

                            var bookArtistResp = await NatClient.ReadAsync<object>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artistEventModel);
                            if (!bookArtistResp.status.IsSuccessStatusCode) throw new Exception("Error in booking artist: " + bookArtistResp.status.message);
                        }

                        //update event
                        if (artistUpdated)
                        {
                            eventEfModel.Artist_ID = servicemodel.ArtistId;
                        }
                    }

                    if (!servicemodel.Virtual)
                    {
                        if (seatingPlanChanged)
                        {
                            var oldSeatingPlanEFModel = uow.RepositoryAsync<NAT_ES_Event_Seating_Plan>().GetEventSeatingPlanById(eventEfModel.Event_ID);
                            oldSeatingPlanEFModel.Active_Flag = false;
                            oldSeatingPlanEFModel.ObjectState = ObjectState.Modified;
                            uow.RepositoryAsync<NAT_ES_Event_Seating_Plan>().Update(oldSeatingPlanEFModel);
                            foreach(var oldseat in oldSeatingPlanEFModel.NAT_ES_Event_Seat)
                            {
                                oldseat.Active_Flag = false;
                                oldseat.ObjectState = ObjectState.Modified;
                                uow.RepositoryAsync<NAT_ES_Event_Seat>().Update(oldseat);
                            }

                            var newSeatingPlanEFModel = new EventSeatingPlanModel().ToDataModel(seatingPlans[0]);
                            newSeatingPlanEFModel.ObjectState = ObjectState.Added;
                            newSeatingPlanEFModel.Total_Seats = newSeatingPlanEFModel.NAT_ES_Event_Seat.Count;
                            foreach (var eventSeat in newSeatingPlanEFModel.NAT_ES_Event_Seat)
                            {
                                eventSeat.Active_Flag = true;
                                eventSeat.Seat_ID = 0;
                                eventSeat.ObjectState = ObjectState.Added;
                            }
                            uow.RepositoryAsync<NAT_ES_Event_Seating_Plan>().Insert(newSeatingPlanEFModel);
                        }
                        else
                        {
                            var seatingPlanEFModel = new EventSeatingPlanModel().ToDataModel(seatingPlans[0]);
                            seatingPlanEFModel.ObjectState = ObjectState.Modified;
                            foreach (var eventSeat in seatingPlanEFModel.NAT_ES_Event_Seat)
                            {
                                if (eventSeat.Seat_ID < 0)
                                {
                                    eventSeat.Seat_ID = -1 * eventSeat.Seat_ID;
                                    eventSeat.ObjectState = ObjectState.Deleted;
                                }
                                else if (eventSeat.Seat_ID == 0)
                                {
                                    eventSeat.Active_Flag = true;
                                    eventSeat.ObjectState = ObjectState.Added;
                                }
                                else
                                {
                                    eventSeat.Active_Flag = true;
                                    eventSeat.ObjectState = ObjectState.Modified;
                                }

                            }
                            uow.RepositoryAsync<NAT_ES_Event_Seating_Plan>().Update(seatingPlanEFModel);
                        }
                    }

                    if (servicemodel.NatEsEventTicketPrice != null)
                    {
                        var ticketPriceListEfModel = new EventTicketPriceModel().ToDataModelList(servicemodel.NatEsEventTicketPrice);
                        foreach (var eventTicketPrice in ticketPriceListEfModel)
                        {
                            if (eventTicketPrice.Event_Ticket_Price_ID == 0)
                            {
                                eventTicketPrice.Active_Flag = true;
                                eventTicketPrice.Event_ID = servicemodel.EventId;
                                eventTicketPrice.ObjectState = ObjectState.Added;
                                uow.RepositoryAsync<NAT_ES_Event_Ticket_Price>().Insert(eventTicketPrice);
                            }
                            else if (eventTicketPrice.Event_Ticket_Price_ID < 0)
                            {
                                eventTicketPrice.ObjectState = ObjectState.Deleted;
                                eventTicketPrice.Event_Ticket_Price_ID = -1 * eventTicketPrice.Event_Ticket_Price_ID;
                                uow.RepositoryAsync<NAT_ES_Event_Ticket_Price>().Delete(eventTicketPrice);
                            }
                            else
                            {
                                eventTicketPrice.ObjectState = ObjectState.Modified;
                                uow.RepositoryAsync<NAT_ES_Event_Ticket_Price>().Update(eventTicketPrice);
                            }
                        }
                    }
                    eventEfModel.Painting_ID = servicemodel.PaintingId;
                    eventEfModel.Event_Name = servicemodel.PaintingName;
                    eventEfModel.Event_Description = servicemodel.EventDescription;
                    eventEfModel.Event_Category_LKP_ID = servicemodel.EventCategoryLKPId;
                    eventEfModel.Event_Age_Group_Type_LKP_ID = servicemodel.EventAgeGroupTypeLKPId;
                    if (eventEfModel.Artist_ID != null && eventEfModel.Venue_ID != null && eventEfModel.Event_Status_LKP_ID == 2)
                    {
                        eventEfModel.Event_Status_LKP_ID = 1;
                    }
                    eventEfModel.Last_Updated_By = UserModel.UserName;
                    eventEfModel.Last_Updated_Date = System.DateTime.UtcNow;
                    eventEfModel.ObjectState = ObjectState.Modified;
                    uow.RepositoryAsync<NAT_ES_Event>().Update(eventEfModel);
                    int updatedRows = uow.SaveChanges();

                    var hash = "";

                    if (venueUpdated || artistUpdated)
                    {
                        TableEntity e = new TableEntity();
                        string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventEfModel.Event_Code);
                        TableStorage ts = new TableStorage();
                        IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                        var data = requests.ToList();
                        if (data == null || data.Count == 0)
                            throw new Exception("Table entry for the event not found");

                        hash = data[0].RowKey;
                        if (venueUpdated)
                        {
                            data[0].ResetTableEntityForVenueCancellation();
                            data[0].CurrentVenue = servicemodel.VenueId;
                            await ts.UpdateTableStorage("EventCancellationEntities", data[0]);
                        }
                        if (artistUpdated)
                        {
                            data[0].ResetTableEntityForArtistCancellation();
                            data[0].CurrentArtist = servicemodel.ArtistId;
                            await ts.UpdateTableStorage("EventCancellationEntities", data[0]);
                        }


                    }



                    if (venueUpdated || artistUpdated || timeUpdated || paintingUpdated)
                    {
                        MarketTimeZoneModel lastUpdatedDate = await MarketTimeZoneClient.GetMarketTimeAsync(System.DateTime.UtcNow, servicemodel.LocationCode, null);
                        // get the artist details of event
                        var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(servicemodel.ArtistId));
                        IDictionary<string, string> emailNameDict = new Dictionary<string, string>();

                        EventModel eventDetails = new EventModel();

                        if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                        {
                            eventDetails.ArtistName = servicemodel.ArtistName;
                            eventDetails.Name = artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName;
                            eventDetails.ArtistId = artistResponse.data.ArtistId;
                            eventDetails.EventCode = servicemodel.EventCode;
                            eventDetails.EventDateAsString = servicemodel.EventDate.Value.ToLongDateString();
                            eventDetails.EventName = servicemodel.PaintingName;
                            eventDetails.OldEventName = oldeventdetails.EventName;
                            eventDetails.EventStartTime = servicemodel.EventStartTime;
                            eventDetails.EventEndTime = servicemodel.EventEndTime;
                            eventDetails.EventDate = servicemodel.EventDate;
                            eventDetails.EventHash = hash;
                            eventDetails.OldEventDate = oldeventdate;
                            eventDetails.OldEventStartTime = oldeventstarttime;
                            eventDetails.OldEventEndTime = oldeventendtime;
                            eventDetails.PaintingName = servicemodel.PaintingName;



                            //convert event time and dates to market timezone
                            var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.EventDate ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);
                            var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.EventStartTime ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);
                            var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.EventEndTime ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);
                            var oldmarkettime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.OldEventDate ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);
                            var oldmarketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.OldEventStartTime ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);
                            var oldmarketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((eventDetails.OldEventEndTime ?? System.DateTime.UtcNow), oldeventdetails.LocationCode, null);

                            eventDetails.EventDate = markettime.MarketTime;
                            eventDetails.EventStartTime = marketStarttime.MarketTime;
                            eventDetails.EventEndTime = marketEndtime.MarketTime;
                            eventDetails.TimeZone = marketStarttime.TimeZone;
                            eventDetails.OldEventDate = oldmarkettime.MarketTime;
                            eventDetails.OldEventStartTime = oldmarketStarttime.MarketTime;
                            eventDetails.OldEventEndTime = oldmarketEndtime.MarketTime;

                            //get venue details

                            var venueResp = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + servicemodel.VenueId);
                            if (venueResp.status.IsSuccessStatusCode && venueResp.data != null)
                            {

                                var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                                if (lookupListAddress.status.IsSuccessStatusCode)
                                {
                                    var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);
                                    var dataitems = addresslov[servicemodel.VenueId];
                                    eventDetails.VenueAddress = dataitems.Address;

                                }

                                var venueData = venueResp.data;
                                var venueContactPerson = venueData.NatVsVenueContactPerson;
                                emailNameDict = new Dictionary<string, string>();
                                eventDetails.VenueName = venueData.VenueName;
                                eventDetails.VenueId = venueData.VenueId;

                                foreach (var contactPerson in venueContactPerson)
                                {
                                    var email = contactPerson.Email;
                                    var name = contactPerson.FirstName + " " + contactPerson.LastName;
                                    emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                                }
                                if (venueUpdated)
                                {
                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventCreationEmailForVenue", venueData.VenueId.ToString());
                                }
                                else if (timeUpdated)
                                {
                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventTimeUpdateTemplate", venueData.VenueId.ToString());
                                }

                            }
                            if (artistUpdated)
                            {
                                if (servicemodel.Virtual)
                                {
                                    await CreateZoomLink(servicemodel, artistResponse.data, marketStarttime, new TableStorage());
                                    await this.SendEventCreationNotificationToArtist(servicemodel, new TableStorage(), false, artistResponse.data);
                                }
                                else
                                {
                                    emailNameDict = new Dictionary<string, string>();

                                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventCreationEmail", artistResponse.data.ArtistId.ToString());
                                }
                            }
                            else
                            {
                                if (timeUpdated)
                                {
                                    emailNameDict = new Dictionary<string, string>();
                                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventTimeUpdateTemplate", artistResponse.data.ArtistId.ToString());
                                }
                                if (venueUpdated)
                                {
                                    emailNameDict = new Dictionary<string, string>();
                                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventVenueUpdateTemplate", artistResponse.data.ArtistId.ToString());
                                }
                                if (paintingUpdated)
                                {
                                    emailNameDict = new Dictionary<string, string>();
                                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));

                                    await SendEmailsAsync(emailNameDict, eventDetails, "EventPaintingUpdateTemplate", artistResponse.data.ArtistId.ToString());
                                }

                            }

                        }

                        if (servicemodel.ArtistId != null && oldeventdetails.ArtistId != null && servicemodel.VenueId != null && oldeventdetails.VenueId != null)
                        {
                            var ArtistlovData = await NatClient.ReadAsync<IEnumerable<ViewModels.ArtistLovViewModel>>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artistslov");
                            if (ArtistlovData.status.IsSuccessStatusCode)
                            {
                                var artistlov = ArtistlovData.data.ToDictionary(item => item.Id, item => item);

                                var newartist = artistlov[servicemodel.ArtistId.Value];
                                var oldartist = oldeventdetails.ArtistId.Value > 0 ? artistlov[oldeventdetails.ArtistId.Value] : null;


                                var paintinglovData = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingLovViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintinglov");
                                if (paintinglovData.status.IsSuccessStatusCode)
                                {
                                    var paintinglov = paintinglovData.data.ToDictionary(item => item.Id, item => item);


                                    var newpainting = paintinglov[servicemodel.PaintingId];
                                    var oldpainting = oldeventdetails.PaintingId > 0 ? paintinglov[oldeventdetails.PaintingId] : null;




                                    var VenuelovData = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                                    if (VenuelovData.status.IsSuccessStatusCode)
                                    {
                                        var Venuelov = VenuelovData.data.ToDictionary(item => item.VenueId, item => item);

                                        var newvenue = Venuelov[servicemodel.VenueId];
                                        var oldvenue = oldeventdetails.VenueId > 0 ? Venuelov[oldeventdetails.VenueId] : null;


                                        var ArtistDynamic = "";
                                        var VenueDynamic = "";
                                        var PaintingDynamic = "";
                                        var DateDynamic = "";

                                        if (artistUpdated)
                                        {
                                            ArtistDynamic = "highlight";
                                        }
                                        if (venueUpdated)
                                        {
                                            VenueDynamic = "highlight";
                                        }
                                        if (paintingUpdated)
                                        {
                                            PaintingDynamic = "highlight";
                                        }
                                        if (timeUpdated)
                                        {
                                            DateDynamic = "highlight";
                                        }

                                        // get the booking details of event
                                        var bookingsData = await NatClient.ReadAsync<List<BookingViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventEfModel.Event_Code);
                                        if (bookingsData.status.IsSuccessStatusCode && bookingsData.data != null)
                                        {
                                            var bookings = bookingsData.data;

                                            if (bookings != null && bookings.Count > 0)
                                            {
                                                emailNameDict = new Dictionary<string, string>();
                                                foreach (var booking in bookings)
                                                {
                                                    if (booking.Tickets != null)
                                                    {
                                                        foreach (var ticket in booking.Tickets)
                                                        {

                                                            var dynamicTemplateData = new EventReminderTemplate
                                                            {
                                                                RecipentName = ticket.CustomerName,
                                                                EventName = eventDetails.EventName,
                                                                OldEventName = eventDetails.OldEventName,
                                                                EventVenueName = newvenue.Name,
                                                                EventDate = (eventDetails.EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                                                EventImage = newpainting.Image,
                                                                ArtistName = newartist.FirstName + " " + newartist.LastName,
                                                                PaintingName = newpainting.Name,
                                                                EventAddress = newvenue.Address,
                                                                EventTime = (eventDetails.EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (eventDetails.EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                                                OldEventDate = (eventDetails.OldEventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                                                OldEventTime = (eventDetails.OldEventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (eventDetails.OldEventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                                                OldArtistName = oldartist != null ? oldartist.FirstName + " " + oldartist.LastName : "-",
                                                                OldPaintingName = oldpainting != null ? oldpainting.Name : "-",
                                                                OldEventImage = oldpainting != null ? oldpainting.Image : "-",
                                                                OldEventVenueName = oldvenue != null ? oldvenue.Name : "-",
                                                                OldEventAddress = oldvenue != null ? oldvenue.Address : "-",
                                                                ArtistDynamic = ArtistDynamic,
                                                                VenueDynamic = VenueDynamic,
                                                                PaintingDynamic = PaintingDynamic,
                                                                DateDynamic = DateDynamic,
                                                                Timezone = eventDetails.TimeZone,


                                                            };

                                                            //creating receiver object
                                                            Receiver receivers = new Receiver();
                                                            receivers.ReceiverID = ticket.CustomerEmail;
                                                            receivers.ReceiverName = ticket.CustomerName;
                                                            receivers.ValueObject = dynamicTemplateData;

                                                            //creating sender object
                                                            Sender sender = new Sender();
                                                            sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                                                            String subject = "Event Details Changed";




                                                            NotificationQueueMessage Notification = new NotificationQueueMessage("EventUpdateTemplate", NotificationType.Email, "EventUpdateTemplate", 1, DateTime.Now, receivers, sender, subject);
                                                            Notification.UserId = userId;
                                                            await new Notification().SendEmail(Notification);
                                                        }

                                                    }
                                                }
                                            }
                                        }


                                        var auth = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                                        if (auth.status.IsSuccessStatusCode && auth.data != null)
                                        {
                                            var userList = auth.data;
                                            if (userList != null)
                                            {
                                                foreach (var user in userList)
                                                {
                                                    var dynamicTemplateData = new EventReminderTemplate
                                                    {
                                                        RecipentName = user.FirstName + " " + user.LastName,
                                                        EventName = eventDetails.EventName,
                                                        OldEventName = eventDetails.OldEventName,
                                                        EventVenueName = newvenue.Name,
                                                        EventDate = (eventDetails.EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                                        EventImage = newpainting.Image,
                                                        ArtistName = newartist.FirstName + " " + newartist.LastName,
                                                        PaintingName = newpainting.Name,
                                                        EventAddress = newvenue.Address,
                                                        EventTime = (eventDetails.EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (eventDetails.EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                                        OldEventDate = (eventDetails.OldEventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                                        OldEventTime = (eventDetails.OldEventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (eventDetails.OldEventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                                        OldArtistName = oldartist != null ? oldartist.FirstName + " " + oldartist.LastName : "-",
                                                        OldPaintingName = oldpainting != null ? oldpainting.Name : "-",
                                                        OldEventImage = oldpainting != null ? oldpainting.Image : "-",
                                                        OldEventVenueName = oldvenue != null ? oldvenue.Name : "-",
                                                        OldEventAddress = oldvenue != null ? oldvenue.Address : "-",
                                                        ArtistDynamic = ArtistDynamic,
                                                        VenueDynamic = VenueDynamic,
                                                        PaintingDynamic = PaintingDynamic,
                                                        DateDynamic = DateDynamic,
                                                        Timezone = eventDetails.TimeZone,
                                                    };

                                                    //creating receiver object
                                                    Receiver receivers = new Receiver();
                                                    receivers.ReceiverID = user.Email;
                                                    receivers.ReceiverName = user.FirstName + " " + user.LastName;
                                                    receivers.ValueObject = dynamicTemplateData;

                                                    //creating sender object
                                                    Sender sender = new Sender();
                                                    sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                                                    String subject = "Event Details Changed";




                                                    NotificationQueueMessage Notification = new NotificationQueueMessage("EventUpdateTemplate", NotificationType.Email, "EventUpdateTemplate", 1, DateTime.Now, receivers, sender, subject);
                                                    Notification.UserId = userId;
                                                    await new Notification().SendEmail(Notification);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        return servicemodel.FromDataModel(eventEfModel);
                    }
                    else
                    {
                        return servicemodel.FromDataModel(eventEfModel); 
                    }
                }
                else
                {
                    throw new Exception("Invalid Event");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: make event Faetured or unFeatured
        /// </summary>
        /// <param name="EventId">id of Event</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public bool FeaturedEvent(EventModel servicemodel)
        {
            try
            {
                if (servicemodel.EventId != 0 || servicemodel.EventId > 0)
                {
                    NAT_ES_Event eventdata = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(servicemodel.EventId);
                    if (eventdata != null)
                    {
                        eventdata.Is_Featured = servicemodel.IsFeatured;
                        eventdata.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<NAT_ES_Event>().Update(eventdata);
                        int updatedRows = uow.SaveChanges();

                        if (updatedRows == 0)
                        {
                            return servicemodel.IsFeatured;
                        }
                        bool isfeatured = eventdata.Is_Featured ?? servicemodel.IsFeatured;
                        return isfeatured;
                    }
                    else { return servicemodel.IsFeatured; }
                }
                else
                {
                    return servicemodel.IsFeatured;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method activates Event 
        /// </summary>
        /// <param name="Id">Id of Event</param>
        public void  ActivateEvent(string Id)
        {
            try
            {
                NAT_ES_Event EventEf = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(Convert.ToInt32(Id));
                if (EventEf != null)
                {
                    uow.RepositoryAsync<NAT_ES_Event>().SetActiveFlag(true, EventEf);
                    uow.SaveChangesAsync();
                }
                else
                    throw new ApplicationException("Event doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        public async Task BulkMarkForPaintingChange(List<string> Ids)
        {
            try
            {
                var eventIds = Ids.Select(x => Convert.ToInt32(x)).ToList();

                var eventEf = uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .AsNoTracking()
                    .Include(x => x.NAT_ES_Event_Seating_Plan)
                    .Include("NAT_ES_Event_Seating_Plan.NAT_ES_Event_Seat")
                    .Where(x => eventIds.Contains(x.Event_ID))
                    .ToList();

                if (eventEf.Count > 0)
                {
                    var serviceModelList = new EventModel().FromDataModelList(eventEf);
                    foreach (var markedEvent in serviceModelList)
                    {
                        await this.ChangePaintingAuto(markedEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task MarkForPaintingChange(string Id)
        {
            try
            {
                var eventID = Convert.ToInt32(Id);
                NAT_ES_Event eventEf = uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .AsNoTracking()
                    .Include(x => x.NAT_ES_Event_Seating_Plan)
                    .Include("NAT_ES_Event_Seating_Plan.NAT_ES_Event_Seat")
                    .Where(x => x.Event_ID == eventID)
                    .FirstOrDefault();

                if (eventEf != null)
                {
                    var eventModel = new EventModel().FromDataModel(eventEf);
                    await this.ChangePaintingAuto(eventModel);
                }
                else
                    throw new ApplicationException("Event doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates Event 
        /// </summary>
        /// <param name="Id">Id of Event</param>
        public void DeactivateEvent(string Id)
        {
            try
            {
                NAT_ES_Event EventEf = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(Convert.ToInt32(Id));
                if (EventEf != null)
                {
                    uow.RepositoryAsync<NAT_ES_Event>().SetActiveFlag(false, EventEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Event doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method return Event Ticket Price Object based on event id and seat type
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="SeatType">Seat Type</param>
        public async Task<IEnumerable<EventTicketPriceModel>> GetEventTicketByEventId(int id)
        {
            try
            {
                IEnumerable<EventTicketPriceModel> data = null;
                IEnumerable<NAT_ES_Event_Ticket_Price> EventTicketPriceModel = await uow.RepositoryAsync<NAT_ES_Event_Ticket_Price>().GetEventTicketByEventId(id);
                if (EventTicketPriceModel != null)
                {
                    data = new EventTicketPriceModel().FromDataModelList(EventTicketPriceModel);
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
        /// This method return price of ticket depending on event code, row number, seat number 
        /// </summary>
        /// <param name="Id">Id of Event</param>
        public async Task<IEnumerable<EventSeatModel>> GetEventSeatPriceFunction(EventTicketModel eventTicketObj)
        {
            try
            {
                EventModel obj = await GetByCodeEvent(eventTicketObj.EventCode, null);

                if (obj != null)
                {
                    EventSeatingPlanModel seatingPlan = await GetEventSeatingPlanByIdAsync(obj.EventId);

                    if (seatingPlan != null)
                    {
                        //List<EventSeatModel> seatModel = seatingPlan.NatEsEventSeat.Where(x => eventTicketObj.RowNumber.Contains(x.RowNumber) && eventTicketObj.SeatNumber.Contains(x.SeatNumber)).ToList();
                        List<EventSeatModel> seatModel = seatingPlan.NatEsEventSeat.Where(x => eventTicketObj.Seats.Any(y => y.RowNumber == x.RowNumber && y.SeatNumber == x.SeatNumber)).ToList();
                        if (seatModel != null)
                        {
                            IEnumerable<EventTicketPriceModel> eventTicketPriceModels = await GetEventTicketByEventId(obj.EventId);
                            List<EventTicketPriceModel> eventTicketPriceList = eventTicketPriceModels.ToList();

                            Dictionary<String, EventTicketPriceModel> eventTicketPriceDictionary = eventTicketPriceList.ToDictionary(item => item.SeatTypeLKP, item => item);

                            seatModel = seatModel.Select((x) =>
                            {
                                Decimal Price = eventTicketPriceDictionary[x.SeatTypeLKPId.ToString()].Price.Value;
                                x.Price = Price;
                                return x;
                            }).ToList();

                            return seatModel;
                        }
                        else
                        {
                            throw new Exception("Seat with Such Row Number and Seat Number not Found!");
                        }
                    }
                    else
                    {
                        throw new Exception("Seating Plan with Such Event Id not Found!");
                    }
                }
                else
                {
                    throw new Exception("Event with Such Code not Found!");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method sends Email to all the artists, customers and venue 
        /// of all upcoming events that are occuring with in 24 hours 
        /// </summary>
        public async Task SendReminderEmailsForUpcomingEventsAsync()
        {

            //call configuration service to get time zone to be included in event start date and end date
            var configResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/EventReminderTime");

            //check artist service is successfull or not else throw exception
            if (configResponse.status.IsSuccessStatusCode != true || configResponse.data == null) throw new Exception("Get event reminder configuration service failed.");

            //get upcoming events with respect to time period fetched from configuration service
            IEnumerable<NAT_ES_Event> upcomingEvents = uow.RepositoryAsync<NAT_ES_Event>().GetUpcomingEvents(Int32.Parse(configResponse.data.Value));

            //if there are no upcoming events with in the time period then return
            if (upcomingEvents.ToList().Count == 0) return;

            //create filter to be passed in table query to get records
            string finalFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, upcomingEvents.ToList().ElementAt(0).Event_Code);

            //Append all the event codes with in the filter query 
            for (int i = 1; i < upcomingEvents.ToList().Count(); i++)
            {
                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, upcomingEvents.ToList().ElementAt(i).Event_Code);
                finalFilter = TableQuery.CombineFilters(finalFilter, TableOperators.Or, filter);
            }

            //retrieve records from table storage
            TableStorage ts = new TableStorage();
            IEnumerable<EventReminderTableEntityModel> requests = await ts.RetrieveTableStorage<EventReminderTableEntityModel>("EventReminderEmails", finalFilter);

            //check the event codes received from table storage and their status 
            //if status is true so remove from the upcoming events list
            requests.ToList().ForEach(eventReminders =>
            {
                if (eventReminders.Status == true) { upcomingEvents.ToList().Remove(upcomingEvents.ToList().Where(x => x.Event_Code == eventReminders.PartitionKey).FirstOrDefault()); }
            });

            //sending emails for each upcoming events
            upcomingEvents.ToList().ForEach(async eventObj =>
            {
                try
                {
                    //Call method to get event details by code
                    var eventDetailsResponse = await this.GetByCodeEvent(eventObj.Event_Code, null);

                    //check if event details are fetched successfully or not else throw exception
                    if (eventDetailsResponse == null) { throw new Exception("Error in getting event details by code"); }

                    //call get artist service to fetch artist details 
                    var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + eventObj.Artist_ID);

                    //check artist service is successfull or not else throw exception
                    if (artistResponse.status.IsSuccessStatusCode != true || artistResponse.data == null) throw new Exception("Get artist details by Id service failed.");

                    //call venue service to fetch venue details
                    var venueResponse = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + eventObj.Venue_ID);

                    //check artist service is successfull or not else throw exception
                    if (venueResponse.status.IsSuccessStatusCode != true || venueResponse.data == null) throw new Exception("Get venue details by Id service failed.");

                    //call booking service to fetch all bookings by event code
                    var bookingsResponse = await NatClient.ReadAsync<IEnumerable<BookingModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventObj.Event_Code);

                    //check booking service is successfull or not else throw exception
                    if (bookingsResponse.status.IsSuccessStatusCode != true || bookingsResponse.data == null) throw new Exception("Get bookings by event code service failed.");

                    //construct a dictionary for the name and email addresses
                    IDictionary<String, String> emailNameDict = new Dictionary<String, String>();

                    //call methods to get email addresses from booking into the email name dictionary
                    bookingsResponse.data.ToList().ForEach(bookingObj => { emailNameDict = GetEmailAddressesFromBooking(bookingObj, emailNameDict); });

                    //add the artist and venue name email addresses to the dictionary
                    emailNameDict = new Dictionary<string, string>();
                    emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, eventDetailsResponse.ArtistName));
                    await SendEmailsAsync(emailNameDict, eventDetailsResponse, "EventReminderEmail", artistResponse.data.ArtistId.ToString());
                    emailNameDict = new Dictionary<string, string>();
                    emailNameDict.Add(new KeyValuePair<String, String>(venueResponse.data.NatVsVenueContactPerson.FirstOrDefault().Email, eventDetailsResponse.VenueName));

                    //finally send the reminder email to each individual associated with the event
                    await SendEmailsAsync(emailNameDict, eventDetailsResponse, "EventReminderEmail", venueResponse.data.VenueId.ToString());

                    //construct a dictionary for the name and Contact Numbers
                    IDictionary<String, String> smsContactDict = new Dictionary<String, String>();

                    //call methods to get email addresses from booking into the email name dictionary
                    bookingsResponse.data.ToList().ForEach(bookingObj => { smsContactDict = GetContactFromBooking(bookingObj, smsContactDict); });

                    //add the artist and venue name contact number to the dictionary
                    smsContactDict.Add(new KeyValuePair<String, String>(artistResponse.data.ContactNumber, eventDetailsResponse.ArtistName));
                    await sendSMSAsync(smsContactDict, eventDetailsResponse, "EVENT_REMINDER_SMS", artistResponse.data.ArtistId.ToString());
                    smsContactDict = new Dictionary<String, String>();
                    smsContactDict.Add(new KeyValuePair<String, String>(venueResponse.data.NatVsVenueContactPerson.FirstOrDefault().ContactNumber, eventDetailsResponse.VenueName));
                    await sendSMSAsync(smsContactDict, eventDetailsResponse, "EVENT_REMINDER_SMS", venueResponse.data.VenueId.ToString());
                }

                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            });
        }

        //Helper method to fetch email addresses and name of customer inside the bookings
        public IDictionary<String, String> GetEmailAddressesFromBooking(BookingModel booking, IDictionary<String, String> emailNameDict)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            booking.Tickets.ForEach(ticket =>
            {
                if (ticket.CustomerEmail != null && ticket.CustomerEmail.Length != 0 && (emailNameDict.ContainsKey(ticket.CustomerEmail) == false))
                {
                    emailNameDict.Add(new KeyValuePair<String, String>(ticket.CustomerEmail, ticket.CustomerName));
                }
            });
            return emailNameDict;
        }

        //Helper method to fetch Contact Number and name of customer inside the bookings
        public IDictionary<String, String> GetContactFromBooking(BookingModel booking, IDictionary<String, String> contactNameDict)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            booking.Tickets.ForEach(ticket =>
            {
                if (contactNameDict.ContainsKey(ticket.CustomerContact) == false)
                {
                    contactNameDict.Add(new KeyValuePair<String, String>(ticket.CustomerContact, ticket.CustomerName));
                }
            });
            return contactNameDict;
        }


        //Helper methods to send reminder emails to event artist, venue and each customer
        public async Task sendSMSAsync(IDictionary<String, String> smsContactDict, EventModel eventDetails, string SMSType, string userId)
        {
            //loop through each entity in email name dictionary and send email
            foreach (var obj in smsContactDict)
            {
                ///////
                //Sending Ticket cancel SMS
                Receiver SMSreceiver = new Receiver();
                SMSreceiver.ReceiverID = obj.Key;
                SMSreceiver.ReceiverName = obj.Value;
                SMSreceiver.ValueObject = new ConfirmationSMSTemplate
                {
                    PhoneNumber = obj.Key,
                    Date = (eventDetails.EventStartTime ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                    VenueName = eventDetails.VenueName,
                    EventName = eventDetails.EventName

                };
                NotificationQueueMessage SMSQueueMessageObj = new NotificationQueueMessage();
                if (SMSType == "EVENT_CANCELLATION_SMS")
                {
                    SMSQueueMessageObj = new NotificationQueueMessage("EventCancellation", NotificationType.Sms, SMSType, 1, DateTime.Now, SMSreceiver);
                }
                else
                {
                    SMSQueueMessageObj = new NotificationQueueMessage(SMSType == "EVENT_REMINDER_SMS" ? "EventReminder" : "EventCreation", NotificationType.Sms, SMSType, 1, DateTime.Now, SMSreceiver);
                }

                SMSQueueMessageObj.UserId = userId;
                Notification notificationSMS = new Notification();
                _ = notificationSMS.SendSMS(SMSQueueMessageObj);

                ////////
                ///

                if (SMSType == "EVENT_REMINDER_SMS")
                {
                    //Guid Generator for each Object
                    Guid newGuid = Guid.NewGuid();

                    //EventReminderTableEntityModel created for entry
                    EventReminderTableEntityModel ob = new EventReminderTableEntityModel(eventDetails.EventCode, newGuid.ToString(), true);

                    //enter the model in table storage to keep record for event reminder emails
                    TableStorage ts = new TableStorage();
                    _ = await ts.InsertTableStorage("EventReminderEmails", ob);

                    ///////
                }
            }
        }

        //Helper methods to send reminder emails to event artist, venue and each customer
        public async Task SendEmailsAsync(IDictionary<String, String> emailNameDict, EventModel eventDetails, string emailType, String userId, MarketTimeZoneModel fetchedModel = null, bool BulkEmail = false)
        {

            //loop through each entity in email name dictionary and send email
            foreach (var obj in emailNameDict)
            {
                //////
                //Send Reminder Email
                //Cretaing Object for Sending Event Reminder Email
                var dynamicTemplateData = new EventReminderTemplate
                {
                    EventName = eventDetails.EventName,
                    OldEventName = eventDetails.OldEventName,
                    RecipentName = obj.Value,
                    EventVenueName = eventDetails.VenueName,
                    EventDate = (eventDetails.EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                    EventImage = eventDetails.PaintingImage,
                    EventPageLink = Environment.GetEnvironmentVariable("EventDetailsUrl") + eventDetails.EventId,
                    ArtistName = eventDetails.ArtistName,
                    ArtistRating = eventDetails.ArtistRating,
                    VenueRating = eventDetails.VenueRating,
                    PaintingRating = eventDetails.PaintingRating,
                    ArtistProfileLink = Environment.GetEnvironmentVariable("ArtistDetailsUrl") + eventDetails.ArtistId,
                    EventAddress = eventDetails.VenueAddress,
                    EventDateSecond = (eventDetails.EventDate ?? System.DateTime.UtcNow).ToString("dd MMM yyyy"),
                    VenueProfileLink = Environment.GetEnvironmentVariable("VenueDetailsUrl") + eventDetails.VenueId,
                    EventTime = (eventDetails.EventStartTime ?? System.DateTime.UtcNow).ToString("hh:mm tt") + " - " + (eventDetails.EventEndTime ?? System.DateTime.UtcNow).ToString("hh:mm tt") + " ",
                    MinPrice = Convert.ToDouble(eventDetails.MinTicketPrice),
                    MaxPrice = Convert.ToDouble(eventDetails.MaxTicketPrice),
                    RefundPolicy = "Refundable within 5 days of booking",
                    EventCancellationURLForVenue = Environment.GetEnvironmentVariable("EventCancellationURLForVenue") + eventDetails.EventHash + "/" + eventDetails.VenueId,
                    IsVenue = eventDetails.IsVenue,
                    VenueName = eventDetails.VenueName,
                    EventCancellationURLForArtist = Environment.GetEnvironmentVariable("EventCancellationURLForArtist") + eventDetails.EventHash + "/" + eventDetails.ArtistId,
                    EventStartTime = eventDetails.EventStartTime != null ? eventDetails.EventStartTime.Value.ToString("dd MMM yyyy") : null,
                    MeetingLink = eventDetails.MeetingLink,
                    PaintingName = eventDetails.PaintingName,
                    OldEventDate = (eventDetails.OldEventDate ?? System.DateTime.UtcNow).ToString("dd MMM yyyy"),
                    OldEventTime = (eventDetails.OldEventStartTime ?? System.DateTime.UtcNow).ToString("hh:mm tt") + " - " + (eventDetails.OldEventEndTime ?? System.DateTime.UtcNow).ToString("hh:mm tt") + " ",
                    Timezone = eventDetails.TimeZone ?? null

                };

                //creating receiver object
                Receiver receivers = new Receiver();
                receivers.ReceiverID = obj.Key;
                receivers.ReceiverName = obj.Value;
                receivers.ValueObject = dynamicTemplateData;

                //creating sender object
                Sender sender = new Sender();
                sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");


                var sortOrder = Convert.ToInt32((eventDetails.EventStartTime ?? default).Subtract(new DateTime(2020, 1, 1)).TotalSeconds);

                String subject = String.Empty;

                switch (emailType)
                {
                    // Add the subject here 
                    case "EventVenueCancellationTemplate":
                        subject = dynamicTemplateData.VenueName + " Released from " + dynamicTemplateData.EventName;
                        break;

                    case "EventCreationEmailForVenue":
                        subject = "New Event for " + dynamicTemplateData.EventVenueName;
                        break;

                    case "EventArtistUpdateTemplatewithVenue":
                        subject = dynamicTemplateData.EventName + " Artist Changed";
                        break;

                    case "EventArtistUpdateTemplatewithoutVenue":
                        subject = dynamicTemplateData.EventName + " Artist Changed";
                        break;

                    case "EventVenueUpdateEmailForVCP":
                        subject = dynamicTemplateData.EventName + " Venue Changed Successfully";
                        break;

                    case "EventCancellationTemplateForNonCustomers":
                        subject = dynamicTemplateData.EventName + " Cancelled - Team Paintception";
                        break;

                    case "EventVenueUpdateTemplate":
                        subject = dynamicTemplateData.EventName + " Venue Changed";
                        break;

                    case "EventCreationEmail":
                        subject = dynamicTemplateData.EventName + " - New Event Created";
                        break;
                    case "VirtualEventCreationEmail":
                        subject = dynamicTemplateData.EventName + " - New Event Created";
                        break;

                    case "EventArtistUpdateTemplateForArtistwithVenue":
                        subject = dynamicTemplateData.EventName + " Consent";
                        break;

                    case "EventArtistUpdateTemplateForArtistwithoutVenue":
                        subject = dynamicTemplateData.EventName + " Consent";
                        break;

                    case "EventReminderEmail":
                        subject = "Get Ready, you have an event coming up!";
                        break;

                    case "EventUpdateTemplate":
                        subject = "Event Details Changed";
                        break;

                    case "EventTimeUpdateTemplate":
                        subject = "Event Time Updated";
                        break;

                    case "EventPaintingUpdateTemplate":
                        subject = "Event Painting Updated";
                        break;


                    default:
                        subject = "New Email Template";
                        break;
                }

                NotificationQueueMessage Notification = new NotificationQueueMessage(emailType, NotificationType.Email, emailType, 1, DateTime.Now, receivers, sender, subject, BulkEmail, sortOrder);
                Notification.UserId = userId;
                await new Notification().SendEmail(Notification);
            }
        }

        public async Task MarkVenueUnavailable(string eventHash)
        {
            try
            {
                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, eventHash);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();
                if (data == null || data.Count == 0)
                {
                    throw new Exception("Event cancellation entity is null");
                }
                else
                {
                    await ProcessVenueUnavailable(data);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<EventModel> GetByHashEvent(string hash, long? customerId)
        {
            TableEntity e = new TableEntity();
            string filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, hash);

            TableStorage ts = new TableStorage();
            IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

            var data = requests.ToList();

            if (data == null || data.Count == 0)
            {
                return null;
            }
            else
            {
                var code = data[0].EventCode;
                return await GetByCodeEvent(code, customerId);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Event venue record
        /// </summary>
        /// <param name="eventCode">Event code</param>
        /// <param name="venueId">Venue Id of the new Venue</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateEventVenue(String eventCode, int venueId, String venueName)
        {
            try
            {
                NAT_ES_Event model = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);
                EventModel servicemodel = new EventModel().FromDataModel(model);

                VenueEventModel venueeventmodel = new VenueEventModel();
                venueeventmodel.VenueId = venueId;
                venueeventmodel.Title = servicemodel.EventName;
                venueeventmodel.Description = servicemodel.EventDescription;
                venueeventmodel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                venueeventmodel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                venueeventmodel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                venueeventmodel.ReferenceId = servicemodel.EventCode;
                venueeventmodel.StatusLKPId = servicemodel.EventStatusLKPId;
                venueeventmodel.Forced = true;
                venueeventmodel.UDF = null;

                var BookVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.POST, NatClient.Service.VenueService, "BookVenueEvent", requestBody: venueeventmodel);

                if (BookVenuedata.status.IsSuccessStatusCode)
                {
                    // Because the venue has been updated, update the Event accordingly
                    model.Venue_ID = venueId;
                    model.ObjectState = ObjectState.Modified;
                    uow.RepositoryAsync<NAT_ES_Event>().Update(model);
                    this.uow.SaveChanges();

                    TableEntity e = new TableEntity();
                    string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);
                    TableStorage ts = new TableStorage();
                    IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                    var data = requests.ToList();
                    if (data == null || data.Count == 0)
                        throw new Exception("Table entry for the event not found");

                    data[0].ResetTableEntityForVenueCancellation();
                    data[0].CurrentVenue = venueId;
                    await ts.UpdateTableStorage("EventCancellationEntities", data[0]);
                    var eventHash = data[0].RowKey;

                    var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(servicemodel.ArtistId));
                    IDictionary<string, string> emailNameDict = new Dictionary<string, string>();
                    EventModel m = new EventModel();
                    m.VenueName = venueName;


                    //Add time and timezone according to event market 
                    var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventDate ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventStartTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventEndTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);

                    Nullable<DateTime> EventDate = markettime.MarketTime;
                    Nullable<DateTime> EventStartTime = marketStarttime.MarketTime;
                    Nullable<DateTime> EventEndTime = marketEndtime.MarketTime;
                    var TimeZone = marketStarttime.TimeZone;

                    m.EventCode = servicemodel.EventCode;
                    m.EventDate = EventDate;
                    m.EventName = servicemodel.EventName;
                    m.EventStartTime = EventStartTime;
                    m.EventEndTime = EventEndTime;
                    m.TimeZone = TimeZone;



                    var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                    if (lookupListAddress.status.IsSuccessStatusCode)
                    {
                        var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);
                        var dataitems = addresslov[venueId];
                        m.VenueAddress = dataitems.Address;

                    }




                    var ArtistName = "Artist not assigned";
                    if (artistResponse.data != null)
                    {
                        m.Name = artistResponse.data.ArtistFirstName;
                        ArtistName = artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName;

                        if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                        {
                            emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, ArtistName));
                        }
                        await SendEmailsAsync(emailNameDict, m, "EventVenueUpdateTemplate", artistResponse.data.ArtistId.ToString());
                    }
                    emailNameDict = new Dictionary<string, string>();
                    var auth = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                    var userList = auth.data;
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            emailNameDict = new Dictionary<string, string>();
                            emailNameDict.Add(new KeyValuePair<String, String>(user.Email, user.FirstName + " " + user.LastName));
                            await SendEmailsAsync(emailNameDict, m, "EventVenueUpdateTemplate", user.UserId.ToString());
                        }

                    }

                    if (data != null && data.Count != 0)
                    {
                        m.EventHash = eventHash;

                        var venueResp = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + venueId);
                        if (venueResp.status.IsSuccessStatusCode && venueResp.data != null)
                        {
                            var venueData = venueResp.data;
                            var venueContactPerson = venueData.NatVsVenueContactPerson;
                            m.VenueId = venueResp.data.VenueId;
                            emailNameDict = new Dictionary<string, string>();

                            foreach (var contactPerson in venueContactPerson)
                            {
                                var email = contactPerson.Email;
                                var name = contactPerson.FirstName + " " + contactPerson.LastName;
                                emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                            }
                            await SendEmailsAsync(emailNameDict, m, "EventCreationEmailForVenue", venueResp.data.VenueId.ToString());
                        }
                    }

                    // Fetch all the bookings
                    var bookingsData = await NatClient.ReadAsync<List<BookingViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventCode);
                    if (bookingsData.status.IsSuccessStatusCode && bookingsData.data != null)
                    {
                        var bookings = bookingsData.data;

                        if (bookings != null && bookings.Count > 0)
                        {
                            emailNameDict = new Dictionary<string, string>();
                            foreach (var booking in bookings)
                            {
                                if (booking.Tickets != null)
                                {
                                    foreach (var ticket in booking.Tickets)
                                    {
                                        emailNameDict = new Dictionary<string, string>();
                                        var email = ticket.CustomerEmail;
                                        var name = ticket.CustomerName;
                                        emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                                        await SendEmailsAsync(emailNameDict, m, "EventVenueUpdateTemplate", email);
                                    }

                                    return true;
                                }
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw ServiceLayerExceptionHandler.Handle(e, logger);
            }
        }

        public async Task VenueUnavailable(string eventCode)
        {
            try
            {
                //Get event details
                //var eventEfModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);
                //if (eventEfModel == null) throw new Exception("Event not found");

                var eventCodeObj = JsonConvert.DeserializeObject<EventModel>(eventCode);
                eventCode = eventCodeObj.EventCode;
                var eventModel = await GetByCodeEvent(eventCode, null);

                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();
                var hash = data[0].RowKey;
                var venueId = data[0].CurrentVenue;

                //Check if event is active, not cancelled
                if (!eventModel.ActiveFlag || eventModel.EventStatusLKPId == 3) throw new Exception("Operation not valid on this event");

                //Call configuration service to get Ticket Threshold
                var ticketConfigResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/BookedTicketThresholdForCancellation");
                if (!ticketConfigResponse.status.IsSuccessStatusCode || ticketConfigResponse.data == null) throw new Exception("Unable to access configuration");
                var ticketThreshold = Convert.ToInt32(ticketConfigResponse.data.Value);

                //Call configuration service to get X Days
                var xDaysConfigResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/XDaysThresholdForCancellation");
                if (!xDaysConfigResponse.status.IsSuccessStatusCode || xDaysConfigResponse.data == null) throw new Exception("Unable to access configuration");
                var xDays = Convert.ToInt32(xDaysConfigResponse.data.Value);

                //Get total booked tickets of the event
                var bookingsData = await NatClient.ReadAsync<IEnumerable<BookingModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventCode);
                if (!bookingsData.status.IsSuccessStatusCode || bookingsData.data == null) throw new Exception("Get Event Bookings service failed.");

                var totalBookedTickets = 0;

                bookingsData.data.ToList().ForEach(booking =>
                {
                    totalBookedTickets = totalBookedTickets + booking.TicketCount;
                });

                var authResp = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                if (!authResp.status.IsSuccessStatusCode) throw new Exception("Unable to get admin info");
                var userList = authResp.data;

                //Add time and timezone according to event market 
                var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventDate ?? System.DateTime.UtcNow), eventModel.LocationCode, null);
                var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventStartTime ?? System.DateTime.UtcNow), eventModel.LocationCode, null);
                var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventEndTime ?? System.DateTime.UtcNow), eventModel.LocationCode, null);

                Nullable<DateTime> EventDate = markettime.MarketTime;
                Nullable<DateTime> EventStartTime = marketStarttime.MarketTime;
                Nullable<DateTime> EventEndTime = marketEndtime.MarketTime;
                var TimeZone = marketStarttime.TimeZone;

                //Check if event qualify for cancellation
                if (totalBookedTickets < ticketThreshold && DateTime.UtcNow.AddDays(xDays) < eventModel.StartDate)
                {
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            var emailData = new
                            {
                                recipent_name = user.FirstName + " " + user.LastName,
                                event_start_time = eventModel.EventStartTime.Value.ToLongDateString(),
                                ArtistName = eventModel.ArtistName,
                                CancelLink = Environment.GetEnvironmentVariable("EventCancellationURL") + eventCode,
                                EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                Timezone = eventModel.TimeZone,
                            };

                            //creating receiver object
                            Receiver receivers = new Receiver();
                            receivers.ReceiverID = user.Email;
                            receivers.ReceiverName = user.FirstName + " " + user.LastName;
                            receivers.ValueObject = emailData;

                            //creating sender object
                            Sender sender = new Sender();
                            sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                            var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "VenueConsentCancelEvent", 1, DateTime.UtcNow, receivers, sender, "Venue Unavailable - Team Paintception");
                            notification.UserId = user.UserId.ToString();

                            await new Notification().SendEmail(notification);
                        }
                    }
                    return;
                }

                //Find new venue
                var findVenueRequest = new FindReplacementVenueQuery()
                {
                    VenueId = venueId.Value,
                    EventStartTime = eventModel.EventStartTime ?? default,
                    EventEndTime = eventModel.EventEndTime ?? default,
                    EventCode = eventCode,
                    TotalTicketsSold = totalBookedTickets
                };
                var replacementVenueResp = await NatClient.ReadAsync<List<VenueModel>>(NatClient.Method.POST, NatClient.Service.VenueService, "FindReplacementVenue", "", findVenueRequest);
                if (!replacementVenueResp.status.IsSuccessStatusCode) throw new Exception("Error while fetching replacement venues");

                if (replacementVenueResp.data.Count == 0)
                {
                    //Notify admin that no venue found
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            var emailData = new
                            {
                                recipent_name = user.FirstName + " " + user.LastName,
                                event_start_time = eventModel.EventStartTime.Value.ToLongDateString(),
                                ArtistName = eventModel.ArtistName,
                                CancelLink = Environment.GetEnvironmentVariable("EventCancellationURL") + eventCode,
                                EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                Timezone = eventModel.TimeZone,
                            };

                            //creating receiver object
                            Receiver receivers = new Receiver();
                            receivers.ReceiverID = user.Email;
                            receivers.ReceiverName = user.FirstName + " " + user.LastName;
                            receivers.ValueObject = emailData;

                            //creating sender object
                            Sender sender = new Sender();
                            sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                            var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "VenueConsentCancelEvent", 1, DateTime.UtcNow, receivers, sender, "Venue Unavailable - Team Paintception");
                            notification.UserId = user.UserId.ToString();
                            await new Notification().SendEmail(notification);
                        }
                    }
                    return;
                }

                //Send Consent Email to VCPs
                foreach (var venue in replacementVenueResp.data)
                {
                    foreach (var vcp in venue.NatVsVenueContactPerson)
                    {
                        var emailData = new
                        {
                            RecipentName = vcp.FirstName,
                            ArtistName = eventModel.ArtistName,
                            AcceptLink = Environment.GetEnvironmentVariable("VCPConsentUrl") + hash + "/" + venue.VenueId + "/" + Uri.EscapeUriString(venue.VenueName) + "/" + eventCode,
                            EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                            EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                            Timezone = eventModel.TimeZone,
                        };


                        Receiver receivers = new Receiver();
                        receivers.ReceiverID = vcp.Email;
                        receivers.ReceiverName = vcp.FirstName;
                        receivers.ValueObject = emailData;
                        Sender sender = new Sender();
                        sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                        var subject = "Venue Consent";
                        var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "VCPConsentEmailTemplate", 1, DateTime.UtcNow, receivers, sender, subject);
                        notification.UserId = venue.VenueId.ToString();
                        await new Notification().SendEmail(notification);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task ProcessArtistUnavailable(List<EventUnavailabilityEntities> data)
        {
            string eventCode = data[0].EventCode;
            var cancelEventData = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.ArtistService, "CancelArtistEvent/" + eventCode);
            if (cancelEventData.status.IsSuccessStatusCode)
            {

                var eventModel = this.uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Event_Code == eventCode)
                    .FirstOrDefault();
                var artistId = eventModel.Artist_ID;

                eventModel.Artist_ID = null;
                eventModel.ObjectState = ObjectState.Modified;

                this.uow.RepositoryAsync<NAT_ES_Event>().Update(eventModel);
                this.uow.SaveChanges();
                if (artistId == null)
                    return;
                var artistResp = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + artistId);
                if (artistResp.status.IsSuccessStatusCode && artistResp.data != null)
                {
                    ArtistModel artistData = artistResp.data;

                    var m = new EventModel().FromDataModel(eventModel);
                    m.ArtistName = artistData.ArtistFirstName + " " + artistData.ArtistLastName;

                    IDictionary<string, string> emailNameDict = new Dictionary<string, string>();
                    emailNameDict.Add(new KeyValuePair<String, String>(artistData.ArtistEmail, artistData.ArtistFirstName + " " + artistData.ArtistLastName));

                    await SendEmailsAsync(emailNameDict, m, "EventArtistCancellationTemplate", artistData.ArtistId.ToString());
                }
                await QueueClient.AddToQueue("startartistunavailabilityflow", new { EventCode = eventCode });
            }
        }

        public async Task ProcessVenueUnavailable(List<EventUnavailabilityEntities> data)
        {
            {
                string eventCode = data[0].EventCode;

                var CancelVenuedata = await NatClient.ReadAsync<Boolean>(NatClient.Method.PUT, NatClient.Service.VenueService, "CancelVenueEvent/" + eventCode);
                if (CancelVenuedata.status.IsSuccessStatusCode)
                {
                    var eventModel = this.uow.RepositoryAsync<NAT_ES_Event>()
                        .Queryable()
                        .Where(x => x.Event_Code == eventCode)
                        .FirstOrDefault();
                    var venueId = eventModel.Venue_ID;

                    eventModel.Venue_ID = null;
                    eventModel.ObjectState = ObjectState.Modified;

                    this.uow.RepositoryAsync<NAT_ES_Event>().Update(eventModel);
                    this.uow.SaveChanges();

                    var venueResp = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + venueId);
                    if (venueResp.status.IsSuccessStatusCode && venueResp.data != null)
                    {
                        var venueData = venueResp.data;
                        var venueContactPerson = venueData.NatVsVenueContactPerson;
                        var m = new EventModel().FromDataModel(eventModel);
                        m.VenueName = venueData.VenueName;

                        IDictionary<string, string> emailNameDict = new Dictionary<string, string>();

                        foreach (var contactPerson in venueContactPerson)
                        {
                            var email = contactPerson.Email;
                            var name = contactPerson.FirstName + " " + contactPerson.LastName;
                            emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                        }
                        await SendEmailsAsync(emailNameDict, m, "EventVenueCancellationTemplate", venueData.VenueId.ToString());
                    }
                    await QueueClient.AddToQueue("startvenueunavailabilityflow", new { EventCode = eventCode });
                }
            }
        }

        public async Task MarkArtistUnavailable(string eventHash)
        {
            try
            {
                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, eventHash);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();

                if (data != null && data.Count > 0)
                {
                    await ProcessArtistUnavailable(data);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        public async Task ArtistUnavailable(string eventCode)
        {
            try
            {
                var eventCodeObj = JsonConvert.DeserializeObject<EventModel>(eventCode);
                eventCode = eventCodeObj.EventCode;

                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();
                var artistId = data[0].CurrentArtist;
                var hash = data[0].RowKey;
                var eventModel = await GetByCodeEvent(eventCode, null);

                //Check if event is active, not cancelled and venue is associated
                if (!eventModel.ActiveFlag || eventModel.VenueId == null || eventModel.EventStatusLKPId == 3) throw new Exception("Operation not valid on this event");

                //Call configuration service to get Ticket Threshold
                var ticketConfigResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/BookedTicketThresholdForArtistCancellation");
                if (!ticketConfigResponse.status.IsSuccessStatusCode || ticketConfigResponse.data == null) throw new Exception("Unable to access configuration");
                var ticketThreshold = Convert.ToInt32(ticketConfigResponse.data.Value);

                //Call configuration service to get X Days
                var xDaysConfigResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/XDaysThresholdForArtistCancellation");
                if (!xDaysConfigResponse.status.IsSuccessStatusCode || xDaysConfigResponse.data == null) throw new Exception("Unable to access configuration");
                var xDays = Convert.ToInt32(xDaysConfigResponse.data.Value);

                var authResp = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                if (!authResp.status.IsSuccessStatusCode) throw new Exception("Unable to get admin info");
                var userList = authResp.data;

                //Get total booked tickets of the event
                var bookingsData = await NatClient.ReadAsync<IEnumerable<BookingModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventCode);
                if (!bookingsData.status.IsSuccessStatusCode || bookingsData.data == null) throw new Exception("Get Event Bookings service failed.");

                var totalBookedTickets = 0;

                bookingsData.data.ToList().ForEach(booking =>
                {
                    totalBookedTickets = totalBookedTickets + booking.TicketCount;
                });

                var venueResp = await NatClient.ReadAsync<VenueModel>(NatClient.Method.GET, NatClient.Service.VenueService, "venues/" + eventModel.VenueId);
                String venueName = String.Empty;
                if (venueResp.status.IsSuccessStatusCode && venueResp.data != null)
                {
                    venueName = venueResp.data.VenueName;
                }

                //Add time and timezone according to event market 
                var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventDate ?? System.DateTime.UtcNow), eventModel.LocationCode, null);
                var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventStartTime ?? System.DateTime.UtcNow), eventModel.LocationCode, null);
                var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((eventModel.EventEndTime ?? System.DateTime.UtcNow), eventModel.LocationCode, null);

                Nullable<DateTime> EventDate = markettime.MarketTime;
                Nullable<DateTime> EventStartTime = marketStarttime.MarketTime;
                Nullable<DateTime> EventEndTime = marketEndtime.MarketTime;
                var TimeZone = marketStarttime.TimeZone;


                //Check if event qualify for cancellation
                if (totalBookedTickets < ticketThreshold && DateTime.UtcNow.AddDays(xDays) < eventModel.StartDate)
                {
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            var emailData = new
                            {
                                recipent_name = user.FirstName + " " + user.LastName,
                                event_start_time = eventModel.EventStartTime.Value.ToShortTimeString() + " (EST) " + eventModel.EventStartTime.Value.ToLongDateString(),
                                // event_start_time = eventModel.EventStartTime.Value.ToLongDateString(),
                                venue_or_artist = "Artist",
                                event_artist_name = eventModel.ArtistName,
                                VenueName = eventModel.VenueName,
                                CancelLink = Environment.GetEnvironmentVariable("EventCancellationURL") + eventCode,
                                EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                Timezone = eventModel.TimeZone,
                            };

                            //creating receiver object
                            Receiver receivers = new Receiver();
                            receivers.ReceiverID = user.Email;
                            receivers.ReceiverName = user.FirstName + " " + user.LastName;
                            receivers.ValueObject = emailData;

                            //creating sender object
                            Sender sender = new Sender();
                            sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");
                            var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "ArtistConsentCancelEvent", 1, DateTime.UtcNow, receivers, sender, String.Empty);
                            notification.UserId = user.UserId.ToString();
                            //Ask admin for cancellation
                            await new Notification().SendEmail(notification);
                        }
                    }
                    return;
                }
                var findArtistRequest = new FindReplacementArtistQuery()
                {
                    ArtistId = artistId ?? default,
                    EventStartTime = eventModel.EventStartTime ?? default,
                    EventEndTime = eventModel.EventEndTime ?? default,
                    EventCode = eventCode,
                    TotalTicketsSold = totalBookedTickets,
                    LocationCode = eventModel.LocationCode
                };

                var replacementArtistResp = await NatClient.ReadAsync<List<ArtistModel>>(NatClient.Method.POST, NatClient.Service.ArtistService, "FindReplacementArtist", "", findArtistRequest);
                if (!replacementArtistResp.status.IsSuccessStatusCode) throw new Exception("Error while fetching replacement venues");
                if (replacementArtistResp.data.Count == 0)
                {
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            var emailData = new
                            {
                                recipent_name = user.FirstName + " " + user.LastName,
                                event_start_time = eventModel.EventStartTime.Value.ToShortTimeString() + " (EST) " + eventModel.EventStartTime.Value.ToLongDateString(),
                                // event_start_time = eventModel.EventStartTime.Value.ToLongDateString(),
                                venue_or_artist = "Artist",
                                event_artist_name = eventModel.ArtistName,
                                VenueName = eventModel.VenueName,
                                CancelLink = Environment.GetEnvironmentVariable("EventCancellationURL") + eventCode,
                                EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                                EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                                Timezone = eventModel.TimeZone,
                            };

                            //creating receiver object
                            Receiver receivers = new Receiver();
                            receivers.ReceiverID = user.Email;
                            receivers.ReceiverName = user.FirstName + " " + user.LastName;
                            receivers.ValueObject = emailData;
                            //creating sender object
                            Sender sender = new Sender();
                            sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                            var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "ArtistConsentCancelEvent", 1, DateTime.UtcNow, receivers, sender, String.Empty);
                            notification.UserId = user.UserId.ToString();
                            await new Notification().SendEmail(notification);
                        }
                    }
                    return;
                }



                //Send Consent Email to VCPs
                foreach (var artist in replacementArtistResp.data)
                {
                    var emailData = new
                    {
                        RecipentName = artist.ArtistFirstName + " " + artist.ArtistLastName,
                        EventStartTime = eventModel.EventStartTime.Value.ToShortTimeString() + " (EST) " + eventModel.EventStartTime.Value.ToLongDateString(),
                        ArtistName = eventModel.ArtistName,
                        AcceptLink = Environment.GetEnvironmentVariable("ArtistConsentUrl") + hash + "/" + artist.ArtistId + "/" + Uri.EscapeUriString(venueName) + "/" + eventCode,
                        VenueName = venueName,
                        EventName = eventModel.EventName,
                        EventDate = (EventDate ?? System.DateTime.UtcNow).ToString("dd MMM, yyyy"),
                        EventTime = (EventStartTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " - " + (EventEndTime ?? System.DateTime.UtcNow).ToString("HH:mm tt") + " ",
                        Timezone = eventModel.TimeZone,
                    };

                    //creating receiver object
                    Receiver receivers = new Receiver();
                    receivers.ReceiverID = artist.ArtistEmail;
                    receivers.ReceiverName = artist.ArtistFirstName;
                    receivers.ValueObject = emailData;

                    //creating sender object
                    Sender sender = new Sender();
                    sender.SenderID = Environment.GetEnvironmentVariable("EmailSenderid");

                    var subject = "Your consent for " + emailData.EventName;
                    var notification = new NotificationQueueMessage("EventService", NotificationType.Email, "ArtistConsentEmailTemplate", 1, DateTime.UtcNow, receivers, sender, subject);
                    notification.UserId = artist.ArtistId.ToString();
                    await new Notification().SendEmail(notification);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Event venue record
        /// </summary>
        /// <param name="eventCode">Event code</param>
        /// <param name="venueId">Venue Id of the new Venue</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateEventArtist(String eventCode, int artistId, String artistName)
        {
            try
            {
                NAT_ES_Event model = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(eventCode);
                EventModel servicemodel = new EventModel().FromDataModel(model);

                ArtistEventModel artistEventModel = new ArtistEventModel();
                artistEventModel.ArtistId = artistId;
                artistEventModel.Title = servicemodel.EventName;
                artistEventModel.Description = servicemodel.EventDescription;
                artistEventModel.EventTypeLKPId = servicemodel.EventTypeLKPId;
                artistEventModel.StartTime = Convert.ToDateTime(servicemodel.EventStartTime);
                artistEventModel.EndTime = Convert.ToDateTime(servicemodel.EventEndTime);
                artistEventModel.ReferenceId = servicemodel.EventCode;
                artistEventModel.StatusLKPId = servicemodel.EventStatusLKPId;
                artistEventModel.Forced = true;
                artistEventModel.LocationCode = servicemodel.LocationCode;
                artistEventModel.UDF = null;

                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);
                var data = requests.ToList();

                if (data == null || data.Count == 0)
                    throw new ServiceLayerException("Event not found");

                var hash = data[0].RowKey;

                var bookArtistData = await NatClient.ReadAsync<object>(NatClient.Method.POST, NatClient.Service.ArtistService, "BookArtistEvent", requestBody: artistEventModel);
                if (bookArtistData.status.IsSuccessStatusCode)
                {
                    // Because the artist has been updated, its time to send the email
                    model.Artist_ID = artistId;
                    model.Event_Status_LKP_ID = Convert.ToInt32(Constants.EVENT_STATUS_SCHEDULED);
                    model.ObjectState = ObjectState.Modified;
                    uow.RepositoryAsync<NAT_ES_Event>().Update(model);
                    this.uow.SaveChanges();

                    // Because the event has been updated, update its entry in the table storage
                    data[0].ResetTableEntityForArtistCancellation();
                    data[0].CurrentArtist = artistId;
                    await ts.UpdateTableStorage("EventCancellationEntities", data[0]);

                    var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(artistId));
                    IDictionary<string, string> emailNameDict = new Dictionary<string, string>();

                    EventModel m = new EventModel();
                    //Add time and timezone according to event market 
                    var markettime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventDate ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketStarttime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventStartTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                    var marketEndtime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventEndTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);

                    Nullable<DateTime> EventDate = markettime.MarketTime;
                    Nullable<DateTime> EventStartTime = marketStarttime.MarketTime;
                    Nullable<DateTime> EventEndTime = marketEndtime.MarketTime;
                    var TimeZone = marketStarttime.TimeZone;

                    m.EventDate = EventDate;
                    m.EventStartTime = EventStartTime;
                    m.EventEndTime = EventEndTime;
                    m.TimeZone = TimeZone;
                    m.ArtistId = artistId;

                    if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                    {
                        m.ArtistName = artistName;
                        m.Name = artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName;
                        m.EventCode = servicemodel.EventCode;
                        m.EventName = servicemodel.EventName;
                        m.EventHash = hash;

                        emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.CompanyEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));
                        if (model.Venue_ID != null)
                        {

                            var lookupListAddress = await NatClient.ReadAsync<IEnumerable<ViewModels.VenueLovViewModel>>(NatClient.Method.GET, NatClient.Service.VenueService, "Venuelov");
                            if (lookupListAddress.status.IsSuccessStatusCode)
                            {
                                var addresslov = lookupListAddress.data.ToDictionary(item => item.VenueId, item => item);
                                var dataitems = addresslov[model.Venue_ID];
                                m.VenueAddress = dataitems.Address;
                                m.VenueName = dataitems.Name;
                            }
                            await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplateForArtistwithVenue", artistResponse.data.ArtistId.ToString());
                        }
                        else
                        {
                            await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplateForArtistwithoutVenue", artistResponse.data.ArtistId.ToString());
                        }

                    }


                    emailNameDict = new Dictionary<string, string>();
                    var auth = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                    var userList = auth.data;
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            emailNameDict = new Dictionary<string, string>();
                            emailNameDict.Add(new KeyValuePair<String, String>(user.Email, user.FirstName + " " + user.LastName));
                            if (model.Venue_ID != null)
                            {
                                await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplatewithVenue", user.UserId.ToString());
                            }
                            else
                            {
                                await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplatewithoutVenue", user.UserId.ToString());
                            }
                        }
                    }

                    var eventHash = data[0].RowKey;
                    m.EventHash = eventHash;

                    // Fetch all the bookings
                    var bookingsData = await NatClient.ReadAsync<List<BookingViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + eventCode);
                    if (bookingsData.status.IsSuccessStatusCode && bookingsData.data != null)
                    {
                        var bookings = bookingsData.data;

                        if (bookings != null && bookings.Count > 0)
                        {
                            emailNameDict = new Dictionary<string, string>();
                            foreach (var booking in bookings)
                            {
                                if (booking.Tickets != null)
                                {
                                    foreach (var ticket in booking.Tickets)
                                    {
                                        emailNameDict = new Dictionary<string, string>();
                                        var email = ticket.CustomerEmail;
                                        var name = ticket.CustomerName;
                                        emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                                        if (model.Venue_ID != null)
                                        {
                                            await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplatewithVenue", ticket.CustomerEmail);
                                        }
                                        else
                                        {
                                            await SendEmailsAsync(emailNameDict, m, "EventArtistUpdateTemplatewithoutVenue", ticket.CustomerEmail);
                                        }
                                    }

                                }
                            }
                        }
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

        public async Task UpdateEventPainting(int eventId, int paintingId, bool autoChange = false)
        {
            try
            {
                int oldPaintingId;
                string oldEventName;


                var paintinglovData = await NatClient.ReadAsync<IEnumerable<ViewModels.PaintingLovViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintinglov");
                if (!paintinglovData.status.IsSuccessStatusCode) throw new Exception("Painting cannot be updated");

                var paintinglov = paintinglovData.data.ToDictionary(item => item.Id, item => item);


                var newpainting = paintinglov[paintingId];


                var eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventById(eventId);
                EventModel servicemodel = new EventModel().FromDataModel(eventModel);
                oldPaintingId = eventModel.Painting_ID;
                oldEventName = eventModel.Event_Name;
                eventModel.Painting_ID = paintingId;
                eventModel.Event_Name = newpainting.Name;
                eventModel.Painting_Updated_Date = DateTime.UtcNow;
                eventModel.Auto_Painting = autoChange;
                eventModel.Mark_For_Painting_Change = false;
                eventModel.ObjectState = ObjectState.Modified;
                uow.RepositoryAsync<NAT_ES_Event>().Update(eventModel);
                await uow.SaveChangesAsync();
                //EventModel servicemodel = new EventModel().FromDataModel(eventModel);
                EventModel m = new EventModel();
                IDictionary<string, string> emailNameDict = new Dictionary<string, string>();
                if (eventModel.Artist_ID != null)
                {
                    var artistResponse = await NatClient.ReadAsyncWithHeaders<ArtistModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + Convert.ToInt32(eventModel.Artist_ID));

                    if (artistResponse.status.IsSuccessStatusCode == true && artistResponse.data != null)
                    {
                        var marketEventStartTime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventStartTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);
                        var marketEventEndTime = await MarketTimeZoneClient.GetMarketTimeAsync((servicemodel.EventEndTime ?? System.DateTime.UtcNow), servicemodel.LocationCode, null);

                        m.ArtistName = artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName;
                        m.Name = artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName;
                        m.EventCode = servicemodel.EventCode;
                        m.EventDateAsString = marketEventStartTime.MarketTime.ToLongDateString();
                        m.EventDate = marketEventStartTime.MarketTime;
                        m.EventName = newpainting.Name;
                        m.EventStartTime = marketEventStartTime.MarketTime;
                        m.EventEndTime = marketEventEndTime.MarketTime;
                        m.PaintingName = newpainting.Name;
                        m.PaintingImage = newpainting.Image;
                        m.OldEventName = oldEventName;
                        m.TimeZone = marketEventStartTime.TimeZone;
                        emailNameDict.Add(new KeyValuePair<String, String>(artistResponse.data.ArtistEmail, artistResponse.data.ArtistFirstName + " " + artistResponse.data.ArtistLastName));

                        await SendEmailsAsync(emailNameDict, m, "EventPaintingUpdateTemplate", artistResponse.data.ArtistId.ToString());

                    }
                }

                var auth = await NatClient.ReadAsync<List<AuthViewModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "GetAllUsersByReferenceType/" + "admin", null);
                if (auth.status.IsSuccessStatusCode && auth.data != null)
                {
                    var userList = auth.data;
                    if (userList != null)
                    {
                        foreach (var user in userList)
                        {
                            emailNameDict = new Dictionary<string, string>();
                            emailNameDict.Add(new KeyValuePair<String, String>(user.Email, user.FirstName + " " + user.LastName));
                            await SendEmailsAsync(emailNameDict, m, "EventPaintingUpdateTemplate", user.UserId.ToString());

                        }
                    }
                }

                // Fetch all the bookings
                var bookingsData = await NatClient.ReadAsync<List<BookingViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetBookingsByEventCode/" + servicemodel.EventCode);
                if (bookingsData.status.IsSuccessStatusCode && bookingsData.data != null)
                {
                    var bookings = bookingsData.data;

                    if (bookings != null && bookings.Count() > 0)
                    {
                        emailNameDict = new Dictionary<string, string>();
                        foreach (var booking in bookings)
                        {
                            if (booking.Tickets != null)
                            {
                                foreach (var ticket in booking.Tickets)
                                {
                                    emailNameDict = new Dictionary<string, string>();
                                    var email = ticket.CustomerEmail;
                                    var name = ticket.CustomerName;
                                    emailNameDict.Add(new KeyValuePair<String, String>(email, name));
                                    await SendEmailsAsync(emailNameDict, m, "EventPaintingUpdateTemplate", email);
                                }
                            }
                        }
                    }
                }




            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<List<EventModel>> GetEventsForOrderCreation()
        {
            try
            {
                var configurationResp = await NatClient.ReadAsync<ConfigurationModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/" + Common.Constants.Constants.DAYS_TO_CREATE_ORDER_BEFORE);
                if (!configurationResp.status.IsSuccessStatusCode || configurationResp.data == null)
                    return null;

                var threshold = int.Parse(configurationResp.data.Value);
                var configResp = await NatClient.ReadAsync<ConfigurationModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/" + Common.Constants.Constants.SHIP_ORDER_BEFORE_IN_DAYS);
                if (!configurationResp.status.IsSuccessStatusCode || configurationResp.data == null)
                    return null;

                var thresholdBooked = int.Parse(configResp.data.Value);
                List<EventModel> eventList = new List<EventModel>();
                var eventStatusLookupResp = await NatClient.ReadAsync<List<LookupModel>>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/" + Constants.EVENT_STATUS_LOOKUP);

                if (!eventStatusLookupResp.status.IsSuccessStatusCode || eventStatusLookupResp.data == null)
                {
                    throw new Exception("Event status lookup not found");
                }

                var eventScheduledStatus = eventStatusLookupResp.data
                    .Where(x => x.HiddenValue == Constants.EVENT_STATUS_SCHEDULED)
                    .Select(x => x.HiddenValue)
                    .FirstOrDefault();

                if (eventScheduledStatus == null)
                    throw new Exception("Event status for scheduled not found");

                var eventStatus = Convert.ToInt32(eventScheduledStatus);
                var startThreshold = DateTime.Now.AddDays(threshold);
                var endThreshold = DateTime.Now.AddDays(threshold + thresholdBooked);
                var list = this.uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Event_Status_LKP_ID == eventStatus && x.Event_Start_Time > startThreshold && x.Event_Start_Time <= endThreshold)
                    .AsNoTracking()
                    .ToList();
                foreach (var item in list)
                    eventList.Add(new EventModel().FromDataModel(item));
                return eventList;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<List<EventModel>> FetchCompletedEvents()
        {
            try
            {
                var now = DateTime.Now;

                var eventStatusLookupResp = await NatClient.ReadAsync<List<LookupModel>>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/" + Constants.EVENT_STATUS_TYPE);
                if (!eventStatusLookupResp.status.IsSuccessStatusCode || eventStatusLookupResp.data == null)
                    throw new Exception("Event status could not be found");

                var eventStatusLkpId = eventStatusLookupResp.data.Where(x => x.HiddenValue == Constants.EVENT_STATUS_SCHEDULED)
                    .Select(x => x.HiddenValue)
                    .FirstOrDefault();

                var eventStatus = Convert.ToInt32(eventStatusLkpId);

                if (eventStatusLkpId == null)
                    throw new Exception("Event status for scheduled not found");

                var eventFromDB = this.uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Event_Status_LKP_ID == eventStatus &&
                                x.Event_End_Time.HasValue &&
                                x.Event_End_Time.Value < now)
                    .OrderByDescending(x => x.Event_Start_Time)
                    .ToList();

                List<NAT_ES_Event> eventsFilter = new List<NAT_ES_Event>();

                if (eventFromDB != null)
                {
                    foreach (var e in eventFromDB)
                    {
                        var endTime = e.Event_End_Time.Value;
                        var totalDays = (now - endTime).TotalDays;

                        if (totalDays > 1) // Such events have already been catered
                            continue;
                        else
                            eventsFilter.Add(e);
                    }
                }

                var events = new EventModel().FromDataModelList(eventsFilter).ToList();
                return events;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<List<EventModel>> GetPastXDaysEvent(int days)
        {
            try
            {
                var now = DateTime.Now;
                var nowMinusXDays = now.AddDays(-days);

                var eventStatusLookupResp = await NatClient.ReadAsync<List<LookupModel>>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/" + Constants.EVENT_STATUS_TYPE);
                if (!eventStatusLookupResp.status.IsSuccessStatusCode || eventStatusLookupResp.data == null)
                    throw new Exception("Event status could not be found");

                var eventStatusLkpId = eventStatusLookupResp.data.Where(x => x.HiddenValue == Constants.EVENT_STATUS_SCHEDULED)
                    .Select(x => x.HiddenValue)
                    .FirstOrDefault();

                var eventStatus = Convert.ToInt32(eventStatusLkpId);

                if (eventStatusLkpId == null)
                    throw new Exception("Event status for scheduled not found");

                var eventFromDB = this.uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Event_Status_LKP_ID == eventStatus &&
                                x.Event_End_Time.HasValue &&
                                x.Event_End_Time.Value < now &&
                                x.Event_Start_Time.Value > nowMinusXDays
                                )
                    .OrderByDescending(x => x.Event_Start_Time)
                    .ToList();

                var events = new EventModel().FromDataModelList(eventFromDB).ToList();
                return events;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<object> SaveEventGalleryPictures(EventGalleryPicturesModel eventImages)
        {
            try
            {
                if (eventImages != null && eventImages.Images != null && eventImages.Images.Length > 0)
                {
                    var imageType = eventImages.ImageType;
                    var eventId = eventImages.EventId;

                    var lookupResp = await NatClient.ReadAsync<List<LookupModel>>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/" + Constants.EVENT_IMAGES_LOOKUP);

                    if (lookupResp == null || !lookupResp.status.IsSuccessStatusCode || lookupResp.data == null)
                        throw new Exception("Lookup not found");

                    var imageTypeLookup = lookupResp.data
                        .Where(x => x.HiddenValue == "1")
                        .Select(x => x.HiddenValue)
                        .FirstOrDefault();

                    eventImages.ImagesURL = new List<string>();

                    foreach (var image in eventImages.Images)
                    {
                        var fileName = Guid.NewGuid() + imageType;
                        byte[] byteArray = Convert.FromBase64String(image);
                        BlobStorage ts = new BlobStorage();
                        var imgname = await ts.InsertBlobStorage("EventImagesContainerName", byteArray, fileName);
                        string url = Environment.GetEnvironmentVariable("EventImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("EventImagesContainerName") + "/" + imgname;
                        eventImages.ImagesURL.Add(url);

                        NAT_ES_Event_Image eventImage = new NAT_ES_Event_Image();
                        eventImage.Active_Flag = true;
                        eventImage.Created_By = "";
                        eventImage.Created_Date = DateTime.Now;
                        eventImage.Event_ID = eventId;
                        eventImage.Image_Path = url;
                        eventImage.Last_Updated_By = "";
                        eventImage.Last_Updated_Date = DateTime.Now;
                        eventImage.Last_Updated_Date = DateTime.Now;
                        eventImage.ObjectState = ObjectState.Added;
                        eventImage.Image_Type_LKP = imageTypeLookup;

                        this.uow.Repository<NAT_ES_Event_Image>().Insert(eventImage);
                    }
                    this.uow.SaveChanges();
                    return eventImages;
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

        public async Task MarkArtistUnavailableByEventCode(String eventCode)
        {
            try
            {
                if (String.IsNullOrEmpty(eventCode))
                {
                    throw new Exception("Event code is null");
                }

                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();
                if (data == null || data.Count == 0)
                {
                    throw new Exception("Event cancellation entity is null");
                }
                else
                {
                    await ProcessArtistUnavailable(data);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkMarkArtistUnavailableByEventCode(List<String> eventCodes)
        {
            try
            {

                if (eventCodes != null && eventCodes.Count() > 0)
                {
                    foreach (String eventcode in eventCodes)
                    {
                        await MarkArtistUnavailableByEventCode(eventcode);
                    }
                }
                else
                {
                    throw new Exception("Event Code List is empty");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkMarkArtistUnavailableByArtistID(int artistID)
        {
            try
            {

                if (artistID>0)
                {
                    List<NAT_ES_Event> eventsList = this.uow.Repository<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Artist_ID == artistID && x.Event_Start_Time.HasValue && x.Event_Start_Time.Value > DateTime.Now)
                    .ToList();

                    List<string> eventCodes = new List<string>();
                        eventsList.ForEach(x => eventCodes.Add(x.Event_Code));

                    if (eventCodes.Count > 0)
                    {
                        foreach(string eventCode in eventCodes)
                            await MarkArtistUnavailableByEventCode(eventCode);
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task MarkVenueUnavailableByEventCodeFunction(String eventCode)
        {
            try
            {
                if (String.IsNullOrEmpty(eventCode))
                {
                    throw new Exception("Event code is null");
                }

                TableEntity e = new TableEntity();
                string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventCode);

                TableStorage ts = new TableStorage();
                IEnumerable<EventUnavailabilityEntities> requests = await ts.RetrieveTableStorage<EventUnavailabilityEntities>("EventCancellationEntities", filter);

                var data = requests.ToList();
                if (data == null || data.Count == 0)
                {
                    throw new Exception("Event cancellation entity is null");
                }
                else
                {
                    await ProcessVenueUnavailable(data);
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkMarkVenueUnavailableByEventCodeFunction(List<String> eventCodes)
        {
            try
            {

                if (eventCodes != null && eventCodes.Count() > 0)
                {
                    foreach (String eventcode in eventCodes)
                    {
                        await MarkVenueUnavailableByEventCodeFunction(eventcode);
                    }
                }
                else
                {
                    throw new Exception("Event Code List is empty");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task BulkMarkVenueUnavailableByVenueID(int venueID)
        {
            try
            {

                if (venueID > 0)
                {
                    List<NAT_ES_Event> eventsList = this.uow.Repository<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Venue_ID == venueID && x.Event_Start_Time.HasValue && x.Event_Start_Time.Value > DateTime.Now)
                    .ToList();

                    List<string> eventCodes = new List<string>();
                    eventsList.ForEach(x => eventCodes.Add(x.Event_Code));

                    if (eventCodes.Count > 0)
                    {
                        foreach (string eventCode in eventCodes)
                            await MarkVenueUnavailableByEventCodeFunction(eventCode);
                    }                 

                }
                else
                {
                    throw new Exception("Venue ID is not correct");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<EventModel>> GetUpcomingEventByArtistId(int artistId)
        {
            try
            {
                DataSourceRequest dataSourceRequest = new DataSourceRequest();
                //dataSourceRequest.Filters.Add()
                //await this.GetAllEventListAsync()
                var events = this.uow.Repository<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Artist_ID == artistId && x.Event_Start_Time.HasValue && x.Event_Start_Time.Value > DateTime.Now)
                    .ToList();

                var eventsModelList = new EventModel().FromDataModelList(events);

                // Find the list of ids of events
                var eventIds = eventsModelList.Select(x => x.EventId).ToList();
                if (eventIds != null && eventIds.Count > 0)
                {
                    var ordersResp = await NatClient.ReadAsync<List<OrderModel>>(NatClient.Method.POST, NatClient.Service.SuppliesService, "", null, eventIds);
                    if (ordersResp != null && ordersResp.status.IsSuccessStatusCode)
                    {
                        var orders = ordersResp.data;
                        foreach (var e in eventsModelList)
                        {
                            var eventId = e.EventId;
                            var order = orders
                                .Where(x => x.EventId == eventId)
                                .FirstOrDefault();
                            e.OrderModel = order;
                        }
                        return eventsModelList;
                    }
                    else
                    {
                        return eventsModelList;
                    }
                }
                else
                {
                    return eventsModelList;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<EventModel>> GetEventsOfArtistByArtistId(int artistId)
        {
            try
            {
                var events = this.uow.Repository<NAT_ES_Event>()
                    .Queryable().AsNoTracking()
                    .Where(x => x.Artist_ID == artistId && x.Event_Start_Time > DateTime.Now)
                    .ToList();

                var eventsModelList = new EventModel().FromDataModelList(events);
                // Find the list of ids of events
                var eventIds = eventsModelList.Select(x => x.EventId).ToList();
                if (eventIds != null && eventIds.Count > 0)
                {
                    var ordersResp = await NatClient.ReadAsync<List<OrderModel>>(NatClient.Method.POST, NatClient.Service.SuppliesService, "", null, eventIds);
                    if (ordersResp != null && ordersResp.status.IsSuccessStatusCode)
                    {
                        var orders = ordersResp.data;
                        foreach (var e in eventsModelList)
                        {
                            var eventId = e.EventId;
                            var order = orders
                                .Where(x => x.EventId == eventId)
                                .FirstOrDefault();
                            e.OrderModel = order;
                        }
                        return eventsModelList;
                    }
                    else
                    {
                        return eventsModelList;
                    }
                }
                else
                {
                    return eventsModelList;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public object GetEventPictures(string eventCode)
        {
            try
            {
                var eventObj = this.uow.Repository<NAT_ES_Event>()
                    .Queryable()
                    .Where(x => x.Event_Code == eventCode)
                    .Include(x => x.NAT_ES_Event_Image)
                    .FirstOrDefault();

                var eventImages = new EventModel().FromDataModel(eventObj);

                return eventImages.NatEsEventImage;

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<object> GetZoomMeetingSignatureAsync(string hash)
        {
            string apiKey = Environment.GetEnvironmentVariable("ZoomApiKey");
            string apiSecret = Environment.GetEnvironmentVariable("ZoomApiSecret");

            string filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, hash);

            TableStorage ts = new TableStorage();
            List<VitualEventLinkTableEntityModel> requests = (await ts.RetrieveTableStorage<VitualEventLinkTableEntityModel>("VirtualEventLink", filter)).ToList();

            if (requests != null && requests.Count > 0)
            {
                var request = requests[0];

                var eventModel = uow.RepositoryAsync<NAT_ES_Event>().GetEventByCode(request.PartitionKey);

                var status = "Ongoing";

                if (eventModel.Event_Start_Time > DateTime.UtcNow.AddMinutes(15))
                {
                    status = "Future";
                }
                else if (DateTime.UtcNow.AddMinutes(-15) > eventModel.Event_End_Time)
                {
                    status = "Past";
                }

                String timeStamp = (ZoomMeeting.ToTimestamp(DateTime.UtcNow.ToUniversalTime()) - 30000).ToString();

                if (status == "Ongoing")
                {
                    if (request.Role == "0")
                    {
                        var checkInResp = await NatClient.ReadAsync<object>(NatClient.Method.PUT, NatClient.Service.TicketBookingService, "checkInCustomerFromVirtualEvent", null, new
                        {
                            TicketCode = request.TicketNumber
                        });

                        if (!checkInResp.status.IsSuccessStatusCode)
                        {
                            throw new Exception("Unable to checkin customer");
                        }
                    }
                    return new
                    {
                        Signature = ZoomMeeting.GenerateToken(apiKey, apiSecret, request.MeetingNumber, timeStamp, request.Role),
                        Password = request.Password,
                        UserName = request.Name,
                        UserEmail = request.Email,
                        MeetingNumber = request.MeetingNumber,
                        IsArtist = request.Role == "1"
                    };
                }
                else
                {
                    return new
                    {
                        EventStartTime = eventModel.Event_Start_Time,
                        EventName = eventModel.Event_Name,
                        Status = status
                    };
                }
            }
            else
            {
                throw new Exception("Invalid link");
            }
        }

        public async Task HandleZoomEvent(ZoomEvent zoomEvent)
        {
            try
            {
                TableStorage ts = new TableStorage();
                await ts.InsertTableStorage("ZoomEvents", zoomEvent.ToTableEntityModel());
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task UpdatePaintingOfMarkedEvents()
        {
            var markedEvents = uow.RepositoryAsync<NAT_ES_Event>()
                .Queryable()
                .AsNoTracking()
                .Include(x => x.NAT_ES_Event_Seating_Plan)
                .Include("NAT_ES_Event_Seating_Plan.NAT_ES_Event_Seat")
                .Where(x => x.Mark_For_Painting_Change).ToList();

            if(markedEvents.Count > 0)
            {
                var serviceModelList = new EventModel().FromDataModelList(markedEvents);
                foreach(var markedEvent in serviceModelList)
                {
                    await this.ChangePaintingAuto(markedEvent);
                }
            }
        }

        public async Task ChangePaintingAuto(EventModel eventModel)
        {
            try
            {
                //Get Configuration
                var configDaysAfterEventCreatedResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingXDaysAfterEventCreated");
                if (configDaysAfterEventCreatedResponse.status.IsSuccessStatusCode != true || configDaysAfterEventCreatedResponse.data == null) throw new Exception("Configuration service failed. " + configDaysAfterEventCreatedResponse.status.message);
                var AutoPaintingXDaysAfterEventCreated = Convert.ToInt32(configDaysAfterEventCreatedResponse.data.Value);

                var configPercentageForSoldTicketsResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingXDaysAfterEventCreated");
                if (configPercentageForSoldTicketsResponse.status.IsSuccessStatusCode != true || configPercentageForSoldTicketsResponse.data == null) throw new Exception("Configuration service failed. " + configPercentageForSoldTicketsResponse.status.message);
                var AutoPaintingPercentageForSoldTickets = Convert.ToInt32(configPercentageForSoldTicketsResponse.data.Value);

                var configIterationsToCheckPaintingInResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingXIterationsToCheckPaintingIn");
                if (configIterationsToCheckPaintingInResponse.status.IsSuccessStatusCode != true || configIterationsToCheckPaintingInResponse.data == null) throw new Exception("Configuration service failed. " + configIterationsToCheckPaintingInResponse.status.message);
                var AutoPaintingXIterationsToCheckPaintingIn = Convert.ToInt32(configIterationsToCheckPaintingInResponse.data.Value);

                var configPaintingUsedInSameVenueInLastXDaysResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingPaintingUsedInSameVenueInLastXDays");
                if (configPaintingUsedInSameVenueInLastXDaysResponse.status.IsSuccessStatusCode != true || configPaintingUsedInSameVenueInLastXDaysResponse.data == null) throw new Exception("Configuration service failed. " + configPaintingUsedInSameVenueInLastXDaysResponse.status.message);
                var AutoPaintingPaintingUsedInSameVenueInLastXDays = Convert.ToInt32(configPaintingUsedInSameVenueInLastXDaysResponse.data.Value);

                var configDaysPastAfterPaintingWasLastUsedResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/AutoPaintingXDaysPastAfterPaintingWasLastUsed");
                if (configDaysPastAfterPaintingWasLastUsedResponse.status.IsSuccessStatusCode != true || configDaysPastAfterPaintingWasLastUsedResponse.data == null) throw new Exception("Configuration service failed. " + configDaysPastAfterPaintingWasLastUsedResponse.status.message);
                var AutoPaintingXDaysPastAfterPaintingWasLastUsed = Convert.ToInt32(configDaysPastAfterPaintingWasLastUsedResponse.data.Value);

                var response = "";
                //Check if Event is eligible for painting change
                if(DateTime.Now.AddDays(-1 * AutoPaintingXDaysAfterEventCreated) <= eventModel.CreatedDate)
                {
                    response = "Event is not old enought to be eligible for auto painting change";
                    return;
                }

                var ticketResponse = await NatClient.ReadAsync<List<TicketModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetTicketsByEventCode/" + eventModel.EventCode);
                if (!ticketResponse.status.IsSuccessStatusCode && ticketResponse.data == null) throw new Exception("Unable to get event tickets");

                var ticketsSold = ticketResponse.data;

                var lastPaintingChangeDate = eventModel.PaintingUpdatedDate != null ? eventModel.PaintingUpdatedDate : eventModel.CreatedDate;

                var ticketsSoldSinceLastPaintingChange = ticketsSold.Where(x => x.CreatedDate > lastPaintingChangeDate).ToList();

                var totalSeats = eventModel.NatEsEventSeatingPlan.ToList()[0].NatEsEventSeat.Count;

                var soldTicketPercentage = ticketsSoldSinceLastPaintingChange.Count / totalSeats * 100;

                if(soldTicketPercentage > AutoPaintingPercentageForSoldTickets)
                {
                    response = "Ticket selling fine with this painting";
                }

                var paintingListResponse = await NatClient.ReadAsync<List<PaintingViewModel>>(NatClient.Method.GET, NatClient.Service.PaintingService, "Paintings/sorted/ticketsSold");
                if (!paintingListResponse.status.IsSuccessStatusCode && paintingListResponse.data == null) throw new Exception("Unable to get paintings");

                var paintingList = paintingListResponse.data;

                var paintingIdwithEventCount = uow.RepositoryAsync<NAT_ES_Event>()
                    .Queryable()
                    .GroupBy(x => x.Painting_ID).Select(x => new { PaintingId = x.Key, Eventsheld = x.Count() })
                    .ToDictionary(x => x.PaintingId, x => x.Eventsheld);
                
                var newPaintings = paintingList.Where(x => !paintingIdwithEventCount.ContainsKey(x.PaintingId)).ToList();

                if(newPaintings.Count > 0)
                {
                    await this.UpdateEventPainting(eventModel.EventId, newPaintings[0].PaintingId, true);
                    return;
                }

                foreach(var painting in paintingList)
                {
                    var iterationList = uow.RepositoryAsync<NAT_ES_Event_Iteration>()
                        .Queryable()
                        .Where(x=>x.Event_ID == eventModel.EventId).OrderBy(x=>x.Created_Date)
                        .ToList();

                    var iterationCheck = false;
                    for(var i = 0; i < Math.Min(iterationList.Count, AutoPaintingXIterationsToCheckPaintingIn); i++)
                    {
                        var iterationValueObj = JsonConvert.DeserializeObject<List<NAT_ES_Event>>(iterationList[i].Value);
                        if(iterationValueObj[0].Painting_ID == painting.PaintingId)
                        {
                            iterationCheck = true;
                        }
                    }
                    if (iterationCheck) continue;

                    var checkVenueDate = (eventModel.EventStartTime ?? default).AddDays(-1 * AutoPaintingPaintingUsedInSameVenueInLastXDays);
                    var venueCheck = uow.RepositoryAsync<NAT_ES_Event>()
                        .Queryable()
                        .Any(x => x.Venue_ID == eventModel.VenueId && x.Painting_ID == painting.PaintingId && x.Event_Start_Time > checkVenueDate && x.Event_End_Time < eventModel.EventStartTime);

                    if (venueCheck) continue;

                    var checkPaintingDate = (eventModel.EventStartTime ?? default).AddDays(-1 * AutoPaintingXDaysPastAfterPaintingWasLastUsed);
                    var paintingMarketCheck = uow.RepositoryAsync<NAT_ES_Event>()
                        .Queryable()
                        .Any(x => x.Painting_ID == painting.PaintingId && x.Location_Code == eventModel.LocationCode && x.Event_Start_Time > checkPaintingDate && x.Event_End_Time < eventModel.EventStartTime);

                    if (paintingMarketCheck) continue;

                    await this.UpdateEventPainting(eventModel.EventId, painting.PaintingId, true);

                    //var eventEFModel = uow.RepositoryAsync<NAT_ES_Event>().Queryable().Where(x => x.Event_ID == eventModel.EventId).FirstOrDefault();
                    //eventEFModel.Painting_ID = painting.PaintingId;
                    //uow.RepositoryAsync<NAT_ES_Event>().Update(eventEFModel);
                    //await uow.SaveChangesAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task CancelEventsWithIssues()
        {
            try
            {
                var configXHoursBeforeEventAutoCancelResponse = await NatClient.ReadAsync<ConfigurationViewModel>(NatClient.Method.GET, NatClient.Service.LookupService, "configuration/XHoursBeforeEventAutoCancel");
                if (configXHoursBeforeEventAutoCancelResponse.status.IsSuccessStatusCode != true || configXHoursBeforeEventAutoCancelResponse.data == null) throw new Exception("Configuration service failed. " + configXHoursBeforeEventAutoCancelResponse.status.message);
                var XHoursBeforeEventAutoCancel = Convert.ToInt32(configXHoursBeforeEventAutoCancelResponse.data.Value);
                var fromDate = DateTime.UtcNow;
                var toDate = DateTime.UtcNow.AddHours(XHoursBeforeEventAutoCancel);

                var eventForCancellation = await uow.RepositoryAsync<NAT_ES_Event>().Queryable().AsNoTracking().Where(x => x.Event_Start_Time > fromDate && x.Event_Start_Time < toDate && (x.Artist_ID == null || x.Venue_ID == null) && x.Event_Status_LKP_ID != 3).ToListAsync();

                foreach (var eventEFModel in eventForCancellation)
                {
                    await this.CancelEvent(eventEFModel.Event_Code);
                }
            } 
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public DataSourceResult GetAllBookedTickets(DataSourceRequest request)
        {
            try
            {
                var result = this.uow.RepositoryAsync<NAT_BOOKED_TICKET_VW>().Queryable().Where(x => x.Status_LKP == "Confirmed").ToDataSourceResult<NAT_BOOKED_TICKET_VW, BookedTicketModel>(request);
                return result;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}