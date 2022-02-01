using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.VenueApp.Models.EFModel;
using System.Threading.Tasks;
using System;

namespace Nat.VenueApp.Models.Repositories
{
    public static class VenueRepository
    {


        #region Repository Methods for Venue  

        /// <summary>
        /// This method return list of all Venues
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Venue EF model</returns>
        public static IEnumerable<NAT_VS_Venue> GetAllVenue(this IRepositoryAsync<NAT_VS_Venue> repository)
        {
            return repository.Queryable().ToList();
        }


        /// <summary>
        /// This method returns Venue with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <returns>Venue EF model</returns>
        public static NAT_VS_Venue GetVenueById(this IRepositoryAsync<NAT_VS_Venue> repository, int VenueID)
        {
            return repository.Queryable()
                        .Include(x => x.NAT_VS_Venue_Address)
                        .Include(x => x.NAT_VS_Venue_Contact_Person)
                        .Include(x => x.NAT_VS_Venue_Facility)
                        .Include(x => x.NAT_VS_Venue_Bank_Account)
                        .Include(x => x.NAT_VS_Venue_Bank_Account.Select(y => y.NAT_VS_Venue_Address))
                        .Include(x => x.NAT_VS_Venue_Hall)
                        .Include(x => x.NAT_VS_Venue_Image)
                        .Include(x => x.NAT_VS_Venue_Rating)
                        .Include(x => x.NAT_VS_Venue_Artist_Preference)
                        .Include(x => x.NAT_VS_Venue_Metro_City_Mapping)
                        .Include(x => x.NAT_VS_Venue_Rating_Log)
                        .Include("NAT_VS_Venue_Hall.NAT_VS_Venue_Seating_Plan")
                        .Include("NAT_VS_Venue_Hall.NAT_VS_Venue_Seating_Plan.NAT_VS_Venue_Seat")
                        .Where(x => x.Venue_ID == VenueID).AsEnumerable().Select(x => {
                            x.NAT_VS_Venue_Hall = x.NAT_VS_Venue_Hall.AsEnumerable().Select(y => {
                                y.NAT_VS_Venue_Seating_Plan = y.NAT_VS_Venue_Seating_Plan.AsEnumerable().Where(z=> z.Active_Flag == true).ToList();
                                return y;
                            }).ToList();
                            return x;
                        }).FirstOrDefault();
        }

        /// <summary>
        /// This method return list of event held count, LastEventDate and Upcomming Events count against all artists
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of artist EF model</returns>
        public static async Task<IEnumerable<Object>> GetAllEventCalculatedDetails(this IRepositoryAsync<NAT_VS_Venue_Event> repository)
        {
            var venueeventdata = repository.Queryable().GroupBy(i => i.Venue_ID)
           .Select(g => new
           {

               VenueId = g.Max(x => x.Venue_ID),
               Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0,
               LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time),
               UpcommingEvents = (int?)g.Count(x => x.Start_Time > System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0
               //.Where(x => x.Start_Time <= System.DateTime.Now && x.Status_LKP_ID == 1)
           });

            return venueeventdata;
        }

        public static async Task<IEnumerable<Object>> GetAllEventCalculatedDetailsByVenueId(this IRepositoryAsync<NAT_VS_Venue_Event> repository, int venueID)
        {
            var venueeventdata = repository.Queryable().Where(x => x.Venue_ID == venueID).GroupBy(i => i.Venue_ID)
           .Select(g => new
           {

               VenueId = g.Max(x => x.Venue_ID),
               Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0,
               LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time),
               UpcommingEvents = (int?)g.Count(x => x.Start_Time > System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0
               //.Where(x => x.Start_Time <= System.DateTime.Now && x.Status_LKP_ID == 1)
           });

            return venueeventdata;
        }

