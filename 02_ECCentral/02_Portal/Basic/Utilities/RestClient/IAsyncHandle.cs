using System;
using System.Net;

namespace ECCentral.Portal.Basic.Utilities
{
    public interface IAsyncHandle
    {
        void Abort();
        IAsyncResult AsyncResult { get; }
    }

    public class RestClientAsyncHandle : IAsyncHandle
    {
        private HttpRequest m_request;
        private IAsyncResult _asyncResult;
        public RestClientAsyncHandle(HttpRequest request)
        {
            m_request = request;
        }

        public RestClientAsyncHandle(HttpRequest request, IAsyncResult asyncResult)
        {
            m_request = request;
            _asyncResult = asyncResult;
        }

        #region IAsyncHandle Members

        public void Abort()
        {
            if (m_request != null)
            {
                m_request.Abort();
            }
        }

        public IAsyncResult AsyncResult {
         get { return _asyncResult; }
        }

        #endregion
    }
}
