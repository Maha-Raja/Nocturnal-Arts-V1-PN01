using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PaintingApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PaintingApp.Services.ServiceModels
{
	public class PaintingImageModel : BaseServiceModel<NAT_PS_Painting_Image, PaintingImageModel>, IObjectState
	{
		public Int32 PaintingImageId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> PaintingId { get; set; }
		public Nullable<Int32> ImageTypeLKPId { get; set; }
		public String ImagePath { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public PaintingModel NatPsPainting { get; set; }
        public string PublicURL { get; set; }
        public string OriginalImagePath { get; set; }
        public ObjectState ObjectState { get; set; }
	}
}
