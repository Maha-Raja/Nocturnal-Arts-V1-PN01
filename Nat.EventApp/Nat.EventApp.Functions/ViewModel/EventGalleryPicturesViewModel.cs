using Nat.Core.ViewModels;
using Nat.EventApp.Services.ServiceModels;

namespace Nat.EventApp.Functions.ViewModel
{
	public class EventGalleryPicturesViewModel : BaseAutoViewModel<EventGalleryPicturesModel, EventGalleryPicturesViewModel>
	{
		public int EventId { get; set; }
		public string ImageType { get; set; }
		public string[] Images { get; set; }
	}
}
