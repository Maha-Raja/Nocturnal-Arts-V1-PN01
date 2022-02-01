using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class EventImageModel : BaseServiceModel<NAT_ES_Event_Image, EventImageModel>, IObjectState
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
		public EventModel NatAsEvent { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
