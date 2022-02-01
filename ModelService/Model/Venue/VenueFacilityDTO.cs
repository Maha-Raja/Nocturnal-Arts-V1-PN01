using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueFacilityDTO : BaseDTO<NAT_VS_Venue_Facility, VenueFacilityDTO>
    {

        public int Venue_Facility_ID { get; set; }
        public int Venue_ID { get; set; }
        public Nullable<int> Facility_LKP_ID { get; set; }
        public string Facility_Name { get; set; }
        public string Facility_Description { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }

        public void FromEFModel(NAT_VS_Venue_Facility _VenueFacility)
        {
            FromEFModel(_VenueFacility, this);
        }

        public NAT_VS_Venue_Facility ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
