using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class VitualEventLinkTableEntityModel : TableEntity
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MeetingNumber { get; set; }
        public string Password { get; set; }
        public string Link { get; set; }
        public string ZoomStartLink { get; set; }
        public string TicketNumber { get; set; }

        public VitualEventLinkTableEntityModel(string eventCode, string guid)
        {
            this.PartitionKey = eventCode;
            this.RowKey = guid;
        }
        public VitualEventLinkTableEntityModel() { }
    }
}
