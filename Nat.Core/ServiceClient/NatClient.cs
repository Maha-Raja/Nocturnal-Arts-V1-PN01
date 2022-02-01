using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nat.Core.BaseModelClass;
using Nat.Core.Http;
using Newtonsoft.Json;

namespace Nat.Core.ServiceClient
{
    public class NatClient
    {
        private const string CONFIG_PROPERTY_POSTFIX = "BaseUrl";
        private static string token;

        public string Token { get => token; set => token = value; }

        public enum Method
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum Service
        {
            NotificationService,
            LoggingService,
            AuthService,
            EventService,
            PlannerService,
            LookupService,
            LocationService,
            VenueService,
            CustomerService,
            ArtistService,
            PaintingService,
            TicketBookingService,
            SuppliesService,
            FinancialService
        }

        public static void SetToken (string reqToken)
        {
            token = reqToken;
        }
        public static async Task<ResponseViewModel<T>> ReadAsyncWithHeaders<T>(Method method, Service service, string route, string queryParameters = "", object requestBody = null)
        {
            HttpClient httpClient = new HttpClient();
            if(token!=null)
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token);
            return await CallRESTService<T>(method, service, route, queryParameters, requestBody, httpClient);
        }

            public static async Task<ResponseViewModel<T>> ReadAsync<T>(Method method, Service service, string route, string queryParameters = "", object requestBody = null)
        {
            HttpClient httpClient = new HttpClient();
            return await CallRESTService<T>(method, service, route, queryParameters, requestBody, httpClient);
        }

        private static async Task<ResponseViewModel<T>> CallRESTService<T>(Method method, Service service, string route, string queryParameters, object requestBody, HttpClient httpClient)
        {
            HttpResponseMessage httpResponse;
            JsonContent content;
            var apiUrl = GetApiUrl(service, route);
            apiUrl = apiUrl + (queryParameters != "" ? "?" + queryParameters : "");
            switch (method)
            {
                case Method.GET:
                    httpResponse = await httpClient.GetAsync(apiUrl);
                    break;
                case Method.POST:
                    content = new JsonContent(requestBody);
                    httpResponse = await httpClient.PostAsync(apiUrl, content);
                    break;
                case Method.PUT:
                    content = new JsonContent(requestBody);
                    httpResponse = await httpClient.PutAsync(apiUrl, content);
                    break;
                case Method.DELETE:
                    httpResponse = await httpClient.DeleteAsync(apiUrl);
                    break;
                default:
                    httpResponse = await httpClient.GetAsync(apiUrl);
                    break;
            }
            return await httpResponse.ToResponseViewModelAsync<T>();
        }

        public static async Task Enqueue<T>(String queueNameConfig, T message)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(Environment.GetEnvironmentVariable(queueNameConfig));
            await queue.CreateIfNotExistsAsync();
            CloudQueueMessage messageObj = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            await queue.AddMessageAsync(messageObj);
        }

        private static string GetServiceUrl(Service service)
        {
            return Environment.GetEnvironmentVariable(Enum.GetName(typeof(Service), service) + CONFIG_PROPERTY_POSTFIX);
        }

        private static string GetApiUrl(Service service, string route)
        {
            return GetServiceUrl(service) + route;
            //return GetServiceUrl(service);
            //return "http://localhost:7071/api/Customerslov";
        }

    }
}
