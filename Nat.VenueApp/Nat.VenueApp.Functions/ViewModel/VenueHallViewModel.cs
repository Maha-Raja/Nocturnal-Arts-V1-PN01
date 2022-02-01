using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueHallViewModel : BaseAutoViewModel<VenueHallModel, VenueHallViewModel>
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
		public VenueViewModel NatAsVenue { get; set; }
        [Complex]
		public ICollection<VenueSeatingPlanViewModel> NatVsVenueSeatingPlan { get; set; }

	}
}
