using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class ArtistModel
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

        // Custom Fields
        public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public Nullable<Int32> UpcommingEvents { get; set; }

        public Boolean IsArtistAvailable { get; set; }

        public Boolean FollowStatus { get; set; }
        //

        public String ArtistFullName { get; set; }
        public Double Rating { get; set; }
        public String LocationCode { get; set; }
        public String LocationName { get; set; }
        
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public Boolean PasswordChanged { get; set; }
        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        public String CityName { get; set; }
        public Int32 EventHosted { get; set; }
        public String ArtistLocations { get; set; }
    }
}
