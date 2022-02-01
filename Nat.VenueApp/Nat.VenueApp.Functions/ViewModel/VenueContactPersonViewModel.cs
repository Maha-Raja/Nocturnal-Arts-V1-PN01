using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueContactPersonViewModel : BaseAutoViewModel<VenueContactPersonModel, VenueContactPersonViewModel>
	{
		public Int32 VenueContactPersonId { get; set; }
		public Nullable<Int32> VenueId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
        public string Notes { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> TextFlag { get; set; }
        public string Greeting { get; set; }

        public String Extension { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public String ProfileImageLink { get; set; }
        public String Email { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public Nullable<Int32> BillingAddressId { get; set; }
        public Nullable<Int32> ResidentialAddressId { get; set; }
        public Nullable<Int32> GenderLKPId { get; set; }
        public Nullable<Int32> TenantId { get; set; }
        public Nullable<Int32> ShippingAddressId { get; set; }
        public String ContactNumber { get; set; }
        public String Designation { get; set; }

        public bool PrimaryVCP { get; set; }
        public VenueViewModel NatVsVenue { get; set; }
        public VenueAddressViewModel NatVsVenueBillingAddress { get; set; }
        public VenueAddressViewModel NatVsVenueResidentialAddress { get; set; }
        public VenueAddressViewModel NatVsVenueShippingAddress { get; set; }
    }
}
