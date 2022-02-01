using System;

namespace Nat.CustomerApp.Services.ViewModels
{
    public class UserReferenceViewModel
    {
        public Int64 UserId { get; set; }
        public Int64 ReferenceId { get; set; }
        public string ReferenceTypeLkp { get; set; }
    }
}
