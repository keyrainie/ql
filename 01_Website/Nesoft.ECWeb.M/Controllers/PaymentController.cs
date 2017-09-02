using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Payment;
using Nesoft.ECWeb.Facade.Payment;
using Nesoft.ECWeb.WebFramework;

namespace Nesoft.ECWeb.M.Controllers
{
    public class PaymentController : SSLControllerBase
    {
        //
        // GET: /Payment/

        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        public void OnlinePay(string SOSysNo)
        {
            Response.Write((new PaymentService()).Payment(SOSysNo));
        }

        /// <summary>
        /// 在线支付后台回调
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public void OnlinePayBgCallback(int PayTypeSysNo)
        {
            CallbackContext context = null;
            Response.Write((new PaymentService()).PaymentCallback(PayTypeSysNo, Request.Form, out context));
        }

        /// <summary>
        /// 在线支付前台回调，处理数据
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public ActionResult OnlinePayShowCallback(int PayTypeSysNo)
        {
            CallbackContext context = null;
            (new PaymentService()).PaymentCallback(PayTypeSysNo, Request.Form, out context);
            return View("Complete", context);
        }

        /// <summary>
        /// 在线支付前台回调
        /// </summary>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public ActionResult OnlinePayCallback()
        {
            return View("Complete", null);
        }

    }
}
