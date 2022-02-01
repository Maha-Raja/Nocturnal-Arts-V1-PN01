using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class ExternalIdentityModel : BaseServiceModel<NAT_AUS_External_Identity, ExternalIdentityModel>, IObjectState
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
		public ObjectState ObjectState { get; set; }
        public UserModel NatAusUser { get; set; }
    }
}
