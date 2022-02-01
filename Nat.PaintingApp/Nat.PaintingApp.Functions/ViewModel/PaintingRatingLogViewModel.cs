using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PaintingApp.Functions.ViewModels
{
	public class PaintingRatingLogViewModel : BaseAutoViewModel<PaintingRatingLogModel, PaintingRatingLogViewModel>
	{
		public Int32 PaintingRatingLogId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> PaintingId { get; set; }
		public Nullable<DateTime> ReviewDate { get; set; }
		public String ReviewTitle { get; set; }
		public String ReviewDetails { get; set; }
		public Nullable<Int32> RatingValue { get; set; }
		public String ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PaintingViewModel NatPsPainting { get; set; }
	}
}
