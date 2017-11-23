using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECommerce.WebFramework.Error
{
    public abstract class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        protected abstract bool HandleException(Exception ex);
        protected abstract ActionResult BuildAjaxJsonActionResult(Exception ex, bool isLocalRequest);
        protected abstract ActionResult BuildAjaxHtmlActionResult(Exception ex, bool isLocalRequest);
        protected abstract ActionResult BuildAjaxXmlActionResult(Exception ex, bool isLocalRequest);
        protected abstract ActionResult BuildWebPageActionResult(Exception ex, bool isLocalRequest, ExceptionContext filterContext);

        protected virtual ActionResult BuildResult(Exception ex, ExceptionContext filterContext)
        {
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            ActionResult result;
            if (request.IsAjaxRequest())
            {
                string acceptType = request.Headers["Accept"].ToLower();
                if (acceptType.Contains("application/json"))
                {
                    result = BuildAjaxJsonActionResult(ex, request.IsLocal);
                }
                else if (acceptType.Contains("text/html"))
                {
                    result = BuildAjaxHtmlActionResult(ex, request.IsLocal);
                }
                else
                {
                    result = BuildAjaxXmlActionResult(ex, request.IsLocal);
                }
            }
            else
            {
                result = BuildWebPageActionResult(ex, request.IsLocal, filterContext);
            }
            return result;
        }

        protected virtual bool TrySkipIisCustomErrors
        {
            get { return true; }
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            

            filterContext.Result = BuildResult(filterContext.Exception, filterContext);
            filterContext.HttpContext.Response.Clear();
            //不能把状态码设置为500，否则客户端不能显示真实的异常信息
            //filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = TrySkipIisCustomErrors;
            filterContext.ExceptionHandled = HandleException(filterContext.Exception);
        }
    }
}
