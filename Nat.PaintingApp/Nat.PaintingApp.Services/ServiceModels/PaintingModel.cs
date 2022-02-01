using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PaintingApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PaintingApp.Services.ServiceModels
{
	public class PaintingModel : BaseServiceModel<NAT_PS_Painting, PaintingModel>, IObjectState
	{
		public Int32 PaintingId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> ArtistId { get; set; }
		public Nullable<Int32> CurrencyId { get; set; }
		public string OrientationLKP { get; set; }
		public string PaintingStatusLKP { get; set; }
		public String PaintingName { get; set; }
		public Nullable<int> Price { get; set; }
		public String Tags { get; set; }
		public String ApprovedFlag { get; set; }
		public String FeaturedFlag { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string InspiredFrom { get; set; }
        public Nullable<int> DifficultyLevel { get; set; }
        public string VideoTutorialUrl { get; set; }
		public Nullable<bool> OwnPaintingConsent { get; set; }
		public Nullable<bool> SellPaintingConsent { get; set; }
		public Nullable<int> NumberofMajorSteps { get; set; }
		public Nullable<int> TimeToComplete { get; set; }
		public Nullable<int> Length { get; set; }
		public Nullable<bool> SpecialEventFlag { get; set; }
		public Nullable<bool> PartnerPaintingFlag { get; set; }
		public Nullable<int> Width { get; set; }
		public string CanvasSize { get; set; }
		public string Notes { get; set; }
		public string Reason { get; set; }
		public string PaintingMediumLKP { get; set; }

		// Custom Fields
		public Nullable<Int32> Eventsheld { get; set; }
        public Nullable<DateTime> LastEventDate { get; set; }
        public string LastEventLocation { get; set; }
        [Complex]
        public ICollection<PaintingImageModel> NatPsPaintingImage { get; set; }
		public ICollection<PaintingRatingLogModel> NatPsPaintingRatingLog { get; set; }
		public ICollection<PaintingRatingModel> NatPsPaintingRating { get; set; }
        [Complex]
        public ICollection<PaintingEventModel> NatAsArtistEvent { get; set; }
        [Complex]
        public ICollection<PaintingKitItemMappingModel> NatPaintingKitItem { get; set; }
        [Complex]
        public ICollection<PaintingVideoModel> NatPsPaintingVideo { get; set; }

        public ObjectState ObjectState { get; set; }
		public string StatusLkp { get; set; }
		[Complex]
		public ICollection<PaintingAttachmentModel> Attachments { get; set; }
		public String SellConsentFileName { get; set; }
		public String OwnConsentFileName { get; set; }
		public string Orientation { get; set; }
		public Nullable<bool> Partner { get; set; }
		public string Occasion { get; set; }
		public Nullable<int> PaintingAgeGroupTypeLKPID { get; set; }
		public Nullable<bool> SingleUse { get; set; }
		public Nullable<int> SingleUseEventID { get; set; }

		// Not in DB

		public string PaintingStatus { get; set; }

		public int TicketSold { get; set; }

	}
}
