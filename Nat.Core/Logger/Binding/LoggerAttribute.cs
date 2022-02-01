using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System.ComponentModel.DataAnnotations;

namespace Nat.Core.Logger
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class LoggerAttribute : Attribute
    {
    }
}
