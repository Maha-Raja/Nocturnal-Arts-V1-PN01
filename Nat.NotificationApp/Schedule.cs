using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SchedulingTimeTrigger
{
    class Schedule : TableEntity
    {
        public string Parameters { get; set; }
        public DateTimeOffset Date { get; set; }
        public string IsActive { get; set; }

        public string NotificationType { get; set; }

        public Schedule(string _NotificationType, string _GUID, string _Parameter, DateTime _Date)
        {

            PartitionKey = _NotificationType;
            RowKey = _GUID;
            Parameters = _Parameter;
            Date = _Date;
        }
        public Schedule()
        {

        }
    }



   
}
