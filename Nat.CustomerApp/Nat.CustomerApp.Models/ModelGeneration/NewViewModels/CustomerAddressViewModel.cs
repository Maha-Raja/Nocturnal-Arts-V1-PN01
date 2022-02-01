using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.CustomerApp.Functions.ViewModels
{
	public class CustomerAddressViewModel : BaseAutoViewModel<CustomerAddressModel, CustomerAddressViewModel>
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
		public ICollection<CustomerViewModel> NatCsCustomer { get; set; }
		public ICollection<CustomerViewModel> NatCsCustomer1 { get; set; }
		public ICollection<CustomerViewModel> NatCsCustomer2 { get; set; }
	}
}
