using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
	public class FetchEmailViewModel
	{
		public DateTime? Begin { get; set; }
		public DateTime? End { get; set; }
		public List<string> Emails { get; set; }
		public List<NotificationQueueMessageEntityModel> SentEmails { get; set; }
		public List<NotificationQueueMessageEntityModel> FailedEmails { get; set; }
	}
}
