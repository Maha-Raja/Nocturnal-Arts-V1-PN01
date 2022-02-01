using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class UserReferenceViewModel : BaseAutoViewModel<UserReferenceModel, UserReferenceViewModel>
    {
        public Int64 UserId { get; set; }
        public Int64 ReferenceId { get; set; }
        public string ReferenceTypeLkp { get; set; }
    }
}
