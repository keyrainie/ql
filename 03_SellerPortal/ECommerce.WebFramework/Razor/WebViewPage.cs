using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using ECommerce.WebFramework.Router;
using System.Web;
using System.Configuration;
using System.Web.Caching;
using System.Text.RegularExpressions;
using ECommerce.Utility;

namespace ECommerce.WebFramework
{
    public class Config
    {
        public static bool HaveSSLWebsite = System.Configuration.ConfigurationManager.AppSettings["HaveSSLWebsite"].ToString().ToLower() == "true" ? true : false;
        public static string CDNWebDomain = System.Configuration.ConfigurationManager.AppSettings["CDNWebDomain"];
        public static string SSLWebsiteHost = System.Configuration.ConfigurationManager.AppSettings["SSLWebsiteHost"];
        public static List<string> SSLControllers = System.Configuration.ConfigurationManager.AppSettings["SSLControllers"].ToString().ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
    }

    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        protected string LanguageCode
        {
            get
            {
                return LanguageHelper.GetLanguageCode();
            }
        }

        protected MvcHtmlString DisplayStringWhen(string @string, Func<bool> expression)
        {
            if (expression())
                return new MvcHtmlString(@string);
            return new MvcHtmlString(string.Empty);
        }

        protected MvcHtmlString DisplayStringWhen(string @string, bool condition)
        {
            if (condition)
                return new MvcHtmlString(@string);
            return new MvcHtmlString(string.Empty);
        }

        protected MvcHtmlString BuildBootModal(string modalid, string modalSizeClass = "modal-lg")
        {
            string modalTemplate = @"
<div class=""modal fade bs-modal-lg"" data-keyboard=""false"" data-backdrop=""static""
    id=""{0}"" role=""basic"" aria-hidden=""true"">
    <div class=""page-loading page-loading-boxed"">
        <img src=""{1}"" alt=""loading"" />
        <span>&nbsp;&nbsp;Loading... </span>
    </div>
    <div class=""modal-dialog modal-lg"">
        <div class=""modal-content"" >
        </div>
    </div>
</div>";
            if (!string.IsNullOrWhiteSpace(modalSizeClass))
            {
                modalTemplate = modalTemplate.Replace(@"<div class=""modal-dialog modal-lg"">", string.Format(@"<div class=""modal-dialog {0}"">", modalSizeClass));
            }
            string modalHtml = string.Format(modalTemplate, modalid,
                Url.Content("~/Content/themes/metronic/assets/global/img/loading-spinner-grey.gif"));

            return new MvcHtmlString(modalHtml);
        }

        protected MvcHtmlString GetText(string key)
        {
            return new MvcHtmlString(LanguageHelper.GetText(key));
        }

        protected MvcHtmlString GetImage(string key)
        {
            return new MvcHtmlString(LanguageHelper.GetImage(key));
        }

        protected MvcHtmlString BuildCssRef(string path)
        {
            //<link href="/Resources/themes/default/css/jqextension.css?201406051400" rel="stylesheet"/>
            if (!Config.HaveSSLWebsite)
            {
                string cdnWebDomain = Config.CDNWebDomain.TrimEnd("/".ToCharArray());
                path = path.TrimStart("~".ToCharArray()).TrimStart("/".ToCharArray());
                path = cdnWebDomain + "/" + path;
            }
            else
            {
                path = "/" + path.TrimStart("~".ToCharArray()).TrimStart("/".ToCharArray());
            }


            string str = string.Format("<link href=\"{0}\" rel=\"stylesheet\"/>", path);
            return new MvcHtmlString(str);
        }

        protected MvcHtmlString BuildJsRef(string path)
        {
            //<script src="/Resources/scripts/common/common.js?201406051400"></script>


            if (!Config.HaveSSLWebsite)
            {
                string cdnWebDomain = Config.CDNWebDomain.TrimEnd("/".ToCharArray());
                path = path.TrimStart("~".ToCharArray()).TrimStart("/".ToCharArray());
                path = cdnWebDomain + "/" + path;
            }
            else
            {
                path = "/" + path.TrimStart("~".ToCharArray()).TrimStart("/".ToCharArray());
            }
            string str = string.Format("<script src=\"{0}\"></script>", path);
            return new MvcHtmlString(str);
        }



