using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Nat.Core.ServiceClient;
using Nat.Core.Exception;


namespace Nat.Core.Authentication
{
    public class Auth
    {
        public static async Task<UserModel> Validate(HttpRequestMessage request)
        {
            if (request.Headers.Contains("Authorization"))
            {
                string UserToken = request.Headers.GetValues("Authorization").FirstOrDefault();
                var Token = new
                { Token = UserToken };
                var Userdata = await NatClient.ReadAsync<UserModel>(NatClient.Method.POST, NatClient.Service.AuthService, "ValidateUser", requestBody: Token);
                if (Userdata.status.IsSuccessStatusCode && Userdata.data != null)
                {
                    return Userdata.data;
                }
                else
                {
                    throw new AuthenticationException("User cannot validate");
                }
            } 
            else
            {
                throw new AuthenticationException("Auth header not found");
            }
        }

        public class UserModel
        {
            public Int64 UserId { get; set; }
            public Nullable<Int64> TenantId { get; set; }
            public Nullable<Int64> ParentUserId { get; set; }
            public String UserName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
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
            public String ReferenceTypeLKP { get; set; }
            public String Password { get; set; }
            public String RoleCode { get; set; }
            public ICollection<RoleModel> Roles
            { get; set; }
            public Nullable<Int64> ReportingManager { get; set; }
            public List<UserLocationMappingViewModel> NatAusUserLocationMapping { get; set; }
        }

        public class UserLocationMappingViewModel
        {
            public int UserLocationMappingId { get; set; }
            public int UserId { get; set; }
            public string LocationCode { get; set; }
            public string LocationName { get; set; }
            public Nullable<bool> ActiveFlag { get; set; }
            public Nullable<System.DateTime> EffectiveStartDate { get; set; }
            public Nullable<System.DateTime> EffectiveEndDate { get; set; }
            public string CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string LastUpdatedBy { get; set; }
            public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        }

        public class RoleModel
        {
            public Int64 RoleId { get; set; }
            public Nullable<Int64> TenantId { get; set; }
            public String RoleCode { get; set; }
            public String RoleName { get; set; }
            public String RoleDescription { get; set; }
            public String RoleType { get; set; }
        }


        public class TokenModel
        {
            public String Token { get; set; }
        }

    }
}