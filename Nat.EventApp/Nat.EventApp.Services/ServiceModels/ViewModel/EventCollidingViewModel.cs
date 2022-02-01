using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ServiceModels.ViewModel
{
    public class EventCollidingViewModel
    {
        [Complex]
        public ICollection<EventDurationViewModel> SelectedEvents { get; set; }

        [Complex]
        public ICollection<string> CollidingEventCodes { get; set; }
    }
}
