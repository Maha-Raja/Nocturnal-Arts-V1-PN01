using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.DataMapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels.ArtistRequest
{
    public class ArtistRequestModel
    {
        public String FirstName { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String LastName { get; set; }
        public String StageName { get; set; }
        public String SIN { get; set; }
        public String TaxNumber { get; set; }
        public String EmergencyContact { get; set; }
        public String ContactNo { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String AddressTwo { get; set; }
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public String Country { get; set; }
        public String City { get; set; }
        public String ZipCode { get; set; }
        public Nullable<Int32> AddressGeographyId { get; set; }
        public String ArtistAbout { get; set; }
        public String StudioBusiness { get; set; }
        public String PortfolioUrl { get; set; }
        public String AccountNumber { get; set; }
        public String ArtistBankLKPId { get; set; }
        public String ArtistBankCode { get; set; }
        public String UserRoleLKPId { get; set; }
        public String PaymentCycleLKPId { get; set; }
        public decimal AvailableCredit { get; set; }
        public string DefaultPaymentMethod { get; set; }
        public String LocationCode { get; set; }
        public string EmergencyContactName { get; set; }

        public string GoogleMapsUrl { get; set; }
        public string GoogleMapsUrlSupply { get; set; }

        public string EmergencyContactRelationship { get; set; }
        public string Greeting { get; set; }
        public string Notes { get; set; }
        public string BizNumber { get; set; }
        public Dictionary<String, int> LocationCodesList { get; set; }
        public String ProfileImageUrl { get; set; }
        public String RowKey { get; set; }
        public string CompanyEmail { get; set; }
        public String PartitionKey { get; set; }
        public String CompanyAccountNumber { get; set; }
        public string CompanyName { get; set; }
        public String CompanyBankLKPId { get; set; }
        public String CompanyBankCode { get; set; }
        public string TransitNumber { get; set; }
        public string BankRoutingNumber { get; set; }

        public String EmergencyContactEmail { get; set; }
        public String Gender { get; set; }
        public String IdType { get; set; }
        public String IdNumber { get; set; }
        public String CompanyPhone { get; set; }
        public String Onboarded { get; set; }
        public Nullable<Boolean> VisibleToPublic { get; set; }
        public Nullable<Boolean> EventsVisibleToPublic { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceModel> NotificationPreferences { get; set; }

        [Complex]
        public ICollection<ArtistDocumentRequestModel> Document { get; set; }
        [Complex]
        public ICollection<ArtistSkillRequestModel> Skill { get; set; }
        [Complex]
        public ICollection<ArtistAvailabilityRequestModel> Availability { get; set; }
        [Complex]
        public ICollection<ArtistVenuePreferenceModel> NatAsArtistVenuePreference { get; set; }

        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        //custom
        public string RejectionReason { get; set; }
        public string BranchCountry { get; set; }
        public string BranchCity { get; set; }
        public string BranchZipCode { get; set; }
        public string BranchAddress { get; set; }
        public string BranchAddressTwo { get; set; }
        public Nullable<Int32> BranchAddressGeographyId { get; set; }
    }
}
