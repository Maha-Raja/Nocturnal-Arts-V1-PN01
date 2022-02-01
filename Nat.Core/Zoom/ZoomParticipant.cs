using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Zoom
{
    public class ZoomParticipant
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string device { get; set; }
        public string ip_address { get; set; }
        public string location { get; set; }
        public string network_type { get; set; }
        public string microphone { get; set; }
        public string camera { get; set; }
        public string speaker { get; set; }
        public string data_center { get; set; }
        public string connection_type { get; set; }
        public Nullable<DateTime> join_time { get; set; }
        public Nullable<DateTime> leave_time { get; set; }
        public Nullable<Boolean> share_application { get; set; }
        public Nullable<Boolean> share_desktop { get; set; }
        public Nullable<Boolean> share_whiteboard { get; set; }
        public Nullable<Boolean> recording { get; set; }
        public string pc_name { get; set; }
        public string domain { get; set; }
        public string mac_addr { get; set; }
        public string harddisk_id { get; set; }
        public string version { get; set; }
        public string leave_reason { get; set; }
    }
}
