using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Zoom
{
    public class ZoomEvent
    {
        [JsonProperty("event")]
        public string Event{ get; set; }
        public ZoomEventPayload payload { get; set; }

        public ZoomMeetingEventTableEntityModel ToTableEntityModel()
        {
            return new ZoomMeetingEventTableEntityModel(this);
        }
    }

    public class ZoomEventPayload
    {
        public string account_id { get; set; }
        [JsonProperty("object")]
        public ZoomEventPayloadObject Object { get; set; }
    }

    public class ZoomEventPayloadObject
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string host_id { get; set; }
        public string topic { get; set; }
        public Nullable<Int32> type { get; set; }
        public Nullable<DateTime> start_time { get; set; }
        public string timezone { get; set; }
        public Nullable<Int32> duration { get; set; }
        public ZoomParticipant participant { get; set; }
    }

    public class ZoomMeetingEventTableEntityModel : TableEntity
    {
        public string EventName { get; set; }
        public string AccountId { get; set; }
        public string MeetingId { get; set; }
        public string MeetingUuid { get; set; }
        public string MeetingHostId { get; set; }
        public string MeetingTopic { get; set; }
        public Nullable<Int32> MeetingType { get; set; }
        public Nullable<DateTime> MeetingStartTime { get; set; }
        public string MeetingTimezone { get; set; }
        public Nullable<Int32> MeetingDuration { get; set; }
        public string ParticipanId { get; set; }
        public string ParticipantUserId { get; set; }
        public string ParticipanUserName { get; set; }
        public Nullable<DateTime> ParticipanJoinTime { get; set; }
        public Nullable<DateTime> ParticipanLeaveTime { get; set; }

        public ZoomMeetingEventTableEntityModel(ZoomEvent Event)
        {
            if (Event != null && Event.payload != null && Event.payload.Object != null)
            {
                this.RowKey = Guid.NewGuid().ToString();
                this.PartitionKey = Event.payload.Object.id;

                this.EventName = Event.Event;
                this.AccountId = Event.payload.account_id;
                this.MeetingId = Event.payload.Object.id;
                this.MeetingUuid = Event.payload.Object.uuid;
                this.MeetingHostId = Event.payload.Object.host_id;
                this.MeetingTopic = Event.payload.Object.topic;
                this.MeetingType = Event.payload.Object.type;
                this.MeetingStartTime = Event.payload.Object.start_time;
                this.MeetingTimezone = Event.payload.Object.timezone;
                this.MeetingDuration = Event.payload.Object.duration;

                if (Event.payload.Object.participant != null)
                {
                    this.ParticipanId = Event.payload.Object.participant.id;
                    this.ParticipantUserId = Event.payload.Object.participant.user_id;
                    this.ParticipanUserName = Event.payload.Object.participant.user_name;
                    this.ParticipanJoinTime = Event.payload.Object.participant.join_time;
                    this.ParticipanLeaveTime = Event.payload.Object.participant.leave_time;
                }
            } 
            else
            {
                throw new System.Exception("Invalid event object");
            }
        }
    }
}
