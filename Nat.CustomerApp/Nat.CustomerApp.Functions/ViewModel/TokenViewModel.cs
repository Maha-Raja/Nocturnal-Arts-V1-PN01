using Nat.CustomerApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.CustomerApp.Functions.ViewModels
{
    public class TokenViewModel : BaseAutoViewModel<TokenModel, TokenViewModel>
    {
        public String Token { get; set; }
        public UserModel User { get; set; }
    }
}