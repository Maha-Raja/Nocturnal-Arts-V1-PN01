using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Nat.EventApp.Functions.ViewModel;

namespace Nat.EventApp.Functions.ViewModels
{
	public class EventSeatViewModel : BaseAutoViewModel<EventSeatModel, EventSeatViewModel>
	{
		public Int32 SeatId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> SeatingPlanId { get; set; }
		public Nullable<Int32> SeatTypeLKPId { get; set; }
		public Nullable<Int32> SeatAllocationTypeLKPId { get; set; }
		public String RowNumber { get; set; }
        public String StatusLKP { get; set; }
        public String SeatNumber { get; set; }
		public String Label { get; set; }
		public Nullable<Int32> OffsetX { get; set; }
		public Nullable<Int32> OffsetY { get; set; }
		public Nullable<Int32> Radius { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public EventSeatingPlanViewModel NatEsEventSeatingPlan { get; set; }
        public EventTicketViewModel NatEsEventTicketViewModel { get; set; }
        public String SeatTypeLKPValue { get; set; }
        public Nullable<Decimal> Price { get; set; }
    }
}
