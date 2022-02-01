using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMethods
{
    public class NAT_Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NAT_Person()
        {
        }
        public int Person_ID { get; set; }
        public Nullable<int> Tenant_ID { get; set; }
        public Nullable<int> Gender_LKP_ID { get; set; }
        public Nullable<int> Residential_Address_ID { get; set; }
        public Nullable<int> Shopping_Address_ID { get; set; }
        public Nullable<int> Billing_Address_ID { get; set; }
        public string Person_First_Name { get; set; }
        public string Person_Last_Name { get; set; }
        public string Person_Middle_Name { get; set; }
        public string Person_Email { get; set; }
        public string Person_Profile_Image_Link { get; set; }
        public Nullable<System.DateTime> Date_Of_Birth { get; set; }
        public string Person_Extension { get; set; }
        public Nullable<bool> Active_Flag { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }
    }
}
