using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ViewModels
{
    class UserReferenceViewModel
    {
        public Int64 UserId { get; set; }
        public long ReferenceId { get; set; }
        public string ReferenceTypeLkp { get; set; }
    }
}
