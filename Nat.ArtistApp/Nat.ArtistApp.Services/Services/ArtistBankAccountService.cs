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


using Nat.ArtistApp.Services.ServiceModels;
using Nat.ArtistApp.Models.EFModel;


using Nat.ArtistApp.Models.Repositories;


namespace Nat.ArtistApp.Services
{
    public class ArtistBankAccountService : BaseService<ArtistBankAccountModel, NAT_AS_Artist_Bank_Account>
    {
        private static ArtistBankAccountService _service;
        public static ArtistBankAccountService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new ArtistBankAccountService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private ArtistBankAccountService() : base()
        {

        }

        /// <summary>
        /// This method returns Bank Accounts list of artist with a given artist Id
        /// </summary>
        /// <param name="Id">Id of artist</param>
        /// <returns>Collection of Artist Bank Account service model</returns>
        public async Task<IEnumerable<ArtistBankAccountModel>> GetByIdArtistBankAccountsAsync(int ArtistId)
        {
            try
            {
                IEnumerable <ArtistBankAccountModel> data = null;
                IEnumerable <NAT_AS_Artist_Bank_Account> artistbankaccountModel = await uow.RepositoryAsync<NAT_AS_Artist_Bank_Account>().GetArtistBankAccountsByIdAsync(ArtistId);
                if (artistbankaccountModel != null)
                {
                    data = new ArtistBankAccountModel().FromDataModelList(artistbankaccountModel);
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
        /// This Method creates artist bank account
        /// </summary>
        /// <param name="servicemodel">Service Artist Bank Account Model</param>
        /// <returns>Artist Bank Account ID generated for artist</returns>
        public async Task<string> CreateArtistBankAccountsAsync(ArtistBankAccountModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    // NAT_AS_Artist_Bank_Account: place checks for tenant id,bank id (database level), Bank Account Number,Bank Routing Number (validations code level)
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;
                    Insert(servicemodel);
                    await uow.SaveChangesAsync();
                    return Convert.ToString(Get().ArtistBankAccountId);
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
        /// This Method updates artist bank account details
        /// </summary>
        /// <param name="servicemodel">Service Artist Bank Account Model</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<bool> UpdateArtistBankAccountsAsync(ArtistBankAccountModel servicemodel)
        {
            try
            {
                if (servicemodel.ArtistBankAccountId != 0 || servicemodel.ArtistBankAccountId > 0)
                {
                     // NAT_AS_Artist_Bank_Account: Bank Account Number,Bank Routing Number(validations code level)
                     if (servicemodel.ArtistBankAccountId > 0)
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
        /// This method activate artist bank account
        /// </summary>
        /// <param name="Id">Id of artist bank account</param>
        public async Task ActivateArtistBankAccountAsync(string ArtistBankAccountId)
        {
            try
            {
                NAT_AS_Artist_Bank_Account ArtistBankAccountEf = await uow.RepositoryAsync<NAT_AS_Artist_Bank_Account>().GetArtistBankAccountBybankaccountIdAsync(Convert.ToInt32(ArtistBankAccountId));
                if (ArtistBankAccountEf != null)
                {
                    uow.RepositoryAsync<NAT_AS_Artist_Bank_Account>().SetActiveFlag(true, ArtistBankAccountEf);
                    await uow.SaveChangesAsync();
                }
                else
                  throw new ApplicationException("Artist bank account doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivate artist bank account
        /// </summary>
        /// <param name="Id">Id of artist bank account</param>
        public async Task DeactivateArtistBankAccountAsync(string ArtistBankAccountId)
        {
            try
            {
                NAT_AS_Artist_Bank_Account ArtistBankAccountEf = await uow.RepositoryAsync<NAT_AS_Artist_Bank_Account>().GetArtistBankAccountBybankaccountIdAsync(Convert.ToInt32(ArtistBankAccountId));
                if (ArtistBankAccountEf != null)
                {
                    uow.RepositoryAsync<NAT_AS_Artist_Bank_Account>().SetActiveFlag(false, ArtistBankAccountEf);
                    await uow.SaveChangesAsync();
                }
                else
                    throw new ApplicationException("Artist bank account doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

}
}

       