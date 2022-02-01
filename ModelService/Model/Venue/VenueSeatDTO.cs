using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueSeatDTO : BaseDTO<NAT_VS_Venue_Seat, VenueSeatDTO>
    {
        public int SeatID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> SeatingPlanID { get; set; }
        public Nullable<int> SeatTypeLKPID { get; set; }
        public Nullable<int> SeatAllocationTypeLKPID { get; set; }
        public Nullable<int> RowNumber { get; set; }
        public string SeatNumber { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public void FromEFModel(NAT_VS_Venue_Seat _VenueSeat)
        {
            FromEFModel(_VenueSeat, this);
        }

        public NAT_VS_Venue_Seat ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
