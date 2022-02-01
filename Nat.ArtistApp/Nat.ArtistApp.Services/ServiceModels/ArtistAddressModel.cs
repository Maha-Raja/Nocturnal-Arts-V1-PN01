using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistAddressModel : BaseServiceModel<NAT_AS_Artist_Address, ArtistAddressModel>, IObjectState
	{
		public Int32 AddressId { get; set; }
        public String Address { get; set; }
        public String AddressLine1 { get; set; }
		public String AddressLine2 { get; set; }
		public String PostalZipCode { get; set; }
		public Nullable<Int32> Coordinate { get; set; }
		public String CityName { get; set; }
		public String OtherCityFlag { get; set; }
		public String CreatedBy { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<Int32> AddressGeographyId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
	}
}
