using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;

using TLX.CloudCore.Patterns.Repository.Infrastructure;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;

using Nat.PlannerApp.Services.ServiceModels;
using Nat.PlannerApp.Models.EFModel;
using Nat.PlannerApp.Models.Repositories;


namespace Nat.PlannerApp.Services
{
    public class PlannerService : BaseService<PlannerModel, NAT_PLS_Planner>
    {
        private static PlannerService _service;
        public static PlannerService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new PlannerService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private PlannerService() : base()
        {

        }

        /// <summary>
        /// This method return list of all planners
        /// </summary>
        /// <returns>Collection of planner service model<returns>
        public IEnumerable<PlannerModel> GetAllPlanner()
        {
            using (logger.BeginServiceScope("Get All Planner"))
            {
                try
                {
                    IEnumerable<PlannerModel> data = null;
                    logger.LogInformation("Fetch all Planner from repo");
                    IEnumerable<NAT_PLS_Planner> plannerModel = uow.RepositoryAsync<NAT_PLS_Planner>().GetAllPlanner();
                    if (plannerModel != null)
                    {
                        data = new PlannerModel().FromDataModelList(plannerModel);
                        return data;
                    }
                    throw new ApplicationException("No Planner Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// Create: Method for creation of Planner
        /// </summary>
        /// <param name="servicemodel">Service Planner Model</param>
        /// <returns>Planner ID generated for Planner</returns>
        public async Task<string> CreatePlannerAsync(PlannerModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    //var gCalender = await GoogleCalendarApi.CreateCalendarAsync("Calendar Summary");
                    //servicemodel.GoogleCalendarId = gCalender.Id;
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().PlannerId);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        
        public async Task<Boolean> plannertypeUpdateAsync(int id)
        {
            try
            {
                    NAT_PLS_Planner plannerdata = await uow.RepositoryAsync<NAT_PLS_Planner>().GetPlannerByIdAsync(id);
                    plannerdata.Planner_Type_LKP_ID = 1;

                uow.RepositoryAsync<NAT_PLS_Planner>().Update(plannerdata);
                   int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        return false;
                    }
                     return true;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<SlotModel>> UpdateSlotsAsync(IEnumerable<SlotModel> servicemodel)
        {
            try
            {
                var insertedSlots = new List<NAT_PLS_Slot>();
                var deletedSlots = new List<SlotModel>();
                if (servicemodel != null)
                {

                    foreach(var slotModel in servicemodel)
                    {
                        if(slotModel.SlotId == 0)
                        {
                            slotModel.StatusTypeLKPId = 1;
                            slotModel.SlotTypeLKPId = 1;
                            slotModel.ActiveFlag = true;
                            slotModel.ObjectState = ObjectState.Added;
                            var slotEFModel = new SlotModel().ToDataModel(slotModel);
                            uow.RepositoryAsync<NAT_PLS_Slot>().Insert(slotEFModel);
                            insertedSlots.Add(slotEFModel);
                        } 
                        else if(slotModel.SlotId < 0 && slotModel.StatusTypeLKPId == 1) //Delete only free slots
                        {
                            slotModel.SlotId *= -1;
                            slotModel.ObjectState = ObjectState.Deleted;
                            var slotEFModel = new SlotModel().ToDataModel(slotModel);
                            uow.RepositoryAsync<NAT_PLS_Slot>().Delete(slotEFModel);
                            slotModel.SlotId *= -1;
                            deletedSlots.Add(slotModel);
                        }
                    }
                    await uow.SaveChangesAsync();
                    var insertedSlotModel = new SlotModel().FromDataModelList(insertedSlots).ToList();
                    return insertedSlotModel.Concat(deletedSlots);
                }
                else
                {
                    throw new Exception("Slots not provided");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<SlotModel> CreateSlotAsync(SlotModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {

                    int plannerid = servicemodel.PlannerId;
                    


                    IEnumerable<SlotModel> data = null;
                    logger.LogInformation("Fetch all Slots from repo for particular planner id");
                    IEnumerable<NAT_PLS_Slot> slotModel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllSlotsByPlannerId(plannerid);
                    if (slotModel != null)
                    {
                        data = new SlotModel().FromDataModelList(slotModel);
                    }

                    foreach( var slot1 in data)
                    {
                        if ((servicemodel.StartTime >= slot1.StartTime && servicemodel.StartTime <= slot1.EndTime) || (servicemodel.EndTime >= slot1.StartTime && servicemodel.EndTime <= slot1.EndTime))
                        {
                            throw new Exception("This slot is overlapping with other slots");
                        }

                    }

                    servicemodel.StatusTypeLKPId = 1;
                    servicemodel.SlotTypeLKPId = 1;
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    uow.RepositoryAsync<NAT_PLS_Slot>().Insert(new SlotModel().ToDataModel(servicemodel));
                   // Insert(servicemodel);
                    uow.SaveChanges();
                    return servicemodel;

                    
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Planner with a given Id
        /// </summary>
        /// <param name="Id">Id of Planner</param>
        /// <returns>Planner service model</returns>
        public async Task<PlannerModel> GetByIdPlannerAsync(int Id)

        {
            try
            {
                PlannerModel data = null;
                NAT_PLS_Planner plannerModel = await uow.RepositoryAsync<NAT_PLS_Planner>().GetPlannerByIdAsync(Id);
                if (plannerModel != null)
                {
                    data = new PlannerModel().FromDataModel(plannerModel);                  
                    return data;
                }
                throw new Exception("No Planner Model Found By Given ID");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Planner record
        /// </summary>
        /// <param name="servicemodel">Service PlannerModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdatePlannerAsync(PlannerModel servicemodel)
        {
            try
            {
                if (servicemodel.PlannerId != 0 || servicemodel.PlannerId > 0)
                {
                    servicemodel.ObjectState = ObjectState.Modified;
                    base.Update(servicemodel);
                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates planner 
        /// </summary>
        /// <param name="Id">Id of planner</param>
        public async Task DeactivatePlannerAsync(string Id)
        {
            try
            {
                NAT_PLS_Planner PlannerEf = await uow.RepositoryAsync<NAT_PLS_Planner>().GetPlannerByIdAsync(Convert.ToInt32(Id));
                if (PlannerEf != null)
                {
                    uow.RepositoryAsync<NAT_PLS_Planner>().SetActiveFlag(false, PlannerEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Planner doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<SlotModel> deleteslotbyid(int Id)
        {
            try
            {
                SlotModel data = null;
                logger.LogInformation("Fetch Slot from repo");
                NAT_PLS_Slot slotModel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetSlotBySlotID(Id);
                if (slotModel != null)
                {
                    data = new SlotModel().FromDataModel(slotModel);
                    if(data.EndTime >= DateTime.UtcNow)
                    { await uow.RepositoryAsync<NAT_PLS_Slot>().DeleteSlotByslotidAsync(Id);
                        uow.SaveChanges();
                        return data;

                    }
                }
                throw new ApplicationException("An error occurred while deleting the slot");

               
               
               
                   
                    uow.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public IEnumerable<PlannerModel> CustomSearchPlanner(PlannerCustomSearchModel serviceModel)
        {
            using (logger.BeginServiceScope("Get All Planner"))
            {
                try
                {
                    IEnumerable<PlannerModel> data = null;
                    IEnumerable<NAT_PLS_Planner> plannerModel = uow.RepositoryAsync<NAT_PLS_Planner>().GetAllRequestedPlanner(serviceModel.StartTime,serviceModel.EndTime,serviceModel.ReferenceType);
                    if (plannerModel != null)
                    {
                        data = new PlannerModel().FromDataModelList(plannerModel);
                        return data;
                    }
                    throw new ApplicationException("No Planner Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<IEnumerable<int>> CustomSearchPlannerAsyncchange(PlannerCustomSearchModel serviceModel)
        {
            using (logger.BeginServiceScope("Get All Planner"))
            {
                try
                {
                    List<int> plannerids = null;

                    plannerids = new List<int>();
                    IEnumerable<SlotModel> slotdata = null;
                    IEnumerable<NAT_PLS_Slot> slotmodel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllSlotByStartimeAndEndtime(serviceModel.StartTime, serviceModel.EndTime, serviceModel.PlannerIds);
                    slotdata = new SlotModel().FromDataModelList(slotmodel);
                    foreach (SlotModel obj in slotdata)
                    {
                        if (!plannerids.Contains(obj.PlannerId))
                        {
                            plannerids.Add(obj.PlannerId);
                        }

                    }

                    if (plannerids != null)
                    {

                        return plannerids;
                    }
                    throw new ApplicationException("No Planner Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<IEnumerable<int>> GetPlannerCollidingEvent(PlannerCustomSearchModel serviceModel)
        {
            using (logger.BeginServiceScope("Return all Planner that are colliding"))
            {
                try
                {
                    List<int> plannerids = null;

                    plannerids = new List<int>();
                    IEnumerable<SlotModel> slotdata = null;
                    IEnumerable<NAT_PLS_Slot> slotmodel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetPlannerCollidingEvent(serviceModel.StartTime, serviceModel.EndTime, serviceModel.PlannerIds);
                    slotdata = new SlotModel().FromDataModelList(slotmodel);
                    foreach (SlotModel obj in slotdata)
                    {
                        if (!plannerids.Contains(obj.PlannerId))
                        {
                            plannerids.Add(obj.PlannerId);
                        }

                    }
                    return plannerids;
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<SlotWarningModel> WarningByPlannerIdAsync(SlotModel serviceModel)
        {
            using (logger.BeginServiceScope("Check for collisions for Artist and Venue"))
            {
                try
                {
                    SlotWarningModel warning = new SlotWarningModel();
                    SlotModel data = null;
                    NAT_PLS_Slot slots =  await uow.RepositoryAsync<NAT_PLS_Slot>().GetSlotByPlannerID( serviceModel.PlannerId, serviceModel.StartTime, serviceModel.EndTime);
                    if (slots != null)
                    {
                        data = new SlotModel().FromDataModel(slots);
            
                        if (data.EventId !=null)
                        {
                          warning.Message = "Slot Already Booked";
                          warning.WarningType = 1;
                          return warning;

                        }
                        else
                        {
                            warning.Message = "Slot Available";
                            warning.WarningType = 2;
                            return warning;

                        }

                    }


                    IEnumerable<NAT_PLS_Slot> CollidingSlots = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllCollidingSlot(serviceModel.PlannerId, serviceModel.StartTime, serviceModel.EndTime);

                    if (CollidingSlots != null)
                    {

                        foreach (var slot1 in CollidingSlots)
                        {
                            if (slot1.Event_ID != null)
                            {
                                warning.Message = "Slot Already Booked";
                                warning.WarningType = 1;
                                return warning;
                            }
                        }
                    }

                    warning.Message = "Slot Not Available";
                    warning.WarningType = 3;
                    return warning;
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<IEnumerable<SlotModel>> SearchSlotInPlanner(List<Int32> plannerIds, DateTime startdate, DateTime enddate)
        {
            using (logger.BeginServiceScope("Get All Availability"))
            {
                try
                {
                    IEnumerable<SlotModel> data = null;
                    logger.LogInformation("Fetch all Slots from repo");
                    if(plannerIds == null)
                    {
                        plannerIds = new List<int>();
                    }
                    IEnumerable<NAT_PLS_Slot> slotModel = await uow.RepositoryAsync<NAT_PLS_Slot>().GetAllSlotByPlannerId(plannerIds, startdate, enddate);
                    if (slotModel != null)
                    {
                        data = new SlotModel().FromDataModelList(slotModel);
                        return data;
                    }
                    throw new ApplicationException("No Slots Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }
    }
}