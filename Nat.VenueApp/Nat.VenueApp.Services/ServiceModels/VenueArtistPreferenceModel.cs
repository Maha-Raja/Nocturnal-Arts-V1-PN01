using Nat.Core.ServiceModels;
using Nat.VenueApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.VenueApp.Services.ServiceModels
{
    public class VenueArtistPreferenceModel : BaseServiceModel<NAT_VS_Venue_Artist_Preference, VenueArtistPreferenceModel>, IObjectState
    {
        public int ArtistPreferenceId { get; set; }
        public bool ActiveFlag { get; set; }
        public int VenueId { get; set; }
        public int ArtistId { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
