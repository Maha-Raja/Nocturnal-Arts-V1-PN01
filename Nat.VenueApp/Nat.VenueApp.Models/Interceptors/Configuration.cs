using System.Data.Entity;
using Nat.Core.EF6.Interceptors;
using Nat.Core.Interceptors;

namespace Nat.VenueApp.Models.Interceptor
{
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            AddInterceptor(new CommandTreeInterceptor());
            //AddInterceptor(new UtcDateTimeConvertingDbCommandInterceptor());
        }
    }
}