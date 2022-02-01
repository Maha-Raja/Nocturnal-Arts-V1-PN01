using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistRatingModel : BaseServiceModel<NAT_AS_Artist_Rating, ArtistRatingModel>, IObjectState
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
		public ArtistModel NatAsArtist { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
