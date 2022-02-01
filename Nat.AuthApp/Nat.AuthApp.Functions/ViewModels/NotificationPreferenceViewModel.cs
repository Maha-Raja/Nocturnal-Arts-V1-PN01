using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nat.AuthApp.Functions.ViewModels
{
	public class NotificationPreferenceViewModel : BaseAutoViewModel<NotificationPreferenceModel, NotificationPreferenceViewModel>
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
      
    }
}
