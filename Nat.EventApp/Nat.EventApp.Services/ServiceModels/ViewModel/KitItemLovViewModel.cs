using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ViewModels
{
    public class KitItemLovViewModel
    {
        public int KitItemId { get; set; }
        public int ItemCategory { get; set; }
        public string KitItemCode { get; set; }
        public string KitItemName { get; set; }
        public string StoreId { get; set; }
    }
}
