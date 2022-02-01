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
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;

namespace Nat.VenueApp.Services
{
    public class VenueFacilityService : BaseService<VenueFacilityModel, NAT_VS_Venue_Facility>
    {
        private static VenueFacilityService _service;
        public static VenueFacilityService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new VenueFacilityService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private VenueFacilityService() : base()
        {

        }

        /// <summary>
        /// This method returns Venue Facility list with a given Venue Id
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        /// <returns>Collection of Venue Facility  service model</returns>
        public IEnumerable<VenueFacilityModel> GetByIdVenueFacility(int VenueId)
        {
            try
            {
                IEnumerable <VenueFacilityModel> data = null;
                IEnumerable <NAT_VS_Venue_Facility> venuefacilityModel = uow.RepositoryAsync<NAT_VS_Venue_Facility>().GetVenueFacilityById(VenueId);
                if (venuefacilityModel != null)
                {
                    data = new VenueFacilityModel().FromDataModelList(venuefacilityModel);
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
        /// This Method creates Venue Facility
        /// </summary>
        /// <param name="servicemodel">Service Venue Facility Model</param>
        /// <returns>Venue Facility ID generated for Venue Facility</returns>
        public string CreateVenueFacility(VenueFacilityModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ObjectState = ObjectState.Added;
                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().VenueFacilityId);
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
        /// This Method updates Venue Facility details
        /// </summary>
        /// <param name="servicemodel">Service Venue Facility Model</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public bool UpdateVenueFacility(VenueFacilityModel servicemodel)
        {
            try
            {
                if (servicemodel.VenueFacilityId != 0 || servicemodel.VenueFacilityId > 0)
                {
                     if (servicemodel.VenueFacilityId > 0)
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
        /// This Method delete Venue Facility
        /// </summary>
        /// <param name="servicemodel">Service Venue Facility Model</param>
        /// <returns>Venue Facility ID generated for Venue Facility</returns>
        public bool DeleteVenueFacility(VenueFacilityModel servicemodel)
        {
            try
            {

                VenueFacilityModel data = null;
                NAT_VS_Venue_Facility venuefacilityModel = uow.RepositoryAsync<NAT_VS_Venue_Facility>().GetVenueFacilityByVenueFacilityId(servicemodel.VenueFacilityId);
                if (venuefacilityModel != null)
                {
                    this.Delete(servicemodel);
                    uow.SaveChanges();
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

    }
}

       