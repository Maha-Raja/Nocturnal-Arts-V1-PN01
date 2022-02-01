using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class LocationViewModel
    {
        public Int32 LocationId { get; set; }
        public String ParentLocationCode { get; set; }
        public String LocationName { get; set; }
        public String LocationDescription { get; set; }
        public String LocationShortCode { get; set; }
        public String LocationTypeLKP { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public string Timezone { get; set; }
        public string VisibleValue { get; set; }
    }
}
