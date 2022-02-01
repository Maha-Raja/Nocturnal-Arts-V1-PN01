using ArtistDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Artist
{
    public class ArtistRatingLogDTO : BaseDTO<NAT_AS_Artist_Rating_Log, ArtistRatingLogDTO>
    {
        public int ArtistRatingLogID { get; set; }
        public Nullable<int> ArtistID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public string ReviewDetail { get; set; }
        public Nullable<int> RatingValue { get; set; }
        public string ReviewTitle { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public void FromEFModel(NAT_AS_Artist_Rating_Log _ArtistRatingLog)
        {
            FromEFModel(_ArtistRatingLog, this);
        }

        public NAT_AS_Artist_Rating_Log ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
