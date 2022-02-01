using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class AvailabilitySlotViewModel : BaseAutoViewModel<AvailabilitySlotModel, AvailabilitySlotViewModel>
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
		public AvailabilityViewModel NatPlsAvailability { get; set; }
	}
}
