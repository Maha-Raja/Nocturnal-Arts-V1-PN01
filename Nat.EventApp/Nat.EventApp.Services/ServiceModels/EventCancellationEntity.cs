using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class EventUnavailabilityEntities : TableEntity
    {
        public string EventCode { get; set; }
        public string ChangedVenueName { get; set; }
        public string ChangedVenueCode { get; set; }
        public bool VenueConsentReceived { get; set; }
        public string VenuesConsentSought { get; set; } // contains the email addresses comma separated
        public string VenueNamesForConsent { get; set; } // contains the venue names, comma separated
        public Nullable<int> CurrentArtist { get; set; }
        public string ChangedArtistName { get; set; }
        public string ChangedArtistCode { get; set; }
        public bool ArtistConsentReceived { get; set; }
        public string ArtistConsentSought { get; set; } // contains the email addresses comma separated
        public string ArtistNamesForConsent { get; set; } // contains the venue names, comma separated
        public Nullable<int> CurrentVenue { get; set; }

        public void ResetTableEntityForVenueCancellation()
        {
            ChangedVenueName = String.Empty;
            ChangedVenueCode = String.Empty;
            VenueConsentReceived = false;
            VenuesConsentSought = String.Empty; // contains the email addresses comma separated
            VenueNamesForConsent = String.Empty;// contains the venue names, comma separated
        }


        public void ResetTableEntityForArtistCancellation()
        {
            ChangedArtistName = String.Empty;
            ChangedArtistCode = String.Empty;
            ArtistConsentReceived = false;
            ArtistConsentSought = String.Empty;
            ArtistNamesForConsent = String.Empty;
            // CurrentArtist = null;
        }

        public EventUnavailabilityEntities()
        {

        }

        public EventUnavailabilityEntities(string eventCodeHash, EventModel eventModel)
        {
            this.PartitionKey = eventModel.EventCode;
            this.RowKey = eventCodeHash;
            this.EventCode = eventModel.EventCode;
        }
    }
}