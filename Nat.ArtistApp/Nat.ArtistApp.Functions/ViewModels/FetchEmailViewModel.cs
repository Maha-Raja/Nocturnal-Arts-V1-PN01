using Nat.ArtistApp.Services.ViewModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class FetchEmailViewModel : BaseAutoViewModel<Nat.ArtistApp.Services.ViewModels.FetchEmailViewModel, Nat.ArtistApp.Functions.ViewModels.FetchEmailViewModel>
	{
		public DateTime? Begin { get; set; }
		public DateTime? End { get; set; }
		public List<string> Emails { get; set; }
	}
}
