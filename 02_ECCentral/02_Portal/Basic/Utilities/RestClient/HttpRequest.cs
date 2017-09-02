using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.Controls;
using System.IO;
using System.Net.Browser;
using Newegg.Oversea.Silverlight.Core.Components;

namespace ECCentral.Portal.Basic.Utilities
{
    public class HttpRequest
    {
        protected HttpWebRequest m_request;

        public string Accept
        {
            get
            {
                return m_request.Accept;
            }
            set
            {
                m_request.Accept = value;
            }
        }

        public bool AllowReadStreamBuffering
        {
            get
            {
                return m_request.AllowReadStreamBuffering;
            }
            set
            {
                m_request.AllowReadStreamBuffering = value;
            }
        }

        public bool AllowWriteStreamBuffering
        {
            get
            {
                return m_request.AllowWriteStreamBuffering;
            }

            set
            {
                m_request.AllowWriteStreamBuffering = value;
            }
        }

        public string ContentType
        {
            get
            {
                return m_request.ContentType;
            }

            set
            {
                m_request.ContentType = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get
            {
                return m_request.CookieContainer;
            }
            set
            {
                m_request.CookieContainer = value;
            }
        }

        public bool HaveResponse
        {
            get
            {
                return m_request.HaveResponse;
            }

        }

        public WebHeaderCollection Headers
        {
            get
            {
                return m_request.Headers;
            }
            set
            {
                m_request.Headers = value;
            }
        }

        public string Method
        {
            get
            {
                return m_request.Method;
            }
            internal set
            {
                m_request.Method = value;
            }
        }

        public Uri RequestUri
        {
            get
            {
                return m_request.RequestUri;
            }
        }

        public bool SupportsCookieContainer
        {
            get
            {
                return m_request.SupportsCookieContainer;
            }
        }

        //public long ContentLength
        //{
        //    get
        //    {
        //        return m_request.ContentLength;
        //    }

        //    set
        //    {
        //        m_request.ContentLength = value;
        //    }
        //}

        public IWebRequestCreate CreatorInstance
        {
            get
            {
                return m_request.CreatorInstance;
            }
        }

        public ICredentials Credentials
        {
            get
            {
                return m_request.Credentials;
            }
            set
            {
                m_request.Credentials = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return m_request.UseDefaultCredentials;
            }
            set
            {
                m_request.UseDefaultCredentials = value;
            }
        }

        public HttpRequest(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (url.IndexOf("/") == 0)
            {
                url = System.IO.Path.GetDirectoryName(Request.Host).Replace("http:\\", "http://")
                                                                   .Replace("https:\\", "https://")
                                                                   .Replace("\\", "/") + url;
            }

            //m_request = WebRequestCreator.ClientHttp.Create(new Uri(url)) as HttpWebRequest;
            //m_request.UseDefaultCredentials = false;

            //// For through ISA proxy
            //var isaAccount = ComponentFactory.GetComponent<IConfiguration>().GetConfigValue(ConstValue.DomainName_Common, ConstValue.ISAAccountKey);
            //var isaPassword = ComponentFactory.GetComponent<IConfiguration>().GetConfigValue(ConstValue.DomainName_Common, ConstValue.ISAPasswordKey);
            //var isaDomain = ComponentFactory.GetComponent<IConfiguration>().GetConfigValue(ConstValue.DomainName_Common, ConstValue.ISADomainKey);
            //if (!string.IsNullOrWhiteSpace(isaAccount) && !string.IsNullOrWhiteSpace(isaPassword))
            //{
            //    m_request.Credentials = string.IsNullOrWhiteSpace(isaDomain) 
            //        ? new NetworkCredential(isaAccount.Trim(), isaPassword.Trim())
            //        : new NetworkCredential(isaAccount.Trim(), isaPassword.Trim(), isaDomain.Trim());
            //}

            m_request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            //m_request.Accept = ContentTypes.Json;
            m_request.Method = Operating.GET;
        }

        public HttpRequest(string url, string accpetType, string contentType)
            : this(url)
        {
            m_request.Accept = ContentTypes.Json;
            m_request.ContentType = ContentTypes.Json;
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return m_request.BeginGetRequestStream(callback, state);
        }

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return m_request.BeginGetResponse(callback, state);
        }

        public Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return m_request.EndGetRequestStream(asyncResult);
        }

        public WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return m_request.EndGetResponse(asyncResult);
        }

        public void Abort()
        {
            m_request.Abort();
        }
    }
}
