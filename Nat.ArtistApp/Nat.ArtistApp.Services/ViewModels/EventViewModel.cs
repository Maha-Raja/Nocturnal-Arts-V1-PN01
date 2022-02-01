using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class EventViewModel
    {
        public Int32 EventId { get; set; }
        public Int32 TenantId { get; set; }
        public Nullable<Int32> ArtistId { get; set; }
        public Int32 EventTypeLKPId { get; set; }
        public Int32 EventStatusLKPId { get; set; }
        public Int32 PaintingId { get; set; }
        public Nullable<Int32> SeatingPlanId { get; set; }
        public Nullable<Int32> EventAgeGroupTypeLKPId { get; set; }
        public Int32 EventCategoryLKPId { get; set; }
        public Nullable<Int32> VenueHallId { get; set; }
        public String EventName { get; set; }
        public String NoteToVCP { get; set; }
        public String EventCode { get; set; }
        public Boolean ActiveFlag { get; set; }
        public Nullable<DateTime> EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveEndDate { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<DateTime> EventDate { get; set; }
        public Nullable<DateTime> EventStartTime { get; set; }
        public Nullable<DateTime> EventEndTime { get; set; }
        public Nullable<Boolean> PrivateEventFlag { get; set; }
        public Nullable<Boolean> FundraisingEventFlag { get; set; }
        public Nullable<Decimal> MaxTicketPrice { get; set; }
        public Nullable<Decimal> MinTicketPrice { get; set; }
        public string LocationCode { get; set; }
        public String VenueCityCode { get; set; }

        public Boolean IsFeatured { get; set; }
        public String EventDescription { get; set; }
        public String EventTags { get; set; }
        public Int32 GoldTicketsCount { get; set; }
        public Int32 SilverTicketsCount { get; set; }
        public Int32 BasicTicketsCount { get; set; }
        public Int32 EventLikesCount { get; set; }


        //Custom Fields Events

        public Nullable<Int32> VenueId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

        public ICollection<int?> Categoryid { get; set; }
        public ICollection<int> Artistid { get; set; }
        public Int32 Sortid { get; set; }
        public Boolean SortAsc { get; set; }
        public Int32 ArtistRatingFilter { get; set; }


        //custom Fields Artist
        public String ArtistName { get; set; }
        public Double ArtistRating { get; set; }
        public String ArtistImage { get; set; }


        //custom Fields Painting
        public String PaintingName { get; set; }
        public Double PaintingRating { get; set; }
        public String PaintingImage { get; set; }

        //custom Fields Venue
        public String VenueAddress { get; set; }
        public String VenueName { get; set; }
        public Double VenueRating { get; set; }
        //custom like status status field
        public bool LikeStatus { get; set; }

        //custom painting video field
        public String PaintingVideo { get; set; }

        //custom Fields for event listing on artist module of mobile application
        public Int32 BookedTickets { get; set; }
        public Int32 TotalTickets { get; set; }
        public Int32 TotalAmount { get; set; }
        public String VCPContactNumber { get; set; }
    }
}