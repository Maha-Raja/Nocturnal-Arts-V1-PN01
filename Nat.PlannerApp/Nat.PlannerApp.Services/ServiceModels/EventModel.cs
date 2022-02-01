using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PlannerApp.Services.ServiceModels
{
	public class EventModel : BaseServiceModel<NAT_PLS_Event, EventModel>, IObjectState
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
		public string GoogleEventId { get; set; }
		public string GoogleHangoutUrl { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PlannerModel NatPlsPlanner { get; set; }

        public ICollection<SlotModel> NatPlsSlot { get; set; }
        public ObjectState ObjectState { get; set; }

		//custom feild to check if the artist is available for event creation or not
		//if an unavailable artist is selected for the event then this flag will be true
		public Boolean Forced { get; set; }
		public Boolean Online { get; set; }
		public string SlotTiming { get; set; }
	}
}
