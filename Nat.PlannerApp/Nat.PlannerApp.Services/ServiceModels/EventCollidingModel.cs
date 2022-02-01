using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;

namespace Nat.PlannerApp.Functions.ViewModels
{
    public class EventCollidingModel 
    {

        [Complex]
        public ICollection<EventDurationModel> SelectedEvents { get; set; }

        [Complex]
        public ICollection<EventModel> CollidingEvents { get; set; }
    }
}
