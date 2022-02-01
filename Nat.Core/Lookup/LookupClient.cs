using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.ServiceClient;

namespace Nat.Core.Lookup
{
    public class LookupClient
    {
        /// <summary>
        /// Call Lookup api to get list of Lookups of specific LookupType
        /// </summary>
        /// <typeparam name="T">View Model</typeparam>
        /// <param name="LookupType">Type of Lookup</param>
        /// <returns>Dictionary with HiddenValue as key and Lookup object as value</returns>
        /// <example>
        /// <code>var genderLookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("GENDER");</code>
        /// </example>
        public static async Task<IDictionary<string,T>> ReadAsync<T>(string LookupType) where T : Model.ILookupModel
        {
            var lookupList = await NatClient.ReadAsync<IEnumerable<T>>(NatClient.Method.GET, NatClient.Service.LookupService, "lookups/" + LookupType);
            if (lookupList.status.IsSuccessStatusCode)
            {
                return lookupList.data.ToDictionary(item => item.GetHiddenValue(), item => item);
            }
            else
            {
                throw new ApplicationException("Failed to fetch Lookup of type: " + LookupType);
            }
        }
    }
}
