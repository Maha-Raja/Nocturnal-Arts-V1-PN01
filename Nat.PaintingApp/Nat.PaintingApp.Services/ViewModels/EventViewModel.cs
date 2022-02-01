using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.Core.ViewModels;

namespace Nat.PaintingApp.Services.ViewModels
{
	public class EventViewModel
	{
		public Int32 EventId { get; set; }
		public Int32 TenantId { get; set; }
        public Int32 ArtistPlannerId { get; set; }
        public Int32 VenuePlannerId { get; set; }
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
    }
}
