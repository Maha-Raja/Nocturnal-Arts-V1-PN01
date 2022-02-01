using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class CustomerEventModel : BaseServiceModel<NAT_CS_Customer_Event, CustomerEventModel>, IObjectState
	{
		public Int32 CustomerEventId { get; set; }
		public Int32 CustomerId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Nullable<Int32> EventTypeLKPId { get; set; }
		public Nullable<DateTime> StartTime { get; set; }
		public Nullable<DateTime> EndTime { get; set; }
		public String ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public Nullable<Int32> TicketsBought { get; set; }
		public Nullable<Int32> TransactionId { get; set; }
		public String UDF { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public CustomerModel NatCsCustomer { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
