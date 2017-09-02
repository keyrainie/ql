using System.Web;
using System.Web.Mvc;

namespace ECommerce.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ECWebHandleErrorAttribute());
            filters.Add(new NoCacheFilter());
            filters.Add(new CPSFlagFilter());
        }
    }
}