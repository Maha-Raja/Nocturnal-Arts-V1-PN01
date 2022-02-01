using ArtistDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Artist
{
    public class ArtistDTO : BaseDTO<NAT_AS_Artist, ArtistDTO>
    {
        public int ArtistID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public string ArtistBio { get; set; }
        public Nullable<int> ArtistStatusLKPID { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<int> StripeID { get; set; }
        public PersonDTO Person { get; set; }

        public void FromEFModel(NAT_AS_Artist _Artist)
        {
            FromEFModel(_Artist, this);
            this.Person = new PersonDTO();
            //this.Person.FromEFModel(_Artist.NAT_AS_Person);
        }

        public NAT_AS_Artist ToEFModel()
        {
            NAT_AS_Artist _Artist = ToEFModel(this);
            //_Artist.NAT_AS_Person = (NAT_AS_Person)new PersonDTO().ToEFModel(this.Person);
            return _Artist;
        }
    }
}
