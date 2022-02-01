using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ServiceModels.VenueRequest
{
    public class VenueContactPersonRequestModel
    {
        public String ImagePath { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Designation { get; set; }
        public String ContactNumber { get; set; }
        public string Notes { get; set; }
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public Boolean PasswordResetFlag { get; set; }
        public String UserRole { get; set; }
        public bool PrimaryVCP { get; set; }
        public Nullable<bool> TextFlag { get; set; }
        public string Greeting { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }

    }
}
