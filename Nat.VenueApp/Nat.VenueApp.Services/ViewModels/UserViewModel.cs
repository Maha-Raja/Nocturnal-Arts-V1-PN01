using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Services.ViewModels
{
    class UserViewModel
    {
        public Int64 UserId { get; set; }
        public Nullable<Int64> TenantId { get; set; }
        public Nullable<Int64> ParentUserId { get; set; }
        public String UserName { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public String UserImageURL { get; set; }
        public String ThumbnailImageURL { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String Password { get; set; }
        public String PasswordHash { get; set; }
        public String PasswordSalt { get; set; }
        public Nullable<long> ReferenceId { get; set; }
        public String ReferenceTypeLKP { get; set; }
        public String RoleCode { get; set; }
        [Complex]
        public List<UserLocationMappingViewModel> NatAusUserLocationMapping { get; set; }
    }
}
