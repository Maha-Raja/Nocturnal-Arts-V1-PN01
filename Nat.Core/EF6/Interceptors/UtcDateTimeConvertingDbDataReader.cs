using System;
using System.Data.Common;

namespace Nat.Core.EF6.Interceptors
{
    public class UtcDateTimeConvertingDbDataReader : DelegatingDbDataReader
    {
        public UtcDateTimeConvertingDbDataReader(DbDataReader source) : base(source) { }
        public override DateTime GetDateTime(int ordinal)
        {
            return DateTime.SpecifyKind(base.GetDateTime(ordinal), DateTimeKind.Utc);
        }
    }
}
