using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Spatial;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueAddressViewModel : BaseAutoViewModel<VenueAddressModel, VenueAddressViewModel>
	{
		public Int32 AddressId { get; set; }

        public String Address { get; set; }
        public String AddressLine1 { get; set; }
		public String AddressLine2 { get; set; }
		public String PostalZipCode { get; set; }
        public String WKTCoordinates { get; set; }
		public String CityName { get; set; }
		public String OtherCityFlag { get; set; }
		public String CreatedBy { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> AddressGeographyId { get; set; }

        public String VenueCoords { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
	}
}
