using Nat.Core.ViewModels;
using Nat.EventApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Functions.ViewModel
{
    class EventFeedbackViewModel : BaseAutoViewModel<EventFeedbackModel, EventFeedbackViewModel>
    {

        public Int32 EventFeedbackId { get; set; }
        public Int32 EventId { get; set; }
        public Int32 NoofAttendees { get; set; }
        public Double VenueRating { get; set; }
        public String ImagePath { get; set; }
        public String Comment { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        


    }
}
