using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PaintingApp.Functions.ViewModel
{
    public class PaintingVideoViewModel : BaseAutoViewModel<PaintingVideoModel, PaintingVideoViewModel>
    {
        public Int32 PaintingVideoId { get; set; }
        public Nullable<Int32> PaintingId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        [Complex]
        public ICollection<PaintingSupplyViewModel> NatPsPaintingSupply { get; set; }

        
    }
}
