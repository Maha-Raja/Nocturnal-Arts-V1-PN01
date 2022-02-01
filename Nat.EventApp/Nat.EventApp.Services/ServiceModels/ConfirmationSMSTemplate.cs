using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class ConfirmationSMSTemplate
    {
        public string PhoneNumber { get; set; }
        public string Date { get; set; }
        public string EventName { get; set; }
        public string VenueName { get; set; }
        public string BookingPdfUrl { get; set; }
    }
}
