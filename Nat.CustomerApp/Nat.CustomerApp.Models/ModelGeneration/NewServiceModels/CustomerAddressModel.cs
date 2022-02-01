using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class CustomerAddressModel : BaseServiceModel<NAT_CS_Customer_Address, CustomerAddressModel>, IObjectState
	{
		public Int32 AddressId { get; set; }
		public String AddressLine1 { get; set; }
		public String AddressLine2 { get; set; }
		public Nullable<Int32> PostalZipCode { get; set; }
		public Nullable<Int32> Coordinate { get; set; }
		public String CityName { get; set; }
		public String OtherCityFlag { get; set; }
		public String CreatedBy { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public String AddressGeographyCode { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public ICollection<CustomerModel> NatCsCustomer { get; set; }
		public ICollection<CustomerModel> NatCsCustomer1 { get; set; }
		public ICollection<CustomerModel> NatCsCustomer2 { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
