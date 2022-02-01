using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.ArtistApp.Models.EFModel;
using System.Threading.Tasks;
using System;
using Nat.Core.ServiceClient;

namespace Nat.ArtistApp.Models.Repositories
{
    public static class ArtistRepository
    {


        #region Repository Methods for Artist  

        /// <summary>
        /// Retunr 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<Object> GetArtistLov(this IRepository<NAT_AS_Artist> repository)
        {
            var lov = await repository.Queryable()
                .Where(x => x.Active_Flag)
                .Select(x => new
                {
                    id = x.Artist_ID,
                    rating = x.NAT_AS_Artist_Rating.FirstOrDefault() == null ? 0.0 : x.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value,
                    value = x.Artist_First_Name + " " + x.Artist_Last_Name,
                    image = x.Artist_Profile_Image_Link,
                    firstName = x.Artist_First_Name,
                    lastName = x.Artist_Last_Name,
                    PlannerId = x.Planner_ID,
                    Email = x.Artist_Email,
                    Phone = x.Contact_Number,
                    locationCode = x.NAT_AS_Artist_Location_Mapping.Where(y => y.Artist_ID == x.Artist_ID).Select(y => y.Location_Code).ToList(),
                    artistLocation = x.Artist_First_Name + " " + x.Artist_Last_Name + " - "
                }).OrderBy(x => x.value).ToListAsync();
            return lov;
        }
        public static async Task<Object> GetArtistLovForArtist(this IRepository<NAT_AS_Artist> repository,long artistId)
        {
            var lov = await repository.Queryable()
                .Where(x => x.Artist_ID == artistId)
                .Select(x => new
                {
                    id = x.Artist_ID,
                    rating = x.NAT_AS_Artist_Rating.FirstOrDefault() == null ? 0.0 : x.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value,
                    value = x.Artist_First_Name + " " + x.Artist_Last_Name,
                    image = x.Artist_Profile_Image_Link,
                    firstName = x.Artist_First_Name,
                    lastName = x.Artist_Last_Name,
                    PlannerId = x.Planner_ID,
                    Email = x.Artist_Email,
                    Phone = x.Contact_Number,
                    locationCode = x.NAT_AS_Artist_Location_Mapping.Where(y => y.Artist_ID == x.Artist_ID).Select(y => y.Location_Code).ToList(),
                    artistLocation = x.Artist_First_Name + " " + x.Artist_Last_Name + " - "
                }).OrderBy(x => x.value).ToListAsync();
            return lov;
        }
        public static async Task<Object> GetArtistLovForArtistManager(this IRepository<NAT_AS_Artist> repository, List<long?> artistIdList)
        {
            var lov = await repository.Queryable()
                .Where(x => artistIdList.Contains(x.Artist_ID))
                .Select(x => new
                {
                    id = x.Artist_ID,
                    rating = x.NAT_AS_Artist_Rating.FirstOrDefault() == null ? 0.0 : x.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value,
                    value = x.Artist_First_Name + " " + x.Artist_Last_Name,
                    image = x.Artist_Profile_Image_Link,
                    firstName = x.Artist_First_Name,
                    lastName = x.Artist_Last_Name,
                    PlannerId = x.Planner_ID,
                    Email = x.Artist_Email,
                    Phone = x.Contact_Number,
                    locationCode = x.NAT_AS_Artist_Location_Mapping.Where(y => y.Artist_ID == x.Artist_ID).Select(y => y.Location_Code).ToList(),
                    artistLocation = x.Artist_First_Name + " " + x.Artist_Last_Name + " - "
                }).OrderBy(x => x.value).ToListAsync();
            return lov;
        }
        /// <summary>
        /// Retunr 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        //public static async Task<Object> GetArtistEventDetails(this IRepository<NAT_AS_Artist_Event> repository, System.DateTime Date)
        //{

        //    var Details = await repository.Queryable().Where
        //        .Select(x => new
        //        {

