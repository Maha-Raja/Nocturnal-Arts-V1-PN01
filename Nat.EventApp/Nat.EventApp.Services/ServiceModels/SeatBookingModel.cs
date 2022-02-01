using Nat.Core.Lookup.Model;
using System;

namespace Nat.EventApp.Services.ViewModels
{
	public class SeatBookingModel
	{
        public String EventCode { get; set; }
        public String Status { get; set; }
		public String SeatNumber { get; set; }
		public String RowNumber { get; set; }
    }
}
