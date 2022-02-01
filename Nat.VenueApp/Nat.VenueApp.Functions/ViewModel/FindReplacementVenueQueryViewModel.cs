using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.ViewModels;
using Nat.VenueApp.Services.ServiceModels;

namespace Nat.VenueApp.Functions.ViewModel
{
    class FindReplacementVenueQueryViewModel : BaseAutoViewModel<FindReplacementVenueQueryModel, FindReplacementVenueQueryViewModel>
    {
        public string EventCode { get; set; }
        public int VenueId { get; set; }
        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}
