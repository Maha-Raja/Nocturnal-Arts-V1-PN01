using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistAddressGeographyModel : BaseServiceModel<NAT_AS_Artist_Address_Geography, ArtistAddressGeographyModel>, IObjectState
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
		public ICollection<ArtistAddressModel> NatAsArtistAddress { get; set; }
		public ICollection<ArtistAddressGeographyModel> NatAsArtistChildAddressGeography { get; set; }
		public ArtistAddressGeographyModel NatAsArtistParentAddressGeography { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
