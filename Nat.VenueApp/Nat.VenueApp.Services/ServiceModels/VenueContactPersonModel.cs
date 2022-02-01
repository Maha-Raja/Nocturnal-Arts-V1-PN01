using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueContactPersonModel : BaseServiceModel<NAT_VS_Venue_Contact_Person, VenueContactPersonModel>, IObjectState
	{
		public Int32 VenueContactPersonId { get; set; }
		public Nullable<Int32> VenueId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
        public string Notes { get; set; }
        public String CreatedBy { get; set; }
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
        public string ContactNumber { get; set; }
        public string Designation { get; set; }

        public bool PrimaryVCP { get; set; }

        public VenueModel NatVsVenue { get; set; }
        public VenueAddressModel NatVsVenueBillingAddress { get; set; }
        public VenueAddressModel NatVsVenueResidentialAddress { get; set; }
        public VenueAddressModel NatVsVenueShippingAddress { get; set; }
        public ObjectState ObjectState { get; set; }
	}
}
