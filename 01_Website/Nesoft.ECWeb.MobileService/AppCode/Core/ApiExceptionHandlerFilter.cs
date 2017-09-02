using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Nesoft.ECWeb.WebFramework;
using System.Configuration;
using System.Web.Mvc;
using Nesoft.Utility;

namespace Nesoft.ECWeb.MobileService.Core
{
    public class ApiExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                if ((new HttpException(null, filterContext.Exception).GetHttpCode() == 500) && this.ExceptionType.IsInstanceOfType(filterContext.Exception))
                {
                    string errorMessage = filterContext.Exception.Message;
                    if (!(filterContext.Exception is BusinessException) && !bool.Parse(ConfigurationManager.AppSettings["MobileApiDebug"]))
                    {
                        errorMessage = "系统错误";
                    }

                    JsonResult jsonResult = new JsonResult();
                    jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    jsonResult.Data = new AjaxResult { Success = false, Message = errorMessage, Code = 0 };
                    filterContext.Result = jsonResult;
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    if (filterContext.Exception is BusinessException)
                    {
                        filterContext.HttpContext.Response.StatusCode = 200;
                    }
                    else
                    {
                        filterContext.HttpContext.Response.StatusCode = 500;
                    }
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }
            }
        }
    }
}