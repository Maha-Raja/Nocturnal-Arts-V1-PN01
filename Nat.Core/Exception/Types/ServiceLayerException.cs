using System.Runtime.Serialization;

namespace Nat.Core.Exception
{
    public class ServiceLayerException : System.Exception, ISerializable
    {
        public ServiceLayerException()
            : base()
        {
            // Add implementation (if required)
        }

        public ServiceLayerException(string message)
            : base(message)
        {
            // Add Implementation (if required)
        }

        public ServiceLayerException(string message, System.Exception inner)
            : base(message, inner)
        {
            // Add implementation (if required)
        }

        protected ServiceLayerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation (if required)
        }
    }
}
