using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.Core.ViewModels;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class EventCollidingViewModel : BaseAutoViewModel<EventCollidingModel, EventCollidingViewModel>
	{

        [Complex]
        public ICollection<EventDurationViewModel> SelectedEvents { get; set; }

        [Complex]
        public ICollection<EventViewModel> CollidingEvents { get; set; }
    }
}
