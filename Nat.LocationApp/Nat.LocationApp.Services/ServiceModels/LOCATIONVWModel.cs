using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.LocationApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.LocationApp.Services.ServiceModels
{
	public class LOCATIONVWModel : BaseServiceModel<NAT_LS_LOCATION_VW, LOCATIONVWModel>, IObjectState
	{
		public Int32 LocationId { get; set; }
		public String ParentLocationCode { get; set; }
		public String LocationName { get; set; }
		public String LocationDescription { get; set; }
		public String LocationShortCode { get; set; }
		public String LocationTypeLKP { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<Boolean> SalesTaxApplicableFlag { get; set; }
		public Nullable<Boolean> SalesTaxInclusiveFlag { get; set; }
		public Nullable<Decimal> TaxRate { get; set; }
		public String FacebookUrl { get; set; }
		public String TwitterUrl { get; set; }
		public String InstagramUrl { get; set; }
		public Nullable<Int32> AddressId { get; set; }
		public Nullable<Decimal> SuppliesTax { get; set; }
		public Nullable<Decimal> StateTax { get; set; }
		public String StripeApiKey { get; set; }
		public DateTime CreatedDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public String Timezone { get; set; }
		public String visiblevalue { get; set; }
		public ObjectState ObjectState { get; set; }
		public string Currency { get; set; }
		public Nullable<int> Precision { get; set; }
		public string AirportCode { get; set; }
	}
}
