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
using Nat.Core.Authentication;
using Nat.Core.Http.Extension;
using Nat.CustomerApp.Services.Services;
using Nat.CustomerApp.Functions.ViewModel;
using Nat.CustomerApp.Services.ViewModels;
using Nat.Common.Constants;

namespace Nat.CustomerApp.Functions
{
    public class CustomerFunction
    {

        [FunctionName("GetAllCustomerFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customers")]HttpRequestMessage req, [Logger] NatLogger logger)
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
                Auth.UserModel UserModel = new Auth.UserModel();
                UserModel = await Auth.Validate(req);
                using (logger.BeginFunctionScope("Get all customer"))
                {
                    logger.LogRequest(req);
                    logger.LogInformation("Test message");
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = new CustomerViewModel().FromServiceModelList(service.GetAllCustomer());
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
                    resp.Headers.Add("Access-Control-Allow-Origin", "*");
                    return resp;
                }
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
        //GET Customer by ID Method
        [FunctionName("GetCustomerByIDFunction")]
        public async static Task<HttpResponseMessage> GetCustomerByIDFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customers/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Customer by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<CustomerViewModel> respVM = new ResponseViewModel<CustomerViewModel>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = new CustomerViewModel().FromServiceModel(await service.GetByIdCustomer(int.Parse(id)));
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

        //GET Followers count of artist and venue
        [FunctionName("GetFollowersCount")]
        public async static Task<HttpResponseMessage> GetFollowersCount([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customers/FollowingCount/{options}/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id, string options)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get following count of artist and venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<Int32> respVM = new ResponseViewModel<Int32>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = service.GetFollowersCount(id, options);
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

        //GET Likes count of event
        [FunctionName("GetEventLikesCount")]
        public async static Task<HttpResponseMessage> GetEventLikesCount([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "EventsLikes/{eventCode}")]HttpRequestMessage req, [Logger] NatLogger logger, string eventCode)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get following count of artist and venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<Int32> respVM = new ResponseViewModel<Int32>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = service.GetEventLikesCount(eventCode);
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




        //GET Customer Events
        [FunctionName("GetCustomerEventsFunction")]
        public async static Task<HttpResponseMessage> GetCustomerEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customer/{id}/events")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Customer event by id"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<CustomerBookedEventViewModel>> respVM = new ResponseViewModel<IEnumerable<CustomerBookedEventViewModel>>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = await service.GetCustomerEvents(int.Parse(id));
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


        //GET Customer Liked Events
        [FunctionName("GetCustomerLikedEventsFunction")]
        public async static Task<HttpResponseMessage> GetCustomerLikedEventsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customer/{id}/likedevents")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Customer liked events"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<CustomerLikedEventsViewModel>> respVM = new ResponseViewModel<IEnumerable<CustomerLikedEventsViewModel>>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = new CustomerLikedEventsViewModel().FromServiceModelList(await service.GetCustomerLikedEvents(int.Parse(id)));
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


        //POST Customer Liked Event
        [FunctionName("CustomerLikeEventFunction")]
        public async static Task<HttpResponseMessage> CustomerLikeEventFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Customer/{CustomerId}/LikeEvent/{EventCode}/{LikeStatus}")]HttpRequestMessage req, [Logger] NatLogger logger, int CustomerId, String EventCode, string LikeStatus)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Post Customer like event"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    CustomerLikedEventsViewModel obj = new CustomerLikedEventsViewModel();
                    obj.CustomerId = CustomerId;
                    obj.EventCode = EventCode;
                    var likeStatus = Constants.LikeStatus[LikeStatus];
                    CustomerLikedEventService service = CustomerLikedEventService.GetInstance(logger);

                    if (likeStatus == "LIKE")
                    {
                        respVM.data = await service.LikeEventasync(new CustomerLikedEventsViewModel().ToServiceModel(obj));
                    }
                    else if (likeStatus == "UNLIKE")
                    {
                        respVM.data = await service.UnlikeEventasync(new CustomerLikedEventsViewModel().ToServiceModel(obj));
                    }
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





