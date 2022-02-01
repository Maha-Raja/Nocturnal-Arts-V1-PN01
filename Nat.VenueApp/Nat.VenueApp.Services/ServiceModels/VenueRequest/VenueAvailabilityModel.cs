using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Services.ServiceModels.VenueRequest
{
    public class VenueAvailabilityModel
    {

        public Int32 AvailabilityId { get; set; }
        public Int32 PlannerId { get; set; }
        public Int32 DayOfWeekLKPId { get; set; }
        [Complex]
        public ICollection<VenueAvailabilitySlotModel> AvailabilitySlot { get; set; }

        public IEnumerable<VenueRequest.VenueAvailabilitySlotModel> NatPlsAvailabilitySlot { get; set; }
    }
}
