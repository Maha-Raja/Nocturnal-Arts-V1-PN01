using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueSeatModel : BaseServiceModel<NAT_VS_Venue_Seat, VenueSeatModel>, IObjectState
	{
		public Int32 SeatId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> SeatingPlanId { get; set; }
		public Nullable<Int32> SeatTypeLKPId { get; set; }
        public String SeatTypeLKPValue { get; set; }
        public Nullable<Int32> SeatAllocationTypeLKPId { get; set; }
        public String SeatAllocationTypeLKPValue { get; set; }
        public String RowNumber { get; set; }
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
		public VenueSeatingPlanModel NatAsVenueSeatingPlan { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
