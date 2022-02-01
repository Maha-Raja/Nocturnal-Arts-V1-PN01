using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ViewModels
{
	public class OrderLineModel
	{
        public string KitItemName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int OrderLineId { get; set; }
        public decimal GrossAmount { get; set; }
        public int CurrencyId { get; set; }
        public string OrderLineDescription { get; set; }
        public int OrderQuantity { get; set; }
        public int OrderId { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public Boolean ActiveFlag { get; set; }
        public decimal NetPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public string LineTypeLkp { get; set; }
        public int KitItemId { get; set; }
        public string StringAttribute1 { get; set; }
        public string StringAttribute2 { get; set; }
        public string StringAttribute3 { get; set; }
        public string StringAttribute4 { get; set; }
        public string StringAttribute5 { get; set; }
        public string StringAttribute6 { get; set; }
        public string StringAttribute7 { get; set; }
        public string StringAttribute8 { get; set; }
        public string StringAttribute9 { get; set; }
        public string StringAttribute10 { get; set; }
        public int? NumericAttribute1 { get; set; }
        public int? NumericAttribute2 { get; set; }
        public int? NumericAttribute3 { get; set; }
        public int? NumericAttribute4 { get; set; }
        public int? NumericAttribute5 { get; set; }
        public decimal? AdditionalDiscount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Markup { get; set; }
        public DateTime ShipDate { get; set; }
        public string LineStatusLkp { get; set; }
        public string TrackingNumber { get; set; }
        [Complex]
        public KitItemModel KitItem { get; set; }
        public OrderModel Order { get; set; }
    }
}
