using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nat.VenueApp.Services.ServiceModels;
using Nat.VenueApp.Models.EFModel;
using Nat.VenueApp.Models.Repositories;


using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;

using TLX.CloudCore.Patterns.Repository.Infrastructure;



namespace Nat.VenueApp.Services
{
    public class VenueHallService : BaseService<VenueHallModel, NAT_VS_Venue_Hall>
    {
        private static VenueHallService _service;
        public static VenueHallService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new VenueHallService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private VenueHallService() : base()
        {

        }

        /// <summary>
        /// This method returns Venue Halls list with a given Venue Id
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        /// <returns>Collection of Venue Halls  service model</returns>
        public IEnumerable<VenueHallModel> GetByIdVenueHall(int VenueId)
        {
            try
            {
                IEnumerable <VenueHallModel> data = null;
                IEnumerable <NAT_VS_Venue_Hall> venuehallModel = uow.RepositoryAsync<NAT_VS_Venue_Hall>().GetVenueHallById(VenueId);
                if (venuehallModel != null)
                {
                    data = new VenueHallModel().FromDataModelList(venuehallModel);
                    return data;
                }
                else
                throw new ApplicationException("Returns with null object");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// This Method creates Venue Halls
        /// </summary>
        /// <param name="servicemodel">Service Venue Halls Model</param>
        /// <returns>Venue Halls ID generated for Venue Halls</returns>
        public string CreateVenueHalls(VenueHallModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().VenueHallId);
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
        /// This Method updates creates Venue Halls details
        /// </summary>
        /// <param name="servicemodel">Service creates Venue Halls Model</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public bool UpdateVenueHalls(VenueHallModel servicemodel)
        {
            try
            {
                if (servicemodel.VenueHallId != 0 || servicemodel.VenueHallId > 0)
                {
                    if (servicemodel.VenueHallId > 0)
                        servicemodel.ObjectState = ObjectState.Modified;
                    base.Update(servicemodel);
                    int updatedRows = uow.SaveChanges();

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
            /// This method activate Venue Hall
            /// </summary>
            /// <param name="Id">Id of venue hall</param>
            public void ActivateVenueHall(string VenueHallId)
        {
            try
            {
                NAT_VS_Venue_Hall VenueHallEf = uow.RepositoryAsync<NAT_VS_Venue_Hall>().GetVenueHallByVenueHallId(Convert.ToInt32(VenueHallId));
                if (VenueHallEf != null)
                {
                    uow.RepositoryAsync<NAT_VS_Venue_Hall>().SetActiveFlag(true, VenueHallEf);
                    uow.SaveChanges();
                }
                else
                  throw new ApplicationException("Venue hall doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivate Venue Hall
        /// </summary>
        /// <param name="Id">Id of  venue hall</param>
        public void DeactivateVenueHall(string VenueHallId)
        {
            try
            {
                NAT_VS_Venue_Hall VenueHallEf = uow.RepositoryAsync<NAT_VS_Venue_Hall>().GetVenueHallByVenueHallId(Convert.ToInt32(VenueHallId));
                if (VenueHallEf != null)
                {
                    uow.RepositoryAsync<NAT_VS_Venue_Hall>().SetActiveFlag(false, VenueHallEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Venue hall doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

}
}

       