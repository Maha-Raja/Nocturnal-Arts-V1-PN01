using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.KendoX;
using TLX.CloudCore.KendoX.Infrastructure;
using TLX.CloudCore.KendoX.UI;

namespace Nat.Core.KendoX
{
    public static class DataSourceRequestMapper
    {
        /// <summary>
        /// Map query parameters to DataSourceRequest
        /// </summary>
        /// <param name="queryParams">Enumerable list of key-value pair</param>
        /// <returns>Populated object of DataSourceRequest</returns>
        public static DataSourceRequest Map(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            DataSourceRequest dr = new DataSourceRequest
            {
                Page = Convert.ToInt32(GetParam(queryParams,GridUrlParameters.Page)),
                PageSize = Convert.ToInt32(GetParam(queryParams, GridUrlParameters.PageSize)),
                Filters = FilterDescriptorFactory.Create(GetParam(queryParams, GridUrlParameters.Filter)),
                Sorts = GridDescriptorSerializer.Deserialize<SortDescriptor>(GetParam(queryParams, GridUrlParameters.Sort)),
                Groups = GridDescriptorSerializer.Deserialize<GroupDescriptor>(GetParam(queryParams, GridUrlParameters.Group)),
                Aggregates = GridDescriptorSerializer.Deserialize<AggregateDescriptor>(GetParam(queryParams, GridUrlParameters.Aggregates)),
            };
            return dr;
        }
        /// <summary>
        /// Extract a parameter value by key from Enumerable list of key-value pair
        /// </summary>
        /// <param name="queryParams">Enumerable list of key-value pair</param>
        /// <param name="key">Key of query parameter</param>
        /// <returns>Query parameter value</returns>
        private static string GetParam(IEnumerable<KeyValuePair<string,string>> queryParams, string key)
        {
            return queryParams.Where(x => x.Key == key).FirstOrDefault().Value;
        }
    }
}
