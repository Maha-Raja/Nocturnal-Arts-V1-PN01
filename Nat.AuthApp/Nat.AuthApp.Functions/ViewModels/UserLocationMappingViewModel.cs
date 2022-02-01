using Nat.Core.ViewModels;
using Nat.AuthApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class UserLocationMappingViewModel : BaseAutoViewModel<UserLocationMappingModel, UserLocationMappingViewModel>
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
    }
}
