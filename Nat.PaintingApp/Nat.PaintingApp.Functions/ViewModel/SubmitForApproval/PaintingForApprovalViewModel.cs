using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TLX.CloudCore.Patterns.DataMapper;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Nat.PaintingApp.Services.ServiceModels.SubmitForApproval;

namespace Nat.PaintingApp.Functions.ViewModel.SubmitForApproval
{
    public class PaintingForApprovalViewModel : BaseAutoViewModel<PaintingForApprovalModel, PaintingForApprovalViewModel>
    {
        
            public Int32 PaintingId { get; set; }
            public String PaintingName { get; set; }
            public String Category { get; set; }
            public String Type { get; set; }
            public Nullable<Int32> ArtistId { get; set; }
            public String InspiredFrom { get; set; }
            public Nullable<Int32> DifficultyLevel { get; set; }
            public Nullable<Int32> VideoTutorialId { get; set; }
            [Complex]
            public ICollection<ImageViewModel> Image { get; set; }


    }

}

