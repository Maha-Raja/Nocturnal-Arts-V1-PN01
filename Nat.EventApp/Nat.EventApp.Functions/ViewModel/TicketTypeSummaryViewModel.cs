using Nat.Core.ViewModels;
using Nat.EventApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Functions.ViewModel
{
    public class TicketTypeSummaryViewModel : BaseAutoViewModel<TicketTypeSummaryModel, TicketTypeSummaryViewModel>
    {
        public string Type { get; set; }
        public int Total { get; set; }
        public int Sold { get; set; }
        public int Available { get
            {
                return Total - Sold;
            } 
        }
    }
}