        //GET Customer following 
        [FunctionName("GetCustomerFollowingsFunction")]
        public async static Task<HttpResponseMessage> GetCustomerFollowingsFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customer/{id}/{options}/following")]HttpRequestMessage req, [Logger] NatLogger logger, string id,string options)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get Venues and Artists followed by a customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<IEnumerable<CustomerFollowingViewModel>> respVM = new ResponseViewModel<IEnumerable<CustomerFollowingViewModel>>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = new CustomerFollowingViewModel().FromServiceModelList(await service.GetCustomerFollowings(int.Parse(id), options));
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
        /// Get list of all Customer
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="logger">Logger instance</param>
        /// <returns></returns>
        [FunctionName("GetListCustomerFunction")]
        public static async Task<HttpResponseMessage> GetListCustomerFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Customers/Search")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get all Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Customer = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    DataSourceResult result = service.GetAllCustomerList(Customer);
                    result.Data = new CustomerViewModel().FromServiceModelList((IEnumerable<CustomerModel>)result.Data);
                    respVM.data = result;
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


        // INSERT Customer Information Method
        [FunctionName("SaveCustomerFunction")]
        public static async Task<HttpResponseMessage> SaveCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SaveCustomers")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<CustomerViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var Artist = body.Value;
                        CustomerService service = CustomerService.GetInstance(logger);
                        respVM.data = await service.CreateCustomer(new CustomerViewModel() { }.ToServiceModel(Artist));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
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


        // INSERT Customer Information Method
        [FunctionName("SaveFacebookCustomerFunction")]
        public static async Task<HttpResponseMessage> SaveFacebookCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SaveFacebookCustomer")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Social Media Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var CustomerToken = body.Value;
                        CustomerService service = CustomerService.GetInstance(logger);
                        respVM.data = await service.SocialMediaCreateCustomer(new TokenViewModel() { }.ToServiceModel(CustomerToken));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
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

        // INSERT Customer Information Method
        [FunctionName("SaveGoogleCustomerFunction")]
        public static async Task<HttpResponseMessage> SaveGoogleCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SaveGoogleCustomer")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Google Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<TokenViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var CustomerToken = body.Value;
                        CustomerService service = CustomerService.GetInstance(logger);
                        respVM.data = await service.SocialMediaCreateGoogleCustomer(new TokenViewModel() { }.ToServiceModel(CustomerToken));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
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


