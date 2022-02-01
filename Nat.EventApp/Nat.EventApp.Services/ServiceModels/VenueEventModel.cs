using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ServiceModels
{
	public class VenueEventModel
    {
        public Int32 VenueId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public Int32 EventTypeLKPId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String ReferenceId { get; set; }
        public Nullable<Int32> StatusLKPId { get; set; }
        public String UDF { get; set; }
        public Boolean Forced { get; set; }
    }
}
