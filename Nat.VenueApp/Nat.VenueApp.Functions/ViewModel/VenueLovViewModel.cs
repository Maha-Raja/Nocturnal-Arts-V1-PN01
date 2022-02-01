using Nat.Core.ViewModels;
using Nat.VenueApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Functions.ViewModel
{
    public class VenueLovViewModel : BaseAutoViewModel<VenueLovModel, VenueLovViewModel>
    {
        public string PostalCode { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine1 { get; set; }
        public string Country { get; set; }
        public Int32 VenueId { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public Int32 AddressId { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Name { get; set; }
        public Nullable<Double> Rating { get; set; }
        public String VCPContactNumber { get; set; }
        public string MetroCityArea { get; set; }
    }
}
