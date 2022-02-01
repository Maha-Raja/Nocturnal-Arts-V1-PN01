using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueFacilityViewModel : BaseAutoViewModel<VenueFacilityModel, VenueFacilityViewModel>
	{
		public Int32 VenueFacilityId { get; set; }
		public Int32 VenueId { get; set; }
		public String FacilityLKPId { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueViewModel NatAsVenue { get; set; }
	}
}
