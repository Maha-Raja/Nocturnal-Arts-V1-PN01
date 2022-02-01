using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    public class ChangePasswordViewModel
    {
        public String Username { get; set; }
        public String NewPassword { get; set; }
        public String OldPassword { get; set; }
    }
}
