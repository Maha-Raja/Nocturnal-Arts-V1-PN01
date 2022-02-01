using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class RoleViewModel : BaseAutoViewModel<RoleModel, RoleViewModel>
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

        public ICollection<UserRoleMappingViewModel> NatAusUserRoleMapping { get; set; }

        [Complex]
        public ICollection<RolePrivilegeMappingViewModel> NatAusRolePrivilegeMapping { get; set; }
    }
}
