using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels.ViewModel
{
	public class AuthViewModel
	{
		public Int64 UserId { get; set; }
		public Nullable<Int64> TenantId { get; set; }
		public Nullable<Int64> ParentUserId { get; set; }
		public String UserName { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public Nullable<bool> PhoneVerified { get; set; }
		public Nullable<bool> EmailVerified { get; set; }
		public String FirstName { get; set; }
		public String MiddleName { get; set; }
		public String LastName { get; set; }
		public String UserImageURL { get; set; }
		public String ThumbnailImageURL { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<long> ReferenceId { get; set; }
		public Boolean Verified { get; set; }
		public String ReferenceTypeLKP { get; set; }
		public String Password { get; set; }
		public String RoleCode { get; set; }
	}
}
