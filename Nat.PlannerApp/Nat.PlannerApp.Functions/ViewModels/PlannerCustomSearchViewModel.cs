using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PlannerApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.PlannerApp.Functions.ViewModels
{
	public class PlannerCustomSearchViewModel : BaseAutoViewModel<PlannerCustomSearchModel, PlannerCustomSearchViewModel>
	{
		public Nullable<DateTime> StartTime { get; set; }
		public Nullable<DateTime> EndTime { get; set; }
		public Int32 ReferenceType { get; set; }
		public List<Int32> PlannerIds { get; set; }
	}
}