        // INSERT Customer Information Method
        [FunctionName("SaveInstagramCustomer")]
        public static async Task<HttpResponseMessage> SaveInstagramCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "SaveInstagramCustomer")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Save Instagram Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<InstagramViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var InstagramCode = body.Value;
                        CustomerService service = CustomerService.GetInstance(logger);
                        respVM.data = service.SocialMediaCreateInstagramCustomer(new InstagramViewModel() { }.ToServiceModel(InstagramCode));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
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



        // UPDATE Customer Information Method
        [FunctionName("UpdateCustomerFunction")]
        public static async Task<HttpResponseMessage> UpdateCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "Customer")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Update Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    var Customer = await req.Content.ReadAsAsync<CustomerViewModel>();
                    ResponseViewModel<CustomerViewModel> respVM = new ResponseViewModel<CustomerViewModel>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = new CustomerViewModel().FromServiceModel(await service.UpdateCustomerAsync(new CustomerViewModel().ToServiceModel(Customer)));
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

        //DELETE Customer Information Method
        [FunctionName("DeleteCustomerFunction")]
        public static async Task<HttpResponseMessage> DeleteCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Customers/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            throw new NotImplementedException();
        }


        //Activate Customer Information Method
        [FunctionName("ActivateCustomerFunction")]
        public static async Task<HttpResponseMessage> ActivateCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Customers/Activate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Activate Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    service.ActivateCustomer(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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


        //Deactivate Customer Information Method
        [FunctionName("DeactivateCustomerFunction")]
        public static async Task<HttpResponseMessage> DeactivateCustomer_Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Customers/Deactivate/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, string id)
        {
            using (logger.BeginFunctionScope("Deactivate Customer"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<int> respVM = new ResponseViewModel<int>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    service.DeactivateCustomer(id);
                    var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                    logger.LogResponse(resp);
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

        [FunctionName("UploadImageFunction")]
        public static async Task<HttpResponseMessage> UploadImageFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Customer/UploadImage")] HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Store Image on Blob"))
            {
                try
                {
                    logger.LogRequest(req);
                    var file = await req.ReadFileAsync();
                    ResponseViewModel<string> respVM = new ResponseViewModel<string>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = await service.UploadImage(file.Content); //<-- Just uploading single file
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



        [FunctionName("FollowArtist")]
        public static async Task<HttpResponseMessage> FollowArtist([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Customer/Artist/{FollowStatus}/{CustomerId}/{ArtistId}")]HttpRequestMessage req, [Logger] NatLogger logger,string FollowStatus, int CustomerId, int ArtistId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("follow artist"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = await service.FollowArtistasync(CustomerId, ArtistId, FollowStatus);
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


        [FunctionName("FollowVenue")]
        public static async Task<HttpResponseMessage> FollowVenue([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "Customer/Venue/{FollowStatus}/{CustomerId}/{VenueId}")]HttpRequestMessage req, [Logger] NatLogger logger, string FollowStatus, int CustomerId, int VenueId)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("follow Venue"))
            {
                try
                {
                    logger.LogRequest(req);
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    respVM.data = await service.FollowVenueasync(FollowStatus, CustomerId, VenueId);
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



        [FunctionName("GetAllArtistFollowersFunction")]
        public static async Task<HttpResponseMessage> GetAllArtistFollowersFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Artist/Followers/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Followers of a specific Artist"))
            {
                try
                {


                    logger.LogRequest(req);
                    var requestt = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllArtistFollowers(requestt, id);
                    result.Data = new CustomerFollowingViewModel().FromServiceModelList((IEnumerable<CustomerFollowingModel>)result.Data);

                    respVM.data = result;
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



        [FunctionName("GetAllVenueFollowersFunction")]
        public static async Task<HttpResponseMessage> GetAllVenueFollowersFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "Venue/Followers/{id}")]HttpRequestMessage req, [Logger] NatLogger logger, int id)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }
            using (logger.BeginFunctionScope("Get All Followers of a specific Venue"))
            {
                try
                {


                    logger.LogRequest(req);
                    var requestt = req.ToDataSourceRequest();
                    ResponseViewModel<DataSourceResult> respVM = new ResponseViewModel<DataSourceResult>();
                    CustomerService service = CustomerService.GetInstance(logger);
                    DataSourceResult result = await service.GetAllVenueFollowers(requestt, id);
                    result.Data = new CustomerFollowingViewModel().FromServiceModelList((IEnumerable<CustomerFollowingModel>)result.Data);

                    respVM.data = result;
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



        [FunctionName("CustomerInquiry")]
        public static async Task<HttpResponseMessage> CustomerInquiry([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "CustomerInquiry")]HttpRequestMessage req, [Logger] NatLogger logger)
        {
            if (req.Method == HttpMethod.Options)
            {
                var resp = req.CreateResponse();
                resp.Headers.Add("Access-Control-Allow-Origin", "*");
                resp.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST");
                resp.Headers.Add("Access-Control-Allow-Headers", "*");
                return resp;
            }

            using (logger.BeginFunctionScope("Contact-us"))
            {
                try
                {
                    logger.LogRequest(req);
                    var body = await req.GetBodyAsync<CustomerInquiriesViewModel>();
                    ResponseViewModel<object> respVM = new ResponseViewModel<object>();
                    if (body.IsValid)
                    {
                        var customermessage = body.Value;
                        CustomerService service = CustomerService.GetInstance(logger);
                        respVM.data = await service.CreateCustomerInquiry(new CustomerInquiriesViewModel() { }.ToServiceModel(customermessage));
                        var resp = respVM.ToHttpResponseMessage(HttpStatusCode.OK);
                        logger.LogResponse(resp);
                        resp.Headers.Add("Access-Control-Allow-Origin", "*");
                        return resp;
                    }
                    else
                    {
                        respVM.data = body.ValidationResults;
                        return respVM.ToHttpResponseMessage(HttpStatusCode.BadRequest);
                    }
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
