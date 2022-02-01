using System;

namespace Nat.CustomerApp.Services.ViewModels
{
	public class AddressGeographyViewModel
	{
		public Int32 AddressGeographyId { get; set; }
		public String ParentGeographyCode { get; set; }
        public String GeographyName { get; set; }
        public String GeographyDescription { get; set; }
		public String GeographyShortCode { get; set; }
		public String GeographyTypeLKP { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
	}
}
