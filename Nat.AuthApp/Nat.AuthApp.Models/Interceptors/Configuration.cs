using System;
using System.Data.Entity;
using Nat.Core;
using Nat.Core.EF6.Interceptors;
using Nat.Core.Interceptors;

namespace Nat.AuthApp.Interceptors
{
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            AddInterceptor(new CommandTreeInterceptor());
            AddInterceptor(new UtcDateTimeConvertingDbCommandInterceptor());
        }
    }
}