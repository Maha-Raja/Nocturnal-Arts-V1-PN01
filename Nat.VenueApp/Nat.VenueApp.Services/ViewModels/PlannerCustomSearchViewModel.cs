using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ViewModels
{
    public class PlannerCustomSearchViewModel
    {
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Int32 ReferenceType { get; set; }
        public List<Int32> PlannerIds { get; set; }
    }
}
