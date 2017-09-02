using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ECommerce.UI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Common",
                routeTemplate: "api/{controller}/{action}"
            );

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }
    }
}
