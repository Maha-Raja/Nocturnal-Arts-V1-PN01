using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;

namespace Nat.PlannerApp.Functions.ViewModels
{
    public class EventDurationModel 
    {
        public Int32 SelectedEventPlannerId { get; set; }
        public Int32 PlannerId { get; set; }
        public String ReferenceId { get; set; }
        public ICollection<string> CollidingEventCodes { get; set; }
        public Nullable<Boolean> IsColliding { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

