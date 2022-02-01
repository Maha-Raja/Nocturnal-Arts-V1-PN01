using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ServiceModels.ViewModel
{
    public class EventDurationViewModel
    {
        public Int32 SelectedEventPlannerId { get; set; }
        public Int32 PlannerId { get; set; }
        public String ReferenceId { get; set; }
        [Complex]
        public ICollection<string> CollidingEventCodes { get; set; }
        public Nullable<Boolean> IsColliding { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
