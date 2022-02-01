using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
	public class EventGalleryPicturesModel
	{
		public int EventId { get; set; }
		public string ImageType { get; set; }
		public string[] Images { get; set; }
		public List<string> ImagesURL { get; set; }
	}
}
