using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.EventApp.Services.ServiceModels
{
    public class VenueFacilityModel
    {
        public Int32 VenueFacilityId { get; set; }
        public Int32 VenueId { get; set; }
        public String FacilityLKPId { get; set; }
        public String FacilityName { get; set; }
        public String FacilityDescription { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public VenueModel NatAsVenue { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
