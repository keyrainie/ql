using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ECommerce.WebFramework.Router;

namespace ECommerce.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
             RouteConfigurationSection section =
                (RouteConfigurationSection)ConfigurationManager.GetSection("routeConfig");

            routes.MapRoute(section);
        }
    }
}