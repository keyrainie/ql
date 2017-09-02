using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.LoginReg;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class LoginRegController : BaseApiController
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login(LoginViewModel request)
        {
            return Json(new AjaxResult() { Data = LoginRegManager.CustomerLogin(request), Success = true });
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Register(CustomerRegisterRequestViewModel request)
        {
            return Json(new AjaxResult() { Data = LoginRegManager.CustomerRegister(request), Success = true });
        }
    }
}
