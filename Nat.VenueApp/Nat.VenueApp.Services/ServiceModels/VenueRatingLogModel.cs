using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueRatingLogModel : BaseServiceModel<NAT_VS_Venue_Rating_Log, VenueRatingLogModel>, IObjectState
	{
		public Int32 VenueRatingLogId { get; set; }
		public Nullable<Int32> TenantId { get; set; }
		public Nullable<Int32> VenueId { get; set; }

        public Nullable<Int32> CustomerId { get; set; }
        public Nullable<DateTime> ReviewDate { get; set; }
		public String ReviewTitle { get; set; }
		public String ReviewDetail { get; set; }
		public Double RatingValue { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public VenueModel NatAsVenue { get; set; }
		public ObjectState ObjectState { get; set; }

        public String CustomerName { get; set; }
        public String CustomerProfileImageUrl { get; set; }
    }
}
