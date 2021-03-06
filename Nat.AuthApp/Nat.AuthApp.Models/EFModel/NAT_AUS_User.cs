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
    
    public partial class NAT_AUS_User : Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_AUS_User()
        {
            this.NAT_AUS_External_Identity = new HashSet<NAT_AUS_External_Identity>();
            this.NAT_AUS_Notification_Preference = new HashSet<NAT_AUS_Notification_Preference>();
            this.NAT_AUS_User1 = new HashSet<NAT_AUS_User>();
            this.NAT_AUS_User_Role_Mapping = new HashSet<NAT_AUS_User_Role_Mapping>();
            this.NAT_AUS_User_Location_Mapping = new HashSet<NAT_AUS_User_Location_Mapping>();
        }
    
        public long User_ID { get; set; }
        public Nullable<long> Parent_User_ID { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string User_Image_URL { get; set; }
        public string Thumbnail_Image_URL { get; set; }
        public string Password_Hash { get; set; }
        public string Password_Salt { get; set; }
        public string Reference_Type_LKP { get; set; }
        public Nullable<long> Reference_ID { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string Phone_Number { get; set; }
        public string Email { get; set; }
        public Nullable<bool> Phone_Verified { get; set; }
        public Nullable<bool> Email_Verified { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
        public Nullable<long> Reporting_Manager { get; set; }
        public string RSA_Special_Key { get; set; }
        public Nullable<bool> RSA_Authentication_Enabled { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AUS_External_Identity> NAT_AUS_External_Identity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AUS_Notification_Preference> NAT_AUS_Notification_Preference { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AUS_User> NAT_AUS_User1 { get; set; }
        public virtual NAT_AUS_User NAT_AUS_User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AUS_User_Role_Mapping> NAT_AUS_User_Role_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AUS_User_Location_Mapping> NAT_AUS_User_Location_Mapping { get; set; }
    }
}
