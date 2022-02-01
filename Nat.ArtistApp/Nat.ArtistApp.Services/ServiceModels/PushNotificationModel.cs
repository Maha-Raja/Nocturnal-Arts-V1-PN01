using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class PushNotificationModel
    {
        public IEnumerable<Int32> UserIds { get; set; }
        public String NotificaitonTitle { get; set; }
        public String NotificaitonBody { get; set; }
    }
}
