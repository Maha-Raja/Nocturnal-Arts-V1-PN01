using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Functions.ViewModels
{
    public class ArtistSkillViewModel : BaseAutoViewModel<ArtistSkillModel, ArtistSkillViewModel>
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
    }
}
