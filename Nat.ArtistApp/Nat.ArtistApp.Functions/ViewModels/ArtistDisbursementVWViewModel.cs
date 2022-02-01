using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.Core.ViewModels;
using Nat.ArtistApp.Services.ServiceModels;

namespace Nat.ArtistApp.Functions.ViewModels
{
    public class ArtistDisbursementVWViewModel : BaseAutoViewModel<ArtistDisbursementVWModel,ArtistDisbursementVWViewModel>
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
