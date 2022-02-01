using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Services.ServiceModels.ArtistRequest
{
    public class ArtistAvailabilityModel
    {
        public Int32 AvailabilityId { get; set; }
        public Int32 PlannerId { get; set; }
        public Int32 DayOfWeekLKPId { get; set; }
        [Complex]
        public ICollection<ArtistAvailabilitySlotModel> AvailabilitySlot { get; set; }

        public IEnumerable<ArtistAvailabilitySlotModel> NatPlsAvailabilitySlot
        {
            get
            {
                return this.AvailabilitySlot;
            }
        }
    }
}
