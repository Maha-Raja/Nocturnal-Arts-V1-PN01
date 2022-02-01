using Microsoft.WindowsAzure.Storage;
using Nat.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Logger
{
    public class LoggerProvider
    {
        private readonly string _queueName;
        private readonly CloudStorageAccount _storageAccount;

        public LoggerProvider(string queueName, CloudStorageAccount storageAccount)
        {
            _queueName = queueName;
            _storageAccount = storageAccount;
        }

        public NatLogger CreateLogger(string serviceName)
        {
            return new NatLogger(serviceName, _queueName, _storageAccount);
        }

        public void Dispose()
        {
        }
    }
}
