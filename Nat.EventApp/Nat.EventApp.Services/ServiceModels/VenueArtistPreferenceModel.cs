using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class VenueArtistPreferenceModel
    {
        public int ArtistPreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
    }
}
