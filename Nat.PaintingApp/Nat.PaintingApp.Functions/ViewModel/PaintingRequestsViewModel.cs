using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.PaintingApp.Functions.ViewModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PaintingApp.Functions.ViewModel
{
    public class PaintingRequestsViewModel : BaseAutoViewModel<PaintingRequestsModel, PaintingRequestsViewModel>
    {
        public int RequestId { get; set; }
        public string PaintingName { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public Nullable<int> ArtistId { get; set; }
        public string InspiredFrom { get; set; }
        public Nullable<int> DifficultyLevel { get; set; }
        public string VideoTutorialUrl { get; set; }
        public string StatusLkp { get; set; }
        public string JsonData { get; set; }
        public Nullable<int> Price { get; set; }
        public String Tags { get; set; }
        public string ArtistName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> OwnPaintingConsent { get; set; }
        public Nullable<bool> SellPaintingConsent { get; set; }
        public Nullable<int> NumberofMajorSteps { get; set; }
        public Nullable<int> TimeToComplete { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<bool> SpecialEventFlag { get; set; }
        public Nullable<bool> PartnerPaintingFlag { get; set; }
        public Nullable<int> Width { get; set; }
        public string CanvasSize { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool ActiveFlag { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public string PaintingMediumLKP { get; set; }
        [Complex]
        public ICollection<PaintingImageViewModel> NatPsPaintingImage { get; set; }
        [Complex]
        public ICollection<PaintingKitItemMappingModel> NatPaintingKitItem { get; set; }
        [Complex]
        public ICollection<PaintingVideoViewModel> NatPsPaintingVideo { get; set; }
        [Complex]
        public ICollection<PaintingAttachmentViewModel> Attachments { get; set; }
        public String SellConsentFileName { get; set; }
        public String OwnConsentFileName { get; set; }
        public string PaintingStatus { get; set; }
        public string Status { get; set; }
        public string Orientation { get; set; }
        public Nullable<bool> Partner { get; set; }
        public string Occasion { get; set; }
        public Nullable<int> PaintingAgeGroupTypeLKPID { get; set; }
        public Nullable<bool> SingleUse { get; set; }
        public Nullable<int> SingleUseEventID { get; set; }
    }
}
