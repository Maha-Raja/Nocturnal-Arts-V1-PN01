using System; 
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nat.Core.BaseModelClass;
using Nat.LookupApp.Functions.ViewModels;
using Nat.LookupApp.Services;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.KendoX.Extension;
using Microsoft.Extensions.Logging;
using TLX.CloudCore.KendoX.UI;
using Nat.LookupApp.Services.ServiceModels;
using Nat.Core.Exception;
using Nat.Core.Validations;

namespace Nat.LookupApp.Functions
{
    public class PlannerFunction
    {

        #region  Methods for Lookup  

        /// <summary>
        /// Get list of lookup of a specific LookupType
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <param name="type">Url parameter for LookupType</param>
        /// <returns></returns>
        [FunctionName("GetLookupByLookupType")]
        public static async Task<HttpResponseMessage> GetAllPlannerFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "lookups/{type}")] HttpRequestMessage req, [Logger] NatLogger logger,string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Lookup By Lookup Type"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Get lookup from service");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = new LookupViewModel().FromServiceModelList(await service.GetLookupByLookupTypeAsync(type));
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
                    return FunctionLayerExceptionHandler.Handle(ex,logger);
                }
            }
        }

        /// <summary>
        /// Get list of lookup of a specific LookupType
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <param name="type">Url parameter for LookupType</param>
        /// <returns></returns>
        [FunctionName("GetLookCountbyLookupType")]
        public static async Task<HttpResponseMessage> GetLookCountbyLookupTypeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetLookCountbyLookupType/{type}")] HttpRequestMessage req, [Logger] NatLogger logger, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Lookup By Lookup Type"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Get lookup from service");
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = await service.GetLookCountbyLookupTypeAsync(type);
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


        [FunctionName("GetAllLookupsFunction")]
        public static async Task<HttpResponseMessage> GetAllLookupsFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "GetAllLookups")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Lookups"))
            {
                try
                {
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = new LookupViewModel().FromServiceModelList(await service.GetAllLookupAsync());
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

        /// <summary>
        /// Get all LookUp Types
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetAllLookUpTypes")]
        public static async Task<HttpResponseMessage> GetAllLookUpTypes([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "AllLookUpTypes")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all LookUp Types from service"))
            {

                logger.LogRequest(req);
                var LookUpType = req.ToDataSourceRequest();
                ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                LookupService service = LookupService.GetInstance(logger);
                DataSourceResult result = await service.GetAllLookUpTypes(LookUpType);
                if (!result.Total.Equals(0))
                {
                    result.Data = new LookupViewModel().FromServiceModelList((IEnumerable<LookupModel>)result.Data);
                }
                respVM.data = result;
                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                logger.LogResponse(resp);
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                return resp;
            }
        }

        // UPDATE Venue Type Values Method
        [FunctionName("UpdateLookupTypeValues")]
        public static async Task<HttpResponseMessage> UpdateLookupTypeValues([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "UpdateLookupTypeValues/{type}")]HttpRequestMessage req, [Logger] NatLogger logger, string type)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Lookup Type Values"))
            {
                try
                {
                    logger.LogRequest(req);
                    var lookup = await req.Content.ReadAsAsync<IEnumerable<LookupViewModel>>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = await service.UpdateLookupValuesByLookupTypeAsync(new LookupViewModel().ToServiceModelList(lookup));
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

        #endregion

            #region  Methods for Configuration

            /// <summary>
            /// Get Configuration by the given Key
            /// </summary>
            /// <param name="req"></param>
            /// <param name="logger"></param>
            /// <param name="key">Url parameter for Configuration Key</param>
            /// <returns></returns>
        [FunctionName("GetConfigurationByKey")]
        public static async Task<HttpResponseMessage> GetConfigurationByKeyAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "configuration/{key}")] HttpRequestMessage req, [Logger] NatLogger logger, string key)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope(" Get Configuration by the given Key"))
            {
                try
                {
                    logger.LogRequest(req);
                    //logger.LogInformation("Get Configuration from service");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = new ConfigurationViewModel().FromServiceModel(await service.GetConfigurationByKeyAsync(key));
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

        /// <summary>
        /// Get all Configuration
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetAllConfiguration")]
        public static async Task<HttpResponseMessage> GetAllConfigurationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "configuration")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope(" Get all Configuration"))
            {
                try
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Get all Configuration from service");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = new ConfigurationViewModel().FromServiceModelList(await service.GetAllConfigurationAsync());
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


          /// <summary>
        /// Get all User Editable Configurations for list Configurations
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("GetAllConfigurationByUserEditable")]
        public static async Task<HttpResponseMessage> GetAllConfigurationByUserEditable([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "ConfigurationByUserEditable")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all User Editable Configuration from service"))
            {

                logger.LogRequest(req);
                var configuration = req.ToDataSourceRequest();
                ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                LookupService service = LookupService.GetInstance(logger);
                DataSourceResult result = await service.GetAllConfigurationByUserEditableAsync(configuration);
                if (!result.Total.Equals(0))
                {
                    result.Data = new ConfigurationViewModel().FromServiceModelList((IEnumerable<ConfigurationModel>)result.Data);
                }
                respVM.data = result;
                var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                logger.LogResponse(resp);
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                return resp;
            }
        }

        // UPDATE Configuration Information Method
        [FunctionName("UpdateConfigurationFunction")]
        public static async Task<HttpResponseMessage> UpdateConfigurationFunction([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "UpdateConfiguration/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    var configuration = await req.Content.ReadAsAsync<ConfigurationViewModel>();
                    ResponseViewModel<bool> respVM = new ResponseViewModel<bool>();
                    LookupService service = LookupService.GetInstance(logger);
                    respVM.data = await service.UpdateConfigurationAsync(new ConfigurationViewModel().ToServiceModel(configuration));
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

        #endregion
    }
}
