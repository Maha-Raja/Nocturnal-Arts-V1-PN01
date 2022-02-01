using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PaintingApp.Services.ServiceModels.SubmitForApproval
{
    public class PaintingForApprovalModel
    {

            public Int32 PaintingId { get; set; }
            public String PaintingName { get; set; }
            public String Category { get; set; }
            public String Type { get; set; }
            public Int32 ArtistId { get; set; }
            public String InspiredFrom { get; set; }
            public Nullable<Int32> DifficultyLevel { get; set; }
            public Nullable<Int32> VideoTutorialId { get; set; }
            [Complex]
            public ICollection<ImageModel> Image { get; set; }

    }

}

