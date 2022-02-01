using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class TicketSummaryModel : BaseServiceModel<NAT_TICKET_SUMMARY_VW, TicketSummaryModel>, IObjectState
	{
		public String EventCode { get; set; }
		public String SeatType { get; set; }
		public String TicketStatus { get; set; }
		public Nullable<Int32> TicketCount { get; set; }
		public Nullable<Int32> TotalSeats { get; set; }
		public ObjectState ObjectState { get; set; }
		public long ID { get; set; }
	}
}
