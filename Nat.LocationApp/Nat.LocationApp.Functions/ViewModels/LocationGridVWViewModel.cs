using Nat.Core.ViewModels;
using Nat.LocationApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.LocationApp.Functions.ViewModels
{
    public class LocationGridVWViewModel : BaseAutoViewModel<LocationGridVWModel, LocationGridVWViewModel>
    {
        public Nullable<int> CountryId { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public Nullable<int> ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        public string ProvinceCode { get; set; }

        public string ProvinceTimezone { get; set; }

        public Nullable<decimal> TaxRate1 { get; set; }

        public Nullable<decimal> TaxRate2 { get; set; }

        public Nullable<decimal> TaxRate3 { get; set; }

        public Nullable<decimal> TaxRate4 { get; set; }

        public Nullable<decimal> TaxRate5 { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public string CityCode { get; set; }

        public Nullable<bool> Active { get; set; }

        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public string LastUpdatedBy { get; set; }
        public Nullable<int> LegalDrinkingAge { get; set; }

        public Nullable<decimal> VirtualTax { get; set; }

        public Nullable<decimal> TotalInPersonTax { get; set; }

        public string MarketId { get; set; }

        public string LocationShortCode { get; set; }

        public string StateCode { get; set; }

        public string DSTTime { get; set; }

        public string DSTZoneCode { get; set; }

        public string StandardTime { get; set; }

        public string StandardTimezoneCode { get; set; }

        public string ManagerName { get; set; }

        public string ManagerNumber { get; set; }

        public string ManagerEmail { get; set; }
        public string TaxRate1Label { get; set; }

        public string TaxRate2Label { get; set; }

        public string TaxRate3Label { get; set; }

        public string TaxRate4Label { get; set; }

        public string TaxRate5Label { get; set; }

        public string CityShortName { get; set; }
        public string LocationName { get; set; }
    }
}
