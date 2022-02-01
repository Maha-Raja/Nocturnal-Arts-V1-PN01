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
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Nullable<bool> PhoneVerified { get; set; }
        public Nullable<bool> EmailVerified { get; set; }
        public String PersonProfileImageLink { get; set; }
        public Nullable<DateTime> DateOfBirth { get; set; }
        public String PersonExtension { get; set; }
        [Complex]
        public ICollection<CustomerEventViewModel> NatCsCustomerEvent { get; set; }
        [Complex]
        public ICollection<CustomerLikedEventsViewModel> NatCsCustomerLikedEvents { get; set; }
        [Complex]
        public CustomerAddressViewModel NatCsCustomerResidentialAddress { get; set; }
        public CustomerAddressViewModel NatCsCustomerShippingAddress { get; set; }
        public CustomerAddressViewModel NatCsCustomerBillingAddress { get; set; }
        [Complex]
        public ICollection<CustomerFollowingViewModel> NatCsCustomerFollowing { get; set; }
        //Custom Field
        public String Password { get; set; }
        public String CountryCode { get; set; }
        public String ContactNumber { get; set; }
        public Nullable<bool> HasDisability { get; set; }
    }
}
