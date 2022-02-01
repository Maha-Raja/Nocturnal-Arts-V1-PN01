using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Logger
{
    public class LogScope
    {
        private readonly string _id;
        public string Id { get { return _id; } }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string LogContext { get; set; }


        public LogScope(string className, string methodName, LogContext logContext)
        {
            this._id = Guid.NewGuid().ToString();
            this.ClassName = className;
            this.MethodName = methodName;
            this.LogContext = Enum.GetName(typeof(LogContext),logContext);
        }
    }

    public class LogScope<TState> : LogScope
    {
        public TState State { get; set; }

        public LogScope(string className, string methodName, LogContext logContext, TState state)
            :base(className,methodName,logContext)
        {
            this.State = state;
        }
    }
}
