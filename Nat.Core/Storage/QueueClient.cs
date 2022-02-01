using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Storage
{
    public class QueueClient
    {
        public static async Task AddToQueue(string queueName, object obj)
        {
            var msg = JsonConvert.SerializeObject(obj);
            await AddToQueue(queueName, msg);
        }
        public static async Task AddToQueue(string queueName, string msg)
        {
            var storageConfig = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            CloudQueueMessage message = new CloudQueueMessage(msg);
            await queue.AddMessageAsync(message);
        }
    }
}
