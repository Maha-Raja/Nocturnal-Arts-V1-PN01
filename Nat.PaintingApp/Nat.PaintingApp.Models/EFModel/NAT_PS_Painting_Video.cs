//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.PaintingApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_PS_Painting_Video: Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_PS_Painting_Video()
        {
            this.NAT_PS_Painting_Supply = new HashSet<NAT_PS_Painting_Supply>();
        }
    
        public int Painting_Video_ID { get; set; }
        public Nullable<int> Painting_ID { get; set; }
        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
    
        public virtual NAT_PS_Painting NAT_PS_Painting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Supply> NAT_PS_Painting_Supply { get; set; }
    }
}
