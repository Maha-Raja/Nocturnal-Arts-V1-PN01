using Nat.Core.ViewModels;
using Nat.VenueApp.Functions.ViewModels;
using Nat.VenueApp.Services.ServiceModels;
using Nat.VenueApp.Services.ServiceModels.VenueRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Functions.ViewModel.VenueRequest
{
    public class VenueRequestViewModel : BaseAutoViewModel<VenueRequestModel, VenueRequestViewModel>
    {
        public string BranchCountry { get; set; }
        public string BranchCity { get; set; }
        public string BranchZipCode { get; set; }
        public string BranchAddress { get; set; }
        public string BranchAddressTwo { get; set; }
        public string BizNumber { get; set; }
        public string GoogleMapURL { get; set; }
        public string BankRoutingNumber { get; set; }
        public string TransitNumber { get; set; }
        public Nullable<Int32> BranchAddressGeographyId { get; set; }
        public String RowKey { get; set; }
        public String PartitionKey { get; set; }
        public String VenueName { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public String Category { get; set; }
        public Int32 Capacity { get; set; }
        public String ZipCode { get; set; }
        public String Country { get; set; }
        public String City { get; set; }
        public String Area { get; set; }
        public String Address { get; set; }
        public String AddressTwo { get; set; }
        public Nullable<Int32> AddressGeographyId { get; set; }
        public String Email { get; set; }
        public String ContactNumber { get; set; }

        public string VenueImageUrl { get; set; }
        public string VenueURl { get; set; }
        public string Onboarded { get; set; }
        public string VEventDescription { get; set; }

        [Complex]
        public ICollection<VenueAvailabilityViewModel> Availability { get; set; }
        [Complex]
        public ICollection<NotificationPreferenceViewModel> NotificationPreferences { get; set; }
        [Complex]
        public VenueAddressViewModel VenueAddress { get; set; }
        [Complex]

        public ICollection<VenueContactPersonRequestModel> ContactPerson { get; set; }
        [Complex]
        public ICollection<VenueFacilityRequestViewModel> Facilities { get; set; }
        [Complex]
        public ICollection<VenueImageRequestViewModel> Images { get; set; }
        [Complex]
        public ICollection<VenueSeatModel> Seats { get; set; }
        [Complex]
        public ICollection<VenueSeatingPlanModel> SeatingPlans { get; set; }
        [Complex]
        public ICollection<VenueArtistPreferenceViewModel> NatVsVenueArtistPreference { get; set; }
        [Complex]
        public ICollection<VenueMetroCityMappingViewModel> NatVsVenueMetroCityMapping { get; set; }
        public String SeatMapName { get; set; }

        public string FacebookProfileUrl { get; set; }
        public string TwitterProfileUrl { get; set; }
        public string InstagramProfileUrl { get; set; }
        public string GoogleProfileUrl { get; set; }
        public string LocationCode { get; set; }
        public string SpecialInstr { get; set; }
        public string Notes { get; set; }

        //custom
        public string RejectionReason { get; set; }
        public String AccountNumber { get; set; }
        public String VenueBankLKPId { get; set; }
        public String PaymentCycleLKPId { get; set; }
        public string SIN { get; set; }
        public string TaxNumber { get; set; }
        public String CreatedBy { get; set; }        
        public String LastUpdatedBy { get; set; }

    }
}
