using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class ChangeEmailViewModel : BaseAutoViewModel<ChangeEmailModel, ChangeEmailViewModel>
    {
        public String OldEmail { get; set; }
        public String NewEmail { get; set; }
    }
}
