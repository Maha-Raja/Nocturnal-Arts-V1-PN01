using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class ExternalIdentityViewModel : BaseAutoViewModel<ExternalIdentityModel, ExternalIdentityViewModel>
    {
        public Int64 ExternalIdentityId { get; set; }
        public String IdentityProviderLKP { get; set; }
        public String AccountId { get; set; }
        public String AccountObject { get; set; }
        public String StatusLKP { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public UserViewModel NatAusUser { get; set; }
    }
}