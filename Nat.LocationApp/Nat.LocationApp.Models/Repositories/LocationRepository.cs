using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.LocationApp.Models.EFModel;
using System.Threading.Tasks;
using System;

namespace Nat.LocationApp.Models.Repositories
{
    public static class LocationRepository
    {

        #region Locations

        /// <summary>
        /// This method return list of all locations
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of location EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Location>> GetAllLocations(this IRepositoryAsync<NAT_LS_Location> repository)
        {
            return repository.Queryable().OrderBy(x=> x.Location_Name).ToList();
        }

        /// <summary>
        /// This method returns Location with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="LocationID">Id of Location</param>
        /// <returns>Location EF model</returns>
        public static async Task<NAT_LS_Location> GetLocationByIdAsync(this IRepositoryAsync<NAT_LS_Location> repository, int locationID)
        {
            return await repository.Queryable()
                         .Where(x => x.Location_ID == locationID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns Parent Location with a given Child Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="LocationID">Id of Child Location</param>
        /// <returns>Location EF model</returns>
        public static async Task<NAT_LS_Location> GetParentLocationByChildIdAsync(this IRepositoryAsync<NAT_LS_Location> repository, int locationID)
        {
            //NAT_LS_Location childObject =  await repository.Queryable()
            //                                .Where(x => x.Location_ID == locationID).Include(x => x.NAT_LS_Parent_Location).FirstOrDefaultAsync();

            //return childObject.NAT_LS_Parent_Location;
            return null;
        }

        /// <summary>
        /// This method list of all immediate active children for a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="parentID">Id of Parent Location</param>
        /// <returns>Collection of location EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Location>> GetImmediateActiveChildrenLocation(this IRepositoryAsync<NAT_LS_Location> repository, string parentCode)
        {
            return repository.Queryable().Where(x => x.Parent_Location_Code == parentCode && x.Active_Flag == true).ToList();
        }

        /// <summary>
        /// This method list of all active children for a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="parentID">Id of Parent Location</param>
        /// <returns>Collection of location EF model</returns>
        public static IEnumerable<NAT_LS_Location> GetAllActiveChildrenLocation(this IRepositoryAsync<NAT_LS_Location> repository, string code, bool activeFlag = true)
        {
            try
            {
                List<NAT_LS_Location> childrenList = new List<NAT_LS_Location>();
                GetLocationChildren(repository, code, ref childrenList, activeFlag);
                int i = childrenList.Count;
                return childrenList;
            }
            catch (Exception e)
            {
                string bug = e.StackTrace;
                return null;
            }
        }

        public static void GetLocationChildren(this IRepositoryAsync<NAT_LS_Location> repository, string locationCode, ref List<NAT_LS_Location> childrenList, bool activeFlag = false)
        {
            List<NAT_LS_Location> list = repository.Queryable().Where(x => x.Parent_Location_Code == locationCode && x.Active_Flag).ToList();
            if (list.Count != 0)
            {
                foreach (NAT_LS_Location location in list)
                {
                    childrenList.Add(location);
                    GetLocationChildren(repository, location.Location_Short_Code, ref childrenList, false);
                }
            }
        }

        /// <summary>
        /// This method return list of all location for a given Type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="type">Type of Location</param>
        /// <returns>Collection of Location EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Location>> GetLocationByType(this IRepositoryAsync<NAT_LS_Location> repository, string type)
        {
            return repository.Queryable().Where(x => x.Location_Type_LKP == type && x.Active_Flag == true).ToList();
        }

        public static async Task<NAT_LS_Location> GetLocationByShortCode(this IRepositoryAsync<NAT_LS_Location> repository, string code)
        {
            return await repository.Queryable().Where(x => x.Location_Short_Code == code && x.Active_Flag == true).FirstOrDefaultAsync();
        }

        public static async Task<NAT_LS_Location> GetLocationByAirportCode(this IRepositoryAsync<NAT_LS_Location> repository, string code)
        {
            return await repository.Queryable().Where(x => x.Airport_Code == code && x.Active_Flag == true).FirstOrDefaultAsync();
        }

        public static async Task<NAT_LS_Location> GetLocationByName(this IRepositoryAsync<NAT_LS_Location> repository, string code)
        {
            return await repository.Queryable().Where(x => x.Location_Name == code && x.Active_Flag == true).FirstOrDefaultAsync();
        }


        public static async Task<NAT_LS_LOCATION_VW> GetLocationViewByCode(this IRepositoryAsync<NAT_LS_LOCATION_VW> repository, string code)
        {
            return await repository.Queryable().Where(x => x.Location_Short_Code == code && x.Active_Flag == true).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns Parent Location with a given Child Code and Type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="code">Code of Child Address Geography</param>
        /// <param name="type">Type of Address Geography</param>
        /// <returns>AddressGeography EF model</returns>
        public static async Task<NAT_LS_Location> GetParentLocationByTypeFunction(this IRepositoryAsync<NAT_LS_Location> repository, string code, string type)
        {
            NAT_LS_Location obj = new NAT_LS_Location();
            while (obj.Location_Type_LKP != type)
            {
                obj = await repository.Queryable().Where(x => x.Location_Short_Code == code).FirstOrDefaultAsync();
                if (obj != null)
                    code = obj.Parent_Location_Code;
            }

            return obj;
        }

        #endregion

        #region Address Geography

        /// <summary>
        /// This method return list of all addressgeography
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of addressgeography EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Address_Geography>> GetAllAddressGeography(this IRepositoryAsync<NAT_LS_Address_Geography> repository)
        {
            return repository.Queryable().OrderBy(x => x.Geography_Name).ToList();
        }

        /// <summary>
        /// This method returns AddressGeography with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="AddressGeographyID">Id of AddressGeography</param>
        /// <returns>AddressGeography EF model</returns>
        public static async Task<NAT_LS_Address_Geography> GetAddressGeographyByIdAsync(this IRepositoryAsync<NAT_LS_Address_Geography> repository, int addressgeographyID)
        {

            return await repository.Queryable()
                         .Where(x => x.Address_Geography_ID == addressgeographyID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method returns Parent Address Geography with a given Child Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="AddressGeographyID">Id of Child Location</param>
        /// <returns>AddressGeography EF model</returns>
        public static async Task<NAT_LS_Address_Geography> GetParentAddressGeographyByChildIdAsync(this IRepositoryAsync<NAT_LS_Address_Geography> repository, int addressgeographyID)
        {
            //NAT_LS_Address_Geography childObject = await repository.Queryable()
            //                                     .Where(x => x.Address_Geography_ID == addressgeographyID).Include(x => x.NAT_LS_Parent_Address_Geography).FirstOrDefaultAsync();

            return null;// childObject.NAT_LS_Parent_Address_Geography;
        }

        /// <summary>
        /// This method returns Parent Address Geography with a given Child Code and Type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="code">Code of Child Address Geography</param>
        /// <param name="type">Type of Address Geography</param>
        /// <returns>AddressGeography EF model</returns>
        public static async Task<NAT_LS_Address_Geography> GetParentAddressGeographyByTypeFunction(this IRepositoryAsync<NAT_LS_Address_Geography> repository, string code, string type)
        {
            NAT_LS_Address_Geography obj = new NAT_LS_Address_Geography();
            while (obj.Geography_Type_LKP != type)
            {
                obj = await repository.Queryable().Where(x => x.Geography_Short_Code == code).FirstOrDefaultAsync();
                if(obj !=null )
                    code = obj.Parent_Geography_Code;
            }

            return obj;
        }

        /// <summary>
        /// This method returns list of all immediate active children for a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="parentID">Id of Parent Address Geography</param>
        /// <returns>Collection of Address Geography EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Address_Geography>> GetImmediateActiveChildrenAddressGeography(this IRepositoryAsync<NAT_LS_Address_Geography> repository, string parentcode)
        {
            return repository.Queryable()
                .Where(x => x.Parent_Geography_Code == parentcode && x.Active_Flag == true)
                .OrderBy(x => x.Geography_Name)
                .ToList();
        }

        /// <summary>
        /// This method returns list of all active children for a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="parentID">Id of Parent Address Geography</param>
        /// <returns>Collection of address geography EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Address_Geography>> GetAllActiveChildrenAddressGeography(this IRepositoryAsync<NAT_LS_Address_Geography> repository, string code)
        {
            try
            {
                List<NAT_LS_Address_Geography> childrenList = new List<NAT_LS_Address_Geography>();
                GetGeographyChildren(repository, code, ref childrenList);                
                return childrenList.Where(x => x.Active_Flag);
            }
            catch (Exception e)
            {
                string bug = e.StackTrace;
                return null;
            }
        }

        public static void GetGeographyChildren(this IRepositoryAsync<NAT_LS_Address_Geography> repository, string code, ref List<NAT_LS_Address_Geography> childrenList)
        {
            List<NAT_LS_Address_Geography> list = repository.Queryable().Where(x => x.Parent_Geography_Code == code).OrderBy(x=> x.Geography_Name).ToList();
            if (list.Count != 0)
            {
                foreach (NAT_LS_Address_Geography geography in list)
                {
                    childrenList.Add(geography);
                    GetGeographyChildren(repository, geography.Geography_Short_Code, ref childrenList);
                }
            }
        }

        /// <summary>
        /// This method return list of all address geography for a given Type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="type">Type of Address Geography</param>
        /// <returns>Collection of Address Geography EF model</returns>
        public static async Task<IEnumerable<NAT_LS_Address_Geography>> GetAddressGeographyByType(this IRepositoryAsync<NAT_LS_Address_Geography> repository, string type)
        {
            return repository.Queryable()
                .Where(x => x.Geography_Type_LKP == type && x.Active_Flag == true)
                .OrderBy(x => x.Geography_Name)
                .ToList();
        }


        #endregion

    }
}
