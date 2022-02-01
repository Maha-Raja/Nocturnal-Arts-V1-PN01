using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistLocationMappingModel : BaseServiceModel<NAT_AS_Artist_Location_Mapping, ArtistLocationMappingModel>, IObjectState
	{
		public int ArtistLocationMappingId { get; set; }
		public int ArtistId { get; set; }
		public string LocationCode { get; set; }
		public Nullable<int> PlannerId { get; set; }
		public string LocationName { get; set; }
		public Nullable<bool> ActiveFlag { get; set; }
		public Nullable<System.DateTime> EffectiveStartDate { get; set; }
		public Nullable<System.DateTime> EffectiveEndDate { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> CreatedDate { get; set; }
		public string LastUpdatedBy { get; set; }
		public Nullable<System.DateTime> LastUpdatedDate { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
