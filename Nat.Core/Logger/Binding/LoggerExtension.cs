using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Logger
{
    public class LoggerExtension : IExtensionConfigProvider
    {

        /// <summary>
        /// This callback is invoked by the WebJobs framework before the host starts execution. 
        /// It should add the binding rules and converters for our new <see cref="SampleAttribute"/> 
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(ExtensionConfigContext context)
        {
            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<LoggerAttribute>();
            rule.Bind(new LoggerBindingProvider());
        }
    }
}
