using CustomerDataAcessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Customer
{
    public class CustomerDTO : BaseDTO<NAT_CS_Customer, CustomerDTO>
    {
        public int CustomerID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public Nullable<int> CustomerStatusLKPID { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public virtual NAT_CS_Person NAT_CS_Person { get; set; }
        public void FromEFModel(NAT_CS_Customer _Customer)
        {
            FromEFModel(_Customer, this);
        }

        public NAT_CS_Customer ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
