using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueImageViewModel : BaseAutoViewModel<VenueImageModel, VenueImageViewModel>
	{
		public Int32 VenueImageId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> VenueId { get; set; }
		public String ImageTypeLKPId { get; set; }
		public String ImagePath { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueViewModel NatAsVenue { get; set; }
	}
}
