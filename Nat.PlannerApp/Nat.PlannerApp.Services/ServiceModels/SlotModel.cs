using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.PlannerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.PlannerApp.Services.ServiceModels
{
    public class SlotModel : BaseServiceModel<NAT_PLS_Slot, SlotModel>, IObjectState
    {
        public Int32 SlotId { get; set; }
        public Int32 PlannerId { get; set; }
        public Nullable<Int32> EventId { get; set; }
        public String EventName { get; set; }
        public String Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String TimingLKP { get; set; }
        public Int32 StatusTypeLKPId { get; set; }
        public Int32 SlotTypeLKPId { get; set; }
        public Nullable<Boolean> ActiveFlag { get; set; }
        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public String LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public PlannerModel NatPlsPlanner { get; set; }

        [Complex]
        public EventModel NatPlsEvent { get; set; }
        public ObjectState ObjectState { get; set; }

        public String ReferenceId
        {
         
            get
            {
                
                    return this.NatPlsEvent != null ? this.NatPlsEvent.ReferenceId : null;
            }
               
        
            //set
            //{
            //    this.Coordinates = value != null ? DbGeography.FromText(value) : null;
            //}
        }
    }
}
