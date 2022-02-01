using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.EventApp.Services.ServiceModels
{
    public class BookedEventsModel
    {
        public String EventCode { get; set; }
        public Int32 TicketsCount { get; set; }
    }
}

