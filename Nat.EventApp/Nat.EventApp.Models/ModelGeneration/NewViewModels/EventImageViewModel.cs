using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class EventImageViewModel : BaseAutoViewModel<EventImageModel, EventImageViewModel>
	{
		public Int32 EventImageId { get; set; }
		public Int32 TenantId { get; set; }
		public Int32 EventId { get; set; }
		public Int32 ImageTypeLKPId { get; set; }
		public String ImagePath { get; set; }
		public Boolean ActiveFlag { get; set; }
		public DateTime EffectiveStartDate { get; set; }
		public DateTime EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public EventViewModel NatEsEvent { get; set; }
	}
}
