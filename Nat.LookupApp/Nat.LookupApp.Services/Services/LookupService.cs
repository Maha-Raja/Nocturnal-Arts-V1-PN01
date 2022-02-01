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

using Nat.LookupApp.Services.ServiceModels;
using Nat.LookupApp.Models.EFModel;
using Nat.LookupApp.Models.Repositories;
using Nat.Core.Caching;


namespace Nat.LookupApp.Services
{
    public class LookupService : BaseService<LookupModel, NAT_LUS_Lookup>
    {
        private static LookupService _service;
        public static LookupService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new LookupService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private LookupService() : base()
        {

        }

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <returns>Collection of artist service model<returns>
        public async Task<IEnumerable<LookupModel>> GetLookupByLookupTypeAsync(string type)
        {
            using (logger.BeginServiceScope("Get Lookup by LookupType"))
            {
                try
                {
                    async Task<IEnumerable<LookupModel>> GetLookupFromDB()
                    {
                        logger.LogInformation("Fetch all active Lookup by Lookup Type: " + type);
                        var lookupModel = await uow.RepositoryAsync<NAT_LUS_Lookup>().GetLookupByLookupTypeAsync(type);
                        if (lookupModel != null)
                        {
                            return new LookupModel().FromDataModelList(lookupModel);
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                    }
                    return await Caching.GetObjectFromCacheAsync("lookups/" + type, 300, GetLookupFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <returns>Collection of artist service model<returns>
        public async Task<int> GetLookCountbyLookupTypeAsync(string type)
        {
            using (logger.BeginServiceScope("Get Lookup count by LookupType"))
            {
                try
                {                  
                        logger.LogInformation("Fetch lookup count by Lookup Type: " + type);
                        var lookupModel = await uow.RepositoryAsync<NAT_LUS_Lookup>().GetLookupTypeCountAsync(type);
                        if (lookupModel != null)
                        {
                            return lookupModel.Count();
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <returns>Collection of artist service model<returns>
        public async Task<IEnumerable<LookupModel>> GetAllLookupAsync()
        {
            using (logger.BeginServiceScope("Get All Lookup"))
            {
                try
                {
                    async Task<IEnumerable<LookupModel>> GetLookupFromDB()
                    {
                        logger.LogInformation("Fetch all active Lookup");
                        var lookupModel = await uow.RepositoryAsync<NAT_LUS_Lookup>().GetAllLookupAsync();
                        if (lookupModel != null)
                        {
                            return new LookupModel().FromDataModelList(lookupModel);
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                    }
                    var lookups = await Caching.GetObjectFromCacheAsync("lookups", 300, GetLookupFromDB);
                    lookups = lookups.OrderBy(x => x.VisibleValue);
                    return lookups;
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method return all Lookup Types
        /// </summary>
        /// <returns>List of  Lookup Types<returns>
        public async Task<DataSourceResult> GetAllLookUpTypes(DataSourceRequest request)
        {
            try
            {
                var lookupModel = uow.RepositoryAsync<NAT_LUS_Lookup>().Queryable().GroupBy(x => x.Lookup_Type)
                    .Select(y => y.FirstOrDefault())
                    .OrderBy(x => x.Lookup_Type)
                    .ThenBy(x => x.Visible_Value)
                    .ToDataSourceResult<NAT_LUS_Lookup, LookupModel>(request);
                if (!lookupModel.Total.Equals(0))
                {
                    var list = ((IEnumerable<LookupModel>)lookupModel.Data).ToList();
                }
                return lookupModel;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Update: Method for Updation of  values against lookup value type
        /// </summary>
        /// <param name="servicemodel">Service ArtistModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateLookupValuesByLookupTypeAsync(IEnumerable<LookupModel> servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    NAT_LUS_Lookup lookup;
                    foreach (LookupModel lookuptype in servicemodel)
                    {
                        if (lookuptype.Lookupid > 0)
                        {
                            lookuptype.ObjectState = ObjectState.Modified;
                            lookup = new NAT_LUS_Lookup();
                            lookup = lookuptype.ToDataModel(lookuptype);
                            uow.Repository<NAT_LUS_Lookup>().Update(lookup);
                        }
                        else if (lookuptype.Lookupid < 0)
                        {
                            lookuptype.Lookupid *= -1;
                            lookuptype.ActiveFlag = false;
                            lookuptype.ObjectState = ObjectState.Modified;
                            lookup = new NAT_LUS_Lookup();
                            lookup = lookuptype.ToDataModel(lookuptype);
                            uow.Repository<NAT_LUS_Lookup>().Update(lookup);
                        }
                        else if (lookuptype.Lookupid == 0)
                        {
                            lookuptype.ActiveFlag = true;
                            lookuptype.ObjectState = ObjectState.Added;
                            lookup = new NAT_LUS_Lookup();
                            lookup = lookuptype.ToDataModel(lookuptype);
                            uow.Repository<NAT_LUS_Lookup>().Insert(lookup);
                        }
                    }

                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        return false;
                    }
                    else
                    {
                        var data = servicemodel.FirstOrDefault();
                        Caching.ClearCache("lookups/" + data.LookupType);
                        Caching.ClearCache("lookups");
                        return true;
                    }
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
        /// This method return configuration object by key
        /// </summary>
        /// <returns>Configuration Service Model<returns>
        public async Task<ConfigurationModel> GetConfigurationByKeyAsync(string key)
        {
            using (logger.BeginServiceScope("Get Configuration by Key"))
            {
                try
                {
                    async Task<ConfigurationModel> GetConfigurationFromDB()
                    {
                        logger.LogInformation("Fetch Configuration by Key: " + key);
                        var configurationModel = await uow.RepositoryAsync<NAT_LUS_Configuration>().GetConfigurationByKeyAsync(key);
                        if (configurationModel != null)
                        {
                            return new ConfigurationModel().FromDataModel(configurationModel);
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                    }
                    return await Caching.GetObjectFromCacheAsync("configuration/" + key, 300, GetConfigurationFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        /// <summary>
        /// This method return all configuration object
        /// </summary>
        /// <returns>List of Configuration Service Model<returns>
        public async Task<IEnumerable<ConfigurationModel>> GetAllConfigurationAsync()
        {
            using (logger.BeginServiceScope("Get All Configuration"))
            {
                try
                {
                    async Task<IEnumerable<ConfigurationModel>> GetAllConfigurationFromDB()
                    {
                        logger.LogInformation("Fetch All Configuration");
                        var configurationModels = await uow.RepositoryAsync<NAT_LUS_Configuration>().GetAllConfiguration();
                        if (configurationModels != null)
                        {
                            return new ConfigurationModel().FromDataModelList(configurationModels);
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                    }
                    return await Caching.GetObjectFromCacheAsync("configuration", 300, GetAllConfigurationFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }



        /// <summary>
        /// This method return all configuration object which are user editable
        /// </summary>
        /// <returns>List of User Editable Configuration Service Model<returns>
        public async Task<DataSourceResult> GetAllConfigurationByUserEditableAsync(DataSourceRequest request)
        {
            try
            {
                var configurationModels = uow.RepositoryAsync<NAT_LUS_Configuration>().Queryable().Where(x => x.User_Editable == true && x.Active_Flag == true).ToDataSourceResult<NAT_LUS_Configuration, ConfigurationModel>(request);
                if (!configurationModels.Total.Equals(0))
                {
                    var list = ((IEnumerable<ConfigurationModel>)configurationModels.Data).ToList();
                }
                return configurationModels;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Update: Method for Updation of configuration record
        /// </summary>
        /// <param name="servicemodel">Service ConfigurationModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateConfigurationAsync(ConfigurationModel servicemodel)
        {
            try
            {
                if (servicemodel.ConfigurationId != 0 || servicemodel.ConfigurationId > 0)
                {
                    NAT_LUS_Configuration configuration = await uow.RepositoryAsync<NAT_LUS_Configuration>().GetConfigurationByIdAsync(servicemodel.ConfigurationId);
                    if (configuration != null) {
                        configuration.Value = servicemodel.Value;
                        configuration.Description = servicemodel.Description;
                        configuration.ObjectState = ObjectState.Modified;
                        uow.Repository<NAT_LUS_Configuration>().Update(configuration);
                        int updatedRows = await uow.SaveChangesAsync();
                        if (updatedRows == 0)
                        {
                            return false;
                        }
                        Caching.ClearCache("configuration/" + configuration.Key);
                        Caching.ClearCache("configuration");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
