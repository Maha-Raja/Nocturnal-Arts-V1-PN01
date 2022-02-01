using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class TicketModel
    {
        public Int32 TicketId { get; set; }
        public Int32 BookingId { get; set; }
        public String TicketCode { get; set; }
        public String SeatNumber { get; set; }
        public String RowNumber { get; set; }
        public String TicketTypeLkp { get; set; }
        public String CancellationReason { get; set; }
        public String StatusLkp { get; set; }
        public String CustomerName { get; set; }
        public String CustomerEmail { get; set; }
        public String CustomerContact { get; set; }
        public Int32 CustomerAge { get; set; }
        public String ImageUrl { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }

    }
}
