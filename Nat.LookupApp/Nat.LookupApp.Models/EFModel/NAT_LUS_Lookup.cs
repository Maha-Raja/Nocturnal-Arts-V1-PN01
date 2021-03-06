//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.LookupApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_LUS_Lookup : Entity
    {
        public int Lookup_id { get; set; }
        public string Hidden_Value { get; set; }
        public string Visible_Value { get; set; }
        public bool User_Editable_Flag { get; set; }
        public string Lookup_Type { get; set; }
        public string Lookup_Description { get; set; }
        public Nullable<bool> Filtered_Flag { get; set; }
        public Nullable<bool> Preferred_Flag { get; set; }
        public Nullable<int> Sort_Order { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
        public string String_Attribute_1 { get; set; }
        public string String_Attribute_2 { get; set; }
        public string String_Attribute_3 { get; set; }
        public string String_Attribute_4 { get; set; }
        public string String_Attribute_5 { get; set; }
    }
}
