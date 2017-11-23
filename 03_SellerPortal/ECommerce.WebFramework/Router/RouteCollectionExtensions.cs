using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web;
using System.Xml.Linq;
using ECommerce.WebFramework;

namespace ECommerce.WebFramework.Router
{
    public static class RouteCollectionExtensions
    {
        public static void MapRoute(this RouteCollection routes, RouteConfigurationSection section)
        {
            // Manipulate the Ignore List
            foreach (IgnoreItem ignoreItem in section.Ignore)
            {
                RouteValueDictionary ignoreConstraints = new RouteValueDictionary();

                foreach (Constraint constraint in ignoreItem.Constraints)
                    ignoreConstraints.Add(constraint.Name, constraint.Value);

                IgnoreRoute(routes, ignoreItem.Url, ignoreConstraints);
            }

            //Register Area
            foreach (AreaItem area in section.Areas)
            {
                foreach (RoutingItem routingItem in area.Map)
                {
                    RouteValueDictionary defaults = new RouteValueDictionary();
                    RouteValueDictionary constraints = new RouteValueDictionary();
                    List<string> namespaces = new List<string>();

                    if (routingItem.Controller != string.Empty)
                        defaults.Add("controller", routingItem.Controller);

                    if (routingItem.Action != string.Empty)
                        defaults.Add("action", routingItem.Action);

                    if (area.Namespaces != null)
                    {
                        foreach (Namespace ns in area.Namespaces)
                        {
                            namespaces.Add(ns.Name);
                        }
                    }                        

                    foreach (Parameter param in routingItem.Paramaters)
                    {
                        defaults.Add(param.Name, param.Value);
                        if (!string.IsNullOrEmpty(param.Constraint))
                        {
                            constraints.Add(param.Name, param.Constraint);
                        }
                    }

                    MapRoute(routes, routingItem.Name, routingItem.Url, defaults, constraints, area.Name, namespaces.ToArray(),routingItem.NeedSSL);
                }
            }
           
            // Manipluate the Routing Table            
            foreach (RoutingItem routingItem in section.Map)
            {
                RouteValueDictionary defaults = new RouteValueDictionary();
                RouteValueDictionary constraints = new RouteValueDictionary();
                List<string> namespaces = new List<string>();

                if (routingItem.Controller != string.Empty)
                    defaults.Add("controller", routingItem.Controller);

                if (routingItem.Action != string.Empty)
                    defaults.Add("action", routingItem.Action);
               
                foreach (Parameter param in routingItem.Paramaters)
                {
                    defaults.Add(param.Name, param.Value);
                    if (!string.IsNullOrEmpty(param.Constraint))
                    {
                        constraints.Add(param.Name, param.Constraint);
                    }
                }

                MapRoute(routes, routingItem.Name, routingItem.Url, defaults, constraints, string.Empty, new string[] { section.DefaultNamespace.Name },routingItem.NeedSSL);
            }           
        }

        public static void IgnoreRoute
            (RouteCollection routes, string url, RouteValueDictionary constraints)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            IgnoreRoute ignore = new IgnoreRoute(url);
            ignore.Constraints = constraints;
            routes.Add(ignore);
        }

        public static void MapRoute(
            RouteCollection routes,
            string name,
            string url,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            string [] namespaces,
            string needSSL)
        {
            MapRoute(routes, name, url, defaults, constraints, null, namespaces,needSSL);
        }

        public static void MapRoute(
            RouteCollection routes,
            string name,
            string url,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            string area,
            string[] namespaces,
            string needSSL)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
                        
            Route route = new Route(url, new MvcRouteHandler());
            

            route.Defaults = defaults;

            route.Constraints = constraints;
            route.DataTokens = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(needSSL))
            {
                route.DataTokens["NeedSSL"] = needSSL;
            }

            if (!string.IsNullOrEmpty(area))
            {
                route.DataTokens["Area"] = area;
            }
            if (namespaces != null && namespaces.Count() > 0)
            {
                route.DataTokens["Namespaces"] = namespaces;                
                route.DataTokens["UseNamespaceFallback"] = (namespaces == null || namespaces.Length == 0);
            }
            routes.Add(name, route);

 
            #region 进行多语言加工，使其增加对多语言Url的支持：Url插入culture路径，然后对Defaults进行culture定义

            RouteValueDictionary cultureDefaults = defaults;
            cultureDefaults.Add("culture", LanguageHelper.DEFAULT_LANGUAGE_CODE);

            string cultureUrl = "{culture}/" + url;
            string cultureName = "culture_" + name;

            Route cultureRoute = new Route(cultureUrl, new MvcRouteHandler());
            cultureRoute.Defaults = cultureDefaults;
            cultureRoute.Constraints = constraints;
            cultureRoute.DataTokens = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(needSSL))
            {
                route.DataTokens["NeedSSL"] = needSSL;
            }

            if (!string.IsNullOrEmpty(area))
            {
                cultureRoute.DataTokens["Area"] = area;
            }
            if (namespaces != null && namespaces.Count() > 0)
            {
                cultureRoute.DataTokens["Namespaces"] = namespaces;
                cultureRoute.DataTokens["UseNamespaceFallback"] = (namespaces == null || namespaces.Length == 0);
            }
            
            routes.Add(cultureName, cultureRoute);
            #endregion
         
        }
    }


    sealed class IgnoreRoute : Route
    {
        // Methods
        public IgnoreRoute(string url)
            : base(url, new StopRoutingHandler())
        {
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }   
}
