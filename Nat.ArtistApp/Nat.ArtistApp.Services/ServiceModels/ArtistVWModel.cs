using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistVWModel : BaseServiceModel<NAT_Artist_VW, ArtistVWModel>
	{
        public int ArtistID { get; set; }
        public string StageName { get; set; }
        public string Status { get; set; }
        public string ArtistFirstName { get; set; }
        public string ArtistMiddleName { get; set; }
        public string ArtistLastName { get; set; }
        public string MarketCode { get; set; }
        public string ArtistEmail { get; set; }
        public string ArtistPhoneNumber { get; set; }
        public string NextEventConfirmed { get; set; }
        public Nullable<System.DateTime> NextEventDate { get; set; }
        public Nullable<System.DateTime> NextEventEndTime { get; set; }
        public string NextEventVenueID { get; set; }
        public string NextEventVenueName { get; set; }
        public Nullable<System.DateTime> LastEventDate { get; set; }
        public Nullable<System.DateTime> LastEventEndTime { get; set; }
        public string NextEventTimezone { get; set; }
        public string LastEventTimezone { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Onboarded { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactEmail { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string ArtistAbout { get; set; }
        public string TaxNumber { get; set; }
        public string SIN { get; set; }
        public string Notes { get; set; }
        public string ArtistAddress { get; set; }
        public string PostalZipCode { get; set; }
        public string NextEventDescription { get; set; }
        public string ArtistCityName { get; set; }
        public string ArtistStateName { get; set; }
        public string ArtistCountryName { get; set; }
        public string NextEventLocationCode { get; set; }
        public string LastEventLocationCode { get; set; }
        public string LocationCode { get; set; }
        public string ArtistStageNameId { get; set; }
    }
}
