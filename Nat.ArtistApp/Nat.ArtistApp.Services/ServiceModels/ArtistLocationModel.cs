using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistLocationModel
	{
		public string LocationCode { get; set; }
		public List<dynamic> Artists { get; set; }
	}
}
