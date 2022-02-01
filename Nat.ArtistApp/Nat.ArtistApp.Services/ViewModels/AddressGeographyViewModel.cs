using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class AddressGeographyViewModel
    {
        public Int32 AddressGeographyId { get; set; }
        public String ParentGeographyCode { get; set; }
        public String GeographyName { get; set; }
        public String GeographyDescription { get; set; }
        public String GeographyShortCode { get; set; }
        public String GeographyTypeLKP { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        
    }
}
