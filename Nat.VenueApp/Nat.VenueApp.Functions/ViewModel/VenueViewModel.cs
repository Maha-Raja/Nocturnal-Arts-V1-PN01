using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System.Linq;
using Nat.VenueApp.Functions.ViewModel;

namespace Nat.VenueApp.Functions.ViewModels
{
    public class VenueViewModel : BaseAutoViewModel<VenueModel, VenueViewModel>
    {
        public Int32 VenueId { get; set; }
        public Nullable<Int32> TenantId { get; set; }
        public Nullable<Int32> AddressId { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public String VenueStatusLKPId { get; set; }
        public String VenueStatusLKPValue { get; set; }
        public String VenueName { get; set; }
        public String VenueTags { get; set; }
        public string BizNumber { get; set; }

        public string VenueImageUrl { get; set; }
        public string GoogleMapURL { get; set; }
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

        public String Email { get; set; }
        public String ContactNumber { get; set; }

        public String VenueCategoryLKPValue { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyEmail { get; set; }
        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        public Decimal HoursPerWeek { get; set; }
        //follow status field
        public Boolean FollowStatus { get; set; }
        public string VenueURl { get; set; }
        public string Onboarded { get; set; }
        public string VEventDescription { get; set; }

        [Complex]
        public ICollection<VenueContactPersonViewModel> NatVsVenueContactPerson { get; set; }
        [Complex]
        public ICollection<VenueFacilityViewModel> NatVsVenueFacility { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceViewModel> NotificationPreferences { get; set; }
        [Complex]
        public ICollection<VenueHallViewModel> NatVsVenueHall { get; set; }
        [Complex]
        public ICollection<VenueImageViewModel> NatVsVenueImage { get; set; }
        [Complex]
        public ICollection<VenueRatingLogViewModel> NatVsVenueRatingLog { get; set; }
        [Complex]
        public ICollection<VenueRatingViewModel> NatVsVenueRating { get; set; }
        [Complex]
        public virtual VenueAddressViewModel NatVsVenueAddress { get; set; }

        public Double Rating { get; set; }

        //{    get
        //    {
        //        if (NatVsVenueRating.Count > 0)
        //        {
        //            VenueRatingViewModel rating = NatVsVenueRating.OfType<VenueRatingViewModel>().FirstOrDefault();
        //            return rating.AverageRatingValue;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //}

        [Complex]
        public ICollection<VenueEventViewModel> NatAsVenueEvent { get; set; }

        [Complex]
        public ICollection<VenueArtistPreferenceViewModel> NatVsVenueArtistPreference { get; set; }
        [Complex]
        public ICollection<VenueMetroCityMappingViewModel> NatVsVenueMetroCityMapping { get; set; }

        // Custom Fields
        public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public Nullable<Int32> UpcommingEvents { get; set; }

        [Complex]
        public IEnumerable<ViewModel.VenueRequest.VenueAvailabilityViewModel> Availability { get; set; }

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
        public ICollection<VenueBankAccountViewModel> NATVSVenueBankAccount { get; set; }
    }
}
