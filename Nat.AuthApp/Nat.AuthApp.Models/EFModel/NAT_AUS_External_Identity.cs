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
    
    public partial class NAT_AUS_External_Identity : Entity
    {
        public int External_Identity_ID { get; set; }
        public string Identity_Provider_LKP { get; set; }
        public string Account_ID { get; set; }
        public Nullable<long> User_ID { get; set; }
        public string Account_Object { get; set; }
        public string Status_LKP { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
    
        public virtual NAT_AUS_User NAT_AUS_User { get; set; }
    }
}
