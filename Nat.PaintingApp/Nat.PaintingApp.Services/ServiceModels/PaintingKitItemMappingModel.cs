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
    public class PaintingKitItemMappingModel : BaseServiceModel<NAT_Painting_Kit_Item, PaintingKitItemMappingModel>, IObjectState       
    {


        public Int32 PaintingKitItemId { get; set; }
        public Nullable<Int32> PaintingId { get; set; }
        public Nullable<Int32> KitItemId { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Int32 Quantity { get; set; }
        public PaintingModel NatPsPainting { get; set; }
        public ObjectState ObjectState { get; set; }

    }
}
