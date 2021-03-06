//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.PlannerApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_PLS_Availability : Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_PLS_Availability()
        {
            this.NAT_PLS_Availability_Slot = new HashSet<NAT_PLS_Availability_Slot>();
        }
    
        public int Availability_ID { get; set; }
        public int Planner_ID { get; set; }
        public int Day_Of_Week_LKP_ID { get; set; }
        public bool Active_Flag { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_Start_Time { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
    
        public virtual NAT_PLS_Planner NAT_PLS_Planner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PLS_Availability_Slot> NAT_PLS_Availability_Slot { get; set; }
    }
}
