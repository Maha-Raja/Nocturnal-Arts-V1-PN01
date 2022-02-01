using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.VenueApp.Services.ServiceModels;
using Nat.VenueApp.Models.EFModel;
using Nat.VenueApp.Models.Repositories;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.Logger;
using Nat.Core.Exception;
using Nat.Core.Logger.Extension;

namespace Nat.VenueApp.Services
{
    public class VenueContactPersonService : BaseService<VenueContactPersonModel, NAT_VS_Venue_Contact_Person>
    {
        private static VenueContactPersonService _service;
        public static VenueContactPersonService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new VenueContactPersonService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private VenueContactPersonService() : base()
        {

        }


        /// <summary>
        /// This method return list of all Venues Contact Person
        /// </summary>
        /// <returns>Collection of Venue Contact Person Service Model<returns>
        public IEnumerable<VenueContactPersonModel> GetAllActiveVenueContactPerson()
        {
            using (logger.BeginServiceScope("Get All Active Venue Contact Person"))
            {
                try
                {
                    IEnumerable<VenueContactPersonModel> data = null;
                    IEnumerable<NAT_VS_Venue_Contact_Person> VenueActiveContactPersaonModel = uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().GetAllActiveVenueContactPerson();
                    if (VenueActiveContactPersaonModel != null)
                    {
                        data = new VenueContactPersonModel().FromDataModelList(VenueActiveContactPersaonModel);
                        return data;
                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method returns Venue Venues Contact Person list with a given Venue Id
        /// </summary>
        /// <param name="Id">Id of Venue</param>
        /// <returns>Collection of Venue  Venues Contact Person  Service Model</returns>
        public IEnumerable<VenueContactPersonModel> GetByIdVenueContactPerson(int VenueId)
        {
            try
            {
                IEnumerable <VenueContactPersonModel> data = null;
                IEnumerable <NAT_VS_Venue_Contact_Person> VenueContactPersonModel = uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().GetVenueContactPersonById(VenueId);
                if (VenueContactPersonModel != null)
                {
                    data = new VenueContactPersonModel().FromDataModelList(VenueContactPersonModel);
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
        /// This Method creates Venue Contact Person
        /// </summary>
        /// <param name="servicemodel">Service Venue Contact Person Model</param>
        /// <returns>  Venues Contact Person ID generated for Venue Contact Person</returns>
        public string CreateVenueContactPerson(VenueContactPersonModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().VenueContactPersonId);
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
        /// This Method updates creates Venue Contact Person details
        /// </summary>
        /// <param name="servicemodel">Service creates Venue Contact Person Model</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public bool UpdateVenueContactPerson(VenueContactPersonModel servicemodel)
        {
            try
            {
                if (servicemodel.VenueContactPersonId != 0 || servicemodel.VenueContactPersonId > 0)
                {
                     if (servicemodel.VenueContactPersonId > 0)
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
        /// This method activate  Venue Contact Person
        /// </summary>
        /// <param name="Id">Id of Venue Contact Person</param>
        public void ActivateVenueContactPerson(string VenueContactPersonId)
        {
            try
            {
                NAT_VS_Venue_Contact_Person VenueContactPersonEf = uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().GetVenueContactPersonByVenueContactPersonId(Convert.ToInt32(VenueContactPersonId));
                if (VenueContactPersonEf != null)
                {
                    uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().SetActiveFlag(true, VenueContactPersonEf);
                    uow.SaveChanges();
                }
                else
                  throw new ApplicationException("Venue Contact Person doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivate Venue Contact Person
        /// </summary>
        /// <param name="Id">Id of Venue Contact Person</param>
        public void DeactivateVenueContactPerson(string VenueContactPersonId)
        {
            try
            {
                NAT_VS_Venue_Contact_Person VenueContactPersonEf = uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().GetVenueContactPersonByVenueContactPersonId(Convert.ToInt32(VenueContactPersonId));
                if (VenueContactPersonEf != null)
                {
                    uow.RepositoryAsync<NAT_VS_Venue_Contact_Person>().SetActiveFlag(false, VenueContactPersonEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Venue Contact Person doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

}
}

       