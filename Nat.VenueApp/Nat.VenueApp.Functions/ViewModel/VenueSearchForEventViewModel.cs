using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.ViewModels;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;


namespace Nat.VenueApp.Functions.ViewModels
{
    public class VenueSearchForEventViewModel : BaseAutoViewModel<VenueSearchForEventModel, VenueSearchForEventViewModel>
    {
        public Nullable<DateTime> AvailabilityStartTime { get; set; }
        public Nullable<DateTime> AvailabilityEndTime { get; set; }
        public Nullable<int> SkillLKPId { get; set; }
        public Nullable<float> MaxRating { get; set; }
        public Nullable<float> MinRating { get; set; }
        public String SearchText { get; set; }
        public String LocationCode { get; set; }
        public Nullable<int> skip { get; set; }
        public Nullable<int> take { get; set; }
    }

}
