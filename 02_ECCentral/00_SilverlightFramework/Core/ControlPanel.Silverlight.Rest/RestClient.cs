using System;
using System.Net;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Net.Browser;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Rest;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Text;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Browser;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Threading;

namespace Newegg.Oversea.Silverlight.ControlPanel.Rest
{
    internal struct Operating
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
    }

    public interface IAsyncHandle
    {
        void Abort();
    }

    internal class RestClientAsyncHandle : IAsyncHandle
    {
        private HttpRequest m_request;

        public RestClientAsyncHandle(HttpRequest request)
        {
            m_request = request;
        }
    
        #region IAsyncHandle Members

        public void  Abort()
        {
            if (m_request != null)
            {
                m_request.Abort();
            }
        }

        #endregion
    }

    public class RestClientEventArgs<T> : EventArgs
    {
        internal object m_result;

        public T Result 
        {
            get
            {
                return (T)m_result;
            }
        }

        public RestServiceError Error { get; internal set; }

        private IPage Page { get; set; }

        internal RestClientEventArgs(IPage page):base()
        {
            Page = page;
        }

        public RestClientEventArgs(T result,IPage page):this(page)
        {
            m_result = result;
        }

        public bool FaultsHandle()
        {
            return FaultsHandle(Page);
        }

        public bool FaultsHandle(IPage page)
        {
            if (this.Error != null)
            {
                StringBuilder build = new StringBuilder();
                bool isBizException = true;

                foreach (Error item in this.Error.Faults)
                {
                    if (isBizException && !item.IsBusinessException)
                    {
                        isBizException = false;
                    }
                    if (item.IsBusinessException)
                    {
                        build.Append(string.Format("{0}", item.ErrorDescription));
                    }
                    else
                    {
                        build.Append(string.Format("{0}", item.ErrorDescription));
                    }
                }

                if (page != null && (page as UserControl).Parent != null)
                {
                    if (isBizException)
                    {
                        page.Context.Window.Alert(build.ToString(), MessageType.Warning);
                    }
                    else
                    {
                        page.Context.Window.MessageBox.Show(build.ToString(), MessageBoxType.Error);
                    }
                }
                
                return true;
            }

            return false;
        }
    }

    public class RestClient:DependencyObject
    {
        public string ServicePath { get; private set; }

        private class AsyncArgs
        {
            public object Handler { get; private set; }
            public object DataSource { get; private set; }
            public string Content { get; private set; }
            public Type DataType { get; private set; }
            public HttpRequest Request { get; private set; }
            public object EventArgs { get; private set; }
            public object UserState { get; set; }


            public AsyncArgs(HttpRequest request, object data, Type type, object handler, object args)
            {
                Type dataType = data == null ? typeof(object) : data.GetType();
                Request = request;
                DataSource = data;
                DataType = type;
                Handler = handler;
                EventArgs = args;

                ISerializer serializer = SerializerFactory.GetSerializer(request.ContentType);
                if (serializer != null)
                {
                    Content = serializer.Serialization(data, dataType);
                }
            }
            public AsyncArgs(HttpRequest request, object data, Type type, object handler, object args, object userState)
                : this(request, data, type, handler, args)
            {
                UserState = userState;
            }
        }

        public IPage Page { get; private set; }

        public string ContentType { get; set; }

        static RestClient()
        {
            WebRequest.RegisterPrefix("http://", WebRequestCreator.BrowserHttp);
            WebRequest.RegisterPrefix("https://", WebRequestCreator.BrowserHttp);
          
        }

        public RestClient()
        {
            ContentType = ContentTypes.Json;
        }

        public RestClient(string servicPath)
            :this()
        {
            ServicePath = servicPath;
        }

        public RestClient(string servicPath, IPage page)
            : this(servicPath)
        {
            this.Page = page;
        }

        public static void RegisterSerializer(string serializerName,ISerializer serializer)
        {
            SerializerFactory.Register(serializerName, serializer);
        }

        public IAsyncHandle Query<T>(string relativeUrl, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : (relativeUrl.TrimStart(new char[]{'/'})));
            return Query<T>(new HttpRequest(url), callback);
        }

        public IAsyncHandle Query<T>(HttpRequest request, EventHandler<RestClientEventArgs<T>> callback)
        {
            string acceptType = ContentType;
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            DisplayLoading(true);
            string url = request.RequestUri.ToString();

            url = SetAcceptType(url, acceptType);
            url = SetLanguageType(url, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            request = new HttpRequest(url);
            request.Method = Operating.GET;
            request.BeginGetResponse(new AsyncCallback(OnGetResponse),
                        new AsyncArgs(request, null, typeof(T), callback, new RestClientEventArgs<T>(this.Page), acceptType));
            return new RestClientAsyncHandle(request);
        }

        private string SetAcceptType(string url,string type)
        {
            if (url != null && url.Length > 0)
            {
                if (url.Contains("?"))
                {
                    url += "&Portal_Accept=" + type;
                }
                else
                {
                    url += "?Portal_Accept=" + type;
                }
            }
            return url;
        }
        private string SetLanguageType(string url,string language)
        {
            if (url != null && url.Length > 0)
            {
                if (url.Contains("?"))
                {
                    url += "&Portal_Language=" + language;
                }
                else
                {
                    url += "?Portal_Language=" + language;
                }
            }
            return url;
        }

        public IAsyncHandle Query<T>(string relativeUrl, object condition, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Query<T>(new HttpRequest(url, ContentType, ContentType), condition, callback);
        }

        public IAsyncHandle Query<T>(HttpRequest request, object condition,EventHandler<RestClientEventArgs<T>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream),
                        new AsyncArgs(request, condition, typeof(T), callback, new RestClientEventArgs<T>(this.Page)));
            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle Create<T>(string relativeUrl, object data, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Create<T>(new HttpRequest(url, ContentType, ContentType), data, callback);
        }

        public IAsyncHandle Create<T>(HttpRequest request, object data, EventHandler<RestClientEventArgs<T>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                request.ContentType = request.Accept;
            }
            AsyncArgs args = new AsyncArgs(request, data, typeof(T), callback, new RestClientEventArgs<T>(this.Page));

            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream),args);

            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle Update<T>(string relativeUrl, object data, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Update<T>(new HttpRequest(url, ContentType, ContentType), data, callback);
        }

        public IAsyncHandle Update<T>(HttpRequest request, object data, EventHandler<RestClientEventArgs<T>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                request.ContentType = request.Accept;
            }

            AsyncArgs args = new AsyncArgs(request, data, typeof(T), callback, new RestClientEventArgs<T>(this.Page));

            request.Headers["X-Http-Method-Override"] = Operating.PUT;
            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream),args);

            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle Update(string relativeUrl, object data, EventHandler<RestClientEventArgs<object>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Update(new HttpRequest(url, ContentType, ContentType), data, callback);
        }

        public IAsyncHandle Update(HttpRequest request, object data, EventHandler<RestClientEventArgs<object>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                request.ContentType = request.Accept;
            }

            AsyncArgs args = new AsyncArgs(request, data, typeof(object), callback, new RestClientEventArgs<object>(this.Page));

            request.Headers["X-Http-Method-Override"] = Operating.PUT;
            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), args);

            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle Delete(string relativeUrl, object data, EventHandler<RestClientEventArgs<object>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Delete(new HttpRequest(url, ContentType, ContentType), data, callback);
        }

        public IAsyncHandle Delete(HttpRequest request, object data, EventHandler<RestClientEventArgs<object>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                request.ContentType = request.Accept;
            }

            AsyncArgs args = new AsyncArgs(request, data, typeof(object), callback, new RestClientEventArgs<object>(this.Page));

            request.Headers["X-Http-Method-Override"] = Operating.DELETE;
            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), args);

            return new RestClientAsyncHandle(request);
        }

        private string CombineUrl(string root, string sub)
        {
            string url = sub.ToLower().Trim();
            if (url.IndexOf("http") == 0)
            {
                return sub;
            }
            return root.TrimEnd(new char[] { '/' }) + "/" + sub.TrimStart(new char[] { '/', '\\' });
        }
