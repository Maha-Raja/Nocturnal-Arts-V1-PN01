using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ServiceModels
{
    public class BookingModel
    {
        public Int32 BookingId { get; set; }
        public String BookingNumber { get; set; }
        public String EventCode { get; set; }
        public Nullable<Int32> CustomerId { get; set; }
        public String StatusLkp { get; set; }
        public String PdfUrl { get; set; }
        public String CancellationReason { get; set; }
        public Int32 TicketCount { get; set; }
        public String PaymentNumber { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public Decimal PaidAmount { get; set; }
        public Boolean PaymentSuccessful { get; set; }
        public Nullable<DateTime> BookingDate { get; set; }
        public Nullable<DateTime> BookingExpiryDate { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public String EventName { get; set; }

        [Complex]
        public List<TicketModel> Tickets { get; set; }
    }
}
