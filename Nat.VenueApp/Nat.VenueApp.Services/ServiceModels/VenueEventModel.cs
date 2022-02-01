using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.VenueApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueEventModel : BaseServiceModel<NAT_VS_Venue_Event, VenueEventModel>, IObjectState
	{
		public Int32 EventId { get; set; }
		public Int32 VenueId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Int32 EventTypeLKPId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public String ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public String UDF { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public VenueModel NatAsVenue { get; set; }
		public ObjectState ObjectState { get; set; }

        //custom field
        public Int32 PlannerId { get; set; }

		//custom feild to check if the venue is available for event creation or not
		//if an unavailable venue is selected for the event then this flag will be true
		public Boolean Forced { get; set; }
	}
}
