using CustomerDataAcessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMethods;

namespace ModelService.Model
{
    public class PersonDTO : BaseDTO<NAT_Person, PersonDTO>
    {
        public int PersonID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> GenderLKPID { get; set; }
        public Nullable<int> ResidentialAddressID { get; set; }
        public Nullable<int> ShoppingAddressID { get; set; }
        public Nullable<int> BillingAddressID { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string PersonMiddleName { get; set; }
        public string PersonEmail { get; set; }
        public string PersonProfileImageLink { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PersonExtension { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public void FromEFModel(NAT_Person _Event)
        {
            FromEFModel(_Event, this);
        }

        public NAT_Person ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
