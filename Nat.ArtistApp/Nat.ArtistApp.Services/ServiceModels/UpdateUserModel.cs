using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class UpdateUserModel
    {
        public long ReferenceId { get; set; }
        public String ReferenceTypeLKP { get; set; }
    }
}
