using System.Runtime.Serialization;

namespace Nat.Core.Exception
{
    public class AuthenticationException : System.Exception, ISerializable
    {
        public AuthenticationException()
            : base()
        {
            // Add implementation (if required)
        }

        public AuthenticationException(string message)
            : base(message)
        {
            // Add Implementation (if required)
        }

        public AuthenticationException(string message, System.Exception inner)
            : base(message, inner)
        {
            // Add implementation (if required)
        }

        protected AuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation (if required)
        }
    }
}