        //        //    id = x.Artist_ID,
        //        //    value = x.Artist_First_Name + " " + x.Artist_Last_Name
        //        }).ToListAsync();
        //    return lov;
        //}

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of artist EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist>> GetAllArtistAsync(this IRepositoryAsync<NAT_AS_Artist> repository)
        {
            return await repository.Queryable()
                .Include(x => x.NAT_AS_Artist_Location_Mapping)
                        .Include(x => x.NAT_AS_Artist_Bank_Account)
                        .Include(x => x.NAT_AS_Artist_Rating).
                        ToListAsync();
        }

        /// <summary>
        /// This method return list of event held count, LastEventDate and Upcomming Events count against all artists
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of artist EF model</returns>
        public static async Task<IEnumerable<Object>> GetAllEventCalculatedDetails(this IRepositoryAsync<NAT_AS_Artist_Event> repository)
        {
            var artisteventdata = repository.Queryable().GroupBy(i => i.Artist_ID)
           .Select(g => new
           {
               
               ArtistId = g.Max(x => x.Artist_ID),

               Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0,
               LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time),
               UpcommingEvents= (int?)g.Count(x => x.Start_Time > System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0
               //.Where(x => x.Start_Time <= System.DateTime.Now && x.Status_LKP_ID == 1)
           });

            return artisteventdata;
        }

        public static async Task<IEnumerable<Object>> GetEventCalculatedDetailsByArtistId(this IRepositoryAsync<NAT_AS_Artist_Event> repository, int artistID)
        {
            var artisteventdata = repository.Queryable().Where(x => x.Artist_ID == artistID).GroupBy(i => i.Artist_ID)
           .Select(g => new
           {

               ArtistId = g.Max(x => x.Artist_ID),

               Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0,
               LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time),
               UpcommingEvents = (int?)g.Count(x => x.Start_Time > System.DateTime.Now && x.Status_LKP_ID == 1) ?? 0
               //.Where(x => x.Start_Time <= System.DateTime.Now && x.Status_LKP_ID == 1)
           });

            return artisteventdata;
        }
        /// <summary>
        /// This method returns artist with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of artist</param>
        /// <returns>Artist EF model</returns>
        public static async Task<NAT_AS_Artist> GetArtistByIdAsync(this IRepositoryAsync<NAT_AS_Artist> repository, int artistID)
        {
            return await repository.Queryable()
                        .Include(x => x.NAT_AS_Artist_Skill)
                        .Include(x => x.NAT_AS_Artist_Bank_Account)
                        .Include(x => x.NAT_AS_Artist_Venue_Preference)
                        .Include(x => x.NAT_AS_Artist_Rating)
                        .Include(x => x.NAT_AS_Artist_Residential_Address)
                        .Include(x => x.NAT_AS_Artist_Document)
                        .Include(x => x.NAT_AS_Artist_Bank_Account.Select(y => y.NAT_AS_Artist_Address))
                        .Include(x => x.NAT_AS_Artist_Rating_Log)
                        .Include(x => x.NAT_AS_Artist_Location_Mapping)
                        .Where(x => x.Artist_ID == artistID ).FirstOrDefaultAsync();
        }

