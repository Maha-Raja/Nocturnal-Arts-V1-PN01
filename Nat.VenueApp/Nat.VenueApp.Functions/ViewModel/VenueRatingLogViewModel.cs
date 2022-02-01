using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.VenueApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.VenueApp.Functions.ViewModels
{
	public class VenueRatingLogViewModel : BaseAutoViewModel<VenueRatingLogModel, VenueRatingLogViewModel>
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
		public VenueViewModel NatAsVenue { get; set; }




        public String CustomerName { get; set; }
        public String CustomerProfileImageUrl { get; set; }
    }
}
