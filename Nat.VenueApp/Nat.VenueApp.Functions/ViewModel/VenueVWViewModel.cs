using Nat.Core.ViewModels;
using Nat.VenueApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Functions.ViewModel
{
    public class VenueVWViewModel : BaseAutoViewModel<VenueVWmodel, VenueVWViewModel>
    {
        public int VenueId { get; set; }
        public bool ActiveFlag { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string VenueName { get; set; }
        public string LocationCode { get; set; }
        public double Rating { get; set; }
        public string ParentLocationCode { get; set; }
        public Nullable<int> UpcomingEvents { get; set; }
        public Nullable<System.DateTime> LastEventDate { get; set; }
        public Nullable<int> EventsHeld { get; set; }
        public string VenueImageUrl { get; set; }
        public string LocationName { get; set; }
        public string ParentLocationName { get; set; }
        public Nullable<int> PlannerId { get; set; }
        public Nullable<System.DateTime> NextEventDate { get; set; }
        public string SeatingPlanCapacity { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactNumber { get; set; }
        public Nullable<System.DateTime> NextEventEndTime { get; set; }
        public Nullable<System.DateTime> LastEventEndTime { get; set; }
        public string VenueNameId { get; set; }
        public string MarketCode { get; set; }
    }
}
