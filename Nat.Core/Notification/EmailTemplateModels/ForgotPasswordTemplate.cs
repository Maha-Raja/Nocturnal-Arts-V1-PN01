using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Notification.EmailTemplateModels
{
    public class ForgotPasswordTemplate
    {
        [JsonProperty("Name")]
        public string Name { get; set; }


        [JsonProperty("VerifyLink")]
        public string VerifyLink { get; set; }
    }
}