        //get Artist by planner id
        public static async Task<NAT_AS_Artist> GetArtistByPlannerIdAsync(this IRepositoryAsync<NAT_AS_Artist> repository, int plannerid)
        {
            return await repository.Queryable().Where(x => x.Planner_ID == plannerid).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns artist with a given Email Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EmailID">Email Id of artist</param>
        /// <returns>true/false</returns>
        public static async Task<bool> GetArtistByEmailAsync(this IRepositoryAsync<NAT_AS_Artist> repository, string EmailID)
        {
            NAT_AS_Artist Artist = await repository.Queryable()
                        .Where(x => x.Artist_Email == EmailID).FirstOrDefaultAsync();
            if (Artist != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method search artists
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="plannerIds"></param>
        /// <param name="rating"></param>
        /// <param name="skill"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NAT_AS_Artist>> SearchArtistForEvent(this IRepositoryAsync<NAT_AS_Artist> repository, List<int> plannerIds, Nullable<float> min_rating, Nullable<float> max_rating, Nullable<int> skill, String searchText, String locationcode)
        {

            Func<NAT_AS_Artist, bool> whereCondition = (i) =>
            {
                return ((min_rating != null ? (i.NAT_AS_Artist_Rating.Count != 0 ? i.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value >= min_rating : false) : true) && 
                        (max_rating != null ? (i.NAT_AS_Artist_Rating.Count != 0 ? i.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value <= max_rating : false) : true) && 
                        (searchText != null ? (i.Artist_First_Name.ToLower().Contains(searchText.ToLower()) || 
                            (i.Artist_Last_Name != null ? i.Artist_Last_Name.ToLower().Contains(searchText.ToLower()) : false) || 
                            (i.Artist_Middle_Name != null ? i.Artist_Middle_Name.ToLower().Contains(searchText.ToLower()) : false)) : true) && 
                        //(plannerIds.Count != 0 ? plannerIds.Contains(i.Planner_ID ?? default(int)) : false) && 
                        (skill != null ? (i.NAT_AS_Artist_Skill.Count != 0 ? i.NAT_AS_Artist_Skill.FirstOrDefault().Skill_LKP_ID == skill : false) : true) &&
                        (locationcode != null ? (i.NAT_AS_Artist_Location_Mapping.Where(x => x.Location_Code == locationcode && plannerIds.Contains(x.Planner_ID ?? -1)).Count() > 0) : false)) &&
                        i.Active_Flag == true;
            };

            return repository.Queryable()
                .Include(x => x.NAT_AS_Artist_Rating)
                .Include(x => x.NAT_AS_Artist_Skill)
                .Include(x => x.NAT_AS_Artist_Residential_Address)
                .Include(x => x.NAT_AS_Artist_Location_Mapping)
                .Where(whereCondition).ToList();

            //return await repository.Queryable()
            //    .Where(i => (i.NAT_AS_Artist_Rating.Count != 0 ? (min_rating != null ? i.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value >= min_rating : true) : false) && (i.NAT_AS_Artist_Rating.Count != 0 ? (max_rating != null ? i.NAT_AS_Artist_Rating.FirstOrDefault().Average_Rating_Value <= max_rating : true) : false))
            //    .Where(i => plannerIds.Contains(i.Planner_ID ?? default(int)))
            //    .Where(i => searchText != null ? (i.Artist_First_Name.Contains(searchText) || i.Artist_Last_Name.Contains(searchText) || i.Artist_Middle_Name.Contains(searchText) || i.Artist_Email.Contains(searchText)) : true)
            //    .Include(x => x.NAT_AS_Artist_Rating)
            //    .ToListAsync();


        }

        /// <summary>
        /// This method returns artist list with location code
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="locationCode">Id of Venue</param>
        /// <returns>Collection of Artist EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist>> GetAllArtistByLocationCode(this IRepositoryAsync<NAT_AS_Artist> repository, String locationcode)
        {

            Func<NAT_AS_Artist, bool> whereCondition = (i) =>
            {
                return (locationcode != null ? (i.NAT_AS_Artist_Location_Mapping.Where(x => x.Location_Code == locationcode).Count() > 0) : true) &&
                        i.Active_Flag == true;
            };

            return repository.Queryable()
                .Include(x => x.NAT_AS_Artist_Rating)
                .Include(x => x.NAT_AS_Artist_Skill)
                .Include(x => x.NAT_AS_Artist_Residential_Address)
                .Include(x => x.NAT_AS_Artist_Location_Mapping)
                .Where(whereCondition).ToList();

        }


        public static async Task<IEnumerable<NAT_AS_Artist_Location_Mapping>> GetAllArtistByLocationcode(this IRepositoryAsync<NAT_AS_Artist_Location_Mapping> repository, string locationCode)
        {
            return await repository.Queryable().Where(x => x.Location_Code == locationCode).ToListAsync();
        }

        /// <summary>
        /// This method activate/deactivate artist against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate artist</param>
        /// <param name="artistEf">Artist EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_AS_Artist> repository, bool flag, NAT_AS_Artist artistEf)
        {
            artistEf.Active_Flag = flag;
            repository.Update(artistEf, x => x.Active_Flag);
        }

        public static int GetTotalEvents(this IRepository<NAT_AS_Artist_Event> repository, int artistId)
        {
            return  repository.Queryable()
                     .Where(x => x.Artist_ID == artistId && x.Start_Time<DateTime.Now).Count();
        }

        #endregion



        #region Repository Methods for Artist Bank Account 

        /// <summary>
        /// This method returns Bank Accounts Details of artist with a given artist Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of artist</param>
        /// <returns>Collection of Artist Bank Account EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist_Bank_Account>> GetArtistBankAccountsByIdAsync(this IRepositoryAsync<NAT_AS_Artist_Bank_Account> repository, int artistID)
        {
            return await repository.Queryable()
                        .Where(x => x.Artist_ID == artistID).ToListAsync();
        }


        /// <summary>
        /// This method returns Bank Account Details of artist with a given bank account Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistbankaccountID">Id of artist</param>
        /// <returns>Artist Bank Account EF model</returns>
        public static async Task<NAT_AS_Artist_Bank_Account> GetArtistBankAccountBybankaccountIdAsync(this IRepositoryAsync<NAT_AS_Artist_Bank_Account> repository, int artistbankaccountID)
        {
            return await repository.Queryable()
                        .Where(x => x.Artist_Bank_Account_ID == artistbankaccountID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method activate/deactivate artist bank account against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate artist</param>
        /// <param name="artistbankaccountEf">Artist Bank Account EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_AS_Artist_Bank_Account> repository, bool flag, NAT_AS_Artist_Bank_Account artistbankaccountEf)
        {
            artistbankaccountEf.Active_Flag = flag;
            repository.Update(artistbankaccountEf, x => x.Active_Flag);
        }


        /// <summary>
        /// This method returns artist event with a given eventcode
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Reference ID</param>
        /// <returns>Artist Event EF model</returns>
        public static async Task<NAT_AS_Artist_Event> GetArtistEventByEventCodeAsync(this IRepositoryAsync<NAT_AS_Artist_Event> repository, string eventcode)
        {
            return await repository.Queryable().Include(x => x.NAT_AS_Artist)
                        .Where(x => x.Reference_ID == eventcode).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns Upcoming Events of artist with a given artist Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistId">Id of artist</param>
        /// <returns>Artist Event EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist_Event>> GetAllArtistPendingEventsAsync(this IRepositoryAsync<NAT_AS_Artist_Event> repository, int artistId)
        {
            return await repository.Queryable()
                        .Where(x => x.Artist_ID == artistId && x.Start_Time > DateTime.Now && x.Status_LKP_ID==1).ToListAsync();
        }
        #endregion



        #region Repository Methods for Artist Rating 
        /// <summary>
        /// This method returns Artist rating object for a prticular artist id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistId">Id of artist</param>
        /// <returns>Artist Event EF model</returns>
        public static async Task<NAT_AS_Artist_Rating> GetArtistRatingRecordAsync(this IRepositoryAsync<NAT_AS_Artist_Rating> repository, int artistId)
        {
            return repository.Queryable()
                        .Where(x => x.Artist_ID == artistId).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// This method returns Artist rating log object for a prticular artist id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistId">Id of artist</param>
        /// <returns>Artist Event EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist_Rating_Log>> GetArtistRatingLogByArtistId(this IRepositoryAsync<NAT_AS_Artist_Rating_Log> repository, int artistId, int requiredRecords)
        {
            return repository.Queryable()
                        .Where(x => x.Artist_ID == artistId).OrderByDescending(x => x.Created_Date).Take(requiredRecords).ToList();
        }


        //public static async Task<NAT_AS_Artist_Rating> artistaveragerating(this IRepositoryAsync<NAT_AS_Artist_Rating> repository, int artistId)
        //{
        //    return repository.Queryable()
        //                .Where(x => x.Artist_ID == artistId).FirstOrDefault();
        //}
        #endregion


        #region Repository Methods for Artist Location Mapping 
        /// <summary>
        /// This method returns artist locations list
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Artist Id against which locations will be fetched</param>
        /// <returns>Collection of Artist location mapping EF model</returns>
        public static async Task<IEnumerable<NAT_AS_Artist_Location_Mapping>> GetArtistLocationsList(this IRepositoryAsync<NAT_AS_Artist_Location_Mapping> repository, int artistID)
        {
            return await repository.Queryable().Where(x => x.Artist_ID == artistID).ToListAsync();
        }
        #endregion
    }
}
