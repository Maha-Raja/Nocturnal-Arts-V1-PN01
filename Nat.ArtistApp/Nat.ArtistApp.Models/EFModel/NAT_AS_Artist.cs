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
    
    public partial class NAT_AS_Artist : Entity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_AS_Artist()
        {
            this.NAT_AS_Artist_Skill = new HashSet<NAT_AS_Artist_Skill>();
            this.NAT_AS_Artist_Document = new HashSet<NAT_AS_Artist_Document>();
            this.NAT_AS_Artist_Location_Mapping = new HashSet<NAT_AS_Artist_Location_Mapping>();
            this.NAT_AS_Artist_Event = new HashSet<NAT_AS_Artist_Event>();
            this.NAT_AS_Artist_Rating_Log = new HashSet<NAT_AS_Artist_Rating_Log>();
            this.NAT_AS_Artist_Rating = new HashSet<NAT_AS_Artist_Rating>();
            this.NAT_AS_Artist_Venue_Preference = new HashSet<NAT_AS_Artist_Venue_Preference>();
            this.NAT_AS_Artist_Bank_Account = new HashSet<NAT_AS_Artist_Bank_Account>();
        }
    
        public int Artist_ID { get; set; }
        public string Artist_Portfolio_Url { get; set; }
        public Nullable<int> Artist_Status_LKP_ID { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public Nullable<int> Stripe_ID { get; set; }
        public Nullable<int> Planner_ID { get; set; }
        public string Artist_Extension { get; set; }
        public Nullable<System.DateTime> Date_Of_Birth { get; set; }
        public string Artist_Profile_Image_Link { get; set; }
        public string Artist_Email { get; set; }
        public string Artist_Middle_Name { get; set; }
        public string Artist_Last_Name { get; set; }
        public string Artist_First_Name { get; set; }
        public string Stage_Name { get; set; }
        public string SIN { get; set; }
        public string Tax_Number { get; set; }
        public string Emergency_Contact { get; set; }
        public Nullable<int> Gender_LKP_ID { get; set; }
        public Nullable<int> Residential_Address_ID { get; set; }
        public Nullable<int> Shipping_Address_ID { get; set; }
        public Nullable<int> Billing_Address_ID { get; set; }
        public string Business_Name { get; set; }
        public string Contact_Number { get; set; }
        public string Payment_Cycle_LKP_ID { get; set; }
        public string Location_Code { get; set; }
        public Nullable<decimal> Hours_Per_Week { get; set; }
        public string Artist_About { get; set; }
        public string Facebook_Profile_Url { get; set; }
        public string Twitter_Profile_Url { get; set; }
        public string Instagram_Profile_Url { get; set; }
        public string Google_Profile_Url { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public bool Active_Flag { get; set; }
        public string Default_Payment_Method { get; set; }
        public Nullable<decimal> Available_Credit { get; set; }
        public string Company_Email { get; set; }
        public string Company_Name { get; set; }
        public Nullable<bool> Visible_To_Public { get; set; }
        public Nullable<bool> Events_Visible_To_Public { get; set; }
        public Nullable<bool> Host_Virtual_Event { get; set; }
        public string Emergency_Contact_Name { get; set; }
        public string Emergency_Contact_Relationship { get; set; }
        public string Greeting { get; set; }
        public string Notes { get; set; }
        public string Biz_Number { get; set; }
        public string Emergency_Contact_Email { get; set; }
        public string Gender { get; set; }
        public string Id_Type { get; set; }
        public string Id_Number { get; set; }
        public string Company_Phone { get; set; }
        public string Onboarded { get; set; }
        public string Google_Maps_Url { get; set; }
        public string Google_Maps_Url_Supply { get; set; }
        public string PerHour { get; set; }
        public string Video_Key { get; set; }
        public string Video_Secret { get; set; }
        public string Video_User { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Skill> NAT_AS_Artist_Skill { get; set; }
        public virtual NAT_AS_Artist_Address NAT_AS_Artist_Address { get; set; }
        public virtual NAT_AS_Artist_Address NAT_AS_Artist_Residential_Address { get; set; }
        public virtual NAT_AS_Artist_Address NAT_AS_Artist_Address2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Document> NAT_AS_Artist_Document { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Location_Mapping> NAT_AS_Artist_Location_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Event> NAT_AS_Artist_Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Rating_Log> NAT_AS_Artist_Rating_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Rating> NAT_AS_Artist_Rating { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Venue_Preference> NAT_AS_Artist_Venue_Preference { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NAT_AS_Artist_Bank_Account> NAT_AS_Artist_Bank_Account { get; set; }
    }
}
