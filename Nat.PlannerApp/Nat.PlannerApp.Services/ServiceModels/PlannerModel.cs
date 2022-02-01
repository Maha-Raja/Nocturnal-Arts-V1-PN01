using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PlannerApp.Services.ServiceModels
{
	public class PlannerModel : BaseServiceModel<NAT_PLS_Planner, PlannerModel>, IObjectState
	{
		public Int32 PlannerId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Int32 PlannerTypeLKPId { get; set; }
		public Nullable<Int32> ReferenceTypeLKPId { get; set; }
		public Nullable<Int32> ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public string GoogleCalendarId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<EventModel> NatPlsEvent { get; set; }
		public ICollection<SlotModel> NatPlsSlot { get; set; }
        [Complex]
        public ICollection<AvailabilityModel> NatPlsAvailability { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
