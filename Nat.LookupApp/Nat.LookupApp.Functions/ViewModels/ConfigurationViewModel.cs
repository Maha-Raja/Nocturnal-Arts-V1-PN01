using Nat.Core.ViewModels;
using Nat.LookupApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.LookupApp.Functions.ViewModels
{
    public class ConfigurationViewModel : BaseAutoViewModel<ConfigurationModel, ConfigurationViewModel>
    {
        public int ConfigurationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool UserEditable { get; set; }
        public bool ActiveFlag { get; set; } 
        public Nullable<DateTime> Effective_Start_Date { get; set; }
        public Nullable<DateTime> Effective_End_Date { get; set; }
        public String Created_By { get; set; }
        public Nullable<DateTime> Created_Date { get; set; }
        public String Last_Updated_By { get; set; }
        public Nullable<DateTime> Last_Updated_Date { get; set; }
    }
}
