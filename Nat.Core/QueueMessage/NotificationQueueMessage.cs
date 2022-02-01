using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.QueueMessage
{
    public enum NotificationType
    {
        Email,
        PushNotification,
        Sms
    }
    public class NotificationQueueMessage
    {
        public string UserId;
        public string ServiceName;
        public NotificationType NotificationType;
        public string TemplateName;
        public string Subject;
        public string Content;
        public int TenantID;
        public DateTime Time;
        public Receiver[] RecipientList;
        public Receiver[] CcRecipientList;
        public Receiver[] BccRecipientList;
        public Sender SenderAccount;
        public List<String> Attachments;
        public IDictionary<String, String> AttachmentsDict;
        public bool BulkEmail;
        public int SortOrder;

        public NotificationQueueMessage() { }

        public NotificationQueueMessage(string ServiceName, NotificationType NotificationType, string TemplateName, int TenantID, DateTime Time, Receiver Receivers, Sender SenderAccount, string Subject, bool BulkEmail = false, int SortOrder = 0)
        {
            this.ServiceName = ServiceName;
            this.NotificationType = NotificationType;
            this.TemplateName = TemplateName;
            this.TenantID = TenantID;
            this.Time = Time;
            this.RecipientList = new Receiver[] { Receivers };
            this.SenderAccount = SenderAccount;
            this.Subject = Subject;
            this.BulkEmail = BulkEmail;
            this.SortOrder = SortOrder;
        }

        public NotificationQueueMessage(string ServiceName, NotificationType NotificationType, string TemplateName, int TenantID, DateTime Time, Receiver Receivers)
        {
            this.ServiceName = ServiceName;
            this.NotificationType = NotificationType;
            this.TemplateName = TemplateName;
            this.TenantID = TenantID;
            this.Time = Time;
            this.RecipientList = new Receiver[]{ Receivers };
            
        }


        public class Receiver
        {
            public string ReceiverID;
            public string ReceiverName;
            public Object ValueObject;
        }

        public class Sender
        {
            public string SenderID;
        }
    }
}