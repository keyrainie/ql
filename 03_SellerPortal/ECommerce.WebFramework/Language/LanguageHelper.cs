using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using System.Configuration;

namespace ECommerce.WebFramework
{
    public class LanguageHelper
    {
        public const string DEFAULT_LANGUAGE_CODE = "zh-cn";
        /// <summary>
        /// 获取当前语言编码
        /// </summary>
        /// <returns></returns>
        public static string GetLanguageCode()
        {
            string currentLanguageCode = DEFAULT_LANGUAGE_CODE;            

            HttpContextBase contextWrapper = new HttpContextWrapper(HttpContext.Current);
            RouteData routeData = RouteTable.Routes.GetRouteData(contextWrapper);
            if (routeData != null)
            {
                object cultureCode;
                if (routeData.Values.TryGetValue("culture", out cultureCode))
                {
                    if(cultureCode!=null)
                    {
                        CultureInfo culture = null;
                        try
                        {
                            culture = new CultureInfo(cultureCode.ToString());
                        }
                        catch
                        {
                            ;
                        }
                        if (culture != null)
                        {
                            currentLanguageCode = cultureCode.ToString().Trim().ToLower();
                        }
                    }                     
                }
            }

            return currentLanguageCode;
        }

        /// <summary>
        /// 获取文字性内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetText(string key)
        {
            return _GetResourceMess("text", key);             
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetImage(string key)
        {
            return _GetResourceMess("image", key);
        }

        /// <summary>
        /// 根据当前页面的语言编码动态设置CSS文件。
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString SetCssResource()
        {
            string resourcePath = _GetLanguageResourceRootPath();
            string languageCode = GetLanguageCode();
            resourcePath += "/" + languageCode + "/Style.css";
            StringBuilder sb = new StringBuilder();
            if (languageCode.ToUpper() != DEFAULT_LANGUAGE_CODE.ToUpper())
            {
                sb.AppendLine(string.Format(" <link href=\"{0}\" rel=\"stylesheet\"/>", resourcePath));
            }
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 根据当前页面的语言编码动态设置JS多语言信息。
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString SetJSResource()
        {
            string resourcePath = _GetLanguageResourceRootPath();
            string languageCode = GetLanguageCode();
            resourcePath += "/" + languageCode + "/ScriptResource.js";


            StringBuilder sb = new StringBuilder();
            if (languageCode.ToUpper() != DEFAULT_LANGUAGE_CODE.ToUpper())
            {
                sb.AppendLine("<script src=\"" + resourcePath + "\"></script>");
                sb.AppendLine("<script language=\"javascript\">");
                sb.AppendLine("function JR(key) { ");
                sb.AppendLine("if (ScriptResource[key]==null || ScriptResource[key]=='' ||  ScriptResource[key] == 'undefine') {");
                sb.AppendLine("    return key;");
                sb.AppendLine(" }");
                sb.AppendLine(" return ScriptResource[key];");
                sb.AppendLine("}");
                sb.AppendLine("</script>");
            }
            else
            {
                sb.AppendLine("<script src=\"" + resourcePath + "\"></script>");
                sb.AppendLine("<script language=\"javascript\">");
                sb.AppendLine("function JR(key) { ");
                sb.AppendLine(" return key;");
                sb.AppendLine("}");
                sb.AppendLine("</script>");
            }

            return new MvcHtmlString(sb.ToString());
        }


        private static object m_SyncObj = new object();
        private static string m_rootPath = null;
        private static string _GetLanguageResourceRootPath()
        {
            if (m_rootPath == null)
            {
                lock (m_SyncObj)
                {
                    string resourcePath = string.Empty;
                    if (ConfigurationManager.AppSettings["TextResourcesPath"] != null)
                    {
                        resourcePath = ConfigurationManager.AppSettings["TextResourcesPath"].ToString().Trim();
                        resourcePath = resourcePath.Replace("\\", "/").TrimStart("/".ToCharArray()).TrimEnd("/".ToCharArray());
                        resourcePath = "/" + resourcePath;
                    }
                    if (string.IsNullOrEmpty(resourcePath))
                    {
                        resourcePath = "/Configuration/LanguageResources";
                    }

                    m_rootPath = resourcePath;
                }
            }


            return m_rootPath;
        }

        private static string _GetResourceMess(string messType, string key)
        {
            string languageCode = GetLanguageCode();
            string text = Utility.TextResourceManager.GetText(languageCode + "."+messType, key,false);
            //if (text == null)
            //{
            //    return key;
            //}
            //if (text.Trim()=="")
            //{
            //    return "";
            //}
            if (string.IsNullOrEmpty(text))
            {
                return key;
            }
            return text;
        }
    }
}
