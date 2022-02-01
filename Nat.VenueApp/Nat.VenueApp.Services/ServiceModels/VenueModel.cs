using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;
using System.Linq;

namespace Nat.VenueApp.Services.ServiceModels
{
    public class VenueModel : BaseServiceModel<NAT_VS_Venue, VenueModel>, IObjectState
    {
        public Int32 VenueId { get; set; }
        public Nullable<Int32> TenantId { get; set; }
        public Nullable<Int32> AddressId { get; set; }
        public String VenueStatusLKPId { get; set; }
        public string BizNumber { get; set; }

        public String VenueStatusLKPValue { get; set; }

        public Nullable<Int32> PlannerId { get; set; }
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
        public String VenueCategoryLKPId { get; set; }
        public string GoogleMapURL { get; set; }
        public String Email { get; set; }
        public String ContactNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyEmail { get; set; }
        public string VenueImageUrl { get; set; }
        public String VenueCategoryLKPValue { get; set; }
        public String LocationCode { get; set; }
        public Nullable<Decimal> HoursPerWeek { get; set; }
        public String LocationName { get; set; }
        //follow status field
        public Boolean FollowStatus { get; set; }
        public string VenueURl { get; set; }
        public string Onboarded { get; set; }
        public string VEventDescription { get; set; }
        public Double Rating
        {
            get
            {
                if (NatVsVenueRating != null && NatVsVenueRating.Count > 0)
                {
                    VenueRatingModel rating = NatVsVenueRating.OfType<VenueRatingModel>().FirstOrDefault();
                    return rating.AverageRatingValue;
                }
                else
                {
                    return 0;
                }
            }
        }


        [Complex]

        public ICollection<VenueContactPersonModel> NatVsVenueContactPerson { get; set; }
        [Complex]

        public ICollection<VenueArtistPreferenceModel> NatVsVenueArtistPreference { get; set; }
        [Complex]
        public ICollection<VenueMetroCityMappingModel> NatVsVenueMetroCityMapping { get; set; }

        [Complex]
        public ICollection<VenueFacilityModel> NatVsVenueFacility { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceModel> NotificationPreferences { get; set; }
        [Complex]

        public ICollection<VenueHallModel> NatVsVenueHall { get; set; }
        [Complex]
        public ICollection<VenueImageModel> NatVsVenueImage { get; set; }
        [Complex]
        public ICollection<VenueRatingLogModel> NatVsVenueRatingLog { get; set; }

        [Complex]
        public ICollection<VenueRatingModel> NatVsVenueRating { get; set; }

        [Complex]
        public virtual VenueAddressModel NatVsVenueAddress { get; set; }

        [Complex]
        public ICollection<VenueEventModel> NatAsVenueEvent { get; set; }
        public ObjectState ObjectState { get; set; }

        // Custom Fields
        public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public Nullable<Int32> UpcommingEvents { get; set; }

        [Complex]
        public IEnumerable<VenueRequest.VenueAvailabilityModel> Availability { get; set; }

        //custom
        [Complex]
        public ICollection<VenueSeatModel> Seats { get; set; }
        public String SeatMapName { get; set; }
        public Nullable<Int32> EventHosted { get; set; }

        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        public string SpecialInstr { get; set; }
        public string Notes { get; set; }

        //custom
        public String RejectionReason { get; set; }

        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public Boolean PasswordChanged { get; set; }

        public string ParentLocationCode { get; set; }
        public string ParentLocationName { get; set; }
        public String AccountNumber { get; set; }
        public String ArtistBankLKPId { get; set; }
        public String PaymentCycleLKPId { get; set; }
        public string SIN { get; set; }
        public string TaxNumber { get; set; }
        [Complex]
        public ICollection<VenueBankAccountModel> NATVSVenueBankAccount { get; set; }

        public string BranchCountry { get; set; }
        public string BranchCity { get; set; }
        public string BranchZipCode { get; set; }
        public string BranchAddress { get; set; }
        public string BranchAddressTwo { get; set; }
    }
}
