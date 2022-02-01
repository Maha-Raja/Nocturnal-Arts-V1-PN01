using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.BaseModelClass;
using System.Net.Http;

namespace Nat.Core.Exception
{
    public class FunctionLayerExceptionHandler
    {
        /// <summary>
        /// Handles exception occuring at Azure Function layer
        /// </summary>
        /// <param name="ex">Exception to be handled</param>
        /// <param name="logger">Instance of logger</param>
        /// <returns>Formatted response message</returns>
        public static HttpResponseMessage Handle(System.Exception ex, NatLogger logger)
        {
            if (ex.GetType().Name == "AuthenticationException")
            {
                FunctionLayerException functionLayerException = new FunctionLayerException(ex.Message, ex);
                logger.LogError(functionLayerException);
                return ResponseViewModel.CreateUnauthorizedErrorResponse(functionLayerException);
            }
            else
            {
                FunctionLayerException functionLayerException = new FunctionLayerException(ex.Message, ex);
                logger.LogError(functionLayerException);
                return ResponseViewModel.CreateErrorResponse(functionLayerException);
            }
        }
    }
}
