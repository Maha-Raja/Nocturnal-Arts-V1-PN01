using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.PlannerApp.Services.ServiceModels
{
	public class PlannerCustomSearchModel
	{
		public Nullable<DateTime> StartTime { get; set; }
		public Nullable<DateTime> EndTime { get; set; }
		public Int32 ReferenceType { get; set; }
		public List<Int32> PlannerIds { get; set; }
	}
}
