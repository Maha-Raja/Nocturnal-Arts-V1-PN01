using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class RolePrivilegeMappingModel : BaseServiceModel<NAT_AUS_Role_Privilege_Mapping, RolePrivilegeMappingModel>, IObjectState
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
        public PrivilegeModel NatAusPrivilege { get; set; }
        public RoleModel NatAusRole { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
