using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nat.Core.QueueMessage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Notification
{
    public class Notification
    {

        public async Task<String> SendEmail(NotificationQueueMessage obj)
        {
            try
            {
                var storageConfig = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("notificationqueue");
                await queue.CreateIfNotExistsAsync();
                CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(obj));
                await queue.AddMessageAsync(message);

                return "Email Sent to the Recipent";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<String> SendSMS(NotificationQueueMessage obj)
        {
            try
            {
                var storageConfig = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("smsnotificationqueue");
                await queue.CreateIfNotExistsAsync();
                CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(obj));
                await queue.AddMessageAsync(message);

                return "SMS Sent to the Recipent";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
