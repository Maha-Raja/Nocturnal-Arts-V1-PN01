using Nat.Core.Logger;
using Nat.Core.BaseModelClass;
using System.Net.Http;
using Nat.Core.Logger.Extension;

namespace Nat.Core.Exception
{
    public class ServiceLayerExceptionHandler
    {
        public static ServiceLayerException Handle(System.Exception ex, NatLogger logger)
        {
            ServiceLayerException serviceLayerException = new ServiceLayerException(ex.Message, ex);
            logger.LogError(serviceLayerException);
            return serviceLayerException;
        }

        public static HttpResponseMessage CreateResponse(System.Exception ex)
        {
            return ResponseViewModel.CreateErrorResponse(ex);
        }
    }
}
