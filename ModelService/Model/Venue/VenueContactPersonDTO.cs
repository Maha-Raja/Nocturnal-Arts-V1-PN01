using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenueDataAccessLayer.Model;

namespace ModelService.Model.Venue
{
    public class VenueContactPersonDTO : BaseDTO<NAT_VS_Venue_Contact_Person, VenueContactPersonDTO>
    {
        public int VenueContactPersonID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public Nullable<int> VenueID { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public PersonDTO Person { get; set; }
        public void FromEFModel(NAT_VS_Venue_Contact_Person _Venue)
        {
            FromEFModel(_Venue, this);
            this.Person = new PersonDTO();
            //this.Person.FromEFModel(_Venue.NAT_VS_Person);
        }

        public NAT_VS_Venue_Contact_Person ToEFModel()
        {
            NAT_VS_Venue_Contact_Person _Venue = ToEFModel(this);
            //_Venue.NAT_VS_Person = (NAT_VS_Person)new PersonDTO().ToEFModel(this.Person);
            return _Venue;
        }
    }
}
