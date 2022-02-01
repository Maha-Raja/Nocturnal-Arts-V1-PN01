using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class SlotViewModel : BaseAutoViewModel<SlotModel, SlotViewModel>
	{
		public Int32 SlotId { get; set; }
		public Int32 PlannerId { get; set; }
		public Nullable<Int32> EventId { get; set; }

        public String EventName { get; set; }

        public String Description { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public String TimingLKP { get; set; }
		public Int32 StatusTypeLKPId { get; set; }
		public Int32 SlotTypeLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PlannerViewModel NatPlsPlanner { get; set; }
        [JsonIgnore]
        public EventViewModel NatPlsEvent { get; set; }
        public string ReferenceId { get; set; }
}
}
