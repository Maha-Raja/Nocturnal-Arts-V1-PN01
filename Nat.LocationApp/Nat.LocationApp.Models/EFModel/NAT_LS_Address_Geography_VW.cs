//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.LocationApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_LS_Address_Geography_VW : Entity
    {
        public int City_Id { get; set; }
        public string City_Name { get; set; }
        public int Province_Id { get; set; }
        public string Province_Name { get; set; }
        public int Country_Id { get; set; }
        public string Country_Name { get; set; }
    }
}
