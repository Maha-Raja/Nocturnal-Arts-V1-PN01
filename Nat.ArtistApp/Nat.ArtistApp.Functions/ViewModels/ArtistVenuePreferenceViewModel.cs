using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Functions.ViewModels
{
    public class ArtistVenuePreferenceViewModel : BaseAutoViewModel<ArtistVenuePreferenceModel, ArtistVenuePreferenceViewModel>
    {
        public int VenuePreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
    }
}
