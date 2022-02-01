using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistRatingLogViewModel : BaseAutoViewModel<ArtistRatingLogModel, ArtistRatingLogViewModel>
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
		public ArtistViewModel NatAsArtist { get; set; }

        public String CustomerName { get; set; }
        public String CustomerProfileImageUrl { get; set; }
    }
}
