using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class ArtistLocationCollection
	{
		public ArtistLocationCollection()
		{
			ArtistByLocation = new List<dynamic>();
			Artists = new List<dynamic>();
			ArtistsByMarket = new Dictionary<string, List<dynamic>>();
		}
		public List<dynamic> ArtistByLocation { get; set; }
		public List<dynamic> Artists { get; set; }
		public Dictionary<string, List<dynamic>> ArtistsByMarket { get; set; }
	}
}
