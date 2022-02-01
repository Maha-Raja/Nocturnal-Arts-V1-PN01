
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
    
public partial class NAT_LS_LOCATION_VW : Entity
{

    public int Location_ID { get; set; }

    public string Parent_Location_Code { get; set; }

    public string Location_Name { get; set; }

    public string Location_Description { get; set; }

    public string Location_Short_Code { get; set; }

    public string Location_Type_LKP { get; set; }

    public bool Active_Flag { get; set; }

    public Nullable<bool> Sales_Tax_Applicable_Flag { get; set; }

    public Nullable<bool> Sales_Tax_Inclusive_Flag { get; set; }

    public Nullable<decimal> Tax_Rate { get; set; }

    public string Facebook_Url { get; set; }

    public string Twitter_Url { get; set; }

    public string Instagram_Url { get; set; }

    public Nullable<int> Address_Id { get; set; }

    public Nullable<decimal> Supplies_Tax { get; set; }

    public Nullable<decimal> State_Tax { get; set; }

    public string Stripe_Api_Key { get; set; }

    public System.DateTime Created_Date { get; set; }

    public string Created_By { get; set; }

    public Nullable<System.DateTime> Last_Updated_Date { get; set; }

    public string Last_Updated_By { get; set; }

    public string Timezone { get; set; }

    public string Visible_Value { get; set; }

    public string Currency { get; set; }

    public string Airport_Code { get; set; }

    public Nullable<int> Precision { get; set; }

}

}