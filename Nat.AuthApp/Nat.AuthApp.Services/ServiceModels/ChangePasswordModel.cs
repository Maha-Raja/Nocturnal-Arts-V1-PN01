using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class ChangePasswordModel
    {
        public String Username { get; set; }
        public String NewPassword { get; set; }
        public String OldPassword { get; set; }
        public String VerificationCode { get; set; }
    }
}
