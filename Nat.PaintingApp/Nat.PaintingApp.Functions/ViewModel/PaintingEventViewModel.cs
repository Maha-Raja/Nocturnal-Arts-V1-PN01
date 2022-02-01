using Nat.Core.ViewModels;
using Nat.PaintingApp.Functions.ViewModels;
using Nat.PaintingApp.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.PaintingApp.Functions.ViewModel
{
    public class PaintingEventViewModel : BaseAutoViewModel<PaintingEventModel, PaintingEventViewModel>

    {

        public Int32 EventId { get; set; }
        public Int32 PaintingId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String EventTypeLKP { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ReferenceId { get; set; }
        public String StatusLKP { get; set; }
        public String UDF { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Location { get; set; }
        public PaintingViewModel NatPsPainting { get; set; }
        public ObjectState ObjectState { get; set; }

    }
}
