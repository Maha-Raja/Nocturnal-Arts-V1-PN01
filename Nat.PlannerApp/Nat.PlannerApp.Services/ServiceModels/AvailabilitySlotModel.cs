using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PlannerApp.Services.ServiceModels
{
	public class AvailabilitySlotModel : BaseServiceModel<NAT_PLS_Availability_Slot, AvailabilitySlotModel>, IObjectState
	{
		public Int32 AvailabilitySlotId { get; set; }
		public Int32 AvailabilityId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public AvailabilityModel NatPlsAvailability { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
