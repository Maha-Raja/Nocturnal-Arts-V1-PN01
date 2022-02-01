using Nat.Core.ViewModels;
using Nat.PlannerApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PlannerApp.Functions.ViewModels
{    public class SlotWarningViewModel : BaseAutoViewModel<SlotWarningModel, SlotWarningViewModel>
    {
        public Int32 WarningType { get; set; }
        public String Message { get; set; }
    }
}
