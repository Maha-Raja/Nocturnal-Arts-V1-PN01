using System;
using System.Data.Entity;
using EventDataAccessLayer.DAL;
using Nat.Core;
using Nat.Core.EF6.Interceptors;
using Nat.Core.Interceptors;

namespace EventDataAccessLayer.DAL
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