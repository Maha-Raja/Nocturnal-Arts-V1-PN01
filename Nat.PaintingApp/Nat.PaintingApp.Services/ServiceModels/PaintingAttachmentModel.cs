using Nat.Core.ServiceModels;
using Nat.PaintingApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.PaintingApp.Services.ServiceModels
{
    public class PaintingAttachmentModel : BaseServiceModel<NAT_PS_Painting_Attachment, PaintingAttachmentModel>, IObjectState
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
        public ObjectState ObjectState { get; set; }
    }
}
