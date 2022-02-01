using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
	public class NotificationPreferenceModel : BaseServiceModel<NAT_AUS_Notification_Preference, NotificationPreferenceModel>, IObjectState
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
        public UserModel NatAusUser { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
