using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
	public class NotificationQueueMessageEntityModel
	{
        public string Subject { get; set; }
        public bool IsFailedEmail { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
        public string ServiceName { get; set; }
        public int NotificationType { get; set; }
        public string TemplateName { get; set; }
        public int TenantID { get; set; }
        public DateTime Time { get; set; }
        public string SenderID { get; set; }
        public string RecieverID { get; set; }
        public string RecieverName { get; set; }
        public string ValueObjectJson { get; set; }
        public string Attachments { get; set; }
        public string FailureResponse { get; set; }
        public string EmailContent { get; set; }
        public DateTime DateTime { get; set; }
    }
}
