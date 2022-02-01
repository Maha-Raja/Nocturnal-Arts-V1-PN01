using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class PrivilegeViewModel : BaseAutoViewModel<PrivilegeModel, PrivilegeViewModel>
	{
        public long PrivilegeId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string PrivilegeName { get; set; }
        public string PrivilegeDescription { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        [Complex]
        public ICollection<RolePrivilegeMappingViewModel> NatAusRolePrivilegeMapping { get; set; }
	}
}
