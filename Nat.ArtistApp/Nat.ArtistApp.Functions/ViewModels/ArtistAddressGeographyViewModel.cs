using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistAddressGeographyViewModel : BaseAutoViewModel<ArtistAddressGeographyModel, ArtistAddressGeographyViewModel>
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
		public ICollection<ArtistAddressViewModel> NatAsArtistAddress { get; set; }
		public ICollection<ArtistAddressGeographyViewModel> NatAsArtistChildAddressGeography { get; set; }
		public ArtistAddressGeographyViewModel NatAsArtistParentAddressGeography { get; set; }
	}
}
