using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Nat.EventApp.Services.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class SeatBookingViewModel : BaseAutoViewModel<SeatBookingModel, SeatBookingViewModel>
	{
        public String EventCode { get; set; }
        public String Status { get; set; }
        public String SeatNumber { get; set; }
        public String RowNumber { get; set; }
    }
}
