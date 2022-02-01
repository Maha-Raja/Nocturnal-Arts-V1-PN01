using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class ForgetPasswordViewModel 
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}