using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
    public class EventReminderTableEntityModel : TableEntity
    {
        public EventReminderTableEntityModel(string eventCode, string guid, bool value)
        {
            this.PartitionKey = eventCode;
            this.RowKey = guid;
            this.Status = value;
        }
        public EventReminderTableEntityModel() { }

        public bool Status { get; set; }

    }
}
