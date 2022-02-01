using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.LookupApp.Models.EFModel;

namespace Nat.LookupApp.Models.Repositories
{
    public static class LookupRepository
    {
        /// <summary>
        /// This method return list of all active Lookup of a type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="type">Lookup Type</param>
        /// <returns>Collection of Lookup EF model</returns>
        public static async System.Threading.Tasks.Task<IEnumerable<NAT_LUS_Lookup>> GetLookupByLookupTypeAsync(this IRepositoryAsync<NAT_LUS_Lookup> repository, string type)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Lookup_Type == type).ToListAsync();
        }

        /// <summary>
        /// This method return list of all Lookup of a type
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="type">Lookup Type</param>
        /// <returns>Collection of Lookup EF model</returns>
        public static async System.Threading.Tasks.Task<IEnumerable<NAT_LUS_Lookup>> GetLookupTypeCountAsync(this IRepositoryAsync<NAT_LUS_Lookup> repository, string type)
        {
            return await repository.Queryable().Where(x => x.Lookup_Type == type).ToListAsync();
        }

        /// <summary>
        /// This method return list of all active Lookup
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="type">Lookup Type</param>
        /// <returns>Collection of Lookup EF model</returns>
        public static async System.Threading.Tasks.Task<IEnumerable<NAT_LUS_Lookup>> GetAllLookupAsync(this IRepositoryAsync<NAT_LUS_Lookup> repository)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true).OrderBy(x => x.Visible_Value).ToListAsync();
        }

        /// <summary>
        /// This method return configuration object by a key
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="key">Configuration Key</param>
        /// <returns>Configuration EF model</returns>
        public static async System.Threading.Tasks.Task<NAT_LUS_Configuration> GetConfigurationByKeyAsync(this IRepositoryAsync<NAT_LUS_Configuration> repository, string key)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Key == key).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method return all configuration objects
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>List of Configuration EF model</returns>
        public static async System.Threading.Tasks.Task<IEnumerable<NAT_LUS_Configuration>> GetAllConfiguration(this IRepositoryAsync<NAT_LUS_Configuration> repository)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true).ToListAsync();
        }

        /// <summary>
        /// This method return configuration object by a configuration Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="key">Configuration Id</param>
        /// <returns>Configuration EF model</returns>
        public static async System.Threading.Tasks.Task<NAT_LUS_Configuration> GetConfigurationByIdAsync(this IRepositoryAsync<NAT_LUS_Configuration> repository, int configurationId)
        {
            return await repository.Queryable().Where(x => x.Active_Flag == true && x.Configuration_ID == configurationId).FirstOrDefaultAsync();
        }


    }
}
