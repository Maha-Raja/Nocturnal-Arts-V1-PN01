using Nat.ArtistApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Functions.ViewModels
{
    public class ArtistDocumentViewModel : BaseAutoViewModel<ArtistDocumentModel, ArtistDocumentViewModel>
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
    }
}
