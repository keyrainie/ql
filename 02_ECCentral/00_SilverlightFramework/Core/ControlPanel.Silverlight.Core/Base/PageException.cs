using System;
using System.Text;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Base
{
    public class PageException : Exception
    {
        private string m_title;
        private string m_message;
        private object m_request;

        public string Title
        {
            get
            {
                return m_title;
            }
        }

        public object Request
        {
            get
            {
                return m_request;
            }
        }

        public new string Message
        {
            get
            {
                return m_message;
            }
        }

        public PageException(string title, string message, object request):this(title,message,null,request)
        { 
            
        }

        public PageException(string title, string message,Exception innerException, object request)
            : base(message,innerException)
        {
            StringBuilder builder = new StringBuilder();
            m_title = title;
            m_request = request;
            m_message = string.Empty;

            if (!string.IsNullOrEmpty(base.Message))
            {
                builder.Append(" ●  " + base.Message);
            }
            
#if DEBUG
            if(this.InnerException != null)
            {
                builder.Append("\r\n ●  Inner Exception:\r\n");
                Exception exception = this.InnerException;
                if(exception != null)
                {
                    builder.Append(exception.Message);
                    builder.Append("\r\n");
                    builder.Append("\r\n ●  Stack Trace:\r\n");
                    builder.Append(exception.ToString());
                }
            }
#endif
            m_message = builder.ToString();
        }
    }
}
