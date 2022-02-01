using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class ArtistEventModel 
    {
        public Int32 ArtistId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public Int32 EventTypeLKPId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String ReferenceId { get; set; }
        public Nullable<Int32> StatusLKPId { get; set; }
        public String UDF { get; set; }
        public Boolean Forced { get; set; }
        public Boolean Online { get; set; }
        public string GoogleHangoutUrl { get; set; }
        public string SlotTiming { get; set; }
        public string LocationCode { get; set; }
    }
}
