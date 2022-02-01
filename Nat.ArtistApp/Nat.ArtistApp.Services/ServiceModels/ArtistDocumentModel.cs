using Nat.ArtistApp.Models.EFModel;
using Nat.Core.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.ArtistApp.Services.ServiceModels
{
    public class ArtistDocumentModel : BaseServiceModel<NAT_AS_Artist_Document, ArtistDocumentModel>, IObjectState
    {
        public Int32 ArtistDocumentId { get; set; }
        public Nullable<Int32> ArtistId { get; set; }
        public String DocumentName { get; set; }
        public String DocumentUrl { get; set; }
        public String FileName { get; set; }
        public String FileType { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public ObjectState ObjectState { get; set; }
    }
}
