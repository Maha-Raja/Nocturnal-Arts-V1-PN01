using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Nat.Core.BaseModelClass;
using Nat.Core.Exception;
using Newtonsoft.Json;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.CustomerApp.Services;
using Nat.CustomerApp.Functions.ViewModels;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.Patterns.Repository.Ef6;
using Nat.Core.KendoX.Extension;
using TLX.CloudCore.KendoX.UI;
using System.Collections.Generic;
using Nat.Core.Validations;

namespace Nat.CustomerApp.Functions
{
    public static class CustomerLovFunction
    {
        ////GET Artist by ID Method
        [FunctionName("GetCustomerLovFunction")]
        public async static Task<HttpResponseMessage> GetCustomerLovFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customerslov")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get customer Lov"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = await service.GetCustomerLov();
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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
}
