using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueHallDTO : BaseDTO<NAT_VS_Venue_Hall, VenueHallDTO>

    {
        public int VenueHallID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> VenueID { get; set; }
        public Nullable<int> VenueHallStatusLKPID { get; set; }
        public string VenueHallName { get; set; }
        public Nullable<int> SeatingCapacity { get; set; }
        public string VerifiedFlag { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public void FromEFModel(NAT_VS_Venue_Hall _VenueHall)
        {
            FromEFModel(_VenueHall, this);
        }

        public NAT_VS_Venue_Hall ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