#if VendorPortal
        private static int m_RequestingCount = 0;
#endif
        private void DisplayLoading(bool isDisplay)
        {
            if (Page != null)
            {
#if VendorPortal
                if (isDisplay)
                {
                    if (Interlocked.Increment(ref m_RequestingCount) == 1)
                    {
                        this.Dispatcher.BeginInvoke(() => { this.Page.Context.Window.LoadingSpin.Show(); });
                    }
                }
                else
                {
                    if (Interlocked.Decrement(ref m_RequestingCount) <= 0)
                    {
                        this.Dispatcher.BeginInvoke(() => { this.Page.Context.Window.LoadingSpin.Hide(); });
                    }
                }
#else
                if (isDisplay)
                {
                    this.Dispatcher.BeginInvoke(() => { this.Page.Context.Window.LoadingSpin.Show(); });
                }
                else
                {
                    this.Dispatcher.BeginInvoke(() => { this.Page.Context.Window.LoadingSpin.Hide(); });
                }
#endif
            }
           
        }

        private void OnGetRequestStream(IAsyncResult result)
        {
            DisplayLoading(true);
            AsyncArgs args = result.AsyncState as AsyncArgs;
            using (StreamWriter stream = new StreamWriter(args.Request.EndGetRequestStream(result)))
            {
                stream.Write(args.Content);
            }

            args.Request.BeginGetResponse(new AsyncCallback(OnGetResponse), args);
        }

        private void OnGetResponse(IAsyncResult result)
        {
            AsyncArgs args = result.AsyncState as AsyncArgs;
            HttpWebResponse response = null;
            object data = null;
            RestServiceError error = null;
            try
            {
                response = args.Request.EndGetResponse(result) as HttpWebResponse;
                ISerializer serializer = SerializerFactory.GetSerializer((args.Request.Accept == null || args.Request.Accept.Length == 0) ? args.UserState.ToString() : args.Request.Accept);

                if (serializer != null)
                {
                    if (!ErrorHandle(out error, response, serializer))
                    {
                        data = serializer.Deserialize(response.GetResponseStream(), args.DataType);
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse e = ex.Response as HttpWebResponse;
                error = new RestServiceError();
                error.StatusCode = e.StatusCode.GetHashCode();
                error.StatusDescription = e.StatusDescription;
                error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();          
                error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                e.Close();
                ComponentFactory.Logger.LogError(ex, string.Format("Call Service {0} Failed.", args.Request.RequestUri.ToString()), null);

            }
            catch (Exception ex)
            {
                error = new RestServiceError();
                error.StatusCode = 500;
                error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                ComponentFactory.Logger.LogError(ex, string.Format("Call Service {0} Failed.", args.Request.RequestUri.ToString()), null);
            }
            finally
            {
                DisplayLoading(false);
                if (response != null)
                {
                    response.Close();
                }
            }
            CallBackHandle(data,error, args);
        }

        private bool ErrorHandle(out RestServiceError error, HttpWebResponse response, ISerializer serializer)
        {
            bool existError = false;
            error = null;
            if (response != null)
            {
                try
                {
                    error = serializer.Deserialize(response.GetResponseStream(), typeof(RestServiceError)) as RestServiceError;
                    if (!(existError = (error != null && error.Faults != null && error.StatusCode > 0)))
                    {
                        error = null;
                    }
                }
                catch
                {
                    existError = false;
                }
            }

            return existError;
        }

        private void CallBackHandle(object dataSource,RestServiceError error, AsyncArgs e)
        {
            if (e.Handler != null)
            {
                dynamic handler = e.Handler;
                dynamic eventArgs = e.EventArgs;

                eventArgs.m_result = dataSource;
                eventArgs.Error = error;

                this.Dispatcher.BeginInvoke(() =>
                {
                    handler(this, eventArgs);
                });
            }
        }
    }
}
