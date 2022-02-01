using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistRatingLogModel : BaseServiceModel<NAT_AS_Artist_Rating_Log, ArtistRatingLogModel>, IObjectState
	{
		public Int32 ArtistRatingLogId { get; set; }
		public Nullable<Int32> ArtistId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> CustomerId { get; set; }
		public Nullable<DateTime> ReviewDate { get; set; }
		public String ReviewDetail { get; set; }
		public double RatingValue { get; set; }
		public String ReviewTitle { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ArtistModel NatAsArtist { get; set; }
		public ObjectState ObjectState { get; set; }
        public String CustomerName { get; set; }
        public String CustomerProfileImageUrl { get; set; }
    }
}
