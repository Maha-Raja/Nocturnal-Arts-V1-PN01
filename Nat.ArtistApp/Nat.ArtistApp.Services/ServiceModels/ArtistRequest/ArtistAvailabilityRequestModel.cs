using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.DataMapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels.ArtistRequest
{
    public class ArtistAvailabilityRequestModel
    {
        public Int32 DayOfWeekLKPId { get; set; }
        [Complex]
        public ICollection<ArtistAvailabilitySlotRequestModel> AvailabilitySlot { get; set; }
    }
}
