using System;
using System.Data.Entity;
using PaintingDataAccessLayer.DAL;
using Nat.Core;
using Nat.Core.Interceptors;
using Nat.Core.EF6.Interceptors;

namespace PaintingDataAccessLayer.DAL
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