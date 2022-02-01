using System;
using System.Data.Entity;
using ArtistDataAccessLayer.DAL;
using Nat.Core;
using Nat.Core.EF6.Interceptors;
using Nat.Core.Interceptors;

namespace ArtistDataAccessLayer.DAL
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