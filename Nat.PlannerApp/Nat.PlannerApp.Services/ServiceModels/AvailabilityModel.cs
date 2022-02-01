using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PlannerApp.Services.ServiceModels
{
	public class AvailabilityModel : BaseServiceModel<NAT_PLS_Availability, AvailabilityModel>, IObjectState
	{
		public Int32 AvailabilityId { get; set; }
		public Int32 PlannerId { get; set; }
		public Int32 DayOfWeekLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveStartTime { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PlannerModel NatPlsPlanner { get; set; }
        [Complex]
		public ICollection<AvailabilitySlotModel> NatPlsAvailabilitySlot { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
