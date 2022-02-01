using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class UserRoleMappingViewModel : BaseAutoViewModel<UserRoleMappingModel, UserRoleMappingViewModel>
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
        public RoleViewModel NatAusRole { get; set; }

        public UserViewModel NatAusUser { get; set; }
	}
}
