using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class ArtistDisbursementHeaderModel
    {
        public Int32 DisbursementHeaderId { get; set; }
        public Int32 EntityId { get; set; }
        public String EntityTypeLKP { get; set; }
        public Nullable<DateTime> DisbursementDate { get; set; }
        public String Method { get; set; }
        public String InstrumentId { get; set; }
        public String StatusLKP { get; set; }
        public decimal Amount { get; set; }
        public Boolean ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public decimal AdjustedAmount { get; set; }
        public String Reason { get; set; }
        public String Description { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public String Action { get; set; }
        public String ArtistLocations { get; set; }
        public String InvoiceNumber { get; set; }
    }
}
