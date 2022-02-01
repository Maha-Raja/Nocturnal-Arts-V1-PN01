using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ServiceModels
{
    public class FindReplacementVenueQueryModel
    {
        public string EventCode { get; set; }
        public int VenueId { get; set; }
        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}
