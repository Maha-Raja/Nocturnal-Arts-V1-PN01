using System.Runtime.Serialization;

namespace Nat.Core.Exception
{
    public class FunctionLayerException : System.Exception, ISerializable
    {
        public FunctionLayerException()
            : base()
        {
            // Add implementation (if required)
        }

        public FunctionLayerException(string message)
            : base(message)
        {
            // Add Implementation (if required)
        }

        public FunctionLayerException(string message, System.Exception inner)
            : base(message, inner)
        {
            // Add implementation (if required)
        }

        protected FunctionLayerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation (if required)
        }
    }
}
