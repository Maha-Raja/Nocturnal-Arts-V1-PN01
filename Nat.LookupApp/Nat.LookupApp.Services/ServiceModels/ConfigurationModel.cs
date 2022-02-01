using Nat.Core.ServiceModels;
using Nat.LookupApp.Models.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.LookupApp.Services.ServiceModels
{
    public class ConfigurationModel : BaseServiceModel<NAT_LUS_Configuration, ConfigurationModel>, IObjectState
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
        public ObjectState ObjectState { get; set; }
    }
}
