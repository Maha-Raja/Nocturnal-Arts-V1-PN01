using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class UsersVWViewModel : BaseAutoViewModel<UsersVWModel, UsersVWViewModel>
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
	}
}
