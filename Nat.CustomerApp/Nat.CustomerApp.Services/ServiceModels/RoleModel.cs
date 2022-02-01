using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.Core.ViewModels;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class RoleModel
	{
		public Int64 RoleId { get; set; }
		public Nullable<Int64> TenantId { get; set; }
		public String RoleCode { get; set; }
		public String RoleName { get; set; }
		public String RoleDescription { get; set; }
		public String RoleType { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
    }
}
