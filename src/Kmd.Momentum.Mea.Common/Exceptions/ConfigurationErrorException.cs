using System;
using System.Runtime.Serialization;

namespace Kmd.Momentum.Mea.Common.Exceptions
{
    [Serializable]
    public class ConfigurationErrorException : Exception
    {
        public ConfigurationErrorException()
        {
        }

        public ConfigurationErrorException(string message) : base(message)
        {
        }

        public ConfigurationErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConfigurationErrorException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
