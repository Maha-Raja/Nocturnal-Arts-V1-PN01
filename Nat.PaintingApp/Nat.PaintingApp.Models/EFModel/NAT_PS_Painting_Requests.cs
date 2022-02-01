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
    
    public partial class NAT_PS_Painting_Requests: Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_PS_Painting_Requests()
        {
            this.NAT_PS_Painting = new HashSet<NAT_PS_Painting>();
        }
    
        public int Request_ID { get; set; }
        public string Painting_Name { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public Nullable<int> Artist_ID { get; set; }
        public string Inspired_From { get; set; }
        public Nullable<int> Difficulty_Level { get; set; }
        public string Video_Tutorial_Url { get; set; }
        public string Json_Data { get; set; }
        public string Status_Lkp { get; set; }
        public Nullable<int> Price { get; set; }
        public string Tags { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> Own_Painting_Consent { get; set; }
        public Nullable<bool> Sell_Painting_Consent { get; set; }
        public Nullable<int> Number_of_Major_Steps { get; set; }
        public Nullable<int> Time_To_Complete { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<bool> Special_Event_Flag { get; set; }
        public Nullable<bool> Partner_Painting_Flag { get; set; }
        public Nullable<int> Width { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public string Painting_Medium_LKP { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting> NAT_PS_Painting { get; set; }
    }
}
