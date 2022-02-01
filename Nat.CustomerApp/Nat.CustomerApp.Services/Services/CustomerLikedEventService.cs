using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.Exception;
using Nat.CustomerApp.Models.Repositories;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;
using Nat.CustomerApp.Services;
using Nat.Core.Caching;
using Nat.Core.ServiceClient;
using Nat.Core.Lookup;
using Nat.Core.Storage;

namespace Nat.CustomerApp.Services.Services
{
    public class CustomerLikedEventService : BaseService<CustomerLikedEventsModel, NAT_CS_Customer_Liked_Events>
    {
        private static CustomerLikedEventService _service;
        public static CustomerLikedEventService GetInstance(NatLogger logger)
        {
            _service = new CustomerLikedEventService();
            _service.SetLogger(logger);
            return _service;
        }

        private CustomerLikedEventService() : base()
        {

        }

        public async Task<bool> LikeEventasync(CustomerLikedEventsModel obj)
        {
            try
            {
                //check if event is already liked so return false
                NAT_CS_Customer_Liked_Events likedEvent = await uow.RepositoryAsync<NAT_CS_Customer_Liked_Events>().GetLikedEventEntryAsync(obj.CustomerId, obj.EventCode);
                if (likedEvent != null) { return false; }

                //else add liked events in DB and return true
                obj.ActiveFlag = true;
                obj.ObjectState = ObjectState.Added;
                obj.CreatedBy = "admin@paintception.com";
                Insert(obj);
                await uow.SaveChangesAsync();
                Caching.ClearCache("CustomersLikedEventslov/" + obj.CustomerId);
                return true;
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<bool> UnlikeEventasync(CustomerLikedEventsModel obj)
        {
            try
            {
                NAT_CS_Customer_Liked_Events likedEvent = await uow.RepositoryAsync<NAT_CS_Customer_Liked_Events>().GetLikedEventEntryAsync(obj.CustomerId, obj.EventCode);
                if (likedEvent != null)
                {
                    //Delete(likedEvent);
                    likedEvent.ObjectState = ObjectState.Deleted;
                    await uow.SaveChangesAsync();
                    Caching.ClearCache("CustomersLikedEventslov/" + obj.CustomerId);
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
