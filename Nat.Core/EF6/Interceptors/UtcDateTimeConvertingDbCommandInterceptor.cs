using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace Nat.Core.EF6.Interceptors
{
    public class UtcDateTimeConvertingDbCommandInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuted(command, interceptionContext);
            if (interceptionContext.Result != null && interceptionContext.Exception == null)
            {
                interceptionContext.Result = new UtcDateTimeConvertingDbDataReader(interceptionContext.Result);
            }
        }   
    }
}
