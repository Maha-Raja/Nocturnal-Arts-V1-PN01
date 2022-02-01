using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.DataMapper;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Functions.ViewModels.ArtistRequest
{
    public class ArtistAvailabilityRequestViewModel
    {
        public Int32 DayOfWeekLKPId { get; set; }
        [Complex]
        public ICollection<ArtistAvailabilitySlotRequestViewModel> AvailabilitySlot { get; set; }
    }
}
