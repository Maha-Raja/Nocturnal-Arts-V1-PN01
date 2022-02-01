using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class LookupViewModel : BaseAutoViewModel<LookupModel, LookupViewModel>
	{
		public Int32 Lookupid { get; set; }
		public Int32 TenantId { get; set; }
		public String HiddenValue { get; set; }
		public String VisibleValue { get; set; }
		public Boolean ActiveFlag { get; set; }
		public Boolean UserEditableFlag { get; set; }
		public String LookupType { get; set; }
		public String LookupDescription { get; set; }
		public Boolean FilteredFlag { get; set; }
		public Boolean PreferredFlag { get; set; }
		public String SortOrder { get; set; }
		public DateTime EffectiveStartDate { get; set; }
		public DateTime EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public DateTime LastUpdatedDate { get; set; }
	}
}
