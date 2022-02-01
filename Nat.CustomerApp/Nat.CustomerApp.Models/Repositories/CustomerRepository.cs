using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.CustomerApp.Models.EFModel;
using System;
using System.Threading.Tasks;
using Nat.Common.Constants;

namespace Nat.CustomerApp.Models.Repositories
{
    public static class CustomerRepository
    {


        #region Repository Methods for Customer  

        /// <summary>
        /// This method return list of all Customers
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Customer EF model</returns>
        public static IEnumerable<NAT_CS_Customer> GetAllCustomer(this IRepositoryAsync<NAT_CS_Customer> repository)
        {
            return repository.Queryable()
                  .Include(x => x.NAT_CS_Customer_Residential_Address)
                  .Include(x => x.NAT_CS_Customer_Event)
                  .Include(x => x.NAT_CS_Customer_Following)
                  .Include(x => x.NAT_CS_Customer_Liked_Events)
                  .ToList();
        }


        /// <summary>
        /// This method returns Customer with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="CustomerID">Id of Customer</param>
        /// <returns>Customer EF model</returns>
        public static async Task<NAT_CS_Customer> GetCustomerByIdAsync(this IRepositoryAsync<NAT_CS_Customer> repository, int customerID)
        {
            return await repository.Queryable()
                  .Include(x => x.NAT_CS_Customer_Residential_Address)
                  .Include(x => x.NAT_CS_Customer_Event)
                  .Include(x => x.NAT_CS_Customer_Following)
                  .Include(x => x.NAT_CS_Customer_Liked_Events)
                  .Where(x => x.Customer_ID == customerID).FirstOrDefaultAsync();
        }

        public static Int32 GetFollowersCount(this IRepositoryAsync<NAT_CS_Customer_Following> repository, int refId, string options)
        {
            if (options == Constants.ReferenceType["ARTIST"])
            {
                return repository.Queryable().Where(x => x.Reference_ID == refId && x.Reference_Type == "Artist").Count();
            }
            if (options == Constants.ReferenceType["VENUE"])
            {
                return repository.Queryable().Where(x => x.Reference_ID == refId && x.Reference_Type == "Venue").Count();
            }
            else
            {
                return -1;
            }
        }


        public static Int32 GetEventLikesCount(this IRepositoryAsync<NAT_CS_Customer_Liked_Events> repository, string eventCode)
        {
            return repository.Queryable().Where(x => x.Event_Code == eventCode).Count();
        }


        public static async Task<IEnumerable<NAT_CS_Customer_Event>> GetCustomerEventsAsync(this IRepositoryAsync<NAT_CS_Customer_Event> repository, int customerID)
        {
            return await repository.Queryable()
                  .Where(x => x.Customer_ID == customerID).ToListAsync();
        }

        public  static async Task<IEnumerable<NAT_CS_Customer_Liked_Events>> GetCustomerLikedEventsAsync(this IRepositoryAsync<NAT_CS_Customer_Liked_Events> repository, int customerID)
        {
            return await repository.Queryable()
                  .Where(x => x.Customer_ID == customerID).ToListAsync();
        }
        public static async Task<NAT_CS_Customer_Liked_Events> GetLikedEventEntryAsync(this IRepositoryAsync<NAT_CS_Customer_Liked_Events> repository, int customerID, string eventCode)
        {
            return await repository.Queryable()
                  .Where(x => x.Customer_ID == customerID && x.Event_Code == eventCode).FirstOrDefaultAsync();
        }
        public static async Task<NAT_CS_Customer_Liked_Events> GetCustomerLikedEventsByCodeAsync(this IRepositoryAsync<NAT_CS_Customer_Liked_Events> repository, String EventCode)
        {
            return await repository.Queryable()
                  .Where(x => x.Event_Code == EventCode).FirstOrDefaultAsync();
        }

        public static async Task<IEnumerable<NAT_CS_Customer_Following>> GetAllVenuesFollowedbyCustomerAsync(this IRepositoryAsync<NAT_CS_Customer_Following> repository, int customerID)
        {
            string blob = Constants.ReferenceType["VENUE"];
            return await repository.Queryable()
            .Where(x => x.Customer_ID == customerID && x.Reference_Type == blob).ToListAsync();
        }
        public static async Task<IEnumerable<NAT_CS_Customer_Following>> GetAllArtistsFollowedbyCustomerAsync(this IRepositoryAsync<NAT_CS_Customer_Following> repository, int customerID)
        {
            
            string blob = Constants.ReferenceType["ARTIST"];
            return await repository.Queryable()
            .Where(x => x.Customer_ID == customerID && x.Reference_Type == blob).ToListAsync();
        }


        /// <summary>
        /// This method returns person with a given Email Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EmailID">Email Id of Customer</param>
        /// <returns>true/false</returns>
        public static bool GetPersonByEmail(this IRepositoryAsync<NAT_CS_Customer> repository, string EmailID)
        {
            NAT_CS_Customer Person = repository.Queryable()
                        .Where(x => x.Person_Email == EmailID).FirstOrDefault();
            if (Person != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method activate/deactivate Customer against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate Customer</param>
        /// <param name="CustomerEf">Customer EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_CS_Customer> repository, bool flag, NAT_CS_Customer CustomerEf)
        {
            CustomerEf.Active_Flag = flag;
            repository.Update(CustomerEf, x => x.Active_Flag);
        }


        /// <summary>
        /// Retunr 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Object</returns>
        public static async Task<Object> GetCustomerLov(this IRepository<NAT_CS_Customer> repository)
        {
            var lov = await repository.Queryable()
                .Select(x => new
                {
                    id = x.Customer_ID,
                    value = x.Person_First_Name + " " + x.Person_Last_Name,
                    image = x.Person_Profile_Image_Link
                }).ToListAsync();
            return lov;
        }


        /// <summary>
        /// Retunr 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Object</returns>
        public static async Task<IEnumerable<NAT_CS_Customer_Following>> GetAllFollowersOfArtist(this IRepository<NAT_CS_Customer_Following> repository, int Artistid)
        {
            string blob = Constants.ReferenceType["ARTIST"];
            return repository.Queryable()
             .Where(x => x.Reference_ID == Artistid && x.Reference_Type==blob)
             .ToList();
        }


        /// <summary>
        /// Retunr 
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Object</returns>
        public static async Task<IEnumerable<NAT_CS_Customer_Following>> GetAllFollowersOfVenue(this IRepository<NAT_CS_Customer_Following> repository, int Venueid)
        {
            string blob = Constants.ReferenceType["VENUE"];
            return repository.Queryable()
             .Where(x => x.Reference_ID == Venueid && x.Reference_Type == blob)
             .ToList();
        }





        public static async Task<NAT_CS_Customer_Following> GetArtistFollowedEntry(this IRepository<NAT_CS_Customer_Following> repository, int CustomerId, int ArtistId)
        {
            string blob = Constants.ReferenceType["ARTIST"];
            return repository.Queryable()
             .Where(x => x.Reference_ID == ArtistId && x.Customer_ID == CustomerId && x.Reference_Type == blob)
             .FirstOrDefault();
        }


        public static async Task<NAT_CS_Customer_Following> GetVenueFollowedEntry(this IRepository<NAT_CS_Customer_Following> repository, int CustomerId, int VenueId)
        {
            string blob = Constants.ReferenceType["VENUE"];
            return repository.Queryable()
             .Where(x => x.Reference_ID == VenueId && x.Customer_ID == CustomerId && x.Reference_Type == blob)
             .FirstOrDefault();
        }
        #endregion







    }
}
