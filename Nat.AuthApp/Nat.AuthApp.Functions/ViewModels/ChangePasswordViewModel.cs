using Nat.Core.ViewModels;
using Nat.AuthApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class ChangePasswordViewModel : BaseAutoViewModel<ChangePasswordModel, ChangePasswordViewModel>
    {
        public String Username { get; set; }
        public String NewPassword { get; set; }
        public String OldPassword { get; set; }
        public String VerificationCode { get; set; }
    }
}
