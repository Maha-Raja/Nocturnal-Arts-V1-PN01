using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class EventViewModel : BaseAutoViewModel<EventModel, EventViewModel>
	{
		public Int32 EventId { get; set; }
		public Int32 PlannerId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Int32 EventTypeLKPId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public String ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public String UDF { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PlannerViewModel NatPlsPlanner { get; set; }

        [Complex]
        public ICollection<SlotViewModel> NatPlsSlot { get; set; }

		//custom feild to check if the artist is available for event creation or not
		//if an unavailable artist is selected for the event then this flag will be true
		public Boolean Forced { get; set; }
		public Boolean Online { get; set; }
		public string SlotTiming { get; set; }
	}
}
