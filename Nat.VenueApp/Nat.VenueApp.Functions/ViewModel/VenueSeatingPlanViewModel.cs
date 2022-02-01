using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueSeatingPlanViewModel : BaseAutoViewModel<VenueSeatingPlanModel, VenueSeatingPlanViewModel>
	{
		public Int32 SeatingPlanId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> VenueHallId { get; set; }
		public String EventSeatingPlanName { get; set; }
		public Nullable<Int32> TotalSeats { get; set; }
		public Nullable<Boolean> DefaultPlan { get; set; }
		public String BackgroundImage { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public Nullable<decimal> GoldPrice { get; set; }
		public Nullable<decimal> SilverPrice { get; set; }
		public Nullable<decimal> BasicPrice { get; set; }
		public Nullable<decimal> ExtraPrice { get; set; }
		public Nullable<bool> TaxFlag { get; set; }
		public string Notes { get; set; }
		public VenueHallViewModel NatVsVenueHall { get; set; }
        [Complex]
        public ICollection<VenueSeatViewModel> NatVsVenueSeat { get; set; }
	}
}
