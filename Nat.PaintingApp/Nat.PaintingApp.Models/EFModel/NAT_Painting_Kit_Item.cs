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
    
    public partial class NAT_Painting_Kit_Item: Entity
    {
        public int Painting_Kit_Item_ID { get; set; }
        public Nullable<int> Painting_ID { get; set; }
        public int Kit_Item_ID { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public int Quantity { get; set; }
    
        public virtual NAT_PS_Painting NAT_PS_Painting { get; set; }
    }
}
