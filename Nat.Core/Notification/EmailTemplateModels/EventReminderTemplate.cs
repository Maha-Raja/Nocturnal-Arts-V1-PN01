using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Notification.EmailTemplateModels
{
    public class EventReminderTemplate
    {
        public string EventName { get; set; }
        public string OldEventName { get; set; }
        public string RecipentName { get; set; }
        public string EventVenueName { get; set; }
        public string EventDate { get; set; }
        public string EventImage { get; set; }
        public string EventPageLink { get; set; }
        public string ArtistName { get; set; }
        public string PaintingName { get; set; }
        public double ArtistRating { get; set; }
        public double VenueRating { get; set; }
        public double PaintingRating { get; set; }
        public string ArtistProfileLink { get; set; }
        public string EventAddress { get; set; }
        public string EventDateSecond { get; set; }
        public string VenueProfileLink { get; set; }
        public string EventTime { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string RefundPolicy { get; set; }
        public string Timezone { get; set; }
        public String EventCancellationURLForVenue { get; set; }
        public bool IsVenue { get; set; }
        public string VenueName { get; set; }
        public String EventCancellationURLForArtist { get; set; }
        public string EventStartTime { get; set; }
        public string MeetingLink { get; set; }
        //old items
        public string OldArtistName { get; set; }
        public string OldPaintingName { get; set; }
        public string OldEventVenueName { get; set; }
        public string OldEventAddress { get; set; }
        public string OldEventImage { get; set; }        
        public string OldEventDate { get; set; }
        public string OldEventTime { get; set; }

        public string ArtistDynamic { get; set; }
        public string PaintingDynamic { get; set; }
        public string VenueDynamic { get; set; }
        public string DateDynamic { get; set; }
    }
}

