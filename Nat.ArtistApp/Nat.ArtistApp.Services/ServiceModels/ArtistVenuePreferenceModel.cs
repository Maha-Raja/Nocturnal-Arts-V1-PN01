using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class ArtistVenuePreferenceModel : BaseServiceModel<NAT_AS_Artist_Venue_Preference, ArtistVenuePreferenceModel>, IObjectState
    {
        public int VenuePreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
