using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PaintingApp.Functions.ViewModels
{
	public class PaintingViewModel : BaseAutoViewModel<PaintingModel, PaintingViewModel>
	{
		public Int32 PaintingId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> ArtistId { get; set; }
		public Nullable<Int32> CurrencyId { get; set; }
		public Nullable<Int32> OrientationLKPId { get; set; }
		public Nullable<Int32> PaintingStatusLKPId { get; set; }
		public String PaintingName { get; set; }
		public Nullable<Double> Price { get; set; }
		public String Tags { get; set; }
		public String ApprovedFlag { get; set; }
		public String FeaturedFlag { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ICollection<PaintingImageViewModel> NatAsPaintingImage { get; set; }
		public ICollection<PaintingRatingLogViewModel> NatAsPaintingRatingLog { get; set; }
		public ICollection<PaintingRatingViewModel> NatAsPaintingRating { get; set; }
	}
}
