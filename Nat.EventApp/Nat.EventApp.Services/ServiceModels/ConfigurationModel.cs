using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.EventApp.Services.ServiceModels
{
	public class ConfigurationModel
	{
        public int ConfigurationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool UserEditable { get; set; }
        public bool ActiveFlag { get; set; }
    }
}
