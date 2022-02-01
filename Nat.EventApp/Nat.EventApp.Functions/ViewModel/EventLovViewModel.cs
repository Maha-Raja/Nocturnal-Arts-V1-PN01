using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.EventApp.Services.ServiceModels;

namespace Nat.EventApp.Functions.ViewModel
{
    public class EventLovViewModel : BaseAutoViewModel<EventLovModel, EventLovViewModel>
    {
        public Int32 EventId { get; set; }
        public String EventCode { get; set; }
        public String EventName { get; set; }
        public Nullable<int> ArtistId { get; set; }
        public Nullable<int> VenueId { get; set; }
        public Nullable<System.DateTime> EventStartTime { get; set; }
        public Nullable<System.DateTime> EventEndTime { get; set; }
    }
}
