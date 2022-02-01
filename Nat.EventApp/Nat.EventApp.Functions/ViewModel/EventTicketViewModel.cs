using Nat.Core.ViewModels;
using Nat.EventApp.Functions.ViewModels;
using Nat.EventApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Functions.ViewModel
{
    public class EventTicketViewModel : BaseAutoViewModel<EventTicketModel, EventTicketViewModel>
    {
        public String EventCode { get; set; }
        [Complex]
        public List<EventSeatViewModel> Seats { get; set; }
    }
}
