using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ViewModels
{
    public class AvailabilitySlotViewModel
    {
        public Int32 AvailabilitySlotId { get; set; }
        public Int32 AvailabilityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public AvailabilityViewModel NatPlsAvailability { get; set; }
    }
}
