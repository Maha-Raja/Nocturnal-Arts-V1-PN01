using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ServiceModels.ViewModels
{
    public class ArtistVenuePreferenceViewModel
    {
        public int VenuePreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
    }
}
