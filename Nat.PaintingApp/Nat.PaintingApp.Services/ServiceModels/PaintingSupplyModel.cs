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
    public class PaintingSupplyModel : BaseServiceModel<NAT_PS_Painting_Supply, PaintingSupplyModel>, IObjectState
    {
        public Int32 PaintingSupplyId { get; set; }
        public Nullable<Int32> PaintingVideoId { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
