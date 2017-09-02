using System;
using System.Net;
using System.Windows;
using Newegg.Oversea.Silverlight.Controls;
using System.Net.Browser;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Core.Components;
using System.IO;
using System.Text;
using System.Windows.Browser;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Threading;

namespace ECCentral.Portal.Basic.Utilities
{
    public class RestClient : DependencyObject
    {
        private const string SIGN_KEY = "ds@H73d12Jds%dN&2#";

        private static int? s_UserSysNo;
        public static int UserSysNo
        {
            get
            {
                if (s_UserSysNo == null)
                {
                    s_UserSysNo = CPApplication.Current.LoginUser.UserSysNo.GetValueOrDefault();
                }
                return s_UserSysNo.Value;
            }
        }

        private static string s_UserAcct;
        public static string UserAcct
        {
            get
            {
                if (s_UserAcct == null)
                {
                    s_UserAcct = CPApplication.Current.LoginUser.LoginName;
                }
                return s_UserAcct;
            }
        }

        private static string s_UserDisplayName;
        public static string UserDisplayName
        {
            get
            {
                if (s_UserDisplayName == null)
                {
                    s_UserDisplayName = CPApplication.Current.LoginUser.DisplayName;
                }
                return s_UserDisplayName;
            }
        }

        private static string s_AuthorizedCompanyCodeList;
        public static string AuthorizedCompanyCodeList
        {
            get
            {
                if (s_AuthorizedCompanyCodeList == null)
                {
                    foreach (var a in CPApplication.Current.CompanyList)
                    {
                        if (a != null)
                        {
                            if (s_AuthorizedCompanyCodeList != null && s_AuthorizedCompanyCodeList.Length > 0)
                            {
                                s_AuthorizedCompanyCodeList = s_AuthorizedCompanyCodeList + ",";
                            }
                            s_AuthorizedCompanyCodeList = s_AuthorizedCompanyCodeList + a.CompanyCode;
                        }
                    }
                }
                return s_AuthorizedCompanyCodeList;
            }
        }

        private static string s_SelectedCompanyCode;
        public static string SelectedCompanyCode
        {
            get
            {
                if (s_SelectedCompanyCode == null)
                {
                    s_SelectedCompanyCode = CPApplication.Current.CompanyCode;
                }
                return s_SelectedCompanyCode;
            }
        }

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

        public RestClient(string servicPath)
            : this(servicPath, CPApplication.Current.CurrentPage)
        {

        }

        public RestClient(string servicPath, IPage page)
        {
            ContentType = ContentTypes.Json;
            ServicePath = servicPath;
            Page = page;
        }

        public static void RegisterSerializer(string serializerName, ISerializer serializer)
        {
            SerializerFactory.Register(serializerName, serializer);
        }

        public IAsyncHandle QueryDynamicData(string relativeUrl, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : (relativeUrl.TrimStart(new char[] { '/' })));
            return QueryDynamicData(new HttpRequest(url, ContentType, ContentType), callback);
        }

