using Nat.Core.Lookup.Model;
using System;

namespace Nat.EventApp.Services.ServiceModels.ViewModel
{
    class VenueImageViewModel
    {
        public Int32 VenueImageId { get; set; }
        public Nullable<Int32> TenantId { get; set; }
        public Nullable<Int32> VenueId { get; set; }
        public String ImageTypeLKPId { get; set; }
        public String ImagePath { get; set; }

    }

}










