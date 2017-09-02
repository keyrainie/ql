using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Utility;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ECommerce.Web
{
    /// <summary>
    /// 用户权限验证Attribute
    /// </summary>
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 重载OnAuthorization进行权限验证
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            if (!UserAuthHelper.HasLogin())
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    throw new BusinessException("抱歉，当前账号登录信息已过期，请刷新页面后重新登录！");
                }
                else
                {
                    string loginUrl = ConfigurationManager.AppSettings["LoginUrl"];
                    if (string.IsNullOrEmpty(loginUrl))
                    {
                        loginUrl = "/Login?ReturnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri);
                    }
                    else
                    {
                        loginUrl = loginUrl + "?ReturnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri);
                    }
                    filterContext.Result = new RedirectResult(loginUrl, false);
                }
                return;
            }

            var isAllowed = UserAuthHelper.HasAuth(controller, action);
            if (!isAllowed)
            {
                throw new BusinessException("抱歉，您没有该页面的访问和操作权限，请联系系统管理员处理！");

            }
        }
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        const string JSONP_CALLBACK_KEY = "callback";
        const string JSONP_CALLBACK_VALUE = "jsonpcallback";
        /// <summary>
        /// 在执行操作方法前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }


        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        ///  OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request[JSONP_CALLBACK_KEY] != null
                && filterContext.RequestContext.HttpContext.Request[JSONP_CALLBACK_KEY].Trim().ToLower() == JSONP_CALLBACK_VALUE)
            {
                if (filterContext.Result is System.Web.Mvc.JsonResult)
                {
                    JsonResult result = filterContext.Result as JsonResult;
                    if (result != null)
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        filterContext.Result = new JavaScriptResult()
                        {
                            Script = string.Format("{0}({1})", JSONP_CALLBACK_VALUE, serializer.Serialize(result.Data))
                        };
                    }


                }
            }

            base.OnResultExecuted(filterContext);
        }
        /// <summary>
        /// OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }

}
