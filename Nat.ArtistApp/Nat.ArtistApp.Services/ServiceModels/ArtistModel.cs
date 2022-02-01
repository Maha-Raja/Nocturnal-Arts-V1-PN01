using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;
using System.Linq;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistModel : BaseServiceModel<NAT_AS_Artist, ArtistModel>, IObjectState
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
        public string CompanyEmail { get; set; }
        public string CompanyName { get; set; }
        public String ContactNumber { get; set; }
        public Nullable<Int32> GenderLKPId { get; set; }
        public String GenderLKPValue { get; set; }
        public Nullable<Int32> ResidentialAddressId { get; set; }
        public Nullable<Int32> ShippingAddressId { get; set; }
        public Nullable<Int32> BillingAddressId { get; set; }
        public String BusinessName { get; set; }
        public String EmergencyContact { get; set; }
        public String PaymentCycleLKPId { get; set; }
        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        public Decimal HoursPerWeek { get; set; }
        public String ArtistAbout { get; set; }
        public Nullable<Boolean> VisibleToPublic { get; set; }
        public Nullable<Boolean> EventsVisibleToPublic { get; set; }

        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string Greeting { get; set; }
        public string Notes { get; set; }
        public string BizNumber { get; set; }
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

        //follow status field
        public Boolean FollowStatus { get; set; }
        public decimal AvailableCredit { get; set; }
        public string DefaultPaymentMethod { get; set; }
        public Boolean HostVirtualEvent { get; set; }


        public string GoogleMapsUrl { get; set; }
        public string GoogleMapsUrlSupply { get; set; }

        //Added Manually
        [Complex]
        public ArtistAddressModel NatAsArtistResidentialAddress { get; set; }
        [Complex]
        public IEnumerable<ArtistDisbursementHeaderModel> ArtistDisbursementHeader { get; set; }
        public ArtistAddressModel NatAsArtistShippingAddress { get; set; }

        public ArtistAddressModel NatAsArtistBillingAddress { get; set; }
        [Complex]
        public ICollection<ArtistVenuePreferenceModel> NatAsArtistVenuePreference { get; set; }

        public Double Rating {
            get
            {
                if (NatAsArtistRating.Count > 0)
                {
                    ArtistRatingModel rating = NatAsArtistRating.OfType<ArtistRatingModel>().FirstOrDefault();
                    return rating.AverageRatingValue;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Complex]
		public ICollection<ArtistBankAccountModel> NatAsArtistBankAccount { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceModel> NotificationPreferences { get; set; }
        [Complex]
        public ICollection<ArtistRatingModel> NatAsArtistRating { get; set; }
        [Complex]
		public ICollection<ArtistRatingLogModel> NatAsArtistRatingLog { get; set; }

        [Complex]
        public ICollection<ArtistDocumentModel> NatAsArtistDocument { get; set; }
        [Complex]
        public ICollection<ArtistSkillModel> NatAsArtistSkill { get; set; }

        [Complex]
        public ICollection<ArtistEventModel> NatAsArtistEvent { get; set; }
        public ObjectState ObjectState { get; set; }
        [Complex]
        public IEnumerable<ArtistRequest.ArtistAvailabilityModel> Availability { get; set; }
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public Boolean PasswordChanged { get; set; }
        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        public String CityName { get; set; }
        public Int32 EventHosted { get; set; }
        public string BranchCountry { get; set; }
        public string BranchCity { get; set; }
        public string BranchZipCode { get; set; }
        public string BranchAddress { get; set; }
        public string BranchAddressTwo { get; set; }
        public string ArtistTimezone { get; set; }

        public string VideoKey { get; set; }
        public string VideoSecret { get; set; }
        public string VideoUser { get; set; }
        public String ArtistFullName
        {
            get
            {
                return ArtistFirstName + " " + ArtistLastName;
            }
        }

        [Complex]
        public List<ArtistLocationMappingModel> NatAsArtistLocationMapping { get; set; }


        public String ArtistLocations
        {
            get
            {
                string locationCodeList = "";
                for (int i=0; i<NatAsArtistLocationMapping.Count; i++)
                {
                    locationCodeList += NatAsArtistLocationMapping[i].LocationCode;
                    if (i != (NatAsArtistLocationMapping.Count - 1))
                    {
                        locationCodeList += ", ";
                    }
                }
                return locationCodeList;
            }
        }
        
    }


}
