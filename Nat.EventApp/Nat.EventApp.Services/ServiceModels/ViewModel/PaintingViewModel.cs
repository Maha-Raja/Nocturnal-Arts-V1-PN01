using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ViewModels
{
	public class PaintingViewModel
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
		public Nullable<DateTime> CreatedDate { get; set; }
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
		public string Notes { get; set; }
		public string Reason { get; set; }
		public string PaintingMediumLKP { get; set; }

		// Custom Fields
		public Nullable<Int32> Eventsheld { get; set; }
		public Nullable<DateTime> LastEventDate { get; set; }
		public string LastEventLocation { get; set; }
		public string StatusLkp { get; set; }
		public String SellConsentFileName { get; set; }
		public String OwnConsentFileName { get; set; }

		// Not in DB

		public string PaintingStatus { get; set; }

		public int TicketSold { get; set; }

		public ICollection<PaintingKitItemMappingViewModel> NatPaintingKitItem { get; set; }
	}
}
