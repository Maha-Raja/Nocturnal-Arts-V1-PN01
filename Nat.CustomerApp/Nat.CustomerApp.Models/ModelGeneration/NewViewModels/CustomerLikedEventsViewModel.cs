using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.CustomerApp.Functions.ViewModels
{
	public class CustomerLikedEventsViewModel : BaseAutoViewModel<CustomerLikedEventsModel, CustomerLikedEventsViewModel>
	{
		public Int32 CustomerLikedEventsId { get; set; }
		public String EventCode { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Int32 CustomerId { get; set; }
		public CustomerViewModel NatCsCustomer { get; set; }
	}
}
