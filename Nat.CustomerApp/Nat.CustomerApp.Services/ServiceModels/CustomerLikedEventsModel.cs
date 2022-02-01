using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class CustomerLikedEventsModel : BaseServiceModel<NAT_CS_Customer_Liked_Events, CustomerLikedEventsModel>, IObjectState
	{
		public Int32 CustomerLikedEventsId { get; set; }
		public String EventCode { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Int32 CustomerId { get; set; }
		public CustomerModel NatCsCustomer { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
