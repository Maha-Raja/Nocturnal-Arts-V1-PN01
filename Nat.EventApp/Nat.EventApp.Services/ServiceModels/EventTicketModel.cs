using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ServiceModels
{
    public class EventTicketModel
    {
        public String EventCode { get; set; }
        [Complex]
        public List<EventSeatModel> Seats { get; set; }
    }
}
