using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.WebFramework;
using Nesoft.ECWeb.MobileService.Core;
using System.Web.Mvc;


namespace Nesoft.ECWeb.MobileService.Core
{
    public class RequireAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookieValue = UserMgr.ReadUserInfo();
            if (cookieValue != null)
            {
                base.OnActionExecuting(filterContext);
            }
            //if (cookieValue != null
            //    && DateTime.Now < cookieValue.Timeout)
            //{
            //     base.OnActionExecuting(filterContext);
            //}
            else if (cookieValue != null
                && cookieValue.RememberLogin == true)
            {
                UserMgr.RefershLoginDate();
                base.OnActionExecuting(filterContext);
            }
            else
            {
                CookieHelper.SaveCookie("IS_LOGIN", "false");
                JsonResult jsonResult = new JsonResult();
                jsonResult.JsonRequestBehavior=JsonRequestBehavior.AllowGet;
                jsonResult.Data=new AjaxResult { Success = false, Message = LanguageHelper.GetText("没有授权!"), Code = 401 };
                filterContext.Result = jsonResult;
            }
        }
    }
}