using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.LocationApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.LocationApp.Functions.ViewModels
{
	public class LocationViewModel : BaseAutoViewModel<LocationModel, LocationViewModel>
	{
		public Int32 LocationId { get; set; }
		public String ParentLocationCode { get; set; }
        public String LocationName { get; set; }
        public String LocationDescription { get; set; }
		public String LocationShortCode { get; set; }
		public String LocationTypeLKP { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }		
		public string Timezone { get; set; }
		public Nullable<bool> SalesTaxApplicableFlag { get; set; }
		public Nullable<bool> SalesTaxInclusiveFlag { get; set; }
		public Nullable<decimal> TaxRate { get; set; }
		public string FacebookUrl { get; set; }
		public string TwitterUrl { get; set; }
		public string InstagramUrl { get; set; }
		public Nullable<int> AddressId { get; set; }
		public Nullable<decimal> SuppliesTax { get; set; }
		public Nullable<decimal> StateTax { get; set; }
		public string StripeApiKey { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public string TimezoneVisibleValue { get; set; }
		public string Currency { get; set; }
		public Nullable<int> Precision { get; set; }
		public string AirportCode { get; set; }
		public string GoogleShortCode { get; set; }
		public Nullable<decimal> Tax3 { get; set; }
		public Nullable<decimal> Tax4 { get; set; }
		public Nullable<decimal> Tax5 { get; set; }
		public Nullable<bool> DaylightSavingApplicable { get; set; }
		public Nullable<DateTime> DaylightStartTime { get; set; }
		public Nullable<DateTime> DaylightEndTime { get; set; }
		public string MarketId { get; set; }
		public Nullable<long> ManagerId { get; set; }

		public Nullable<decimal> VirtualTax { get; set; }

		public Nullable<int> LegalDrinkingAge { get; set; }

		public string StateTaxLabel { get; set; }

		public string TaxRateLabel { get; set; }

		public string Tax3Label { get; set; }

		public string Tax4Label { get; set; }

		public string Tax5Label { get; set; }
		public string VirtualTaxLabel { get; set; }

		public Nullable<bool> TaxRateInclude { get; set; }

		public Nullable<bool> StateTaxInclude { get; set; }

		public Nullable<bool> Tax3Include { get; set; }

		public Nullable<bool> Tax4Include { get; set; }

		public Nullable<bool> Tax5Include { get; set; }

		public string VirtualTaxArea { get; set; }

		public string TaxRateArea { get; set; }

		public string Tax3Area { get; set; }

		public string Tax4Area { get; set; }

		public string Tax5Area { get; set; }

		public string URL { get; set; }
		public string VirtualTaxInclude { get; set; }
		public Nullable<Boolean> NewUserCheck { get; set; }
		public String UserName { get; set; }
		public String PhoneNumber { get; set; }
		public String Email { get; set; }
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String Password { get; set; }
	}
}
