using Nat.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.QueueMessage
{
    public class LoggerQueueMessage
    {
        public string LogLevel;
        public string ServiceName;
        public string Message;
        public string UserId;
        public DateTime Time;
        public System.Exception Exception;
        public List<LogScope> Scope;

        public LoggerQueueMessage(string LogLevel, string ServiceName, string UserId, string Message, DateTime Time, System.Exception Exception, List<LogScope> Scope)
        {
            this.LogLevel = LogLevel;
            this.ServiceName = ServiceName;
            this.UserId = UserId;
            this.Message = Message;
            this.Time = Time;
            this.Exception = Exception;
            this.Scope = Scope;
        }
    }
}
