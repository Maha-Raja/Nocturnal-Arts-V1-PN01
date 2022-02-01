using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.CustomerApp.Functions.ViewModels
{
	public class CustomerViewModel : BaseAutoViewModel<CustomerModel, CustomerViewModel>
	{
		public Int32 CustomerId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> CustomerStatusLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<Int32> GenderLKPId { get; set; }
		public Nullable<Int32> ResidentialAddressId { get; set; }
		public Nullable<Int32> ShoppingAddressId { get; set; }
		public Nullable<Int32> BillingAddressId { get; set; }
		public String PersonFirstName { get; set; }
		public String PersonLastName { get; set; }
		public String PersonMiddleName { get; set; }
		public String PersonEmail { get; set; }
		public String PersonProfileImageLink { get; set; }
		public Nullable<DateTime> DateOfBirth { get; set; }
		public String PersonExtension { get; set; }
		public ICollection<CustomerEventViewModel> NatCsCustomerEvent { get; set; }
		public ICollection<CustomerLikedEventsViewModel> NatCsCustomerLikedEvents { get; set; }
		public CustomerAddressViewModel NatCsCustomerAddress { get; set; }
		public CustomerAddressViewModel NatCsCustomerAddress1 { get; set; }
		public CustomerAddressViewModel NatCsCustomerAddress2 { get; set; }
		public ICollection<CustomerFollowingViewModel> NatCsCustomerFollowing { get; set; }
	}
}
