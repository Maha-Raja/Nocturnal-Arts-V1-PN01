using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class ChangePasswordResultModel
    {
        public Boolean Success { get; set; }
        public String Reason { get; set; }
    }
}
