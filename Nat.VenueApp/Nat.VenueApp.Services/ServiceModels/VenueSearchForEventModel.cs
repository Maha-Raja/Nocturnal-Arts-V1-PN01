using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ServiceModels
{
    public class VenueSearchForEventModel
    {
        public Nullable<DateTime> AvailabilityStartTime { get; set; }
        public Nullable<DateTime> AvailabilityEndTime { get; set; }
        public Nullable<int> SkillLKPId { get; set; }
        public Nullable<float> MaxRating { get; set; }
        public Nullable<float> MinRating { get; set; }
        public String SearchText { get; set; }
        public Nullable<int> skip { get; set; }
        public Nullable<int> take { get; set; }
        public String LocationCode { get; set; }
    }
}
