using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class EventFacilityModel : BaseServiceModel<NAT_ES_Event_Facility, EventFacilityModel>, IObjectState
	{
		public Int32 EventFacilityId { get; set; }
		public String FacilityName { get; set; }
		public Nullable<Int32> FacilityLKPId { get; set; }
		public String FacilityDescription { get; set; }
		public Nullable<Int32> EventId { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public EventModel NatEsEvent { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
