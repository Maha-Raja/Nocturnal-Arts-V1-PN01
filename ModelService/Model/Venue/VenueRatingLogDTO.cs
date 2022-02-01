using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueRatingLogDTO : BaseDTO<NAT_VS_Venue_Rating_Log, VenueRatingLogDTO>
    {
        public int VenueRatingLogID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> VenueID { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewDetail { get; set; }
        public double RatingValue { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public void FromEFModel(NAT_VS_Venue_Rating_Log _VenueRatingLog)
        {
            FromEFModel(_VenueRatingLog, this);
        }

        public NAT_VS_Venue_Rating_Log ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
