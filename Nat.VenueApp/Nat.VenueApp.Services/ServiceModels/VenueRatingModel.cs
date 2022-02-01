using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueRatingModel : BaseServiceModel<NAT_VS_Venue_Rating, VenueRatingModel>, IObjectState
	{
		public Int32 VenueRatingId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> VenueId { get; set; }
		public Double AverageRatingValue { get; set; }
		public Nullable<Int32> NumberOfRatings { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueModel NatAsVenue { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
