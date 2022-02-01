using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class UserViewModel : BaseAutoViewModel<UserModel, UserViewModel>
	{
        public int? PlannerId { get; set; }
		public Int64 UserId { get; set; }
		public Nullable<Int64> TenantId { get; set; }
		public Nullable<Int64> ParentUserId { get; set; }
        [Required]
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
        public Nullable<Int64> ReportingManager { get; set; }
        public String ReferenceTypeLKP { get; set; }
        [Required]
        public String Password { get; set; }
        [Required]
        public String RoleCode { get; set; }
        public Boolean PasswordChanged { get; set; }
        public ICollection<RoleViewModel> userroles { get; set; }
        public ICollection<RoleViewModel> Roles
        {
            get
            {
                if (NatAusUserRoleMapping != null && NatAusUserRoleMapping.Count > 0)
                {
                    var roles = new List<RoleViewModel>();
                    Parallel.ForEach(NatAusUserRoleMapping, (role) => {
                        roles.Add(role.NatAusRole);
                    });
                    return roles;
                }
                return null;
            }
        }
        [Complex]        
        public ICollection<UserRoleMappingViewModel> NatAusUserRoleMapping { get; set; }

        [Complex]
        public ICollection<ExternalIdentityViewModel> NatAusExternalIdentity { get; set; }
        [Complex]
        public ICollection<PrivilegeModel> NatAusPrivilege { get; set; }

        [Complex]
        public ICollection<NotificationPreferenceModel> NatAusNotificationPreference { get; set; }

        [Complex]
        public List<UserLocationMappingViewModel> NatAusUserLocationMapping { get; set; }
    }
}
