using Nat.Core.ViewModels;
using Nat.PlannerApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PlannerApp.Functions.ViewModels
{
    public class SlotSearchViewModel : BaseAutoViewModel<SlotSearchModel, SlotSearchViewModel>
    {
        public Int32 PlannerId { get; set; }
        public List<Int32> PlannerIds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
