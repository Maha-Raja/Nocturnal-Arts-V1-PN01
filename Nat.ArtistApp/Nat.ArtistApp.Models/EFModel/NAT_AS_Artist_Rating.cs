//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.ArtistApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_AS_Artist_Rating : Entity
    {
        public int Artist_Rating_ID { get; set; }
        public Nullable<int> Artist_ID { get; set; }
        public double Average_Rating_Value { get; set; }
        public Nullable<int> Number_Of_Ratings { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
    
        public virtual NAT_AS_Artist NAT_AS_Artist { get; set; }
    }
}
