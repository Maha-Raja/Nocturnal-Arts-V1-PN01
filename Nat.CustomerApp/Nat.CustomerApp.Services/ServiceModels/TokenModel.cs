using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.CustomerApp.Services.ServiceModels
{
	public class TokenModel
    {
        public String Token { get; set; }
        public Boolean UserFound { get; set; }
        public UserModel User { get; set; }
        public Boolean NewUserCreated { get; set; }
    }
}
