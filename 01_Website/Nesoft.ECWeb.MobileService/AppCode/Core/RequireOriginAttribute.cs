using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Nesoft.ECWeb.WebFramework;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace Nesoft.ECWeb.MobileService.Core
{
    public class RequireOriginAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!bool.Parse(ConfigurationManager.AppSettings["MobileApiDebug"]))
            {
                JsonResult jsonResult = new JsonResult();
                jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                if (!filterContext.HttpContext.Request.Headers.AllKeys.Contains("x-newegg-app-id"))
                {
                    jsonResult.Data = new AjaxResult { Success = false, Message = "没有权限访问", Code = 0 };
                    filterContext.Result = jsonResult;
                    return;
                }
                var appId = filterContext.HttpContext.Request.Headers.GetValues("x-newegg-app-id").ToList().FirstOrDefault();
                if (appId != ConfigurationManager.AppSettings["AppId"])
                {
                    jsonResult.Data = new AjaxResult { Success = false, Message = "没有权限访问", Code = 0 };
                    filterContext.Result = jsonResult;
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.RequestType.ToUpper() == "GET")
            {
                JsonResult result = filterContext.Result as JsonResult;
                if (result != null && result.JsonRequestBehavior == JsonRequestBehavior.DenyGet)
                {
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}