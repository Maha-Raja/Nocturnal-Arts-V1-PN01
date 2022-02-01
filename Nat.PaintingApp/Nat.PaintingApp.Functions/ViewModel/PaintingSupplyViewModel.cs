using Nat.Core.ViewModels;
using Nat.PaintingApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PaintingApp.Functions.ViewModel
{
    public class PaintingSupplyViewModel : BaseAutoViewModel<PaintingSupplyModel, PaintingSupplyViewModel>
    {
        public Int32 PaintingSupplyId { get; set; }
        public Nullable<Int32> PaintingVideoId { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
    }
}
