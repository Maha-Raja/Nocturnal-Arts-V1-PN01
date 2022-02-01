using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nat.Core.QueueMessage;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
using System.Net;
using Nat.Core.Validations;
using Microsoft.AspNetCore.Http;
using System;
using Nat.Core.Exception;
using Nat.Core.Logger;

namespace Nat.CustomerApp.Functions
{
    public static class KeepAliveFunction
    {
        //[FunctionName("KeepAliveFunction")]
        //public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        //{
        //    log.Info($"Keep alive executed at: {DateTime.Now}");
        //}



        [FunctionName("KeepAliveFunction")]
        public static HttpResponseMessage KeepAliveFunctions([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "KeepAlive")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }

            try
            {
                var respVM = new ResponseViewModel<object>();
                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                return resp;
            }
            catch (ServiceLayerException ex)
            {
                return ServiceLayerExceptionHandler.CreateResponse(ex);
            }
            catch (Exception ex)
            {
                return FunctionLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}
