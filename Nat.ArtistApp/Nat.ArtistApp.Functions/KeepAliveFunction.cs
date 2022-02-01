using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Nat.Core.BaseModelClass;
using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.ServiceClient;
using System;
using System.Net;
using System.Net.Http;

namespace Nat.ArtistApp.Functions
{
    public static class KeepAliveFunction
    {
        [FunctionName("KeepAliveTimerFunction")]
        public static void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"Keep alive executed at: {DateTime.Now}");
            var Artistlov = NatClient.ReadAsync<Object>(NatClient.Method.GET, NatClient.Service.VenueService, "Artistlov");
            log.Info(Artistlov.ToString());
            
        }

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
