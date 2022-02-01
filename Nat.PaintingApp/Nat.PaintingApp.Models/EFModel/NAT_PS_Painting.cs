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
    
    public partial class NAT_PS_Painting: Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_PS_Painting()
        {
            this.NAT_Painting_Kit_Item = new HashSet<NAT_Painting_Kit_Item>();
            this.NAT_PS_Painting_Attachment = new HashSet<NAT_PS_Painting_Attachment>();
            this.NAT_PS_Painting_Rating_Log = new HashSet<NAT_PS_Painting_Rating_Log>();
            this.NAT_PS_Painting_Rating = new HashSet<NAT_PS_Painting_Rating>();
            this.NAT_PS_Painting_Image = new HashSet<NAT_PS_Painting_Image>();
            this.NAT_PS_Painting_Video = new HashSet<NAT_PS_Painting_Video>();
        }
    
        public int Painting_ID { get; set; }
        public Nullable<int> Artist_ID { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public string Orientation_LKP { get; set; }
        public string Painting_Status_LKP { get; set; }
        public string Painting_Name { get; set; }
        public Nullable<int> Price { get; set; }
        public string Tags { get; set; }
        public string Approved_Flag { get; set; }
        public string Featured_Flag { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Inspired_From { get; set; }
        public Nullable<int> Difficulty_Level { get; set; }
        public string Video_Tutorial_Url { get; set; }
        public Nullable<bool> Own_Painting_Consent { get; set; }
        public Nullable<bool> Sell_Painting_Consent { get; set; }
        public Nullable<int> Painting_Request_ID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
        public Nullable<int> Number_of_Major_Steps { get; set; }
        public Nullable<int> Time_To_Complete { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<bool> Special_Event_Flag { get; set; }
        public Nullable<bool> Partner_Painting_Flag { get; set; }
        public string Notes { get; set; }
        public string Reason { get; set; }
        public string Painting_Medium_LKP { get; set; }
        public string Canvas_Size { get; set; }
        public string Orientation { get; set; }
        public Nullable<bool> Partner { get; set; }
        public string Occasion { get; set; }
        public Nullable<int> Painting_Age_Group_Type_LKP_ID { get; set; }
        public Nullable<bool> Single_Use { get; set; }
        public Nullable<int> Single_Use_Event_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_Painting_Kit_Item> NAT_Painting_Kit_Item { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Attachment> NAT_PS_Painting_Attachment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Rating_Log> NAT_PS_Painting_Rating_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Rating> NAT_PS_Painting_Rating { get; set; }
        public virtual NAT_PS_Painting_Requests NAT_PS_Painting_Requests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Image> NAT_PS_Painting_Image { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_PS_Painting_Video> NAT_PS_Painting_Video { get; set; }
    }
}
