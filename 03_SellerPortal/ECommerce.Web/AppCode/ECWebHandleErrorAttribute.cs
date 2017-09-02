using System;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.WebFramework.Error;

namespace ECommerce.Web
{
    public class ECWebHandleErrorAttribute : CustomHandleErrorAttribute
    {
        protected override bool HandleException(Exception ex)
        {
            if (!IsBizException(ex))
            {
                if (!(ex is FaultException))
                {
                    ECommerce.Utility.Logger.WriteLog(ex.ToString(), "Portal_Exception");
                }
            }
            return true;
        }

        private bool IsBizException(Exception ex)
        {
            if (ex is BusinessException || ((ex is FaultException) && ((FaultException)ex).Code.Name == "1"))
            {
                return true;
            }
            return false;
        }

        private string GetExceptionInfo(Exception ex, bool isLocalRequest)
        {
            if (IsBizException(ex))
            {
                string errMsg = ex.Message;
                errMsg = LanguageHelper.GetText(errMsg);

                return errMsg;
            }
            if (isLocalRequest)
            {
                if (ex is FaultException)    // throw on wcf service
                {
                    return ((FaultException)ex).Reason.ToString();
                }
                else  // throw on web portal
                {
                    return ex.ToString();
                }
            }
            else
            {
                return LanguageHelper.GetText("系统发生异常，请稍后再试。");
            }
        }


        protected override System.Web.Mvc.ActionResult BuildAjaxJsonActionResult(Exception ex, bool isLocalRequest)
        {
            var data = new
            {
                error = true,
                message = GetExceptionInfo(ex, isLocalRequest)
            };
            JsonResult jr = new JsonResult();
            jr.Data = data;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        protected override System.Web.Mvc.ActionResult BuildAjaxHtmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"service_Error_Message_Panel\">");
            sb.AppendFormat("<input id=\"errorMessage\" type=\"hidden\" value=\"{0}\" />", HttpUtility.HtmlEncode(message));
            sb.Append("</div>");
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/html"
            };
        }

        protected override System.Web.Mvc.ActionResult BuildAjaxXmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<result>");
            sb.AppendLine("<error>true</error>");
            sb.AppendLine("<message>" + message.Replace("<", "&lt;").Replace(">", "&gt;") + "</message>");
            sb.AppendLine("</result>");
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/xml"
            };
        }





        protected override ActionResult BuildWebPageActionResult(Exception ex, bool isLocalRequest, ExceptionContext filterContext)
        {
            string errorStr = GetExceptionInfo(ex, isLocalRequest);
            Exception exception = new Exception(errorStr);

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            exception.HelpLink = IsBizException(ex) ? "BizEx" : "";
            HandleErrorInfo model = new HandleErrorInfo(exception, controller, action);

            return new ViewResult
            {
                ViewName = this.View,
                MasterName = this.Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
        }
    }
}