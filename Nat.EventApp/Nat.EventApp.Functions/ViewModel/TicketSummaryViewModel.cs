using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class TicketSummaryViewModel : BaseAutoViewModel<TicketSummaryModel, TicketSummaryViewModel>
	{
		public String EventCode { get; set; }
		public String SeatType { get; set; }
		public String TicketStatus { get; set; }
		public Nullable<Int32> TicketCount { get; set; }
		public Nullable<Int32> TotalSeats { get; set; }
	}
}
