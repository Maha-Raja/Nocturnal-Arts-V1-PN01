using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Logger
{
    class LoggerValueProvider : IValueProvider
    {
        public Type Type => typeof(NatLogger);

        //private readonly HttpRequest _request;
        private readonly string _queueName;
        private readonly string _serviceName;
        private readonly CloudStorageAccount _storageAccount;

        public LoggerValueProvider(string queueName, string serviceName, CloudStorageAccount storageAccount)
        {
            //_request = request;
            _queueName = queueName;
            _serviceName = serviceName;
            _storageAccount = storageAccount;
        }

        public Task<object> GetValueAsync()
        {
            return Task.FromResult<object>(new LoggerProvider(_queueName, _storageAccount).CreateLogger(_serviceName));
        }

        public string ToInvokeString()
        {
            return string.Empty;
        }
    }
}
