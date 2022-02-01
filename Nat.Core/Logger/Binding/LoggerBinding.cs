using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Logger
{
    class LoggerBinding : IBinding
    {
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            //var request = context.BindingData["req"] as DefaultHttpRequest;

            // Get the configuration files for the OAuth token issuer
            var queueName = Environment.GetEnvironmentVariable("LoggingQueue") ?? "natloggingqueue";
            var serviceName = Environment.GetEnvironmentVariable("ServiceName");
            var storageConfig = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            return Task.FromResult<IValueProvider>(new LoggerValueProvider(queueName, serviceName, CloudStorageAccount.Parse(storageConfig)));
        }
        public bool FromAttribute => true;
        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) => null;
        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}
