using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.PaintingApp.Services.ServiceModels.SubmitForApproval
{
    public class PaintingForApprovalEntityModel : TableEntity
    {

            public PaintingForApprovalEntityModel(string guid, PaintingForApprovalModel obj)
            {
                this.PartitionKey = "Paintings for approval";
                this.RowKey = guid;
                this.PaintingId = obj.PaintingId;
                this.PaintingName = obj.PaintingName;
                this.Category = obj.Category;
                this.Type = obj.Type;
                this.ArtistId = obj.ArtistId;
                this.InspiredFrom = obj.InspiredFrom;
                this.DifficultyLevel = obj.DifficultyLevel;
                this.VideoTutorialId = obj.VideoTutorialId;

            }


        public PaintingForApprovalEntityModel () { }
        
             public Int32 PaintingId { get; set; }
             public String PaintingName { get; set; }
             public String Category { get; set; }
             public String Type { get; set; }
             public Int32 ArtistId { get; set; }
             public String InspiredFrom { get; set; }
             public Nullable<Int32> DifficultyLevel { get; set; }
             public Nullable<Int32> VideoTutorialId { get; set; }
             public String ImageJsonData { get; set; }
    }
    }

