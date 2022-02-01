using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.LookupApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.LookupApp.Functions.ViewModels
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
		public Nullable<Boolean> FilteredFlag { get; set; }
		public Nullable<Boolean> PreferredFlag { get; set; }
		public Nullable<Int32> SortOrder { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public String StringAttribute1 { get; set; }
		public String StringAttribute2 { get; set; }
		public String StringAttribute3 { get; set; }
		public String StringAttribute4 { get; set; }
		public String StringAttribute5 { get; set; }
	}
}
