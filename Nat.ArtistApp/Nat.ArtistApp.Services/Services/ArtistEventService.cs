using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nat.ArtistApp.Services.ServiceModels;
using Nat.ArtistApp.Models.EFModel;
using Nat.Core.Logger;
using Nat.Core.Exception;
using Nat.ArtistApp.Models.Repositories;

namespace Nat.ArtistApp.Services.Services
{
    public class ArtistEventService : BaseService<ArtistEventModel, NAT_AS_Artist_Event>
    {
        private static ArtistEventService _service;
        public static ArtistEventService GetInstance(NatLogger logger)
        {
            {
                _service = new ArtistEventService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private ArtistEventService() : base()
        {

        }

        /// <summary>
        /// This method returns Upcoming events of a given artist id
        /// </summary>
        /// <param name="Id">Id of artist</param>
        /// <returns>Collection of Artist Event service model</returns>
        public async Task<IEnumerable<ArtistEventModel>> GetAllArtistPendingEventsAsync(int ArtistId)
        {
            try
            {
                IEnumerable<ArtistEventModel> data = null;
                IEnumerable<NAT_AS_Artist_Event> artisteventModel = await uow.RepositoryAsync<NAT_AS_Artist_Event>().GetAllArtistPendingEventsAsync(ArtistId);
                if (artisteventModel != null)
                {
                    data = new ArtistEventModel().FromDataModelList(artisteventModel);
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
    }
}
