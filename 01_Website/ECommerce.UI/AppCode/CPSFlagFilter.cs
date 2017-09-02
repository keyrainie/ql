using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.WebFramework;

namespace ECommerce.UI
{
    public class CPSFlagFilter : IActionFilter
    {
        private const string CookieName_AdvEffectMonitor_cm_mmc = "adveffectmonitor.cm_mmc";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Do nothing.
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string queryString_cm_mmc = filterContext.HttpContext.Request.QueryString["cm_mmc"];
            if (!string.IsNullOrWhiteSpace(queryString_cm_mmc))
            {
                CookieHelper.SaveCookie(CookieName_AdvEffectMonitor_cm_mmc, filterContext.HttpContext.Server.UrlEncode(queryString_cm_mmc));
            }
        }
    }
}