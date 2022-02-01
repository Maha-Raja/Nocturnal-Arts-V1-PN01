using Nat.AuthApp.Models.EFModel;
using Nat.Core.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class UserLocationMappingModel : BaseServiceModel<NAT_AUS_User_Location_Mapping, UserLocationMappingModel>, IObjectState
    {
        public int UserLocationMappingId { get; set; }
        public int UserId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
