using System.Collections.Generic;
using System.Linq;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.EventApp.Models.EFModel;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nat.EventApp.Models.Repositories
{
    public static class EventRepository
    {


        #region Repository Methods for Event  

        /// <summary>
        /// This method return list of all Events
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetAllEvent(this IRepositoryAsync<NAT_ES_Event> repository)
        {
            return repository.Queryable().ToList();
        }


        /// <summary>
        /// This method returns Event with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EventID">Id of Event</param>
        /// <returns>Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetAllEventsFilter(this IRepositoryAsync<NAT_ES_Event> repository, string EventName, DateTime? StartDate, DateTime? EndDate, string VenueCityCode, ICollection<int> ArtistId, int? paintingId, ICollection<int?> Category, decimal? MinPrice, decimal? MaxPrice,bool isAsc, string sort_column)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                return (x.Event_Start_Time >= StartDate || StartDate == null) 
                        && (x.Event_Start_Time <= EndDate || EndDate == null) 
                        && (x.Venue_City_Code == VenueCityCode || x.Location_Code == VenueCityCode || VenueCityCode == "") 
                        && (ArtistId.Contains(Convert.ToInt32(x.Artist_ID)) || ArtistId.Count == 0)
                        && (x.Painting_ID == paintingId || paintingId == null || paintingId == 0)
                        && (x.Min_Ticket_Price >= MinPrice || MinPrice == null) 
                        && (x.Max_Ticket_Price <= MaxPrice || MaxPrice == null)
                        && (Category.Contains(x.Event_Category_LKP_ID) || Category.Count == 0)
                        && (x.Event_End_Time >= System.DateTime.UtcNow)
                        && (x.Event_Status_LKP_ID != 3);
            };
            var qry = repository.Queryable()
                .Include(x => x.NAT_ES_Event_Ticket_Price)
                .Include(x => x.NAT_ES_Event_Seating_Plan)
                .Include("NAT_ES_Event_Seating_Plan.NAT_ES_Event_Seat")
                .Where(whereCondition).ToList();
            return qry.OrderBy("Event_Start_Time", true).OrderBy(sort_column, isAsc).ToList();
        }

        public static IEnumerable<NAT_ES_Event> GetAllFeaturedEvents(this IRepositoryAsync<NAT_ES_Event> repository)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                return (x.Is_Featured == true && x.Event_Start_Time >= System.DateTime.UtcNow);
            };
            var qry = repository.Queryable()
                        .Where(whereCondition).ToList();
            return qry.ToList();
        }

        public static IEnumerable<string> GetSuggestedEvents(this IRepositoryAsync<NAT_ES_Event> repository)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                return (x.Event_Date >= System.DateTime.Now && x.Event_Status_LKP_ID==1);
            };
            
            var query = repository.Queryable().Where(whereCondition).OrderBy("Event_Date", false).Select(x => x.Event_Code).ToList();
            return query.Take(3);
        }

        public static IEnumerable<NAT_ES_Event> GetUpcomingEvents(this IRepositoryAsync<NAT_ES_Event> repository, Int32 time)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                return (x.Event_Status_LKP_ID == 1 && ((x.Event_Date ?? System.DateTime.UtcNow).Date - System.DateTime.UtcNow.Date).Days == time) &&
                      ((x.Event_Date ?? System.DateTime.UtcNow).TimeOfDay.Hours == System.DateTime.UtcNow.TimeOfDay.Hours);
            };

            return repository.Queryable().Where(whereCondition).OrderBy("Event_Date", false).ToList();
        }

        private static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> input, string queryString,bool isAsc)
        {
            if (string.IsNullOrEmpty(queryString))
                return input;

            int i = 0;
            foreach (string propname in queryString.Split(','))
            {
                var subContent = propname.Split('|');
                if (isAsc == true)
                {
                    if (i == 0)
                        input = input.OrderBy(x => GetPropertyValue(x, subContent[0].Trim()));

                }
                else
                {
                    if (i == 0)
                        input = input.OrderByDescending(x => GetPropertyValue(x, subContent[0].Trim()));
                }
                i++;
            }

            return input;
        }

        public static NAT_ES_Event GetEventDetail(this IRepositoryAsync<NAT_ES_Event> repository, int? EventId)
        {
            return repository.Queryable()
                .Include(x => x.NAT_ES_Event_Facility)
                .Include(x => x.NAT_ES_Event_Ticket_Price)
                .Include(x => x.NAT_ES_Event_Seating_Plan)
                .Where(x => (x.Event_ID == EventId && EventId != null))
                .AsNoTracking()
                .FirstOrDefault();
        }



        public static async Task<IEnumerable<NAT_ES_Event>> GetEventLov(this IRepository<NAT_ES_Event> repository)
        {
            var lov = await repository.Queryable().ToListAsync();
            return lov;
        }

        /// <summary>
        /// This method returns Event with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EventID">Id of Event</param>
        /// <returns>Event EF model</returns>
        public static NAT_ES_Event GetEventById(this IRepositoryAsync<NAT_ES_Event> repository, int eventID)
        {
            return repository.Queryable().Include(x => x.NAT_ES_Event_Ticket_Price)
                        .Where(x => x.Event_ID == eventID).AsNoTracking().FirstOrDefault();
        }

        public static NAT_ES_Event GetEventForUpdateById(this IRepositoryAsync<NAT_ES_Event> repository, int eventID)
        {
            return repository.Queryable()
                        .Where(x => x.Event_ID == eventID).FirstOrDefault();
        }

        /// <summary>
        /// This method returns Event with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EventID">Id of Event</param>
        /// <returns>Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetTodayEventsWithArtistIdAndVenueId(this IRepositoryAsync<NAT_ES_Event> repository, int? artistId, int? venueId)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                if (artistId != null && venueId != null)
                {
                    return (((x.Event_Start_Time ?? System.DateTime.UtcNow).Date == System.DateTime.UtcNow.Date) && x.Artist_ID == artistId && x.Venue_ID == venueId);
                } 
                else if (artistId != null && venueId == null) 
                {
                    return (((x.Event_Start_Time ?? System.DateTime.UtcNow).Date == System.DateTime.UtcNow.Date) && x.Artist_ID == artistId);
                }
                else if (artistId == null && venueId != null) 
                {
                    return (((x.Event_Start_Time ?? System.DateTime.UtcNow).Date == System.DateTime.UtcNow.Date) && x.Venue_ID == venueId);
                }
                else 
                {
                    return ((x.Event_Start_Time ?? System.DateTime.UtcNow).Date == System.DateTime.UtcNow.Date);
                }
                
            };

            var qry = repository.Queryable()
                        .Include(x => x.NAT_ES_Event_Ticket_Price)
                        .Where(whereCondition).ToList();
            return qry.ToList();
        }

        /// <summary>
        /// This method returns upcoming Events with a given artist Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ArtistID">Id of Artist</param>
        /// <returns>Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetUpcomingEventsWithArtistId(this IRepositoryAsync<NAT_ES_Event> repository, int artistId)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {
                return (((x.Event_Start_Time ?? System.DateTime.UtcNow) > System.DateTime.UtcNow) && x.Artist_ID == artistId && x.Event_Status_LKP_ID != 3);
            };

            var qry = repository.Queryable()
                        .Where(whereCondition).ToList().OrderBy("Event_Start_Time", false);
            return qry.ToList();
        }

        /// <summary>
        /// This method returns all Events with a given artist Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ArtistID">Id of Artist</param>
        /// <returns>Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetAllEventsWithArtistId(this IRepositoryAsync<NAT_ES_Event> repository, int artistId)
        {
            var qry = repository.Queryable().AsNoTracking()
                        .Where(x=>x.Artist_ID == artistId).ToList().OrderBy("Event_Start_Time", false);
            return qry;
        }


        /// <summary>
        /// This method returns Events Happening Now with a given artist Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ArtistID">Id of Artist</param>
        /// <returns>Event EF model</returns>
        public static IEnumerable<NAT_ES_Event> GetEventsHappeningNowByArtistId(this IRepositoryAsync<NAT_ES_Event> repository, int artistId, Double time)
        {
            Func<NAT_ES_Event, bool> whereCondition = (x) => {

                return (((x.Event_Start_Time <= System.DateTime.UtcNow && System.DateTime.UtcNow <= x.Event_End_Time)
                        || (x.Event_Start_Time > System.DateTime.UtcNow && x.Event_Start_Time < System.DateTime.UtcNow.AddHours(time)))
                        && (x.Artist_ID == artistId)
                        && (x.Event_Status_LKP_ID != 3));
            };

            var qry = repository.Queryable()
                        .Where(whereCondition).ToList();
            return qry.OrderBy(x => x.Event_Start_Time).ToList();
        }

        /// <summary>
        /// This method returns Event with a given Code
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EventCode">Code of Event</param>
        /// <returns>Event EF model</returns>
        public static NAT_ES_Event GetEventByCode(this IRepositoryAsync<NAT_ES_Event> repository, string eventCode)
        {
            return repository.Queryable()
                        .Where(x => x.Event_Code == eventCode).AsNoTracking().Include(x => x.NAT_ES_Event_Facility).Include(x => x.NAT_ES_Event_Ticket_Price).Include(x => x.NAT_ES_Event_Seating_Plan).FirstOrDefault();
        }

        /// <summary>
        /// This method returns Event SeatingPlan with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="SeatingPlanID">Id of Event</param>
        /// <returns>Event EF model</returns>
        public static NAT_ES_Event_Seating_Plan GetEventSeatingPlanById(this IRepositoryAsync<NAT_ES_Event_Seating_Plan> repository, int eventID)
        {
            return repository.Queryable()
                        .Include(x => x.NAT_ES_Event_Seat)
                        .Where(x => x.Event_ID == eventID && x.Active_Flag).FirstOrDefault();
        }

        public static async System.Threading.Tasks.Task<NAT_ES_Event_Seat> GetEventSeatingPlanByEventCodeAsync(this IRepositoryAsync<NAT_ES_Event_Seat> repository, string eventCode, string row, string seat )
        {
            return await repository.Queryable()
                        .Include(x => x.NAT_ES_Event_Seating_Plan)
                        .Include("NAT_ES_Event_Seating_Plan.NAT_ES_Event")
                        .Where(x => x.NAT_ES_Event_Seating_Plan.NAT_ES_Event.Event_Code == eventCode && x.NAT_ES_Event_Seating_Plan.Active_Flag && x.Row_Number == row && x.Seat_Number == seat)
                        .FirstOrDefaultAsync();
        }


        /// <summary>
        /// This method activate/deactivate Event against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate Event</param>
        /// <param name="EventEf">Event EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_ES_Event> repository, bool flag, NAT_ES_Event EventEf)
        {
            EventEf.Active_Flag = flag;
            repository.Update(EventEf, x => x.Active_Flag);
        }

        /// <summary>
        /// This method return Event Ticket Price Object based on event id and seat type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="eventId">Event Id</param>
        /// <param name="SeatType">Seat Type</param>
        public static async System.Threading.Tasks.Task<IEnumerable<NAT_ES_Event_Ticket_Price>> GetEventTicketByEventId(this IRepositoryAsync<NAT_ES_Event_Ticket_Price> repository, int eventid)
        {
            return await repository.Queryable().Where(x => x.Event_ID == eventid).ToListAsync();
        }
        #endregion







    }
}
