using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class PhoneVerificationModel
    {
        public String PhoneNumber { get; set; }
        public String VerificationCode { get; set; }
    }
}
