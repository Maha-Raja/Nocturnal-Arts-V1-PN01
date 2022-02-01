using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueFacilityModel : BaseServiceModel<NAT_VS_Venue_Facility, VenueFacilityModel>, IObjectState
	{
		public Int32 VenueFacilityId { get; set; }
		public Int32 VenueId { get; set; }
		public String FacilityLKPId { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueModel NatAsVenue { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
