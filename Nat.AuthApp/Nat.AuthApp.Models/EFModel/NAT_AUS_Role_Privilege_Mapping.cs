//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.AuthApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_AUS_Role_Privilege_Mapping : Entity
    {
        public long Role_Privilege_Mapping_ID { get; set; }
        public long Role_ID { get; set; }
        public long Privilege_ID { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
    
        public virtual NAT_AUS_Privilege NAT_AUS_Privilege { get; set; }
        public virtual NAT_AUS_Role NAT_AUS_Role { get; set; }
    }
}
