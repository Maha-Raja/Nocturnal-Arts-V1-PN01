using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.CustomerApp.Services.ServiceModels
{
    public class CustomerModel : BaseServiceModel<NAT_CS_Customer, CustomerModel>, IObjectState
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
        public ICollection<CustomerEventModel> NatCsCustomerEvent { get; set; }
        [Complex]
        public ICollection<CustomerLikedEventsModel> NatCsCustomerLikedEvents { get; set; }
        [Complex]
        public CustomerAddressModel NatCsCustomerResidentialAddress { get; set; }
        public CustomerAddressModel NatCsCustomerShippingAddress { get; set; }
        public CustomerAddressModel NatCsCustomerBillingAddress { get; set; }
        [Complex]
        public ICollection<CustomerFollowingModel> NatCsCustomerFollowing { get; set; }
        public ObjectState ObjectState
        {
            get; set;
        }

        //Custom Field
        public String Password { get; set; }
        public String CountryCode { get; set; }
        public String ContactNumber { get; set; }
        public Nullable<bool> HasDisability { get; set; }
    }
}
