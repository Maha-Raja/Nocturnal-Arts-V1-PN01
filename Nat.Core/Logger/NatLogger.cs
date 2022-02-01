using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nat.Core.QueueMessage;
using Newtonsoft.Json;

namespace Nat.Core.Logger
{
    //
    // Summary:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        //
        // Summary:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,
        //
        // Summary:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,
        //
        // Summary:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,
        //
        // Summary:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,
        //
        // Summary:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,
        //
        // Summary:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,
        //
        // Summary:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None = 6
    }

    public enum LogContext
    {
        Function = 0,
        Service = 1
    }

    public class NatLogger
    {
        private readonly string _serviceName;
        private readonly string _queueName;
        private readonly CloudStorageAccount _storageAccount;
        private List<LogScope> _loggingContext;
        private string _userId;

        public NatLogger(string serviceName, string queueName, CloudStorageAccount storageAccount)
        {
            _queueName = queueName;
            _serviceName = serviceName;
            _storageAccount = storageAccount;
            _loggingContext = new List<LogScope>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void SetLoggedInUser(string UserId)
        {
            this._userId = UserId;
        }

        public void Log<TState>(LogLevel logLevel, string eventId, TState state, System.Exception exception, Func<TState, System.Exception, string> formatter)
        {
            try
            {
                RecordMsg(logLevel, eventId, state, exception, formatter);
            }
            catch (System.Exception ex)
            {
                RecordMsg(logLevel, eventId, state, ex, formatter);
            }
        }

        private void RecordMsg<TState>(LogLevel logLevel, string eventId, TState state, System.Exception exception, Func<TState, System.Exception, string> formatter)
        {
            LoggerQueueMessage logMessage = new LoggerQueueMessage(
                Enum.GetName(typeof(LogLevel), logLevel),
                _serviceName,
                _userId,
                formatter(state, exception),
                DateTime.Now,
                exception,
                _loggingContext
                );
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(_queueName);
            queue.CreateIfNotExists();
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(logMessage));
            queue.AddMessage(message);
        }

        public IDisposable BeginScope<TState>(TState state, LogContext context)
        {
            var s = state as IDisposable;
            var frame = new StackFrame(2, false);
            var className = "";// frame.GetMethod().DeclaringType.FullName;
            var methodName = frame.GetMethod().Name;
            var scope = new LogScope<TState>(className, methodName, context, state);
            _loggingContext.Add(scope);
            return s;
        }
    }
}
