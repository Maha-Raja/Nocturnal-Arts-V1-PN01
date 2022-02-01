using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class EventModel : BaseServiceModel<NAT_ES_Event, EventModel>, IObjectState
	{
		public Int32 EventId { get; set; }
		public Int32 TenantId { get; set; }
		public Int32 ArtistId { get; set; }
		public Int32 EventTypeLKPId { get; set; }
		public Int32 EventStatusLKPId { get; set; }
		public Int32 PaintingId { get; set; }
		public Nullable<Int32> SeatingPlanId { get; set; }
		public Nullable<Int32> EventAgeGroupTypeLKPId { get; set; }
		public Nullable<Int32> EventCategoryLKPId { get; set; }
		public Nullable<Int32> VenueHallId { get; set; }
		public String EventName { get; set; }
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
		public String EventDescription { get; set; }
		public String EventTags { get; set; }
		public String VenueCityCode { get; set; }
		public Nullable<Boolean> IsFeatured { get; set; }
		public String EventCode { get; set; }
		public String LocationCode { get; set; }
		public ICollection<EventWaitListModel> NatEsEventWaitList { get; set; }
		public ICollection<EventImageModel> NatEsEventImage { get; set; }
		public ICollection<EventFacilityModel> NatEsEventFacility { get; set; }
		public ICollection<EventSeatingPlanModel> NatEsEventSeatingPlan { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
