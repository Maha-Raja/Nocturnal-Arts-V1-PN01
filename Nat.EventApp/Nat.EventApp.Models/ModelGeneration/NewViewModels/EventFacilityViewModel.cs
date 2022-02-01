using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class EventFacilityViewModel : BaseAutoViewModel<EventFacilityModel, EventFacilityViewModel>
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
		public EventViewModel NatEsEvent { get; set; }
	}
}
