using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class ChangeEmailModel
    {
        public String OldEmail { get; set; }
        public String NewEmail { get; set; }
    }
}