        //get Venue by planner id
        public static async Task<NAT_VS_Venue> GetVenueByPlannerIdAsync(this IRepositoryAsync<NAT_VS_Venue> repository, int plannerid)
        {
            return await repository.Queryable().Where(x => x.Planner_ID == plannerid).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns Seating Plan with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <returns>Venue EF model</returns>
        public static async Task<List<NAT_VS_Venue_Seating_Plan>> GetSeatingPlans(this IRepositoryAsync<NAT_VS_Venue_Seating_Plan> repository, int venueHallId)
        {
            return await repository.Queryable()
                        .Include(x => x.NAT_VS_Venue_Seat)
                        .Where(x => x.Venue_Hall_ID == venueHallId).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NAT_VS_Venue>> GetVenueLov(this IRepository<NAT_VS_Venue> repository)
        {
            var lov = await repository.Queryable().Include(x => x.NAT_VS_Venue_Address).Include(y => y.NAT_VS_Venue_Rating).Include(y => y.NAT_VS_Venue_Metro_City_Mapping).Include(z => z.NAT_VS_Venue_Contact_Person).AsNoTracking().ToListAsync();
            return lov;
        }
        public static async Task<IEnumerable<NAT_VS_Venue>> GetVenueLovForVCP(this IRepository<NAT_VS_Venue> repository,long? venueId)
        {
            var lov = await repository.Queryable().Where(x=>x.Venue_ID == venueId).Include(x => x.NAT_VS_Venue_Address).Include(x => x.NAT_VS_Venue_Metro_City_Mapping).Include(y => y.NAT_VS_Venue_Rating).Include(z => z.NAT_VS_Venue_Contact_Person).AsNoTracking().ToListAsync();
            return lov;
        }



        public static async Task<IEnumerable<NAT_VS_Venue>> SearchVenueForEvent(this IRepositoryAsync<NAT_VS_Venue> repository, List<int> plannerIds, Nullable<float> min_rating, Nullable<float> max_rating, Nullable<int> skill, String searchText,List<string> location)
        {

            Func<NAT_VS_Venue, bool> whereCondition = (i) =>
            {
            return !i.Virtual && i.Active_Flag == true && ((min_rating != null ? (i.NAT_VS_Venue_Rating.Count != 0 ? i.NAT_VS_Venue_Rating.FirstOrDefault().Average_Rating_Value >= min_rating : false) : true) && (max_rating != null ? (i.NAT_VS_Venue_Rating.Count != 0 ? i.NAT_VS_Venue_Rating.FirstOrDefault().Average_Rating_Value <= max_rating : false) : true) &&
            (searchText != null ? (i.Venue_Name != null ? (i.Venue_Name.ToLower().Contains(searchText.ToLower())) : false) : true) &&
            (plannerIds.Count != 0 ? plannerIds.Contains(i.Planner_ID ?? default(int)) : false) &&
            (plannerIds.Count != 0 ? plannerIds.Contains(i.Planner_ID ?? default(int)) : false) &&
            ((location.Count != 0 ? location.Contains(i.Location_Code) : true))); 
            };

            return repository.Queryable()
                .Include(x => x.NAT_VS_Venue_Image)
                .Include(x => x.NAT_VS_Venue_Address)
                .Include(x => x.NAT_VS_Venue_Facility)
                .Include(x => x.NAT_VS_Venue_Rating)
                .Include(x => x.NAT_VS_Venue_Hall)
                .AsNoTracking()
                .Where(whereCondition).ToList();

        }

        public static async Task<NAT_VS_Venue> GetVirtualVenue(this IRepositoryAsync<NAT_VS_Venue> repository)
        {
            return repository.Queryable()
                .Include(x => x.NAT_VS_Venue_Image)
                .Include(x => x.NAT_VS_Venue_Address)
                .Include(x => x.NAT_VS_Venue_Facility)
                .Include(x => x.NAT_VS_Venue_Rating)
                .Include(x => x.NAT_VS_Venue_Hall)
                .AsNoTracking()
                .Where(x => x.Virtual).FirstOrDefault();
        }

        /// <summary>
        /// This method returns venue list with location code
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="locationCode">Id of Venue</param>
        /// <returns>Collection of Venue EF model</returns>
        public static async Task<IEnumerable<NAT_VS_Venue>> GetVenueByLocationCode(this IRepositoryAsync<NAT_VS_Venue> repository, string locationCode)
        {

            Func<NAT_VS_Venue, bool> whereCondition = (i) =>
            {
                return (i.Location_Code == locationCode) && i.Active_Flag == true;
            };

            return repository.Queryable()
                .Include(x => x.NAT_VS_Venue_Image)
                .Include(x => x.NAT_VS_Venue_Address)
                .Include(x => x.NAT_VS_Venue_Facility)
                .Include(x => x.NAT_VS_Venue_Rating)
                .Include(x => x.NAT_VS_Venue_Hall)
                .AsNoTracking()
                .Where(whereCondition).ToList();
        }

        /// <summary>
        /// This method returns venue list by location list
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="locationCode">Id of Venue</param>
        /// <returns>Collection of Venue EF model</returns>
        public static async Task<IEnumerable<NAT_VS_Venue>> GetVenueByLocations(this IRepositoryAsync<NAT_VS_Venue> repository, List<string> locations)
        {

            Func<NAT_VS_Venue, bool> whereCondition = (i) =>
            {
                return (locations.Count != 0 ? locations.Contains(i.Location_Code) : true);
            };

            return repository.Queryable()
                .Include(x => x.NAT_VS_Venue_Image)
                .Include(x => x.NAT_VS_Venue_Address)
                .Include(x => x.NAT_VS_Venue_Facility)
                .Include(x => x.NAT_VS_Venue_Rating)
                .Include(x => x.NAT_VS_Venue_Hall)
                .Where(whereCondition).ToList();
        }


        /// <summary>
        /// This method checks venue facility exists against given venue Id and facility lookup id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <param name="FacilityLKPID">Id of Facility LKP</param>
        /// <returns>Collection of Venue Facility EF model</returns>
        public static bool GetVenueFacilityById(this IRepositoryAsync<NAT_VS_Venue> repository, int VenueID, string FacilityLKPID)
        {
            NAT_VS_Venue VenueFacilityModel = repository.Queryable().Include(x => x.NAT_VS_Venue_Facility.Select(i => i.Facility_LKP_ID  == FacilityLKPID && i.Venue_ID == VenueID))
                         .Where(x => x.Venue_ID == VenueID).FirstOrDefault();
            if (VenueFacilityModel.NAT_VS_Venue_Facility != null) { return true; } else { return false; }
        }


      

        /// <summary>
        /// This method activate/deactivate Venue against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate Venue</param>
        /// <param name="VenueEf">Venue EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_VS_Venue> repository, bool flag, NAT_VS_Venue VenueEf)
        {
            VenueEf.Active_Flag = flag;
            repository.Update(VenueEf);
      
        }


        /// <summary>
        /// This method returns venue event with a given eventcode
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Reference ID</param>
        /// <returns>Artist Event EF model</returns>
        public static async Task<NAT_VS_Venue_Event> GetVenueEventByEventCodeAsync(this IRepositoryAsync<NAT_VS_Venue_Event> repository, string eventcode)
        {
            return await repository.Queryable().Include(x => x.NAT_VS_Venue)
                        .Where(x => x.Reference_ID == eventcode).FirstOrDefaultAsync();
        }


        public static async Task<IEnumerable<NAT_VS_Venue_Rating_Log>> GetVenueRatingLogByVenueId(this IRepositoryAsync<NAT_VS_Venue_Rating_Log> repository, int venueId, int requiredRecords)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_ID == venueId).OrderByDescending(x => x.Created_Date).Take(requiredRecords).ToList();
        }

