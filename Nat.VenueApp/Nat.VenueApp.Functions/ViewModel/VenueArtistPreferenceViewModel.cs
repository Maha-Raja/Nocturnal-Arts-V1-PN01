using Nat.Core.ViewModels;
using Nat.VenueApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Functions.ViewModel
{
    public class VenueArtistPreferenceViewModel : BaseAutoViewModel<VenueArtistPreferenceModel, VenueArtistPreferenceViewModel>
    {
        public int ArtistPreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
    }
}
