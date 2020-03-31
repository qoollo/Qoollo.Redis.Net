using System;
using System.Runtime.Serialization;

namespace RedisService.Exceptions
{
    /// <summary>
    /// Exception raised when one or few necessary Redis instances are null.
    /// </summary>
    [Serializable]
    public class RedisNullReferenceException : RedisServiceException
    {
        public RedisNullReferenceException()
            : base() { }

        public RedisNullReferenceException(string message)
            : base(message) { }

        public RedisNullReferenceException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public RedisNullReferenceException(string message, Exception innerException)
            : base(message, innerException) { }

        public RedisNullReferenceException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected RedisNullReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}