        public IAsyncHandle QueryDynamicData(HttpRequest request, EventHandler<RestClientEventArgs<dynamic>> callback)
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
            url = SetIdentityAndTimeZone(url);
            request = new HttpRequest(url);
            request.Method = Operating.GET;
            request.BeginGetResponse(new AsyncCallback(OnGetResponse_DynamicData),
                        new AsyncArgs(request, null, null, callback, new RestClientEventArgs<dynamic>(this.Page), acceptType));
            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle QueryDynamicData(string relativeUrl, object condition, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : (relativeUrl.TrimStart(new char[] { '/' })));
            return QueryDynamicData(new HttpRequest(url, ContentType, ContentType), condition, callback);
        }

        public IAsyncHandle QueryDynamicData(HttpRequest request, object condition, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            HandlePostRequestHeader(request, Operating.POST);
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream_DynamicData),
                        new AsyncArgs(request, condition, null, callback, new RestClientEventArgs<dynamic>(this.Page)));
            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle Query<T>(string relativeUrl, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : (relativeUrl.TrimStart(new char[] { '/' })));
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
            url = SetIdentityAndTimeZone(url);
            request = new HttpRequest(url);
            request.Method = Operating.GET;
            request.BeginGetResponse(new AsyncCallback(OnGetResponse),
                        new AsyncArgs(request, null, typeof(T), callback, new RestClientEventArgs<T>(this.Page), acceptType));
            return new RestClientAsyncHandle(request);
        }

        public IAsyncHandle QueryEx<T>(string relativeUrl, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : (relativeUrl.TrimStart(new char[] { '/' })));
            return QueryEx<T>(new HttpRequest(url), callback);
        }

        public IAsyncHandle QueryEx<T>(HttpRequest request, EventHandler<RestClientEventArgs<T>> callback)
        {
            string acceptType = ContentType;
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            DisplayLoading(true);
            string url = request.RequestUri.ToString();

            url = SetAcceptType(url, acceptType);
            url = SetLanguageType(url, Thread.CurrentThread.CurrentUICulture.Name);
            url = SetIdentityAndTimeZone(url);
            request = new HttpRequest(url) {Method = Operating.GET};
            var args = new AsyncArgs(request, null, typeof (T), callback, new RestClientEventArgs<T>(this.Page),
                                     acceptType);
            var asyncResult = request.BeginGetResponse(OnGetResponse, args);
            return new RestClientAsyncHandle(request, asyncResult);
        }

        private string SetAcceptType(string url, string type)
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
        private string SetLanguageType(string url, string language)
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

        private string SetIdentityAndTimeZone(string url)
        {
            if (url != null && url.Length > 0)
            {
                string firstChar = url.Contains("?") ? "&" : "?";
                string hour = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString();
                string sign = SignParameters(UserSysNo.ToString(), UserAcct, hour);
                url += firstChar 
                    + "Portal_UserSysNo=" + UserSysNo 
                    + "&Portal_UserAcct=" + HttpUtility.UrlEncode(UserAcct) 
                    + "&Portal_UserDisplayName=" + HttpUtility.UrlEncode(UserDisplayName) 
                    + "&Portal_SelectedCompanyCode=" + HttpUtility.UrlEncode(SelectedCompanyCode) 
                    + "&Portal_AuthorizedCompanyCodeList=" + HttpUtility.UrlEncode(AuthorizedCompanyCodeList) 
                    + "&Portal_TimeZone=" + hour 
                    + "&Portal_Sign=" + HttpUtility.UrlEncode(sign);
            }
            return url;
        }

        public IAsyncHandle Query<T>(string relativeUrl, object condition, EventHandler<RestClientEventArgs<T>> callback)
        {
            string url = CombineUrl(this.ServicePath, string.IsNullOrEmpty(relativeUrl) ? "" : relativeUrl);
            return Query<T>(new HttpRequest(url, ContentType, ContentType), condition, callback);
        }

        public IAsyncHandle Query<T>(HttpRequest request, object condition, EventHandler<RestClientEventArgs<T>> callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            HandlePostRequestHeader(request, Operating.POST);
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

            HandlePostRequestHeader(request, Operating.POST);
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), args);

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

            HandlePostRequestHeader(request, Operating.PUT);
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), args);

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

            HandlePostRequestHeader(request, Operating.PUT);
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

            HandlePostRequestHeader(request, Operating.DELETE);
            request.Method = Operating.POST;
            request.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), args);

            return new RestClientAsyncHandle(request);
        }

        public string SignParameters(string userSysNo, string userAcct, string hour)
        {
            return MD5CryptoHelper.ComputeHash(userSysNo + UserAcct + hour + SIGN_KEY);
        }

        private void HandlePostRequestHeader(HttpRequest request, string operating)
        {
            if (operating != Operating.POST)
            {
                request.Headers["X-Http-Method-Override"] = operating;
            }
            string hour = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString();
            string sign = SignParameters(UserSysNo.ToString(), UserAcct, hour);

            request.Headers["X-Accept-Language-Override"] = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            request.Headers["X-User-SysNo"] = UserSysNo.ToString();
            request.Headers["X-User-Acct"] = UserAcct;
            request.Headers["X-User-Display-Name"] = HttpUtility.UrlEncode(UserDisplayName);
            request.Headers["X-User-Selected-CompanyCode"] = HttpUtility.UrlEncode(SelectedCompanyCode);
            request.Headers["X-User-CompanyCode-List"] = HttpUtility.UrlEncode(AuthorizedCompanyCodeList);
            request.Headers["X-Portal-TimeZone"] = hour;
            request.Headers["X-Portal-Sign"] = sign;
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

        private static int m_RequestingCount = 0;
        public void DisplayLoading(bool isDisplay)
        {
            if (Page != null)
            {
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
            }
        }

        private void OnGetRequestStream_DynamicData(IAsyncResult result)
        {
            DisplayLoading(true);
            AsyncArgs args = result.AsyncState as AsyncArgs;
            using (StreamWriter stream = new StreamWriter(args.Request.EndGetRequestStream(result)))
            {
                stream.Write(args.Content);
            }

            args.Request.BeginGetResponse(new AsyncCallback(OnGetResponse_DynamicData), args);
        }

        private void OnGetResponse_DynamicData(IAsyncResult result)
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
                        StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        readStream.BaseStream.Position = 0;
                        string str = readStream.ReadToEnd();
                        data = DynamicXml.Parse(str); //serializer.Deserialize(response.GetResponseStream(), args.DataType);
                    }
                }
            }
            catch (WebException ex)
            {
                //HttpWebResponse e = ex.Response as HttpWebResponse;
                //error = new RestServiceError();
                //error.StatusCode = e.StatusCode.GetHashCode();
                //error.StatusDescription = e.StatusDescription;
                //error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                //error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                //e.Close();
                HttpWebResponse e = ex.Response as HttpWebResponse;
                if (e != null)
                {
                    error = new RestServiceError();
                    error.StatusCode = e.StatusCode.GetHashCode();
                    error.StatusDescription = e.StatusDescription;
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                    e.Close();
                }
                else if (ex.Response != null)
                {
                    error = new RestServiceError();
                    error.StatusCode = -1;
                    error.StatusDescription = "未知错误，返回的WebException.Response类型为：" + ex.Response.GetType();
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error()
                    {
                        ErrorCode = "00000",
                        ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString())
                    });
                }
                else
                {
                    error = new RestServiceError();
                    error.StatusCode = -2;
                    error.StatusDescription = "未知错误，返回的WebException.Response为null。";
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error()
                    {
                        ErrorCode = "00000",
                        ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString())
                    });
                }
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
            CallBackHandle(data, error, args);
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
                //HttpWebResponse e = ex.Response as HttpWebResponse;
                //error = new RestServiceError();
                //error.StatusCode = e.StatusCode.GetHashCode();
                //error.StatusDescription = e.StatusDescription;
                //error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                //error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                //e.Close();
                HttpWebResponse e = ex.Response as HttpWebResponse;
                if (e != null)
                {
                    error = new RestServiceError();
                    error.StatusCode = e.StatusCode.GetHashCode();
                    error.StatusDescription = e.StatusDescription;
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error() { ErrorCode = "00000", ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString()) });
                    e.Close();
                }
                else if (ex.Response != null)
                {
                    error = new RestServiceError();
                    error.StatusCode = -1;
                    error.StatusDescription = "未知错误，返回的WebException.Response类型为：" + ex.Response.GetType();
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error()
                    {
                        ErrorCode = "00000",
                        ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString())
                    });
                }
                else
                {
                    error = new RestServiceError();
                    error.StatusCode = -2;
                    error.StatusDescription = "未知错误，返回的WebException.Response为null。";
                    error.Faults = new System.Collections.ObjectModel.ObservableCollection<Error>();
                    error.Faults.Add(new Error()
                    {
                        ErrorCode = "00000",
                        ErrorDescription = string.Format("Call Service {0} Failed.\r\n\r\nError Detail:{1}", args.Request.RequestUri.ToString(), ex.ToString())
                    });
                }
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
            CallBackHandle(data, error, args);
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

        private void CallBackHandle(object dataSource, RestServiceError error, AsyncArgs e)
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
