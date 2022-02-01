using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class UserRoleMappingModel : BaseServiceModel<NAT_AUS_User_Role_Mapping, UserRoleMappingModel>, IObjectState
	{
		public Int64 UserRoleMappingId { get; set; }
		public Int64 RoleId { get; set; }
		public Int64 UserId { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
        [Complex]
        public RoleModel NatAusRole { get; set; }
        
        public UserModel NatAusUser { get; set; }

        public ObjectState ObjectState { get; set; }
	}
}
