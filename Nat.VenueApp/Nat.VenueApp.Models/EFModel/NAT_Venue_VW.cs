//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.VenueApp.Models.EFModel
{
    using System;
    using System.Collections.Generic;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class NAT_Venue_VW : Entity
    {
        public int Venue_ID { get; set; }
        public bool Active_Flag { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public string Venue_Name { get; set; }
        public string Location_Code { get; set; }
        public double Rating { get; set; }
        public string Parent_Location_Code { get; set; }
        public Nullable<int> Upcoming_Events { get; set; }
        public Nullable<System.DateTime> Last_Event_Date { get; set; }
        public Nullable<int> Events_Held { get; set; }
        public string Venue_Image_Url { get; set; }
        public string Location_Name { get; set; }
        public string Parent_Location_Name { get; set; }
        public Nullable<int> Planner_ID { get; set; }
        public Nullable<System.DateTime> Next_Event_Date { get; set; }
        public string Seating_Plan_Capacity { get; set; }
        public string Primary_Contact_Name { get; set; }
        public string Primary_Contact_Email { get; set; }
        public string Primary_Contact_Number { get; set; }
        public Nullable<System.DateTime> Next_Event_End_Time { get; set; }
        public Nullable<System.DateTime> Last_Event_End_Time { get; set; }
        public string Venue_Name_ID { get; set; }
        public string Market_Code { get; set; }
    }
}