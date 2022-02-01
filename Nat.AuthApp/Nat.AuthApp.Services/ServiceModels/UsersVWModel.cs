using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class UsersVWModel : BaseServiceModel<NAT_USERS_VW, UsersVWModel>, IObjectState
	{
		public Nullable<Int64> ParentUserId { get; set; }
		public String UserName { get; set; }
		public String FirstName { get; set; }
		public String MiddleName { get; set; }
		public String LastName { get; set; }
		public Nullable<Int64> UserId { get; set; }
		public String UserImageURL { get; set; }
		public String ReferenceTypeLKP { get; set; }
		public Nullable<Int64> ReferenceId { get; set; }
		public String PhoneNumber { get; set; }
		public String Email { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String RoleName { get; set; }
		public String LocationCode { get; set; }
		public Int64 RoleId { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
