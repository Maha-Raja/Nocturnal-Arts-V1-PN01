using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class PlannerViewModel : BaseAutoViewModel<PlannerModel, PlannerViewModel>
	{
		public Int32 PlannerId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Int32 PlannerTypeLKPId { get; set; }
		public Nullable<Int32> ReferenceTypeLKPId { get; set; }
		public Nullable<Int32> ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<EventViewModel> NatPlsEvent { get; set; }
		public ICollection<SlotViewModel> NatPlsSlot { get; set; }
        [Complex]
        public ICollection<AvailabilityViewModel> NatPlsAvailability { get; set; }
	}
}
