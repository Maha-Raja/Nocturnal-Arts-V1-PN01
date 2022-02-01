using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.BaseModelClass;
using Newtonsoft.Json;

namespace Nat.Core.ServiceClient
{
    public static class HttpResponseMessageExtension
    {
        public static async Task<ResponseViewModel<T>> ToResponseViewModelAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            string data = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseViewModel<T>>(data);
        }
    }
}
