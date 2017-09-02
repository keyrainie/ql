using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Version;

namespace Nesoft.ECWeb.MobileService.Core
{
    public class HeaderHelper
    {
        public static string GetUserAgent()
        {
            return HttpContext.Current.Request.Headers.Get("User-Agent");
        }

        public static string GetValue(string key)
        {
            NameValueCollection collection = HttpContext.Current.Request.Headers;
            if (collection != null && collection.AllKeys.Contains(key))
            {
                return collection.Get(key);
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取客户端类型
        /// </summary>
        /// <returns></returns>
        public static ClientType GetClientType()
        {
            string userAgent = GetUserAgent();
            if (!string.IsNullOrEmpty(userAgent) && !string.IsNullOrEmpty(userAgent.Trim()))
            {
                userAgent = userAgent.Trim().ToLower();
                if (userAgent.Contains("iphone"))
                {
                    return ClientType.IPhone;
                }
                else if (userAgent.Contains("android phone"))
                {
                    return ClientType.Android;
                }
            }

            return ClientType.Android;
        }

        public static int GetHighResolution()
        {
            string dpi = GetValue("X-HighResolution");
            int result;
            int.TryParse(dpi, out result);
            return result;
        }

        public static bool IsAndroidXHigh()
        {
            int result = GetHighResolution();
            return result >= 320;
        }

        public static string GetOSVersion()
        {
            return GetValue("X-OSVersion");
        }
    }
}