using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System.Linq;
using Nat.ArtistApp.Services;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistViewModel : BaseAutoViewModel<ArtistModel, ArtistViewModel>
	{
		public Int32 ArtistId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
        public String ArtistPortfolioUrl { get; set; }
        public Nullable<Int32> ArtistStatusLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> StripeId { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public String ArtistExtension { get; set; }
        public Nullable<DateTime> DateOfBirth { get; set; }
        public String ArtistProfileImageLink { get; set; }
        public String ArtistEmail { get; set; }
        public String ArtistMiddleName { get; set; }
        public String ArtistLastName { get; set; }
        public String ArtistFirstName { get; set; }
        public String StageName { get; set; }
        public String SIN { get; set; }
        public String TaxNumber { get; set; }
        public String EmergencyContact { get; set; }
        public decimal AvailableCredit { get; set; }
        public string DefaultPaymentMethod { get; set; }
        public Nullable<Int32> GenderLKPId { get; set; }
        public String GenderLKPValue { get; set; }
        public Nullable<Int32> ResidentialAddressId { get; set; }
        public Nullable<Int32> ShippingAddressId { get; set; }
        public Nullable<Int32> BillingAddressId { get; set; }
        public String BusinessName { get; set; }
        public String ContactNumber { get; set; }
        public String PaymentCycleLKPId { get; set; }
        public Decimal HoursPerWeek { get; set; }
        public String ArtistAbout { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyName { get; set; }
        public Nullable<Boolean> VisibleToPublic { get; set; }
        public Nullable<Boolean> EventsVisibleToPublic { get; set; }
        public Boolean HostVirtualEvent { get; set; }

        public string GoogleMapsUrl { get; set; }
        public string GoogleMapsUrlSupply { get; set; }

        public String EmergencyContactEmail { get; set; }
        public String Gender { get; set; }
        public String IdType { get; set; }
        public String IdNumber { get; set; }
        public String CompanyPhone { get; set; }
        public String Onboarded { get; set; }

        // Custom Fields
        public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public Nullable<Int32> UpcommingEvents { get; set; }

        public Boolean IsArtistAvailable { get; set; }

        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string Greeting { get; set; }
        public string Notes { get; set; }
        public string BizNumber { get; set; }

        //Added Manually
        [Complex]
        public ArtistAddressModel NatAsArtistResidentialAddress { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceViewModel> NotificationPreferences { get; set; }

        public ArtistAddressModel NatAsArtistShippingAddress { get; set; }

        public ArtistAddressModel NatAsArtistBillingAddress { get; set; }
        //follow status field
        public Boolean FollowStatus { get; set; }
        //
        [Complex]
		public ICollection<ArtistBankAccountViewModel> NatAsArtistBankAccount { get; set; }
        [Complex]
        public ICollection<ArtistRatingViewModel> NatAsArtistRating { get; set; }
        [Complex]
		public ICollection<ArtistRatingLogViewModel> NatAsArtistRatingLog { get; set; }
        [Complex]
        public ICollection<ArtistDocumentViewModel> NatAsArtistDocument { get; set; }
        [Complex]
        public ICollection<ArtistSkillViewModel> NatAsArtistSkill { get; set; }

        [Complex]
        public ICollection<ArtistEventViewModel> NatAsArtistEvent { get; set; }
        [Complex]
        public ICollection<ArtistVenuePreferenceViewModel> NatAsArtistVenuePreference { get; set; }
        public String ArtistFullName { get; set; }
    public Double Rating { get; set; }
        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        [Complex]
        public IEnumerable<ArtistRequest.ArtistAvailabilityViewModel> Availability { get; set; }

        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public Boolean PasswordChanged { get; set; }
        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        public String CityName { get; set; }
        public Int32 EventHosted { get; set; }

        [Complex]
        public List<ArtistLocationMappingViewModel> NatAsArtistLocationMapping { get; set; }
        public String ArtistLocations { get; set; }
        public IEnumerable<ArtistDisbursementHeaderViewModel> ArtistDisbursementHeader { get; set; }

        public string VideoKey { get; set; }
        public string VideoSecret { get; set; }
        public string VideoUser { get; set; }
    }
}
