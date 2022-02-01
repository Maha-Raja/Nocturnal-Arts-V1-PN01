using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistLocationMappingViewModel : BaseAutoViewModel<ArtistLocationMappingModel, ArtistLocationMappingViewModel>
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
	}
}
