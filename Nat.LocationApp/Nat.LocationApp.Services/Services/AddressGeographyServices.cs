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

using Nat.LocationApp.Services.ServiceModels;
using Nat.LocationApp.Models.EFModel;
using Nat.LocationApp.Models.Repositories;
using Nat.Core.Caching;

namespace Nat.LocationApp.Services
{
    public class AddressGeographyService : BaseService<AddressGeographyModel, NAT_LS_Address_Geography>
    {
        private static AddressGeographyService _service;
        public static AddressGeographyService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            //{
            //    _service = new AddressGeographyService();
            //}
            _service = new AddressGeographyService();
            _service.SetLogger(logger);
            return _service;
        }

        private AddressGeographyService() : base()
        {

        }

        /// <summary>
        /// This method return list of all addressgeography
        /// </summary>
        /// <returns>Collection of address geography service model<returns>
        public async Task<IEnumerable<AddressGeographyModel>> GetAllAddressGeography()
        {
            using (logger.BeginServiceScope("Get All AddressGeography"))
            {
                try
                {
                    IEnumerable<AddressGeographyModel> data = null;
                    logger.LogInformation("Fetch all addressgeography from repo");
                    IEnumerable<NAT_LS_Address_Geography> addressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetAllAddressGeography();
                    if (addressGeographyModel != null)
                    {
                        data = new AddressGeographyModel().FromDataModelList(addressGeographyModel);
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
        /// Create: Method for creation of address geography
        /// </summary>
        /// <param name="servicemodel">Service AddressGeography Model</param>
        /// <returns>AddressGeography ID generated for addressgeography</returns>
        public async Task<string> CreateAddressGeographyAsync(AddressGeographyModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    await uow.SaveChangesAsync();
                    return Convert.ToString(Get().AddressGeographyId);
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
        /// This method returns AddressGeography with a given Id
        /// </summary>
        /// <param name="Id">Id of AddressGeography</param>
        /// <returns>AddressGeography service model</returns>
        public async Task<AddressGeographyModel> GetAddressGeographyByIDAsync(int Id)
        {
            try
            {
                AddressGeographyModel data = null;
                NAT_LS_Address_Geography AddressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetAddressGeographyByIdAsync(Id);
                if (AddressGeographyModel != null)
                {
                    data = new AddressGeographyModel().FromDataModel(AddressGeographyModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Update: Method for Updation of AddressGeography record
        /// </summary>
        /// <param name="servicemodel">Service AddressGeography Model Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateAddressGeographyAsync(AddressGeographyModel servicemodel)
        {
            try
            {
                if (servicemodel.AddressGeographyId != 0 || servicemodel.AddressGeographyId > 0)
                {
                    // NAT_LS_Address_Geography: stripe id  (validations code level)
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
        /// This method returns parent address geography with a given child Id
        /// </summary>
        /// <param name="Id">Child Id of address geography</param>
        /// <returns>Address Geography service model</returns>
        public async Task<AddressGeographyModel> GetParentAddressGeographyAsync(int Id)
        {
            try
            {
                AddressGeographyModel data = null;
                NAT_LS_Address_Geography AddressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetParentAddressGeographyByChildIdAsync(Id);
                if (AddressGeographyModel != null)
                {
                    data = new AddressGeographyModel().FromDataModel(AddressGeographyModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns parent address geography with a given child code and type
        /// </summary>
        /// <param name="code">Child Code of address geography</param>
        /// <param name="type">Type of address geography</param>
        /// <returns>Address Geography service model</returns>
        public async Task<AddressGeographyModel> GetParentAddressGeographyByTypeFunction(string code, string type)
        {
            try
            {
                AddressGeographyModel data = null;
                NAT_LS_Address_Geography AddressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>()
                    .GetParentAddressGeographyByTypeFunction(code, type);
                if (AddressGeographyModel != null)
                {
                    data = new AddressGeographyModel().FromDataModel(AddressGeographyModel);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns list of all immediate active children for a given Id
        /// </summary>
        /// <returns>Collection of address geography service model<returns>
        public async Task<IEnumerable<AddressGeographyModel>> GetImmediateActiveChildrenAddressGeography(string code)
        {
            using (logger.BeginServiceScope("Get All Immediate Active Children Address Geography for a Given ID"))
            {
                try
                {
                    IEnumerable<AddressGeographyModel> data = null;
                    IEnumerable<NAT_LS_Address_Geography> addressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetImmediateActiveChildrenAddressGeography(code);
                    if (addressGeographyModel != null)
                    {
                        data = new AddressGeographyModel().FromDataModelList(addressGeographyModel);
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
        /// This method returns list of all active children for a given Id
        /// </summary>
        /// <returns>Collection of address geography service model<returns>
        public async Task<IEnumerable<AddressGeographyModel>> GetAllActiveChildrenAddressGeography(string code)
        {
            using (logger.BeginServiceScope("Get All Active Children Address Geography for a Given ID"))
            {
                try
                {
                    IEnumerable<AddressGeographyModel> data = null;
                    IEnumerable<NAT_LS_Address_Geography> addressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetAllActiveChildrenAddressGeography(code);
                    if (addressGeographyModel != null)
                    {
                        data = new AddressGeographyModel().FromDataModelList(addressGeographyModel);
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
        /// This method returns list of all address geography for a given Type
        /// </summary>
        /// <returns>Collection of address geography service model<returns>
        public async Task<IEnumerable<AddressGeographyModel>> GetAddressGeographyByType(string type)
        {
            using (logger.BeginServiceScope("Get All Address Geography for a Given Type"))
            {
                try
                {
                    IEnumerable<AddressGeographyModel> data = null;
                    IEnumerable<NAT_LS_Address_Geography> addressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetAddressGeographyByType(type);
                    if (addressGeographyModel != null)
                    {
                        data = new AddressGeographyModel().FromDataModelList(addressGeographyModel);
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
        /// This method returns list of all address geography for a given Type
        /// </summary>
        /// <returns>Collection of address geography service model<returns>
        public async Task<IEnumerable<AddressGeographyModel>> GetAddressGeographyChildrenByType(string code, string type)
        {
            using (logger.BeginServiceScope("Get All Address Geography Children by Given Type"))
            {
                try
                {
                    async Task<IEnumerable<AddressGeographyModel>> GetAddressGeographyChildrenByTypeFromDB()
                    {
                        IEnumerable<AddressGeographyModel> data = null;
                        IEnumerable<NAT_LS_Address_Geography> addressGeographyModel = await uow.RepositoryAsync<NAT_LS_Address_Geography>().GetAllActiveChildrenAddressGeography(code);
                        var childrenByType = addressGeographyModel.Where(x => x.Geography_Type_LKP == type).ToList();
                        if (addressGeographyModel != null)
                        {
                            data = new AddressGeographyModel().FromDataModelList(childrenByType);
                            return data;
                        }
                        throw new ApplicationException("asdd");
                    }
                    return await Caching.GetObjectFromCache("address_geography_" + code + "_" + type, 300, GetAddressGeographyChildrenByTypeFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task CreateHierarchy(AddressGeographyModel serviceModel)
        {
            try
            {
                string parentCode = null;
                AddressGeographyModel modelToBeCreated = serviceModel;

                while (modelToBeCreated != null)
                {
                    var efModel = new AddressGeographyModel().ToDataModel(modelToBeCreated);
                    efModel.Parent_Geography_Code = parentCode;
                    efModel.Active_Flag = true;
                    var exist = this.uow.RepositoryAsync<NAT_LS_Address_Geography>().Queryable().Where(x => x.Geography_Short_Code == efModel.Geography_Short_Code).FirstOrDefault();
                    if (exist == null && efModel.Geography_Short_Code != null && efModel.Geography_Short_Code != "")
                    {
                        this.uow.RepositoryAsync<NAT_LS_Address_Geography>().Insert(efModel);
                        await this.uow.SaveChangesAsync();
                    }
                    parentCode = modelToBeCreated.GeographyShortCode;
                    modelToBeCreated = modelToBeCreated.Child;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

    }
}

        




//Check: Insert data only once in person table if a person wants to sign up for multiple roles with same email id. (for Create)
//bool found = uow.RepositoryAsync<NAT_AS_Artist>().GetPersonByEmail(servicemodel.NatAsPerson.PersonEmail);
//if (!found)
//{
//    servicemodel.NatAsPerson.ObjectState = ObjectState.Added;
//}