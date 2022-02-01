using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Services.ServiceModels
{
    public class UpdateUserModel
    {
        
        public String UserName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String UserImageURL { get; set; }
        public long ReferenceId { get; set; }
        public String ReferenceTypeLKP { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String VerificationCode { get; set; }
    }

    public class UserActivation
    {
        public long UserId { get; set; }

    }
}
