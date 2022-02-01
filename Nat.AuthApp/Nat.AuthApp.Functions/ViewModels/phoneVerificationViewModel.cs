using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class PhoneVerificationViewModel
    {
        public String PhoneNumber { get; set; }
        public String VerificationCode { get; set; }
        public Boolean IsLogin { get; set; }
    }
}
