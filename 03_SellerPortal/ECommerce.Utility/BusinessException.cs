using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Utility
{
    public class BusinessException : Exception
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        public BusinessException() : base() { }
        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        /// <param name="message">The error message to be provided to the exception.</param>
        public BusinessException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        /// <param name="message">The error message to be provided to the exception.</param>
        /// <param name="innerException">The inner exception which causes this exception to occur.</param>
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        /// <param name="format">The string formatter for the error message.</param>
        /// <param name="args">The parameters that are used by the string formatter to format the error message.</param>
        public BusinessException(string format, params object[] args) : base(string.Format(format, args)) { }
        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }

}
