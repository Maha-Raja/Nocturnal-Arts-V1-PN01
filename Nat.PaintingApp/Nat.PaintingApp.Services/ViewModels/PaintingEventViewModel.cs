using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PaintingApp.Services.ViewModels
{
    class PaintingEventViewModel
    {
        public Int32 PaintingId { get; set; }
        public Int32? Eventsheld { get; set; }
        public DateTime? LastEventDate { get; set; }
        public String LastEventLocation { get; set; }
    }
}
