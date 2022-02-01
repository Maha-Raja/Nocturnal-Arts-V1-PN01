using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ViewModels
{
	public class PaintingKitItemModel
	{
        public int PaintingKitItemId { get; set; }
        public int? PaintingId { get; set; }
        public int KitItemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int Quantity { get; set; }
        [Complex]
        public KitItemModel KitItem { get; set; }
    }
}
