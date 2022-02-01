using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.PlannerApp.Models.EFModel;
using System.Threading.Tasks;
using System;

namespace Nat.PlannerApp.Models.Repositories
{
    public static class PlannerRepository
    {
        /// <summary>
        /// This method return list of all planners
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of planner EF model</returns>
        public static IEnumerable<NAT_PLS_Planner> GetAllPlanner(this IRepositoryAsync<NAT_PLS_Planner> repository)
        {
            return repository.Queryable().ToList();
        }


        /// <summary>
        /// This method returns Planner with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="plannerID">Id of Planner</param>
        /// <returns>Planner EF model</returns>
        public static async Task<NAT_PLS_Planner> GetPlannerByIdAsync(this IRepositoryAsync<NAT_PLS_Planner> repository, int plannerID)
        {
            return await repository.Queryable().Include(x => x.NAT_PLS_Availability.Select(y => y.NAT_PLS_Availability_Slot)).Where(x => x.Planner_ID == plannerID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// This method activate/deactivate planner against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate planner</param>
        /// <param name="plannerEf">planner EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_PLS_Planner> repository, bool flag, NAT_PLS_Planner plannerEf)
        {
            plannerEf.Active_Flag = flag;
            repository.Update(plannerEf, x => x.Active_Flag);
        }

        /// <summary>
        /// This method return list of all Availability
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Availability EF model</returns>
        public static  IEnumerable<NAT_PLS_Availability> GetAllAvailability(this IRepositoryAsync<NAT_PLS_Availability> repository)
        {
            return repository.Queryable().Include(x => x.NAT_PLS_Availability_Slot).ToList();
        }

        /// <summary>
        /// This method returns Availability with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="availabilityID">Id of Availability</param>
        /// <returns>Availability EF model</returns>
        public static async Task<NAT_PLS_Availability> GetAvailabilityByIdAsync(this IRepositoryAsync<NAT_PLS_Availability> repository, int availabilityID)
        {
            return await repository.Queryable().Include(x => x.NAT_PLS_Availability_Slot).Where(x => x.Availability_ID == availabilityID).AsNoTracking().FirstOrDefaultAsync();
        }

        public static IEnumerable<NAT_PLS_Availability> GetAvailabilityByPlannerId(this IRepositoryAsync<NAT_PLS_Availability> repository, int plannerID)
        {
           return repository.Queryable().Include(x => x.NAT_PLS_Availability_Slot).Where(x => x.Planner_ID == plannerID).AsNoTracking().ToList();
        }

        //public static IEnumerable<NAT_PLS_Availability> GetAvailabilitySlotByPlannerId(this IRepositoryAsync<NAT_PLS_Availability> repository, int plannerID)
        //{
        //    return repository.Queryable().Include(x => x.NAT_PLS_Availability_Slot.Select(y=> y.Availability_Slot_ID)).Where(x => x.Planner_ID == plannerID).AsNoTracking().ToList();
        //}

        



        //public static async Task DeleteAvailabilitySlotByPlannerIdAsync(this IRepositoryAsync<NAT_PLS_Availability_Slot> repository, int plannerId)
        //{
        //    await GetAllAvaliableSlotByPlannerId(IRepositoryAsync<NAT_PLS_Availability>, plannerId);
        //    repository.Delete(await repository.GetAvailabilitySlotByIdAsync(id));
        //}

        public static async Task DeleteAvailabilityByPlannerIdAsync(this IRepositoryAsync<NAT_PLS_Availability> repository, int plannerID)
        {
            var listOfAvailabilityId = await repository.Queryable().Where(x => x.Planner_ID == plannerID).Select(x => x.Availability_ID).ToListAsync();
            for (int i = 0; i < listOfAvailabilityId.Count; i++)
            {
                repository.Delete(listOfAvailabilityId[i]);
            }
        }
        /// <summary>
        /// This method returns Availability with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="availabilityID">Id of Availability</param>
        /// <returns>Availability EF model</returns>
        public static async Task<NAT_PLS_Availability_Slot> GetAvailabilitySlotByIdAsync(this IRepositoryAsync<NAT_PLS_Availability_Slot> repository, int availabilitySlotID)
        {
            return await repository.Queryable().Where(x => x.Availability_Slot_ID == availabilitySlotID).FirstOrDefaultAsync();
        }

        public static async Task DeleteAvailabilitySlotAsync(this IRepositoryAsync<NAT_PLS_Availability_Slot> repository, int id)
        {
            repository.Delete(await repository.GetAvailabilitySlotByIdAsync(id));
        }


        public static async Task DeleteAvailabilitySlotByPlannerIdAsync(this IRepositoryAsync<NAT_PLS_Availability_Slot> repository, int plannerID)
        {
            var listOfAvailabilitySlotId = await repository.Queryable().Include(x => x.NAT_PLS_Availability).Where(x => x.NAT_PLS_Availability.Planner_ID == plannerID).Select(x => x.Availability_Slot_ID).ToListAsync();
            for (int i = 0; i < listOfAvailabilitySlotId.Count; i++)
            {
                repository.Delete(listOfAvailabilitySlotId[i]);
            }
        }


        public static async Task<NAT_PLS_Availability> GetAvailabilityByAIdAsync(this IRepositoryAsync<NAT_PLS_Availability> repository, int availabilityID)
        {
            return await repository.Queryable().Where(x => x.Availability_ID == availabilityID).AsNoTracking().FirstOrDefaultAsync();
        }

        public static async Task DeleteAvailabilityAsync(this IRepositoryAsync<NAT_PLS_Availability> repository, int id)
        {
            repository.Delete(await repository.GetAvailabilityByAIdAsync(id));
        }


        /// <summary>
        /// This method return list of all event with planner ID
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="plannerID"></param>
        /// <returns>Collection of event EF model</returns>
        public static IEnumerable<NAT_PLS_Event> GetAllEventsByPlannerId(this IRepositoryAsync<NAT_PLS_Event> repository, int plannerID)
        {
            return repository.Queryable().Where(x => x.Planner_ID == plannerID).ToList();
        }

        /// <summary>
        /// This method return list of all event By planner ID With range
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="plannerID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>Collection of event EF model</returns>
        public static IEnumerable<NAT_PLS_Event> GetAllEventsByPlannerIdWithRange(this IRepositoryAsync<NAT_PLS_Event> repository, int plannerID, DateTime startTime, DateTime endTime)
        {
            return repository.Queryable().Where(x => (startTime < x.End_Time && x.Start_Time < endTime) &&
                                                      x.Planner_ID == plannerID).ToList();
        }

        /// <summary>
        /// This method return list of all colliding events By planner ID With timeRanges of selected events
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="plannerID"></param>
        /// <param name="eventTimeRanges"></param>
        /// <returns>Collection of event EF model</returns>
        public static IEnumerable<NAT_PLS_Event> GetAllCollidingEventsByPlanIDWithTimeRanges(this IRepositoryAsync<NAT_PLS_Event> repository, int plannerID, List<IDictionary<string,DateTime>> eventTimeRanges)
        {
            return repository.Queryable().Where(x => (eventTimeRanges.Any(y=> y["StartTime"] < x.End_Time && x.Start_Time < y["EndTime"])) &&
                                                      x.Planner_ID == plannerID).ToList();
        }

        public static IEnumerable<NAT_PLS_Event> GetAllCollidingSlotbytime(this IRepositoryAsync<NAT_PLS_Event> repository, int plannerID, System.DateTime startTime, System.DateTime endTime, string eventcode)
        {
            return repository.Queryable().Where(x => startTime < x.End_Time && x.Start_Time < endTime &&
                                                      x.Planner_ID == plannerID && x.Reference_ID != eventcode && x.Status_LKP_ID != 3).ToList();
        }


        //public static async Task<IEnumerable<NAT_PLS_Event>> GetAllCollidingSlotbytime(this IRepositoryAsync<NAT_PLS_Event> repository, int id, System.DateTime startTime, System.DateTime endTime)
        //{
        //    return repository.Queryable().Where(x => x.Planner_ID == id && (startTime < x.End_Time && x.Start_Time < endTime)).ToList();

        //}


        /// <summary>
        /// This method return list of all event
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of event EF model</returns>
        public static IEnumerable<NAT_PLS_Event> GetAllEvent(this IRepositoryAsync<NAT_PLS_Event> repository)
        {
            return repository.Queryable().ToList();
        }

        public static async Task<NAT_PLS_Event> GetEventByIdAsync(this IRepositoryAsync<NAT_PLS_Event> repository, int eventID)
        {
            return await repository.Queryable().FirstOrDefaultAsync(x => x.Event_ID == eventID);
        }

        public static async Task DeleteSlotByPlannerIdAsync(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            var listOfSlotId = await repository.Queryable().Where(x => x.Planner_ID == id && x.Status_Type_LKP_ID == 1 && x.End_Time >= System.DateTime.UtcNow).Select(x => x.Slot_ID).ToListAsync();
            for(int i = 0; i < listOfSlotId.Count; i++)
            {
                repository.Delete(listOfSlotId[i]);
            }
        }

        public static async Task DeleteSlotByslotidAsync(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            var listOfSlotId = await repository.Queryable().Where(x => x.Slot_ID == id).Select(x => x.Slot_ID).ToListAsync();
            for (int i = 0; i < listOfSlotId.Count; i++)
            {
                repository.Delete(listOfSlotId[i]);
            }
        }

        public static async Task DeleteEventByeventidAsync(this IRepositoryAsync<NAT_PLS_Event> repository, int id)
        {
            var listOfEventId = await repository.Queryable().Where(x => x.Event_ID == id).Select(x => x.Event_ID).ToListAsync();
            for (int i = 0; i < listOfEventId.Count; i++)
            {
                repository.Delete(listOfEventId[i]);
            }
        }


        public static List<System.DateTime> GetAllStartTime(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            var listOfStartTime = repository.Queryable().Where(x => x.Planner_ID == id && x.Slot_Type_LKP_ID == 1 && x.Start_Time >= System.DateTime.UtcNow).Select(x => x.Start_Time).ToList();
            return listOfStartTime;
        }

        public static List<System.DateTime> GetAllEndTime(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            var listOfEndTime = repository.Queryable().Where(x => x.Planner_ID == id && x.Slot_Type_LKP_ID == 1 && x.Start_Time >= System.DateTime.UtcNow).Select(x => x.End_Time).ToList();
            return listOfEndTime;
        }

        public static IEnumerable<NAT_PLS_Planner> GetAllRequestedPlanner(this IRepositoryAsync<NAT_PLS_Planner> repository , Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, System.Int32 referenceType)
        {
            Func<NAT_PLS_Slot, bool> slotWhereCondition = (x) =>
            {
                return (((startTime != null ? (x.Start_Time <= startTime && x.End_Time >= startTime) : true) || 
                        (endTime != null ? (x.Start_Time <= endTime && x.End_Time >= endTime) : true)) &&
                        (x.Status_Type_LKP_ID == 1));
            };

            Func<NAT_PLS_Planner, bool> whereCondition = (x) => {
                return (x.Reference_Type_LKP_ID == referenceType && x.NAT_PLS_Slot.Where(slotWhereCondition).Any());
            };

            return repository.Queryable().Include(x => x.NAT_PLS_Slot)
                        .Where(whereCondition).ToList();

        //    return repository.Queryable().Where(x => x.Reference_Type_LKP_ID == referenceType && x.NAT_PLS_Slot.Any(y => y.Start_Time <= startTime && y.End_Time >= endTime)).ToList();
            
        }

        public static async Task<IEnumerable<NAT_PLS_Slot>> GetAllSlotByStartimeAndEndtime(this IRepositoryAsync<NAT_PLS_Slot> repository, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, List<int> plannerIds = null)
        {
            if(plannerIds == null)
            {
                plannerIds = new List<int>();
            }
            return repository.Queryable().Where(
                x => x.Status_Type_LKP_ID == 1 &&
                ((x.Start_Time <= startTime && x.End_Time >= startTime) && (x.Start_Time <= endTime && x.End_Time >= endTime)) &&
                (plannerIds.Count == 0 || plannerIds.Contains(x.Planner_ID))
                ).ToList();
        }

        public static async Task<IEnumerable<NAT_PLS_Slot>> GetAllSlotByPlannerId(this IRepositoryAsync<NAT_PLS_Slot> repository, List<Int32> plannerIds, System.DateTime startTime, System.DateTime endTime)
        {
            return repository.Queryable().Where(x=> plannerIds.Contains(x.Planner_ID) && x.Start_Time >= startTime && x.End_Time <= endTime).Include(x =>x.NAT_PLS_Event).ToList();

        }

        public static async Task<NAT_PLS_Slot> GetSlotByPlannerID(this IRepositoryAsync<NAT_PLS_Slot> repository, int id, System.DateTime startTime, System.DateTime endTime)
        {
            return await repository.Queryable().Where(x => x.Planner_ID == id && x.Start_Time <= startTime && x.End_Time >= endTime).Include(x => x.NAT_PLS_Event).FirstOrDefaultAsync();
        }



        public static async Task<NAT_PLS_Slot> GetSlotBySlotID(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            return await repository.Queryable().Where(x => x.Slot_ID == id).FirstOrDefaultAsync();
        }



        public static async Task<IEnumerable<NAT_PLS_Slot>> GetAllSlotsByPlannerId(this IRepositoryAsync<NAT_PLS_Slot> repository, int id)
        {
            return repository.Queryable().Where(x => x.Planner_ID == id).Include(x => x.NAT_PLS_Event).ToList();

        }

        public static async Task<NAT_PLS_Slot> GetAllAvaliableSlotByPlannerId(this IRepositoryAsync<NAT_PLS_Slot> repository, int plannerid, System.DateTime startTime, System.DateTime endTime)
        {
            Func<NAT_PLS_Slot, bool> whereCondition = (x) => {
                return (x.Planner_ID == plannerid && x.Start_Time <= startTime && x.End_Time >= endTime && x.Status_Type_LKP_ID == 1);
            };

            return repository.Queryable()
                        .Where(whereCondition).FirstOrDefault();

            //return a;

         //   return  repository.Queryable().Where(x => x.Planner_ID == plannerid && x.Start_Time <= startTime && x.End_Time >= endTime && x.Status_Type_LKP_ID == 1).FirstOrDefault();
        }



        public static async Task<IEnumerable<NAT_PLS_Slot>> GetAllCollidingSlot(this IRepositoryAsync<NAT_PLS_Slot> repository, int id, System.DateTime startTime, System.DateTime endTime)
        {
                return repository.Queryable().Where(x => x.Planner_ID == id && ((x.Start_Time <= startTime && x.End_Time>= startTime) || (x.Start_Time <= endTime && x.End_Time>=endTime))).Include(x => x.NAT_PLS_Event).ToList();

        }

        public static async Task<IEnumerable<NAT_PLS_Slot>> GetPlannerCollidingEvent(this IRepositoryAsync<NAT_PLS_Slot> repository, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, List<int> plannerIds)
        {
            if (plannerIds == null)
            {
                plannerIds = new List<int>();
            }
            return repository.Queryable().Where(x => (plannerIds.Count == 0 || plannerIds.Contains(x.Planner_ID)) && x.Status_Type_LKP_ID == 2 && ((x.Start_Time <= startTime && x.End_Time >= startTime) || (x.Start_Time <= endTime && x.End_Time >= endTime))).Include(x => x.NAT_PLS_Event).ToList();
        }



        public static async Task<NAT_PLS_Slot> GetAvaliableSlotByStartTime(this IRepositoryAsync<NAT_PLS_Slot> repository, int plannerid, System.DateTime startTime)
        {
            Func<NAT_PLS_Slot, bool> whereCondition = (x) => {
                return (x.Planner_ID == plannerid && x.Start_Time == startTime && x.Status_Type_LKP_ID == 1);
            };
            return repository.Queryable()
                        .Where(whereCondition).FirstOrDefault();
        }
        public static async Task<NAT_PLS_Slot> GetAvaliableSlotByEndTime(this IRepositoryAsync<NAT_PLS_Slot> repository, int plannerid, System.DateTime endTime)
        {
            Func<NAT_PLS_Slot, bool> whereCondition = (x) => {
                return (x.Planner_ID == plannerid && x.End_Time == endTime && x.Status_Type_LKP_ID == 1);
            };
            return repository.Queryable()
                        .Where(whereCondition).FirstOrDefault();
        }

        public static async Task<NAT_PLS_Slot> GetAllScheduledSlotByPlannerId(this IRepositoryAsync<NAT_PLS_Slot> repository, int plannerid, System.DateTime startTime, System.DateTime endTime)
        {
            return repository.Queryable().Where(x => x.Planner_ID == plannerid && x.Start_Time <= startTime && x.End_Time >= endTime && x.Status_Type_LKP_ID == 2).FirstOrDefault();
        }

        public static async Task<NAT_PLS_Event> GetEventCancelAsync(this IRepositoryAsync<NAT_PLS_Event> repository,int plannerid, string eventcode)
        {
            return repository.Queryable().Where(x => x.Planner_ID == plannerid && x.Reference_ID == eventcode && x.Status_LKP_ID != 3).FirstOrDefault();
        }
    }
}
