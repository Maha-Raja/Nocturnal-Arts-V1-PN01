using PaintingDataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model.Painting
{
    public class PaintingRatingLogDTO : BaseDTO<NAT_PS_Painting_Rating_Log, PaintingRatingLogDTO>
    {
        public int Painting_Rating_Log_ID { get; set; }
        public Nullable<int> Tenant_ID { get; set; }
        public Nullable<int> Painting_ID { get; set; }
        public Nullable<System.DateTime> Review_Date { get; set; }
        public string Review_Title { get; set; }
        public string Review_Details { get; set; }
        public double Rating_Value { get; set; }
        public string Active_Flag { get; set; }
        public Nullable<System.DateTime> Effective_Start_Date { get; set; }
        public Nullable<System.DateTime> Effective_End_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Last_Updated_By { get; set; }
        public Nullable<System.DateTime> Last_Updated_Date { get; set; }

        public void FromEFModel(NAT_PS_Painting_Rating_Log _PaintingRatingLog)
        {
            FromEFModel(_PaintingRatingLog, this);
        }

        public NAT_PS_Painting_Rating_Log ToEFModel()
        {
            return ToEFModel(this);
        }
    }
}
