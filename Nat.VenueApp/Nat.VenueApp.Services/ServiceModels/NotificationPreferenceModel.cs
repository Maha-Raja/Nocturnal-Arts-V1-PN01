using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class NotificationPreferenceModel 
	{

        public int NotificationPreferenceID { get; set; }
        public long UserID { get; set; }
        public string ChannelLKPID { get; set; }
        public string ChannelLKPValue { get; set; }
        public string FrequencyLKPID { get; set; }
        public string FrequencyLKPValue { get; set; }

        public Boolean ActiveFlag { get; set; }
        public Nullable<DateTime> EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveEndDate { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
