using System.Web.Mvc;
using Nesoft.ECWeb.WebFramework;

namespace Nesoft.ECWeb.M
{
    /// <summary>
    /// 普通页面Controller从此基类基础
    /// </summary>
    public class WWWControllerBase : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WWWControllerBase()
        {

        }



    }

    /// <summary>
    /// Member账户中心相关页面Controller从此基类基础
    /// </summary>
    [Auth(NeedAuth = true)]
    public class SSLControllerBase : ControllerBase
    {
    }
    public class ControllerBase : Controller
    {
        protected LoginUser CurrUser = UserManager.ReadUserInfo();

        protected ActionResult GotoErrorPage(string msg)
        {
            TempData["ErrorMessage"] = msg;
            return Redirect("/ErrorMsg");
        }
        protected object BuildAjaxErrorObject(string msg)
        {
            return new
            {
                error = true,
                message = msg
            };
        }
    }
}