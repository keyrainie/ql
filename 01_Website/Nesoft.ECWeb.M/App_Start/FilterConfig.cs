using System.Web;
using System.Web.Mvc;

namespace Nesoft.ECWeb.M
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ECWebHandleErrorAttribute());
            filters.Add(new NoCacheFilter());
        }
    }
}