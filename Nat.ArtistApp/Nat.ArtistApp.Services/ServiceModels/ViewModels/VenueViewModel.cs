using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Services.ServiceModels.ViewModels
{
    public class VenueViewModel
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
        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        public Decimal HoursPerWeek { get; set; }
        //follow status field
        public Boolean FollowStatus { get; set; }

        public Double Rating { get; set; }


        [Complex]
        public ICollection<VenueArtistPreferenceViewModel> NatVsVenueArtistPreference { get; set; }

        // Custom Fields
        public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public Nullable<Int32> UpcommingEvents { get; set; }

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
    }
}
