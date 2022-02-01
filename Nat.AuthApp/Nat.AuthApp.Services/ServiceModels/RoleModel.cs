using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class RoleModel : BaseServiceModel<NAT_AUS_Role, RoleModel>, IObjectState
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
        public ICollection<Int64> MappedUserIds { get; set; }
        public ICollection<UserRoleMappingModel> NatAusUserRoleMapping { get; set; }
        [Complex]
        public ICollection<RolePrivilegeMappingModel> NatAusRolePrivilegeMapping { get; set; }
        public ObjectState ObjectState { get; set; }
	}
}
