using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueHallModel : BaseServiceModel<NAT_VS_Venue_Hall, VenueHallModel>, IObjectState
	{
		public Int32 VenueHallId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> VenueId { get; set; }
		public Nullable<Int32> VenueHallStatusLKPId { get; set; }
		public String VenueHallName { get; set; }
		public Nullable<Int32> SeatingCapacity { get; set; }
		public String VerifiedFlag { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueModel NatAsVenue { get; set; }
        [Complex]
		public ICollection<VenueSeatingPlanModel> NatVsVenueSeatingPlan { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
