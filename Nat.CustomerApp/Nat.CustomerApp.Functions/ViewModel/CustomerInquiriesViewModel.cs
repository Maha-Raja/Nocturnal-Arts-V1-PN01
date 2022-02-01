using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.ViewModels;
using Nat.CustomerApp.Services.ServiceModels;



namespace Nat.CustomerApp.Functions.ViewModel
{
    public class CustomerInquiriesViewModel : BaseAutoViewModel<CustomerInquiriesModel, CustomerInquiriesViewModel>
    {

        public Int32 Id { get; set; }

        public string RequestId { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public bool ActiveFlag { get; set; }
    }
}
