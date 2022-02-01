using EventDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Event
{
    public class EventImageDTO : BaseDTO<NAT_ES_Event_Image, EventImageDTO>
    {
        public int EventImageID { get; set; }
        public int TenantID { get; set; }
        public int EventID { get; set; }
        public int ImageTypeLKPID { get; set; }
        public string ImagePath { get; set; }
        public bool ActiveFlag { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public void FromEFModel(NAT_ES_Event_Image _ArtistEventImage)
        {
            FromEFModel(_ArtistEventImage, this);
        }

        public NAT_ES_Event_Image ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
