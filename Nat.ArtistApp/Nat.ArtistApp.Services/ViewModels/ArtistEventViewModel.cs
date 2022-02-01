using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class ArtistEventViewModel
    {
        public Int32 ArtistId { get; set; }
        public Int32? Eventsheld { get; set; }
        public DateTime? LastEventDate { get; set; }
        public Int32? UpcommingEvents { get; set; }
    }
}
