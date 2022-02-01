using Nat.Core.Http;
using Nat.Common.Constants;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System;

namespace Nat.Core.BaseModelClass
{
    public class ResponseViewModel {
        public static HttpResponseMessage CreateErrorResponse(System.Exception ex)
        {
            var resp = new ResponseViewModel<object>();
            resp.status.code = (int)HttpStatusCode.InternalServerError;
            resp.exception = ex;
            resp.status.message = ex.Message;
            resp.status.description = Constants.GENERAL_ERROR_MESSAGE; 
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new JsonContent(resp);
            return response;
        }

        public static HttpResponseMessage CreateUnauthorizedErrorResponse(System.Exception ex)
        {
            var resp = new ResponseViewModel<object>();
            resp.status.code = (int)HttpStatusCode.Unauthorized;
            resp.exception = ex;
            resp.status.message = Constants.UNAUTHORISED_ERROR_MESSAGE;
            resp.status.description = ex.Message;

            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new JsonContent(resp);
            return response;
        }
    }
    public class ResponseViewModel<TViewModel> : ResponseViewModel
    {
        public Status status;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Navigation navigation;

        public TViewModel data;

        public object exception;

        public ResponseViewModel(TViewModel viewModel)
        {
            this.data = viewModel;
            this.status = new Status((int)HttpStatusCode.OK, Constants.GENERAL_SUCCESS_MESSAGE, "");
        }

        public ResponseViewModel()
        {
            this.status = new Status((int)HttpStatusCode.OK, Constants.GENERAL_SUCCESS_MESSAGE, "");
        }

        public void AddNavigation(string prevLink, string nextLink, int totalPages, int totalCount)
        {
            this.navigation = new Navigation(prevLink, nextLink, totalPages, totalCount);
        }

        public void AddStatus(int code, string message, string description)
        {
            this.status = new Status(code, message, description);
        }

        public HttpResponseMessage ToHttpResponseMessage(HttpStatusCode statusCode)
        {
            this.status.code = (int)statusCode;
            var httpResp = new HttpResponseMessage(statusCode)
            {
                Content = new JsonContent(this),
            };
            //httpResp.Headers.Add("Access-Control-Allow-Origin", Environment.GetEnvironmentVariable("Cors.Access-Control-Allow-Origin"));
            return httpResp;
        }

        public class Status
        {
            public Status()
            {
            }

            public Status(int code, string message, string description)
            {
                this.code = code;
                this.message = message;
                this.description = description;
            }

            public int code { get; set; }

            [JsonIgnore]
            public bool IsSuccessStatusCode
            {
                get { return code >= 200 && code <= 299; }
            }

            public string message { get; set; }

            public string description { get; set; }
        }

        public class Navigation
        {
            public Navigation(string prevLink, string nextLink, int totalPages, int totalCount)
            {
                this.prevLink = prevLink;
                this.nextLink = nextLink;
                this.totalPages = totalPages;
                this.totalCount = totalCount;
            }

            public string prevLink { get; set; }

            public string nextLink { get; set; }

            public int totalPages { get; set; }

            public int totalCount { get; set; }
        }

    }
}