        public static async Task<IEnumerable<NAT_VS_Venue>> FindVenueWithInRadius(this IRepositoryAsync<NAT_VS_Venue> repository, int venueId, int radius)
        {
            var location = await repository.Queryable().Where(x => x.Venue_ID == venueId).Include(x => x.NAT_VS_Venue_Address).Select(x => new { x.NAT_VS_Venue_Address.Coordinates, x.Location_Code } ).FirstOrDefaultAsync();
            return await repository.Queryable().Include(x => x.NAT_VS_Venue_Address).Include(x => x.NAT_VS_Venue_Contact_Person).Where(x => x.NAT_VS_Venue_Address.Coordinates.Distance(location.Coordinates) <= radius && x.Location_Code == location.Location_Code && x.Venue_ID != venueId).ToListAsync();
        }


        #endregion
        #region Repository Methods for Venue Rating 
        /// <summary>
        /// This method returns venue rating object given a venueid
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="venueId">Reference ID</param>
        /// <returns>NAT_VS_Venue_RatingEF model</returns>
        public static async Task<NAT_VS_Venue_Rating> GetVenueRatingRecordAsync(this IRepositoryAsync<NAT_VS_Venue_Rating> repository, int venueId)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_ID == venueId).AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// This method returns venue rating log object given a venueid
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="venueId">Reference ID</param>
        /// <returns>NAT_VS_Venue_Rating_Log model</returns>
        public static async Task<IEnumerable<NAT_VS_Venue_Rating_Log>> venueratinglog(this IRepositoryAsync<NAT_VS_Venue_Rating_Log> repository, int venueId)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_ID == venueId).ToList();
        }
        #endregion





        #region Repository Methods for Venue Hall

        /// <summary>
        /// This method returns Venue Halls with a given venue Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <returns>Collection of Venue Hall EF model</returns>
        public static IEnumerable<NAT_VS_Venue_Hall> GetVenueHallById(this IRepositoryAsync<NAT_VS_Venue_Hall> repository, int VenueID)
        {
            return repository.Queryable()
                         .Where(x => x.Venue_ID == VenueID).ToList();
        }



        /// <summary>
        /// This method returns venue hall Details o with a given venue hall Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="venuehallID">Id of venue hall</param>
        /// <returns>Venue Hall EF model</returns>
        public static NAT_VS_Venue_Hall GetVenueHallByVenueHallId(this IRepositoryAsync<NAT_VS_Venue_Hall> repository, int VenueHallID)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_Hall_ID == VenueHallID).FirstOrDefault();
        }



        /// <summary>
        /// This method activate/deactivate venue hall against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate venue hall</param>
        /// <param name="venuehallEf">Venue Hall EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_VS_Venue_Hall> repository, bool flag, NAT_VS_Venue_Hall VenueHallEf)
        {
            VenueHallEf.Active_Flag = flag;
            repository.Update(VenueHallEf, x => x.Active_Flag);
        }
        #endregion



        #region Repository Methods for Venue Facility

        /// <summary>
        /// This method returns venue facilities with a given venue Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <returns>Collection of Venue Facility EF model</returns>
        public static IEnumerable<NAT_VS_Venue_Facility> GetVenueFacilityById(this IRepositoryAsync<NAT_VS_Venue_Facility> repository, int VenueID)
        {
            return repository.Queryable()
                         .Where(x => x.Venue_ID == VenueID).ToList();
        }


        


        /// <summary>
        /// This method returns venue facility with a given venue facility Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="venuefacilityID">Id of venue facility</param>
        /// <returns>Venue Facility EF model</returns>
        public static NAT_VS_Venue_Facility GetVenueFacilityByVenueFacilityId(this IRepositoryAsync<NAT_VS_Venue_Facility> repository, int VenueFacilityID)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_Facility_ID == VenueFacilityID).FirstOrDefault();
        }

        #endregion

        #region Repository Methods for Venue Contact Person 

        /// <summary>
        /// This method return list of all active Venue contact persons
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Venue contact persons EF model</returns>
        public static IEnumerable<NAT_VS_Venue_Contact_Person> GetAllActiveVenueContactPerson(this IRepositoryAsync<NAT_VS_Venue_Contact_Person> repository)
        {
            return repository.Queryable().Where(x => x.Active_Flag == true).ToList();
        }


        /// <summary>
        /// This method returns Venue contact persons with a given venue Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="VenueID">Id of Venue</param>
        /// <returns>Collection of Venue contact persons  EF model</returns>
        public static IEnumerable<NAT_VS_Venue_Contact_Person> GetVenueContactPersonById(this IRepositoryAsync<NAT_VS_Venue_Contact_Person> repository, int VenueID)
        {
            return repository.Queryable()
                         .Where(x => x.Venue_ID == VenueID).ToList();
        }


        /// <summary>
        /// This method returns venue contact person with a given venue contact person Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="venuehallID">Id of venue contact persons</param>
        /// <returns>Venue contact persons EF model</returns>
        public static NAT_VS_Venue_Contact_Person GetVenueContactPersonByVenueContactPersonId(this IRepositoryAsync<NAT_VS_Venue_Contact_Person> repository, int VenueContactPersonID)
        {
            return repository.Queryable()
                        .Where(x => x.Venue_Contact_Person_ID == VenueContactPersonID).FirstOrDefault();
        }



        /// <summary>
        /// This method activate/deactivate venue contact person against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate venue contact person</param>
        /// <param name="venuehallEf">Venue contact person EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_VS_Venue_Contact_Person> repository, bool flag, NAT_VS_Venue_Contact_Person VenueContactPersonEf)
        {
            VenueContactPersonEf.Active_Flag = flag;
            repository.Update(VenueContactPersonEf, x => x.Active_Flag);
        }
        #endregion

        #region Repository Methods For Venue Event

        public static async Task<IEnumerable<NAT_VS_Venue_Event>> GetAllVenuePendingEventsAsync(this IRepositoryAsync<NAT_VS_Venue_Event> repository, int venueId)
        {
            return await repository.Queryable()
                        .Where(x => x.Venue_ID == venueId && x.Start_Time > DateTime.Now && x.Status_LKP_ID == 1).ToListAsync();
        }
        public static int GetTotalEvents(this IRepository<NAT_VS_Venue_Event> repository, int venueId)
        {
            return  repository.Queryable()
                     .Where(x => x.Venue_ID == venueId && x.Start_Time < DateTime.Now).Count();
        }

        #endregion
    }
}

