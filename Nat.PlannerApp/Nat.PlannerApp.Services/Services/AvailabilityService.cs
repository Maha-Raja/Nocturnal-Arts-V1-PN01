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
using Nat.Common.Constants;

namespace Nat.PlannerApp.Services
{
    public class AvailabilityService : BaseService<AvailabilityModel, NAT_PLS_Availability>
    {
        private static AvailabilityService _service;
        public static AvailabilityService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new AvailabilityService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private AvailabilityService() : base()
        {

        }

        /// <summary>
        /// This method return list of all availabilitys
        /// </summary>
        /// <returns>Collection of availability service model<returns>
        public IEnumerable<AvailabilityModel> GetAllAvailability()
        {
            using (logger.BeginServiceScope("Get All Availability"))
            {
                try
                {
                    IEnumerable<AvailabilityModel> data = null;
                    logger.LogInformation("Fetch all Availability from repo");
                    IEnumerable<NAT_PLS_Availability> availabilityModel = uow.RepositoryAsync<NAT_PLS_Availability>().GetAllAvailability();
                    if (availabilityModel != null)
                    {
                        data = new AvailabilityModel().FromDataModelList(availabilityModel);
                        return data;
                    }
                    throw new ApplicationException("No Availability Available");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// Create: Method for creation of Availability
        /// </summary>
        /// <param name="servicemodel">Service Availability Model</param>
        /// <returns>Availability ID generated for Availability</returns>
        public async Task<string> CreateAvailabilityAsync(IEnumerable<AvailabilityModel> servicemodel)
        {
            try
            {
                foreach (var tempServiceModel in servicemodel)
                {
                    if (tempServiceModel != null)
                    {
                        tempServiceModel.ActiveFlag = true;
                        tempServiceModel.ObjectState = ObjectState.Added;
                        CreateSlot(tempServiceModel);
                        Insert(tempServiceModel);
                    }
                    else
                    {
                        return null;
                    }
                }
                await uow.SaveChangesAsync();
                return Convert.ToString(Get().AvailabilityId);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Availability with a given Id
        /// </summary>
        /// <param name="Id">Id of Availability</param>
        /// <returns>Availability service model</returns>
        public async Task<AvailabilityModel> GetByIdAvailabilityAsync(int Id)

        {
            try
            {
                AvailabilityModel data = null;
                NAT_PLS_Availability availabilityModel = await uow.RepositoryAsync<NAT_PLS_Availability>().GetAvailabilityByIdAsync(Id);
                if (availabilityModel != null)
                {
                    data = new AvailabilityModel().FromDataModel(availabilityModel);
                    return data;
                }
                throw new Exception("No Availability Model Found By Given ID");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public IEnumerable<AvailabilityModel> GetByPlannerIdAvailability(int Id)

        {
            try
            {
                IEnumerable<AvailabilityModel> data = null;
                IEnumerable<NAT_PLS_Availability> availabilityModel =  uow.RepositoryAsync<NAT_PLS_Availability>().GetAvailabilityByPlannerId(Id);
                if (availabilityModel != null)
                {
                    data = new AvailabilityModel().FromDataModelList(availabilityModel);
                    return data;
                }
                throw new Exception("No Availability Model Found By Given ID");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of Availability record
        /// </summary>
        /// <param name="servicemodel">Service AvailabilityModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateAvailabilityAsync(IEnumerable<AvailabilityModel> serviceModelList)
        {
            try
            {
                var tempModel = serviceModelList.FirstOrDefault();
                await uow.RepositoryAsync<NAT_PLS_Slot>().DeleteSlotByPlannerIdAsync(tempModel.PlannerId);
                await uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlotByPlannerIdAsync(tempModel.PlannerId);             
                await uow.RepositoryAsync<NAT_PLS_Availability>().DeleteAvailabilityByPlannerIdAsync(tempModel.PlannerId);


                int updatedRow = await uow.SaveChangesAsync();



                foreach (var servicemodel in serviceModelList) {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    CreateSlot(servicemodel);
                    Insert(servicemodel);

                    //if (servicemodel.AvailabilityId > 0)
                    //{
                    //    //NAT_PLS_Availability availabilityModel = await uow.RepositoryAsync<NAT_PLS_Availability>().GetAvailabilityByIdAsync(id);
                    //    NAT_PLS_Availability availabilityModel = await uow.RepositoryAsync<NAT_PLS_Availability>().GetAvailabilityByIdAsync(servicemodel.AvailabilityId);
                    //    if (availabilityModel.NAT_PLS_Availability_Slot != null)
                    //    {
                    //        //Deleting Old Availability Slots
                    //        foreach (var previousAvailabilitySlots in availabilityModel.NAT_PLS_Availability_Slot)
                    //        {
                    //            //previousAvailabilitySlots.Active_Flag = false;
                    //            //previousAvailabilitySlots.ObjectState = ObjectState.Deleted;
                    //            //uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlot(previousAvailabilitySlots);
                    //            await uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlotAsync(previousAvailabilitySlots.Availability_Slot_ID);
                    //        }
                    //    }
                    //    servicemodel.ObjectState = ObjectState.Modified;
                    //    CreateSlot(servicemodel);
                    //    Update(servicemodel);
                    //}

                    //if (servicemodel.AvailabilityId < 0)
                    //{
                    //    servicemodel.AvailabilityId *= -1;
                    //    NAT_PLS_Availability availabilityModel = await uow.RepositoryAsync<NAT_PLS_Availability>().GetAvailabilityByIdAsync(servicemodel.AvailabilityId);
                    //    if (availabilityModel.NAT_PLS_Availability_Slot != null)
                    //    {
                    //        //Deleting Old Availability Slots
                    //        foreach (var previousAvailabilitySlots in availabilityModel.NAT_PLS_Availability_Slot)
                    //        {
                    //            //previousAvailabilitySlots.Active_Flag = false;
                    //            //previousAvailabilitySlots.ObjectState = ObjectState.Deleted;
                    //            //uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlot(previousAvailabilitySlots);
                    //            await uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlotAsync(previousAvailabilitySlots.Availability_Slot_ID);
                    //        }
                    //    }
                    //    await uow.RepositoryAsync<NAT_PLS_Availability>().DeleteAvailabilityAsync(servicemodel.AvailabilityId);

                    //}
                    //if (servicemodel.AvailabilityId == 0)
                    //{
                    //    servicemodel.ActiveFlag = true;
                    //    servicemodel.ObjectState = ObjectState.Added;
                    //    CreateSlot(servicemodel);
                    //    Insert(servicemodel);
                    //}

                }
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

        /// <summary>
        /// Update: Method for Updation of Availability record against multiple planner ids 
        /// </summary>
        /// <param name="servicemodel">Service AvailabilityModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateEntityAvailabilityAsync(IEnumerable<AvailabilityModel> serviceModelList)
        {
            try
            {
                var PlannerIdList = new List<int>();
                PlannerIdList = serviceModelList.Select((x) => {
                    return x.PlannerId;
                }).ToList();

                PlannerIdList = PlannerIdList.Distinct().ToList();
                foreach (var PlannerId in PlannerIdList)
                {
                    await uow.RepositoryAsync<NAT_PLS_Slot>().DeleteSlotByPlannerIdAsync(PlannerId);
                    await uow.RepositoryAsync<NAT_PLS_Availability_Slot>().DeleteAvailabilitySlotByPlannerIdAsync(PlannerId);
                    await uow.RepositoryAsync<NAT_PLS_Availability>().DeleteAvailabilityByPlannerIdAsync(PlannerId);
                }

                int Row = await uow.SaveChangesAsync();
                foreach (var servicemodel in serviceModelList)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    CreateSlot(servicemodel);
                    Insert(servicemodel);
                }
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



        public void CreateSlot(AvailabilityModel servicemodel)
        {

            if (servicemodel.NatPlsAvailabilitySlot != null)
            {
                //Creating New Updated Availability Slots
                foreach (AvailabilitySlotModel AvailabilitySlots in servicemodel.NatPlsAvailabilitySlot)
                {
                    AvailabilitySlots.ActiveFlag = true;
                    AvailabilitySlots.ObjectState = ObjectState.Added;
                    int dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
                    DateTime dateTime = DateTime.UtcNow;
                    TimeSpan slotSpan = AvailabilitySlots.EndTime.Subtract(AvailabilitySlots.StartTime);
                    int count = 30;
                    while (count > 0)
                    {

                        if (dayOfWeek == servicemodel.DayOfWeekLKPId)
                        {
                            SlotModel slotModel = new SlotModel();
                            slotModel.PlannerId = servicemodel.PlannerId;
                            slotModel.StartTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, AvailabilitySlots.StartTime.Hour, AvailabilitySlots.StartTime.Minute, AvailabilitySlots.StartTime.Second);


                            slotModel.EndTime = slotModel.StartTime.Add(slotSpan);

                            // slotModel.EndTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, AvailabilitySlots.EndTime.Hour, AvailabilitySlots.EndTime.Minute, AvailabilitySlots.EndTime.Second);
                            slotModel.StatusTypeLKPId = 1;
                            slotModel.SlotTypeLKPId = 1;

                            List<DateTime> startTime = uow.RepositoryAsync<NAT_PLS_Slot>().GetAllStartTime(servicemodel.PlannerId);
                            List<DateTime> endTime = uow.RepositoryAsync<NAT_PLS_Slot>().GetAllEndTime(servicemodel.PlannerId);
                            bool overlapFlag = false;
                            for (int itr = 0; itr < startTime.Count; itr++)
                            {
                                if ((slotModel.StartTime >= startTime[itr] && slotModel.StartTime <= endTime[itr]) || (slotModel.EndTime >= startTime[itr] && slotModel.EndTime <= endTime[itr]))
                                {
                                    overlapFlag = true;
                                }
                            }
                            if (!overlapFlag)
                                uow.RepositoryAsync<NAT_PLS_Slot>().Insert(slotModel.ToDataModel(slotModel));
                        }
                        dayOfWeek = (dayOfWeek + 1) % 7;
                        dateTime = dateTime.AddDays(1);
                        count--;
                    }
                }
            }
        }


        /// <summary>
        /// Update: Method for adding one more day to the slot table after 30-1 days left
        /// </summary>
        /// <param name="servicemodel">Service AvailabilityModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task AddAvailabilityDay()
        {
            try
            {
                IEnumerable<AvailabilityModel> serviceModelList =  GetAllAvailability();



                foreach (var servicemodel in serviceModelList)
                {
                    
                    if (servicemodel.NatPlsAvailabilitySlot != null)
                    {
                        //Creating New Updated Availability Slots
                        foreach (AvailabilitySlotModel AvailabilitySlots in servicemodel.NatPlsAvailabilitySlot)
                        {
                            int dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
                            DateTime dateTime = DateTime.UtcNow;
                            TimeSpan slotSpan = AvailabilitySlots.EndTime.Subtract(AvailabilitySlots.StartTime);

                            dateTime = dateTime.AddDays(Constants.NO_DAYS_FOR_SLOTS);
                            dayOfWeek = (dayOfWeek + Constants.NO_DAYS_FOR_SLOTS) % 7;
                            
                             if (dayOfWeek == servicemodel.DayOfWeekLKPId)
                             {
                                SlotModel slotModel = new SlotModel
                                {
                                    PlannerId = servicemodel.PlannerId,
                                    StartTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, AvailabilitySlots.StartTime.Hour, AvailabilitySlots.StartTime.Minute, AvailabilitySlots.StartTime.Second)
                                };

                                slotModel.EndTime = slotModel.StartTime.Add(slotSpan);


                                // slotModel.EndTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, AvailabilitySlots.EndTime.Hour, AvailabilitySlots.EndTime.Minute, AvailabilitySlots.EndTime.Second);

                                slotModel.StatusTypeLKPId = 1;
                                slotModel.SlotTypeLKPId = 1;

                                List<DateTime> startTime = uow.RepositoryAsync<NAT_PLS_Slot>().GetAllStartTime(servicemodel.PlannerId);
                                List<DateTime> endTime = uow.RepositoryAsync<NAT_PLS_Slot>().GetAllEndTime(servicemodel.PlannerId);
                                bool overlapFlag = false;
                                for (int itr = 0; itr < startTime.Count; itr++)
                                {
                                    if ((slotModel.StartTime >= startTime[itr] && slotModel.StartTime <= endTime[itr]) || (slotModel.EndTime >= startTime[itr] && slotModel.EndTime <= endTime[itr]))
                                    {
                                        overlapFlag = true;
                                    }
                                }
                                if (!overlapFlag)
                                {
                                    uow.RepositoryAsync<NAT_PLS_Slot>().Insert(slotModel.ToDataModel(slotModel));
                                }
                             }
                        }
                    }
                }
                await uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        public DataSourceResult GetAllSlotList(DataSourceRequest request)
        {
            try
            {
                return uow.RepositoryAsync<NAT_PLS_Slot>().Queryable().ToDataSourceResult<NAT_PLS_Slot, SlotModel>(request);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}
