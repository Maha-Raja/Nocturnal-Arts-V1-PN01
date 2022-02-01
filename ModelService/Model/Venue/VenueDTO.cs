using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;


namespace ModelService.Model
{
    public class VenueDTO : BaseDTO<NAT_VS_Venue, VenueDTO>
    {
        public int VenueID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> AddressID { get; set; }
        public Nullable<int> VenueStatusLKPID { get; set; }
        public string VenueName { get; set; }
        public Nullable<int> SeatingCapacity { get; set; }
        public string VerifiedFlag { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public void FromEFModel(NAT_VS_Venue _Venue)
        {
            FromEFModel(_Venue, this);
        }
        
        public NAT_VS_Venue ToEFModel()
        {
            return ToEFModel(this);
        }
    }

}
