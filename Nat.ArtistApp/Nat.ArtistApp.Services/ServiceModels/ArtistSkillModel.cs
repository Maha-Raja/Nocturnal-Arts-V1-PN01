using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class ArtistSkillModel : BaseServiceModel<NAT_AS_Artist_Skill, ArtistSkillModel>, IObjectState
    {
        public Int32 ArtistSkillId { get; set; }
        public Nullable<Int32> ArtistId { get; set; }
        public Nullable<Int32> SkillLKPId { get; set; }
        public String SkillLKPValue { get; set; }
        public Nullable<Int32> SkillRating { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
