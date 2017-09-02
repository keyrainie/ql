using System.Web.Mvc;

namespace Nesoft.ECWeb.M
{
    public class NoCacheFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Expires = -1;
            filterContext.HttpContext.Response.Cache.SetNoStore();
        }
    }
}