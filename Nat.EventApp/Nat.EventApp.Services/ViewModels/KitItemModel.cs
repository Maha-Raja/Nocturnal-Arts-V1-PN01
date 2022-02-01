﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ViewModels
{
	public class KitItemModel
	{
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool ActiveFlag { get; set; }
        public int KitItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public string KitItemCode { get; set; }
        public string KitItemName { get; set; }
        public string KitItemDescription { get; set; }
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
        public decimal ItemCost { get; set; }
        public int Quantity { get; set; }
        public bool TaxableFlag { get; set; }
        public int MinQuantity { get; set; }
        public bool SoldoutFlag { get; set; }
        public decimal Markup { get; set; }
        public int? SortOrder { get; set; }
        public string Uom { get; set; }
        public ICollection<OrderLineModel> NatOrderLine { get; set; }
        [Complex]
        public ICollection<PaintingKitItemModel> NatPaintingKitItem { get; set; }
        public Boolean AutoGeneratedCode { get; set; }
    }
}