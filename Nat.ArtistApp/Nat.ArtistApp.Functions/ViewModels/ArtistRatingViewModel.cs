using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistRatingViewModel : BaseAutoViewModel<ArtistRatingModel, ArtistRatingViewModel>
	{
		public Int32 ArtistRatingId { get; set; }
		public Nullable<Int32> ArtistId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Double AverageRatingValue { get; set; }
        public Nullable<Int32> NumberOfRatings { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ArtistViewModel NatAsArtist { get; set; }
	}
}
