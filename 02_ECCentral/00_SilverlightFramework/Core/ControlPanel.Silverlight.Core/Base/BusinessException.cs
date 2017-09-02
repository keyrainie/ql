using System;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Base
{
    public class BusinessException : Exception
    {
        public bool NeedLog { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public BusinessException()
            : this(null)
        {
        }

        public BusinessException(string message)
            : this(message, null, null, false)
        {
        }

        public BusinessException(string message, Exception innerException)
            : this("0", message, innerException, false)
        {
        }

        public BusinessException(string errorCode, string message)
            : this(errorCode, message, false)
        {
        }

        public BusinessException(string errorCode, string message, bool needLog)
            : this("0", message, null, needLog)
        {
        }

        public BusinessException(string errorCode, string message, Exception innerException)
            : this(errorCode, message, innerException, false)
        {
        }

        public BusinessException(string message, Exception innerException, bool needLog)
            : this("0", message, innerException, needLog)
        {
        }

        public BusinessException(string errorCode, string message, Exception innerException, bool needLog)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorDescription = message;
            NeedLog = needLog;
        }
    }
}