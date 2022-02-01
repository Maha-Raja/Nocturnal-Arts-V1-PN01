using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class UserReferenceModel
    {
        public Int64 UserId { get; set; }
        public Int64 ReferenceId { get; set; }
        public string ReferenceTypeLkp { get; set; }
    }
}
