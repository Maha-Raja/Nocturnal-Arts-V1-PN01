using System.Net.Http;
using TLX.CloudCore.KendoX.UI;

namespace Nat.Core.KendoX.Extension
{
    public static class KendoXExtensionMethods
    {
        /// <summary>
        /// Map query parameters from request object to DataSourceRequest object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static DataSourceRequest ToDataSourceRequest(this HttpRequestMessage request)
        {
            return DataSourceRequestMapper.Map(request.GetQueryNameValuePairs());
        }
    }
}
