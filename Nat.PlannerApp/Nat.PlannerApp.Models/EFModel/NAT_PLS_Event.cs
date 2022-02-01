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
    
    public partial class NAT_PLS_Event : Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_PLS_Event()
        {
            this.NAT_PLS_Slot = new HashSet<NAT_PLS_Slot>();
        }
    
        public int Event_ID { get; set; }
        public int Planner_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Event_Type_LKP_ID { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
        public string Reference_ID { get; set; }
        public Nullable<int> Status_LKP_ID { get; set; }
        public string UDF { get; set; }
        public bool Active_Flag { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Google_Event_ID { get; set; }
        public string Google_Hangout_URL { get; set; }
    
        public virtual NAT_PLS_Planner NAT_PLS_Planner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PLS_Slot> NAT_PLS_Slot { get; set; }
    }
}
