using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class RolePrivilegeMappingViewModel : BaseAutoViewModel<RolePrivilegeMappingModel, RolePrivilegeMappingViewModel>
	{
        public long RolePrivilegeMappingId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public long RoleId { get; set; }
        public long PrivilegeId { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        [Complex]
        public PrivilegeViewModel NatAusPrivilege { get; set; }
        public RoleViewModel NatAusRole { get; set; }
 
	}
}
