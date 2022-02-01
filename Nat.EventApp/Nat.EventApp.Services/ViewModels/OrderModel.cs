using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ViewModels
{
	public class OrderModel
	{
        public object ArtistRating { get; set; }
        public string ArtistEmail { get; set; }
        public string ArtistPhone { get; set; }
        public bool ShipFromOwnWarehouse { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal OrderGrossAmount { get; set; }
        public string OrderDescription { get; set; }
        public string OrderTypeLkp { get; set; }
        public DateTime RequiredByDate { get; set; }
        public int ArtistId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal OrderNetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public Boolean ActiveFlag { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatusLkp { get; set; }
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
        public DateTime? DateAttribute1 { get; set; }
        public DateTime? DateAttribute2 { get; set; }
        public DateTime? DateAttribute3 { get; set; }
        public DateTime? DateAttribute4 { get; set; }
        public DateTime? DateAttribute5 { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentUrl { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingAddress { get; set; }
        public string SpecialInstruction { get; set; }
        public decimal? ShippingCharges { get; set; }
        public int EventId { get; set; }
        [Complex]
        public ICollection<OrderLineModel> NatOrderLine { get; set; }
        public string ArtistName { get; set; }
        public string EventName { get; set; }
        public string OrderStatus { get; set; }
        public bool? MakePayment { get; set; }
        public string InstrumentId { get; set; }
        public string PaymentOption { get; set; }
        public string CustomerId { get; set; }
        public DateTime? EventTime { get; set; }
        public String PaintingName { get; set; }
        public int? PaintingId { get; set; }
        public int? TicketsSold { get; set; }
        public long? InstrumentDate { get; set; }
        public string InstrumentType { get; set; }
        [Complex]
        public Dictionary<int, int> ArtistInventory { get; set; }
        [Complex]
        public List<PaintingKitItemModel> PaintingKitItem { get; set; }
        public string EventNameAndDate { get; set; }
        public DateTime? OrderBookingDate { get; set; }
    }
}
