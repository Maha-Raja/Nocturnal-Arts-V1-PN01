using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class VenueModel
	{
		public Int32 VenueId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> AddressId { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public Nullable<Int32> VenueStatusLKPId { get; set; }
		public String VenueName { get; set; }
        public String VenueTags { get; set; }
        public String VerifiedFlag { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> MinSeatingCapacity { get; set; }
		public Nullable<Int32> MaxSeatingCapacity { get; set; }
		public Nullable<Int32> VenueCategoryLKPId { get; set; }

        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        public Decimal HoursPerWeek { get; set; }
        [Complex]
        public ICollection<VenueContactPersonModel> NatVsVenueContactPerson { get; set; }

        [Complex]
        public ICollection<VenueFacilityModel> NatVsVenueFacility { get; set; }
		[Complex]
		public virtual VenueAddressModel NatVsVenueAddress { get; set; }

		[Complex]
		public ICollection<VenueArtistPreferenceModel> NatVsVenueArtistPreference { get; set; }
	}
}
