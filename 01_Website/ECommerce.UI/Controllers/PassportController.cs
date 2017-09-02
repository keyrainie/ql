using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Passport;
using ECommerce.Facade.Passport;
using ECommerce.WebFramework;

namespace ECommerce.UI.Controllers
{
    /// <summary>
    /// 第三方登录
    /// </summary>
    public class PassportController : WWWControllerBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Identify">第三方标识</param>
        public void Login(string Identify)
        {
            string returnUrl = Request.Params["ReturnUrl"];
            returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : HttpUtility.UrlDecode(returnUrl);
            Response.Redirect((new PassportService()).Login(Identify, returnUrl));            
        }

        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="Identify">第三方标识</param>
        public void LoginBack(string Identify)
        {
            NameValueCollection collection = Request.Params;
            var result = (new PassportService()).LoginBack(Identify, collection);
            if (result.ActionType == PassportActionType.Accept)
            {
                LoginUser user = new LoginUser();
                user.UserDisplayName = result.Customer.CustomerName;
                user.UserID = result.Customer.CustomerID;
                user.UserSysNo = result.Customer.SysNo;
                user.RememberLogin = false;
                user.LoginDateText = DateTime.Now.ToString();
                user.TimeoutText = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["LoginTimeout"].ToString())).ToString();

                CookieHelper.SaveCookie<LoginUser>("LoginCookie", user);
                if (string.IsNullOrWhiteSpace(result.ReturnUrl))
                {
                    Response.Redirect("/");
                }
                else
                {
                    Response.Redirect(HttpUtility.UrlDecode(result.ReturnUrl));
                }
            }
        }
    }
}
