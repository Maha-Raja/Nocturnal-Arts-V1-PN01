using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PlannerApp.Services.ServiceModels
{
    public class SlotSearchModel
    {
        public Nullable<Int32> PlannerId { get; set; }
        public List<Int32> PlannerIds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
