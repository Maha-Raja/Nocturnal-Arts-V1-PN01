using Nat.Core.ViewModels;
using Nat.EventApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Functions.ViewModels
{
	public class PaintingKitItemMappingViewModel : BaseAutoViewModel<PaintingKitItemMappingModel, PaintingKitItemMappingViewModel>
    {
        public Int32 PaintingKitItemId { get; set; }
        public Nullable<Int32> PaintingId { get; set; }
        public Nullable<Int32> KitItemId { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Int32 Quantity { get; set; }
        // public PaintingViewModel NatPsPainting { get; set; }
    }
}