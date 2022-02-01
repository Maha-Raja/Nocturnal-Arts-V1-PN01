using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using Nat.Core.BaseModelClass;

namespace Nat.Core.ServiceClient
{
    public class NatResponseMessage : HttpResponseMessage
    {
        private ResponseViewModel<object> Data;
        public static async Task<NatResponseMessage> FromHttpResponseMessage(HttpResponseMessage msg)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<HttpResponseMessage, NatResponseMessage>());
            NatResponseMessage natmsg = Mapper.Map<NatResponseMessage>(msg);
            string data = await msg.Content.ReadAsStringAsync();
            natmsg.Data = JsonConvert.DeserializeObject<ResponseViewModel<object>>(data);
            return natmsg;
        }

        public ResponseViewModel<object> GetResponseData()
        {
            return Data;
        }
    }
}
