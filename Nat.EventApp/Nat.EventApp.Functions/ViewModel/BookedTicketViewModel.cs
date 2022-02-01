using Nat.Core.ViewModels;
using Nat.EventApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Functions.ViewModel
{
    public class BookedTicketViewModel : BaseAutoViewModel<BookedTicketModel, BookedTicketViewModel>
    {
        public int BookingId { get; set; }
        public int TicketId { get; set; }
        public string TicketCode { get; set; }
        public string RowNumber { get; set; }
        public string SeatNumber { get; set; }
        public string CancellationReason { get; set; }
        public string TicketTypeLKP { get; set; }
        public string TicketType { get; set; }
        public string StatusLKP { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContact { get; set; }
        public Nullable<int> CustomerAge { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<bool> CheckedIn { get; set; }
        public Nullable<System.DateTime> CheckedInTime { get; set; }
        public Nullable<int> CheckedInBy { get; set; }
        public string QRCodeImageUrl { get; set; }
        public Nullable<int> EventPaintingId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool ActiveFlag { get; set; }
        public Nullable<bool> HasDisability { get; set; }
        public Nullable<System.DateTime> BookingDate { get; set; }
        public string BookingNumber { get; set; }
        public string EventCode { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string EventName { get; set; }
        public Nullable<System.DateTime> EventStartTime { get; set; }
        public Nullable<System.DateTime> EventEndTime { get; set; }
        public Nullable<int> EventId { get; set; }
        public string LocationCode { get; set; }
        public Nullable<bool> Virtual { get; set; }
        public string EventStatus { get; set; }
    }
}
