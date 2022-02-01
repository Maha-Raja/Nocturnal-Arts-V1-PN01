using Nat.Core.ServiceModels;
using Nat.VenueApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.VenueApp.Services.ServiceModels
{
	public class VenueBankAccountModel : BaseServiceModel<NAT_VS_Venue_Bank_Account, VenueBankAccountModel>, IObjectState
    {
        public int VenueBankAccountID { get; set; }
        public Nullable<int> VenueID { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }

        public string BankRoutingNumber { get; set; }
        public string TransitNumber { get; set; }
        public string BankLKPID { get; set; }
        public string BankAccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountName { get; set; }
        public string InstitutionalNumber { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool ActiveFlag { get; set; }
        public ObjectState ObjectState { get; set; }
        [Complex]
        public VenueAddressModel NatVsVenueAddress { get; set; }
    }
}
