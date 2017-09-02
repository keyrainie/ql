using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Facade.Passport.Partner
{
    /// <summary>
    /// 
    /// </summary>
    internal class HttpHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// url
        /// or
        /// requestEncoding
        /// </exception>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return e.Response as HttpWebResponse;
                }
                else
                {
                    Logger.WriteLog(e.ToString(), "ECommerce.Facade.Passport.Partner", "HttpHelper");

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.ToString(), "ECommerce.Facade.Passport.Partner", "HttpHelper");
                return null;
            }
        }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool Get(string url, out string result)
        {
            try
            {
                HttpWebResponse response = CreateGetHttpResponse(url, null, null, null);
                return TryConvertResponseToData(url, response, out result);
            }
            catch (Exception e)
            {
                result = "";
                Logger.WriteLog(e.ToString(), "ECommerce.Facade.Passport.Partner", "HttpHelper");
                return false;
            }
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool Post(string url, IDictionary<string, string> parameters, out string result)
        {
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, parameters, null, null, Encoding.UTF8, null);
                return TryConvertResponseToData(url, response, out result);
            }
            catch (Exception e)
            {
                result = "";
                Logger.WriteLog(e.ToString(), "ECommerce.Facade.Passport.Partner", "HttpHelper");
                return false;
            }
        }

        /// <summary>
        /// Converts the response to data.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="response">The response.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static bool TryConvertResponseToData(string url, HttpWebResponse response, out string result)
        {
            result = string.Empty;
            if (response != null)
            {
                string responseString = string.Empty;
                Stream dataStream     = null;
                StreamReader reader   = null;
                try
                {
                    dataStream     = response.GetResponseStream();
                    reader         = new StreamReader(dataStream);
                    responseString = reader.ReadToEnd();
                    dataStream.Close();
                }
                catch { }
                finally
                {
                    if (dataStream != null)
                    {
                        dataStream.Close();
                    }
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = responseString;
                    return true;
                }
                else
                {
                    if (!String.IsNullOrEmpty(responseString))
                    {
                        result = responseString;
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the validation result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }
}

