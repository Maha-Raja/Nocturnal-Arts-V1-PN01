using System;
using Nat.VenueApp.Models.EFModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
    public class VenueMetroCityMappingModel : BaseServiceModel<NAT_VS_Venue_Metro_City_Mapping, VenueMetroCityMappingModel>, IObjectState
    {
        public int VenueMetroCityMappingId { get; set; }
        public int VenueId { get; set; }
        public string MetroCityLKPId { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
