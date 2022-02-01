using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class AvailabilityViewModel
    {
        public Int32 AvailabilityId { get; set; }
        public Int32 PlannerId { get; set; }
        public Int32 DayOfWeekLKPId { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public Nullable<DateTime> EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveStartTime { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public PlannerViewModel NatPlsPlanner { get; set; }
        [Complex]
        public ICollection<AvailabilitySlotViewModel> NatPlsAvailabilitySlot { get; set; }

        public ICollection<AvailabilitySlotViewModel> AvailabilitySlot
        {
            set
            {
                this.NatPlsAvailabilitySlot = value;
            }
        }

    }
}
