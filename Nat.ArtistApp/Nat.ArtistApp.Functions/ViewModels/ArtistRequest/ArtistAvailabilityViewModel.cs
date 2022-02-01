using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.ArtistApp.Functions.ViewModels.ArtistRequest
{
    public class ArtistAvailabilityViewModel
    {
        public Int32 AvailabilityId { get; set; }
        public Int32 PlannerId { get; set; }
        public Int32 DayOfWeekLKPId { get; set; }
        [Complex]
        public ICollection<ArtistAvailabilitySlotViewModel> AvailabilitySlot { get; set; }
    }
}
