using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.EventApp.Models.EFModel;
using Nat.Core.ServiceModels;

namespace Nat.EventApp.Services.ServiceModels
{
	public class ArtistModel
    {
        public Int32 ArtistId { get; set; }
        public String ArtistEmail { get; set; }
        public String CompanyEmail { get; set; }
        public String CompanyPhone { get; set; }
        public String ArtistMiddleName { get; set; }
        public String ArtistLastName { get; set; }
        public String ArtistFirstName { get; set; }
        public String ContactNumber { get; set; }
        public bool IsArtistAvailable { get; set; }

        public string VideoKey { get; set; }
        public string VideoSecret { get; set; }
        public string VideoUser { get; set; }

    }
}
