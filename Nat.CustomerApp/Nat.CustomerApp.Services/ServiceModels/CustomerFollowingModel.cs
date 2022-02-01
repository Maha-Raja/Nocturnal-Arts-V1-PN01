using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class CustomerFollowingModel : BaseServiceModel<NAT_CS_Customer_Following, CustomerFollowingModel>, IObjectState
	{
		public Int32 CustomerFollowingId { get; set; }
		public Nullable<Int32> ReferenceId { get; set; }
		public String ReferenceType { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> CustomerId { get; set; }
        public CustomerModel NatCsCustomer { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
