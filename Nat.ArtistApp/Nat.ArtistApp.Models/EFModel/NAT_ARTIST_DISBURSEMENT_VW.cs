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
    
    public partial class NAT_ARTIST_DISBURSEMENT_VW : Entity
    {
        public long NID { get; set; }
        public int Artist_ID { get; set; }
        public string PerHour { get; set; }
        public Nullable<int> Event_Category_LKP_ID { get; set; }
        public string Event_Code { get; set; }
        public Nullable<bool> Virtual { get; set; }
        public Nullable<int> Gold { get; set; }
        public Nullable<int> Basic { get; set; }
    }
}
