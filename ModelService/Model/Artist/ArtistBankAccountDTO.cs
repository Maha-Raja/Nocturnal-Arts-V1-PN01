using ArtistDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Artist
{
    public class ArtistBankAccountDTO : BaseDTO<NAT_AS_Artist_Bank_Account, ArtistBankAccountDTO>
    {
        public int ArtistBankAccountID { get; set; }
        public Nullable<int> ArtistID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> BankAccountNumber { get; set; }
        public Nullable<int> BankRoutingNumber { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public void FromEFModel(NAT_AS_Artist_Bank_Account _ArtistBankAccount)
        {
            FromEFModel(_ArtistBankAccount, this);
        }

        public NAT_AS_Artist_Bank_Account ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
