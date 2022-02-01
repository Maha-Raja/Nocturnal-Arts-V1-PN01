using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class LocationModel
	{
		public Int32 LocationId { get; set; }
		public String ParentLocationCode { get; set; }
		public String LocationName { get; set; }
		public String LocationDescription { get; set; }
		public String LocationShortCode { get; set; }
		public String LocationTypeLKP { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
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
	}
}
