using Nat.Core.ServiceModels;
using Nat.EventApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.EventApp.Services.ServiceModels
{
    public class EventTicketPriceModel : BaseServiceModel<NAT_ES_Event_Ticket_Price, EventTicketPriceModel>, IObjectState
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
        public ObjectState ObjectState { get; set; }

        public EventModel NatEsEvent { get; set; }
    }
}
