using Nat.Core.Lookup.Model;
using System;

namespace Nat.VenueApp.Services.ViewModels
{
    public class ChangePasswordViewModel
    {
        public String Username { get; set; }
        public String NewPassword { get; set; }
        public String OldPassword { get; set; }
    }
}
