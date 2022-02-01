using Nat.Core.ViewModels;
using Nat.PaintingApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PaintingApp.Functions.ViewModel
{
    public class PaintingAttachmentViewModel : BaseAutoViewModel<PaintingAttachmentModel, PaintingAttachmentViewModel>
    {
        public Int32 PaintingAttachmentID { get; set; }
        public Nullable<Int32> PaintingID { get; set; }
        public String AttachmentName { get; set; }
        public String AttachmentUrl { get; set; }
        public String AttachmentType { get; set; }
        public String FileType { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Boolean ActiveFlag { get; set; }
    }
}
