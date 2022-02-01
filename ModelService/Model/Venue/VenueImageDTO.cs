using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueImageDTO : BaseDTO<NAT_VS_Venue_Image, VenueImageDTO>
    {
        public int VenueImageID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> VenueID { get; set; }
        public Nullable<int> ImageTypeLKPID { get; set; }
        public string ImagePath { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public void FromEFModel(NAT_VS_Venue_Image _VenueImage)
        {
            FromEFModel(_VenueImage, this);
        }

        public NAT_VS_Venue_Image ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
