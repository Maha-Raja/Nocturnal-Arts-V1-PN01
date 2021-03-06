//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.VenueApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_VS_Venue_Hall : Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_VS_Venue_Hall()
        {
            this.NAT_VS_Venue_Seating_Plan = new HashSet<NAT_VS_Venue_Seating_Plan>();
        }
    
        public int Venue_Hall_ID { get; set; }
        public Nullable<int> Venue_ID { get; set; }
        public Nullable<int> Venue_Hall_Status_LKP_ID { get; set; }
        public string Venue_Hall_Name { get; set; }
        public Nullable<int> Seating_Capacity { get; set; }
        public string Verified_Flag { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
    
        public virtual NAT_VS_Venue NAT_VS_Venue { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_VS_Venue_Seating_Plan> NAT_VS_Venue_Seating_Plan { get; set; }
    }
}
