using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PaintingApp.Functions.ViewModels
{
	public class PaintingRatingViewModel : BaseAutoViewModel<PaintingRatingModel, PaintingRatingViewModel>
	{
		public Int32 PaintingRatingId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> PaintingId { get; set; }
		public Nullable<Double> AverageRatingValue { get; set; }
		public Nullable<Int32> NumberOfRatings { get; set; }
		public String ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PaintingViewModel NatAsPainting { get; set; }
	}
}
