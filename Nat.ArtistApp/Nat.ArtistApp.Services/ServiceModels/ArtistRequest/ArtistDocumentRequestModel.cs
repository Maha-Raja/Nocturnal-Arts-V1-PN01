using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels.ArtistRequest
{
    public class ArtistDocumentRequestModel
    {
        public Int32 ArtistDocumentId { get; set; }
        public String FileName { get; set; }
        public String DocumentUrl { get; set; }
        public String FileType { get; set; }
        public String DocumentName { get; set; }
    }
}
