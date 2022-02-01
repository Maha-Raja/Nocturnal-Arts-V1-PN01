using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.LocationApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.LocationApp.Models
{
	public class AddressGeographyModel : BaseServiceModel<NAT_LS_Address_Geography, AddressGeographyModel>, IObjectState
	{
		public Int32 AddressGeographyId { get; set; }
		public Nullable<Int32> ParentGeographyId { get; set; }
		public String GeographyDescription { get; set; }
		public Nullable<Int32> GeographyShortCode { get; set; }
		public Nullable<Int32> GeographyTypeLKPId { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<AddressGeographyModel> NatLsChildAddressGeography { get; set; }
		public AddressGeographyModel NatLsParentAddressGeography { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
