using Nat.Core.ServiceModels;
using Nat.PaintingApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.PaintingApp.Services.ServiceModels
{
    public class PaintingVideoModel : BaseServiceModel<NAT_PS_Painting_Video, PaintingVideoModel>, IObjectState
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
        public ICollection<PaintingSupplyModel> NatPsPaintingSupply { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
