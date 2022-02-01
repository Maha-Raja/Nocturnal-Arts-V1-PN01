using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;



namespace SchedulingTimeTrigger
{
    public static class SchedulingTrigger
    {
        [FunctionName("SchedulingNotification")]
        public static  void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            string _Parameter = "";
            string _Guid = "";
            try
            {
                //Azure Connection Creation
                CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"));
                CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

                //Get Table Access
                CloudTable _Table = tableClient.GetTableReference("Schedule");


                // filter for Retrieving Notifiation
                string _DateStart = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.Now);                 //Start Date and Time
                string _DateEnd = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.LessThanOrEqual, DateTimeOffset.Now.AddMinutes(5));        //End Date and Time
                string _PartitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "TicketBookingNotification");                      //PartitionKey Equal to ("Notification Type")            
                string _IsActive = TableQuery.GenerateFilterCondition("IsActive", QueryComparisons.Equal, "N");                                                 //Notification should not be send yet.

                //Azure Filter Query
                string _Filter = TableQuery.CombineFilters(TableQuery.CombineFilters(
                                        TableQuery.CombineFilters(
                                            _DateStart,
                                            TableOperators.And,
                                            _DateEnd),
                                        TableOperators.And, _PartitionFilter), TableOperators.And, _IsActive);

                //Azure Query Execution
                TableQuery<Schedule> query = new TableQuery<Schedule>().Where(_Filter);
                IEnumerable<Schedule> ISchedule = _Table.ExecuteQuery(query);

                if (ISchedule != null)
                {
                    foreach (Schedule schedule in ISchedule)
                    {
                        _Parameter = schedule.Parameters;
                        _Guid = schedule.RowKey;

                        ////////Send Email Code 

                        ////////Send Email Code

                        //Update IsActive = "Y" after sending notification 
                        schedule.IsActive = "Y";
                        TableOperation tableOperation = TableOperation.Replace(schedule);
                        _Table.Execute(tableOperation);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }


        //HttpTrigger Function for Inserting Notification in Azure Stoarge Table
        [FunctionName("PostNotification")]
        public static async Task<HttpResponseMessage> PostRun([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, TraceWriter log)
        {
            HttpResponseMessage Response;
            try
            {


                //Azure Connection Creation
                CloudStorageAccount _StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"));
                CloudTableClient tableClient = _StorageAccount.CreateCloudTableClient();

                //Get Table Access
                CloudTable _Table = tableClient.GetTableReference("Schedule");


                var _schedule = await req.Content.ReadAsAsync<Schedule>();
                Schedule schedule = new Schedule();


                schedule.PartitionKey = _schedule.NotificationType;
                schedule.RowKey = Guid.NewGuid().ToString();
                schedule.Parameters = schedule.Parameters == "" ? null : schedule.Parameters;
                schedule.Date = _schedule.Date;
                schedule.IsActive = "N";

                //Insertion of data in table
                TableOperation tableOperation = TableOperation.Insert(schedule);
                _Table.Execute(tableOperation);
                Response = req.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Response = req.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
           return Response;
        }



        /// <summary>
        /// Test Method for calling hhtptrigger function
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        [FunctionName("TriggerHttp")]
        public static async void TriggerHttpRun([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, TraceWriter log)
        {
            HttpResponseMessage Response;
            try
            {
                ////////for calling another httptrigger function
                HttpClient client = new HttpClient();
                var url = "http://localhost:7071/api/PostNotification";
                //var response = await client.GetAsync(url);
                var _schedule = await req.Content.ReadAsAsync<Schedule>();
                var response = await client.PostAsJsonAsync(url, _schedule);
                ////////for calling another httptrigger function
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Response = req.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
 