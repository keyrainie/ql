using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.ControlPannel;
using ECommerce.Web.Utility;
using ECommerce.Utility;
using ECommerce.WebFramework;
using ECommerce.Service.Common;

namespace ECommerce.Web.Controllers
{
    public class HomeController : WWWControllerBase
    {
        public ActionResult Login()
        {
            string returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult UserLogin()
        {
            string returnUrl = Request["ReturnUrl"];
            string currentLoginUserName = Request.Form["UserName"];
            string currentLoginUserPassword = Request.Form["UserPassword"];
            string currentVCode = Request.Form["ValidateCode"];
            string isRemember = Request.Form["Remember"];

            bool isLoginCheckFailed = false;
            if (string.IsNullOrEmpty(currentLoginUserName))
            {
                TempData["LoginError"] = "请输入用户名!";
                isLoginCheckFailed = true;
            }
            else if (string.IsNullOrEmpty(currentLoginUserPassword))
            {
                TempData["LoginName"] = currentLoginUserName;
                TempData["LoginError"] = "请输入登录密码!";
                isLoginCheckFailed = true;
            }
            else if (string.IsNullOrEmpty(currentVCode))
            {
                TempData["LoginName"] = currentLoginUserName;
                TempData["LoginError"] = "请输入登录验证码!";
                isLoginCheckFailed = true;
            }
            else if (string.IsNullOrEmpty(CookieHelper.GetCookie<string>("VerifyCode")) || currentVCode.Trim() != CookieHelper.GetCookie<string>("VerifyCode"))
            {
                TempData["LoginName"] = currentLoginUserName;
                TempData["LoginError"] = "验证码输入错误或已过期，请重新输入!";
                isLoginCheckFailed = true;
            }
            if (isLoginCheckFailed)
            {
                TempData["ReturnUrl"] = returnUrl;
                return RedirectToAction("Login", "Home", !string.IsNullOrEmpty(returnUrl) ? new { returnUrl = returnUrl } : null);
            }
            try
            {
                UserInfo loginUser = UserAuthHelper.Login(string.Empty, currentLoginUserName, currentLoginUserPassword, string.Empty);
                if (loginUser != null && loginUser.SysNo > 0)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Main");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    TempData["LoginName"] = currentLoginUserName;
                    TempData["LoginError"] = "登录失败，请检查用户名和密码是否正确!";
                    TempData["ReturnUrl"] = returnUrl;
                    return RedirectToAction("Login", "Home", !string.IsNullOrEmpty(returnUrl) ? new { returnUrl = returnUrl } : null);
                }
            }

            catch
            {
                TempData["ReturnUrl"] = returnUrl;
                TempData["LoginError"] = "登录失败,系统异常,请联系管理员";
                return RedirectToAction("Login", "Home", !string.IsNullOrEmpty(returnUrl) ? new { returnUrl = returnUrl } : null);
            }



        }

        public ActionResult Logout()
        {
            if (UserAuthHelper.HasLogin())
            {
                UserAuthHelper.Logout();
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult LoginValidationCode()
        {
            string code = ValidationCodeHelper.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = ValidationCodeHelper.CreateValidateGraphic(code, 34);
            CookieHelper.SaveCookie<string>("VerifyCode", code.Trim());
            return File(bytes, @"image/jpeg");
        }

        public ActionResult SendLoginValidSMS(FormCollection form)
        {

            //step one :get customer cellphone from db
            string customerID = form["CustomerID"];
            if (string.IsNullOrEmpty(customerID))
            {
                return Json("用户名未提供，发送验证码失败。", JsonRequestBehavior.AllowGet);
            }

            string vendorUserCellphone =CommonService.GetVendorCellPhone(customerID);

            //step two :send sms
            if (string.IsNullOrEmpty(vendorUserCellphone))
            {
                return Json("账户未绑定手机或是未完成验证绑定，获取验证码失败。", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string code = ValidationCodeHelper.CreateValidateCode(5);
                Session["ValidateCode"] = code;
                CookieHelper.SaveCookie<string>("VerifyCode", code.Trim());
                string SMSContent = string.Format(AppSettingManager.GetSetting("SMSTemplate", "CreateConfirmPhoneCode"),
                    DateTime.Now.ToString("MM月dd日 HH:mm"), code);
                if (SMSService.SendSMS(vendorUserCellphone, SMSContent))
                    return Json("s", JsonRequestBehavior.AllowGet);
            }
            return Json("服务器忙,稍后重试", JsonRequestBehavior.AllowGet);
        }

    }
}
