using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueSeatingPlanDTO : BaseDTO<NAT_VS_Venue_Seating_Plan, VenueSeatingPlanDTO>
    {
        public int SeatingPlanID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> VenueHallID { get; set; }
        public string EventSeatingPlanName { get; set; }
        public Nullable<int> TotalSeats { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public void FromEFModel(NAT_VS_Venue_Seating_Plan _VenueSeatingPlan)
        {
            FromEFModel(_VenueSeatingPlan, this);
        }

        public NAT_VS_Venue_Seating_Plan ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
