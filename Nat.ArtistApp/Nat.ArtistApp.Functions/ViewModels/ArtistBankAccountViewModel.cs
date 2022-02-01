using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistBankAccountViewModel : BaseAutoViewModel<ArtistBankAccountModel, ArtistBankAccountViewModel>
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
        public string BankTypeLKPId { get; set; }
		public string InstitutionCode { get; set; }

		public ArtistViewModel NatAsArtist { get; set; }
		[Complex]
		public ArtistAddressViewModel NatAsArtistAddress { get; set; }
	}
}
