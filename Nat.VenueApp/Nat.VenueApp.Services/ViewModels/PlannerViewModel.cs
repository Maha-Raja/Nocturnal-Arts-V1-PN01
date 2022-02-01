using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ViewModels
{
    public class PlannerViewModel
    {
        public Int32 PlannerId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public Int32 PlannerTypeLKPId { get; set; }
        public Nullable<Int32> ReferenceTypeLKPId { get; set; }
        public Nullable<Int32> ReferenceId { get; set; }
        public Nullable<Int32> StatusLKPId { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public Nullable<DateTime> EffectiveStartDate { get; set; }
        public Nullable<DateTime> EffectiveEndDate { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
    }
}
