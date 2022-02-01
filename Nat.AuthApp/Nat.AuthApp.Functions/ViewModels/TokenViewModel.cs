using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class TokenViewModel : BaseAutoViewModel<TokenModel, TokenViewModel>
    {
        public String Token { get; set; }
        public String RefreshToken { get; set; }
        [Complex]
        public UserViewModel User { get; set; }
        [JsonIgnore]
        public Boolean UserFound { get; set; }
        [JsonIgnore]
        public Boolean LoginSuccess { get; set; }
        public Boolean NewUserCreated { get; set; }
    }
}