using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ECommerce.WebFramework.Router;
using System.Configuration;
using System.Web.Caching;
using ECommerce.Entity;

namespace ECommerce.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteConfigurationSection section = (RouteConfigurationSection)ConfigurationManager.GetSection("routeConfig");

            string domainOnlyHost = ConstValue.WebDomainOnlyHost.Trim();

            

            routes.Add("SellerSubDomainPage", new DomainRoute(
                   "{subdomain}." + domainOnlyHost,      
                   "Store/{SellerSysNo}/{PageSysNo}",     
                   new { subdomain = "", controller = "Store", action = "Index", SellerSysNo = "{SellerSysNo}", PageSysNo = "{PageSysNo}", Preview = false }  // Parameter defaults
                   ));

            routes.Add("SellerDomainRoute", new DomainRoute(
                   "{subdomain}." + domainOnlyHost,     
                   "",   
                   new { subdomain = "", controller = "Home", action = "Index" }  
                   ));

            routes.MapRoute(section);

            

             //<route name="Web_Index" url="" controller="Home" action="Index"></route>
            //RouteValueDictionary defaults = new RouteValueDictionary();
            //defaults.Add("controller", "Home");
            //defaults.Add("action", "Index");
            //Route route = new Route("", new MvcRouteHandler());
            //route.Defaults = defaults;
            //routes.Add("Web_Index", route);
            
        }


         
    }
}