using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class TicketTypeSummaryModel
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Sold { get; set; }
        public string Status { get; set; }
    }
}
