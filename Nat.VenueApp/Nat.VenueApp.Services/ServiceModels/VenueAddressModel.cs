using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;
using System.Data.Entity.Spatial;
using Newtonsoft.Json;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueAddressModel : BaseServiceModel<NAT_VS_Venue_Address, VenueAddressModel>, IObjectState
	{
		public Int32 AddressId { get; set; }
        public String Address { get; set; }
		public String AddressLine1 { get; set; }
		public String AddressLine2 { get; set; }
		public String PostalZipCode { get; set; }
        [JsonIgnore]
        public System.Data.Entity.Spatial.DbGeography Coordinates { get; set; }
        public String WKTCoordinates {
            get {
                return  this.Coordinates !=  null? this.Coordinates.WellKnownValue.WellKnownText : null;
            }
            set {
                this.Coordinates = (value !=null && value !="")? DbGeography.FromText(value): null;
            }
        }
        public String CityName { get; set; }
		public String OtherCityFlag { get; set; }
		public String CreatedBy { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> AddressGeographyId { get; set; }
		public ObjectState ObjectState { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public ICollection<VenueBankAccountModel> NatVsVenueBankAccount { get; set; }
	}
}
