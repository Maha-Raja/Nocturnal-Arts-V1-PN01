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
    public class EventTicketPriceViewModel : BaseAutoViewModel<EventTicketPriceModel, EventTicketPriceViewModel>
    {
        public Int32 EventTicketPriceID { get; set; }
        public Int32 EventID { get; set; }
        public String SeatTypeLKP { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public String LastUpdatedBy { get; set; }

        public EventViewModel NatEsEvent { get; set; }
    }
}
