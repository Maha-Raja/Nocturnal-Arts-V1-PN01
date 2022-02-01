using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.EventApp.Services.ServiceModels;
using Nat.Core.ViewModels;

namespace Nat.EventApp.Functions.ViewModels
{
	public class ArtistViewModel : BaseAutoViewModel<ArtistModel, ArtistViewModel>
    {
        public Int32 ArtistId { get; set; }
    }
}
