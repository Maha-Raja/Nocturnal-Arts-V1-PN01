using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nat.Core.QueueMessage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoggingService
{
    public static class Log
    {
        public static object ConfigurationManager { get; private set; }

        [FunctionName("Log")]
        public static void Run([QueueTrigger("natloggingqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            
            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string containerString = Environment.GetEnvironmentVariable("LoggingContainer");

            CloudStorageAccount storage = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudBlobClient client = storage.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerString);
            CloudAppendBlob blob = container.GetAppendBlobReference(fileName);
            if (!blob.Exists())
                blob.CreateOrReplace();

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(myQueueItem);
                sw.Flush();
                stream.Position = 0;
                blob.AppendFromStream(stream);
            }
        }
    }
}
