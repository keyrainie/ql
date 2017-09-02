using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.WebFramework;
using Nesoft.ECWeb.Facade.Common;
using Nesoft.ECWeb.Entity;

namespace Nesoft.ECWeb.UI
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
        protected LoginUser CurrUser = UserMgr.ReadUserInfo();

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