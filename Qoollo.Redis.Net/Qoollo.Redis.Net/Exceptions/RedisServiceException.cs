using System;
using System.Runtime.Serialization;

namespace RedisService.Exceptions
{
    [Serializable]
    public class RedisServiceException : Exception
    {
        public RedisServiceException()
            : base() { }

        public RedisServiceException(string message)
            : base(message) { }

        public RedisServiceException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public RedisServiceException(string message, Exception innerException)
            : base(message, innerException) { }

        public RedisServiceException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected RedisServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}