using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.CustomerApp.Functions.ViewModels
{
	public class PersonViewModel : BaseAutoViewModel<PersonModel, PersonViewModel>
	{
		public Int32 PersonId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
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
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<CustomerViewModel> NatAsCustomer { get; set; }
	}
}
