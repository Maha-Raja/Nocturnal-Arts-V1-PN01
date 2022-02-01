using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class EventSeatingPlanModel : BaseServiceModel<NAT_VS_Event_Seating_Plan, EventSeatingPlanModel>, IObjectState
	{
		public Int32 SeatingPlanId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> EventId { get; set; }
		public Nullable<Int32> VenueHallId { get; set; }
		public String EventSeatingPlanName { get; set; }
		public Nullable<Int32> TotalSeats { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<EventSeatModel> NatAsEventSeat { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
