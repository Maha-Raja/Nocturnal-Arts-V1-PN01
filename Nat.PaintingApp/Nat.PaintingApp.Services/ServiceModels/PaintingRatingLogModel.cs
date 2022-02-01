using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PaintingApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PaintingApp.Services.ServiceModels
{
	public class PaintingRatingLogModel : BaseServiceModel<NAT_PS_Painting_Rating_Log, PaintingRatingLogModel>, IObjectState
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
		public PaintingModel NatPsPainting { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
