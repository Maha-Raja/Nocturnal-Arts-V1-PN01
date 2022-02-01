using Nat.Core.Lookup.Model;
using System;

namespace Nat.VenueApp.Services.ViewModels
{
	public class VenueEventViewModel
    {
        public Int32 VenueId { get; set; }
        public Int32 Eventsheld { get; set; }
        public DateTime? LastEventDate { get; set; }
        public Int32 UpcommingEvents { get; set; }
    }
}
