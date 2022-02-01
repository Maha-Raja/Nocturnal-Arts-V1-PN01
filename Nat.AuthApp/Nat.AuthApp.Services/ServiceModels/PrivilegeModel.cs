using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class PrivilegeModel : BaseServiceModel<NAT_AUS_Privilege, PrivilegeModel>, IObjectState
    {
        public long PrivilegeId { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string PrivilegeName { get; set; }
        public string PrivilegeDescription { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public ICollection<RolePrivilegeMappingModel> NatAusRolePrivilegeMapping { get; set; }
        public ObjectState ObjectState { get; set; }
	}
}
