using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class TokenModel
    {
        public String Token { get; set; }
        public String RefreshToken { get; set; }
        [Complex]
        public UserModel User { get; set; }
        public Boolean UserFound { get; set; }
        public Boolean LoginSuccess { get; set; }
        public String Reason { get; set; }
        public Boolean NewUserCreated { get; set; }
    }
}
