using EventDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Event
{
    public class EventDTO : BaseDTO<NAT_ES_Event, EventDTO>
    {
        public int EventID { get; set; }
        public int TenantID { get; set; }
        public int ArtistID { get; set; }
        public int EventTypeLKPID { get; set; }
        public int EventStatusLKPID { get; set; }
        public int PaintingID { get; set; }
        public int SeatingPlanID { get; set; }
        public int EventAgeGroupTypeLKPID { get; set; }
        public int EventCategoryLKPID { get; set; }
        public int VenueHallID { get; set; }
        public string EventName { get; set; }
        public bool ActiveFlag { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.DateTime EventDate { get; set; }
        public System.TimeSpan EventStartTime { get; set; }
        public System.TimeSpan EventEndTime { get; set; }
        public bool PrivateEventFlag { get; set; }
        public bool FundraisingEventFlag { get; set; }

        public void FromEFModel(NAT_ES_Event _Event)
        {
            FromEFModel(_Event, this);
        }

        public NAT_ES_Event ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
