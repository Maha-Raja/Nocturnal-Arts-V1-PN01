using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class FindReplacementArtistQueryModel
	{
		public string EventCode { get; set; }
		public int ArtistId { get; set; }
		public DateTime EventStartTime { get; set; }
		public DateTime EventEndTime { get; set; }
		public int TotalTicketsSold { get; set; }
		public string LocationCode { get; set; }
	}
}
