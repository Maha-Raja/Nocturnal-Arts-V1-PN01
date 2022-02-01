using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
	public class PaintingKitItemMappingModel
	{
        public Int32 PaintingKitItemId { get; set; }
        public Nullable<Int32> PaintingId { get; set; }
        public Nullable<Int32> KitItemId { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Int32 Quantity { get; set; }
    }
}
