using Nat.Core.ViewModels;
using Nat.VenueApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Functions.ViewModel
{
    public class VenueMetroCityMappingViewModel : BaseAutoViewModel<VenueMetroCityMappingModel, VenueMetroCityMappingViewModel>
    {
        public int VenueMetroCityMappingId { get; set; }
        public int VenueId { get; set; }
        public string MetroCityLKPId { get; set; }
    }
}
