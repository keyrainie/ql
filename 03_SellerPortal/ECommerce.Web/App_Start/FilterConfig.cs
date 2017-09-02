using System.Web;
using System.Web.Mvc;
using ECommerce.Web;

namespace ECommerce.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ECWebHandleErrorAttribute());
        }
    }
}