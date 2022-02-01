using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.LocationApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.LocationApp.Functions.ViewModels
{
	public class AddressGeographyViewModel : BaseAutoViewModel<AddressGeographyModel, AddressGeographyViewModel>
	{
		public Int32 AddressGeographyId { get; set; }
		public Nullable<Int32> ParentGeographyId { get; set; }
		public String ParentGeographyCode { get; set; }
        public String GeographyName { get; set; }
        public String GeographyDescription { get; set; }
		public String GeographyShortCode { get; set; }
		public String GeographyTypeLKP { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public AddressGeographyModel Child { get; set; }
		//public String CreatedBy { get; set; }
		//public Nullable<DateTime> CreatedDate { get; set; }
		//public String LastUpdatedBy { get; set; }
		//public Nullable<DateTime> LastUpdatedDate { get; set; }
	}
}
