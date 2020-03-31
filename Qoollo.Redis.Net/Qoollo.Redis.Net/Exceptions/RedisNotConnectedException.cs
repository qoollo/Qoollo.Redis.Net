using System;
using System.Runtime.Serialization;

namespace RedisService.Exceptions
{
    /// <summary>
    /// Exception raised when there is no connection to Redis.
    /// </summary>
    [Serializable]
    public class RedisNotConnectedException : RedisServiceException
    {
        public RedisNotConnectedException()
            : base() { }

        public RedisNotConnectedException(string message)
            : base(message) { }

        public RedisNotConnectedException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public RedisNotConnectedException(string message, Exception innerException)
            : base(message, innerException) { }

        public RedisNotConnectedException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected RedisNotConnectedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}