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
    public class ArtistService : BaseService<ArtistModel, NAT_AS_Artist>
    {
        private static ArtistService _service;
        public static ArtistService GetInstance(NatLogger logger)
        {
            if (_service == null)
            {
                _service = new ArtistService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private ArtistService() : base()
        {

        }

        /// <summary>
        /// This method return list of all artists
        /// </summary>
        /// <returns>Collection of artist service model<returns>
        public IEnumerable<ArtistModel> GetAllArtist()
        {
            using (logger.BeginServiceScope("Get All Artist"))
            {
                try
                {
                    IEnumerable<ArtistModel> data = null;
                    logger.LogInformation("Fetch all artist from repo");
                    IEnumerable<NAT_AS_Artist> artistModel = uow.RepositoryAsync<NAT_AS_Artist>().GetAllArtist();
                    if (artistModel != null)
                    {
                        data = new ArtistModel().FromDataModelList(artistModel);
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

        public DataSourceResult GetAllArtistList(DataSourceRequest request)
        {
            try
            {
                return uow.RepositoryAsync<NAT_AS_Artist_VW>().Queryable().ToDataSourceResult<NAT_AS_Artist_VW, ArtistVWModel>(request);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns artist with a given Id
        /// </summary>
        /// <param name="Id">Id of artist</param>
        /// <returns>Artist service model</returns>
        public ArtistModel GetByIdArtist(int Id)

        {
            try
            {
                ArtistModel data = null;
                NAT_AS_Artist artistModel = uow.RepositoryAsync<NAT_AS_Artist>().GetArtistById(Id);
                if (artistModel != null)
                {
                    data = new ArtistModel().FromDataModel(artistModel);
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
        /// Create: Method for creation of artist
        /// </summary>
        /// <param name="servicemodel">Service Artist Model</param>
        /// <returns>Artist ID generated for artist</returns>
        public string CreateArtist(ArtistModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    // NAT_AS_Artist: Place checks for tenant id, Artist_Status_LKP_ID (database level), stripe id  (validations code level) 
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    // NAT_AS_Artist_Bank_Account: place checks for tenant id,bank id (database level), Bank Account Number,Bank Routing Number (validations code level)
                    if (servicemodel.NatAsArtistBankAccount != null)
                    {
                        foreach (ArtistBankAccountModel ArtistBankAccount in servicemodel.NatAsArtistBankAccount)
                        {
                            ArtistBankAccount.ActiveFlag = true;
                            ArtistBankAccount.ObjectState = ObjectState.Added;
                        }
                    }
                    // NAT_AS_Artist_Rating: place checks for tenant id (database level)
                    if (servicemodel.NatAsArtistRating != null)
                    {
                        foreach (ArtistRatingModel ArtistRating in servicemodel.NatAsArtistRating)
                        {
                            ArtistRating.AverageRatingValue = 0;
                            ArtistRating.NumberOfRatings = 0;
                            ArtistRating.ActiveFlag = true;
                            ArtistRating.ObjectState = ObjectState.Added;
                        }
                    }
                    // NAT_AS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                    // Person Email, Date_Of_Birth, Person_Extension (validations code level)
                    if (servicemodel.NatAsPerson != null)
                    {
                        servicemodel.NatAsPerson.ActiveFlag = true;
                        servicemodel.NatAsPerson.ObjectState = ObjectState.Added;
                    }

                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().ArtistId);
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
        /// Update: Method for Updation of artist record
        /// </summary>
        /// <param name="servicemodel">Service ArtistModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public bool UpdateArtist(ArtistModel servicemodel)
        {
            try
            {
                if (servicemodel.ArtistId != 0 || servicemodel.ArtistId > 0)
                {
                    // NAT_AS_Artist: stripe id  (validations code level)
                    servicemodel.ObjectState = ObjectState.Modified;

                    // NAT_AS_Artist_Bank_Account: Bank Account Number,Bank Routing Number(validations code level)
                    if (servicemodel.NatAsArtistBankAccount != null)
                    {
                        foreach (ArtistBankAccountModel ArtistBankAccount in servicemodel.NatAsArtistBankAccount)
                        {
                            if (ArtistBankAccount.ArtistBankAccountId > 0)
                                ArtistBankAccount.ObjectState = ObjectState.Modified;

                            else if (ArtistBankAccount.ArtistBankAccountId < 0)
                            {
                                ArtistBankAccount.ArtistBankAccountId *= -1;
                                ArtistBankAccount.ObjectState = ObjectState.Deleted;
                            }
                            else if (ArtistBankAccount.ArtistBankAccountId == 0)
                            {
                                ArtistBankAccount.ActiveFlag = true;
                                ArtistBankAccount.ObjectState = ObjectState.Added;
                            }
                        }
                    }
                    // NAT_AS_Artist_Rating:
                    servicemodel.NatAsArtistRating = null;

                    // NAT_AS_Person: First_Name, Last_Name, Middle_Name, Person Email, Date_Of_Birth, Person_Extension (validations code level)
                    if (servicemodel.NatAsPerson != null)
                    {
                        if (servicemodel.NatAsPerson.PersonId > 0)
                            servicemodel.NatAsPerson.ObjectState = ObjectState.Modified;
                    }

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
        /// This method activates artist 
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public void ActivateArtist(string Id)
        {
            try
            {
                NAT_AS_Artist ArtistEf = uow.RepositoryAsync<NAT_AS_Artist>().GetArtistById(Convert.ToInt32(Id));
                if (ArtistEf != null)
                {
                    uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(true, ArtistEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Artist doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates artist 
        /// </summary>
        /// <param name="Id">Id of artist</param>
        public void DeactivateArtist(string Id)
        {
            try
            {
                NAT_AS_Artist ArtistEf = uow.RepositoryAsync<NAT_AS_Artist>().GetArtistById(Convert.ToInt32(Id));
                if (ArtistEf != null)
                {
                    uow.RepositoryAsync<NAT_AS_Artist>().SetActiveFlag(false, ArtistEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Artist doesnot exists");
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