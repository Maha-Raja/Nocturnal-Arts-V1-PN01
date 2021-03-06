using Nat.Core.Lookup.Model;
using System;

namespace Nat.ArtistApp.Services.ViewModels
{
	public class LookupViewModel : ILookupModel
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
		public Nullable<DateTime> Created_Date { get; set; }
        public string GetHiddenValue()
        {
            return this.HiddenValue;
        }
        public string GetVisibleValue()
        {
            return this.VisibleValue;
        }
    }
}
