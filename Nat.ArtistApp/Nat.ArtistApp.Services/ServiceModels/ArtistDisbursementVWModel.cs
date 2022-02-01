using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class ArtistDisbursementVWModel : BaseServiceModel<NAT_ARTIST_DISBURSEMENT_VW, ArtistDisbursementVWModel>
    {
        public long NID { get; set; }
        public int ArtistID { get; set; }
        public string PerHour { get; set; }
        public Nullable<int> EventCategoryLKPID { get; set; }
        public string EventCode { get; set; }
        public Nullable<bool> Virtual { get; set; }
        public Nullable<int> Gold { get; set; }
        public Nullable<int> Basic { get; set; }
    }
}
