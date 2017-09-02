using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECCentral.Portal.WebHost
{
    /// <summary>
    /// Summary description for HtmlViewHandler
    /// </summary>
    public class HtmlViewHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context)
        {
            string content = context.Request.Form["Content"];
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(content.Trim()));
            }

            // 禁用缓存
            context.Response.Expires = -1;
            context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            context.Response.CacheControl = "no-cache";
            context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            context.Response.ContentType = "text/html";
            context.Response.Write(content);
            context.Response.Flush();
        }
     
        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}