using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PaintingApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PaintingApp.Services.ServiceModels
{
	public class PaintingRatingModel : BaseServiceModel<NAT_PS_Painting_Rating, PaintingRatingModel>, IObjectState
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
		public PaintingModel NatAsPainting { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
