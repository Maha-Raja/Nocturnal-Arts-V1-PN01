using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class VerifyEmailModel
    {
        public String UserName { get; set; }
        public DateTime ValidTime { get; set; }
    }
}
