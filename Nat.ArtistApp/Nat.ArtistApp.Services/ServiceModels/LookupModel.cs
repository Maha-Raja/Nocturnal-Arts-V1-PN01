using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
	public class LookupModel //: BaseServiceModel<NAT_AS_Lookup, LookupModel>, IObjectState
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
		public String SortOrder { get; set; }
		public Nullable<DateTime> EffectiveStartDate { get; set; }
		public Nullable<DateTime> EffectiveEndDate { get; set; }
		public String CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public String LastUpdatedBy { get; set; }
		public Nullable<DateTime> LastUpdatedDate { get; set; }
		public ObjectState ObjectState { get; set; }
	}
}