        protected void SetSEO(SEOInfo seoInfo)
        {
            if (seoInfo != null)
            {
                ViewBag.Title = string.IsNullOrWhiteSpace(seoInfo.PageTitle) ? ViewBag.Title : seoInfo.PageTitle;
                ViewBag.SEOKeywords = seoInfo.PageKeywords;
                ViewBag.SEODescription = seoInfo.PageDescription;
            }
            else
            {
                ViewBag.SEOKeywords = "";
                ViewBag.SEODescription = "";
                ViewBag.SEOAdditionContent = "";
            }
        }


        /// <summary>
        /// 根据输入的相对路径自动构造包含多语言信息的Url
        /// </summary>
        /// <param name="url">相对路径</param>
        /// <returns>网页包含多语言信息的相对Url</returns>
        protected string BuildUrlCA(string controllerName, string actionName, params object[] routeValues)
        {
            return PageHelper.BuildUrlCA(controllerName, actionName, routeValues);

        }

        /// <summary>
        /// 根据RouteName自动构造包含多语言、是否需要SSL的URL
        /// </summary>
        /// <param name="routeName">路由名称，route.config中的路由名称</param>
        /// <param name="routeValues">路由参数，请与路由配置文件中的参数顺序保持一致</param>
        /// <returns>包含多语言，是否是https访问的绝对Url</returns>
        protected string BuildUrl(string routeName, params object[] routeValues)
        {
            return PageHelper.BuildUrl(routeName, routeValues);
        }

        /// <summary>
        /// 截取指定长度的字符串
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="count"></param>
        /// <param name="isSuffix">是否加上三个小点</param>
        /// <returns></returns>
        protected string SubString(string targetString, int count, bool isSuffix = false)
        {
            if (string.IsNullOrWhiteSpace(targetString))
            {
                return string.Empty;
            }

            targetString = Regex.Replace(targetString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase); //删除HTML

            if (targetString.Length <= count)
            {
                return targetString;
            }
            targetString = targetString.Substring(0, count);
            if (isSuffix)
            {
                targetString += "...";
            }
            return targetString;
        }



