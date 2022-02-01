using Nat.Core.Lookup.Model;
using System;

namespace Nat.EventApp.Services.ViewModels
{

	public class VenueLovViewModel
    {
        public string PostalCode { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine1 { get; set; }
        public string Country { get; set; }
        public Nullable<Int32> VenueId { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public Nullable<Int32> AddressId { get; set; }
        public String Address { get; set; }
        public String Name { get; set; }
        public String City { get; set; }
        public Double Rating { get; set; }
        public String VCPContactNumber { get; set; }
        public string MetroCityArea { get; set; }

    }
}
