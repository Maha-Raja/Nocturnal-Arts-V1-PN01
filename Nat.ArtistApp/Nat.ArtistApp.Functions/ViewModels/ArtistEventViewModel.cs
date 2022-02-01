using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
	public class ArtistEventViewModel : BaseAutoViewModel<ArtistEventModel, ArtistEventViewModel>
	{
		public Int32 EventId { get; set; }
		public Int32 ArtistId { get; set; }
		public String Title { get; set; }
		public String Description { get; set; }
		public Int32 EventTypeLKPId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string ReferenceId { get; set; }
		public Nullable<Int32> StatusLKPId { get; set; }
		public String UDF { get; set; }
		public Nullable<Boolean> ActiveFlag { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ArtistViewModel NatAsArtist { get; set; }

        //custom field
        public Int32 PlannerId { get; set; }

        //custom field to check if the artist is available for event creation or not
        //if an unavailable artist is selected for the event then this flag will be true
        public Boolean Forced { get; set; }

		//custom field
		//passed to planner service to generate hangout link incase of online event
		public Boolean Online { get; set; }

		//custom field
		public string GoogleHangoutUrl { get; set; }
		public string SlotTiming { get; set; }
		public string LocationCode { get; set; }
	}
}