        protected string Escape(string targetString)
        {
            if (string.IsNullOrWhiteSpace(targetString))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            byte[] ba = System.Text.Encoding.Unicode.GetBytes(targetString);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));

                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();

        }

    }

    public static class PageHelper
    {

        public static string BuildUrlCA(string controllerName, string actionName, params object[] routeValues)
        {
            string protocol = "http";
            string hostName = GetHost();

            //获取所有的Controller，看是不是基于SSL的，如果是，则使用HTTPS
            if (Config.HaveSSLWebsite)
            {
                if (Config.SSLControllers.Exists(f => f.ToLower().Trim() == controllerName.ToLower().Trim()))
                {
                    protocol = "https";
                    hostName = GetSecureHost();
                }
            }

            string relUrl = string.Format("{0}/{1}", controllerName, actionName);

            if (routeValues != null && routeValues.Length > 0)
            {
                for (int i = 0; i < routeValues.Length; i++)
                {
                    relUrl = relUrl + "/" + routeValues[i].ToString();
                }
                relUrl = relUrl.TrimStart("/".ToCharArray());
            }

            string urlLink = string.Format("{0}://{1}/{2}", protocol, hostName, relUrl).ToLower();

            return urlLink;
        }


        /// <summary>
        /// 根据RouteName自动构造包含多语言、是否需要SSL的URL
        /// </summary>
        /// <param name="routeName">路由名称，route.config中的路由名称</param>
        /// <param name="routeValues">路由参数，请与路由配置文件中的参数顺序保持一致</param>
        /// <returns>包含多语言，是否是https访问的绝对Url</returns>
        public static string BuildUrl(string routeName, params object[] routeValues)
        {
            RouteConfigurationSection section = GetRouteConfig();

            //AreaItem curArea = null;
            RoutingItem curRouteItem = null;

            //先从Areas中查找
            foreach (AreaItem area in section.Areas)
            {
                foreach (RoutingItem routingItem in area.Map)
                {
                    if (routingItem.Name.ToLower().Trim() == routeName.Trim().ToLower())
                    {
                        curRouteItem = routingItem;
                        //curArea = area;
                        break;
                    }
                }
                if (curRouteItem != null)
                {
                    break;
                }
            }
            //string lc = LanguageHelper.GetLanguageCode();
            //如果不存在，则从Maps中查找
            if (curRouteItem == null)
            {
                foreach (RoutingItem map in section.Map)
                {
                    if (map.Name.ToLower().Trim() == routeName.Trim().ToLower())
                    {
                        curRouteItem = map;
                        break;
                    }
                    if (curRouteItem != null)
                    {
                        break;
                    }
                }
            }

            if (curRouteItem == null)
            {
                //return string.Format("/{0}/{1}", lc, routeName);
                return string.Format("/{0}", routeName);
            }

            string relUrl = curRouteItem.Url.ToLower();

            string hostName = GetHost();

            string protocol = "http";
            if (!string.IsNullOrWhiteSpace(curRouteItem.NeedSSL)
                && curRouteItem.NeedSSL.Trim() == "1"
                && Config.HaveSSLWebsite)
            {
                protocol = "https";
                hostName = GetSecureHost();
            }

            if (curRouteItem != null
                && routeValues != null
                && curRouteItem.Paramaters != null
                && routeValues.Length == curRouteItem.Paramaters.Count)
            {
                for (int i = 0; i < curRouteItem.Paramaters.Count; i++)
                {
                    Parameter paramter = curRouteItem.Paramaters[i];
                    relUrl = relUrl.Replace(paramter.Value.ToLower(), routeValues[i].ToString());
                }
                relUrl = relUrl.TrimStart("/".ToCharArray());
            }

            //string urlLink = string.Format("{0}://{1}/{2}/{3}", protocol, hostName, lc, relUrl).ToLower();
            string urlLink = string.Format("{0}://{1}/{2}", protocol, hostName, relUrl).ToLower();

            return urlLink;
        }

        private static RouteConfigurationSection GetRouteConfig()
        {
            string cacheKey = "GetRouteConfigSection";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (RouteConfigurationSection)HttpRuntime.Cache[cacheKey];
            }
            RouteConfigurationSection section = (RouteConfigurationSection)ConfigurationManager.GetSection("routeConfig");

            HttpRuntime.Cache.Insert(cacheKey, section, null, DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            return section;
        }

        internal static string GetHost()
        {
            Uri uri = HttpContext.Current.Request.Url;
            string domainName = string.Empty;
            if (ConfigurationManager.AppSettings["WebDomain"] != null
                && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["WebDomain"].ToString()))
            {
                domainName = ConfigurationManager.AppSettings["WebDomain"].ToString().Trim();
                domainName = domainName.Replace("http://", "");
            }
            else
            {
                domainName = uri.Host.ToLower();
            }

            string portName = uri.Port == 80 ? "" : ":" + uri.Port.ToString();

            string hostName = domainName + portName;
            return hostName;
        }
        internal static string GetSecureHost()
        {
            if (string.IsNullOrWhiteSpace(Config.SSLWebsiteHost))
            {
                string host = GetHost();
                host = host.Replace("www.", "secure.");
                host = host.Replace("http://", "");
                return host;
            }
            else
            {
                return Config.SSLWebsiteHost;
            }
        }
    }

    public class SEOInfo
    {
        public int SysNo { get; set; }
        public int PageID { get; set; }
        public int PageType { get; set; }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageKeywords { get; set; }

    }
}
