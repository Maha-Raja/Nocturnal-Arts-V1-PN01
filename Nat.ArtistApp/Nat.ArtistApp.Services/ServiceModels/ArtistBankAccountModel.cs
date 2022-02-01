using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistBankAccountModel : BaseServiceModel<NAT_AS_Artist_Bank_Account, ArtistBankAccountModel>, IObjectState
	{
		public Int32 ArtistBankAccountId { get; set; }
		public Nullable<Int32> ArtistId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> BankId { get; set; }
		public string TransitNumber { get; set; }
		public string BankRoutingNumber { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
        public String BankLKPId { get; set; }
		public String BankCode { get; set; }
		public String BankAccountNumber { get; set; }
        public ArtistModel NatAsArtist { get; set; }
		public ObjectState ObjectState { get; set; }
		public Nullable<int> AddressID { get; set; }
		public string BankTypeLKPId { get; set; }
		public string InstitutionCode { get; set; }
		[Complex]
		public ArtistAddressModel NatAsArtistAddress { get; set; }
	}
}
