using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Payment;
using ECommerce.Enums;
using ECommerce.Facade.Member;
using ECommerce.Facade.Payment;
using ECommerce.WebFramework;
using ECommerce.Utility;
using System.Text;

namespace ECommerce.UI.Controllers
{
    /// <summary>
    /// 支付
    /// </summary>
    public class PaymentController : SSLControllerBase
    {
        #region 在线支付

        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        public void OnlinePay(int SOSysNo)
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
            Response.Write((new PaymentService()).PaymentCallback(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString), out context));
        }

        /// <summary>
        /// 在线支付前台回调，处理数据
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public ActionResult OnlinePayShowCallback(int PayTypeSysNo)
        {
            var service = new PaymentService();

            service.OnlineShowPayCallbackPreCheck(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString));

            CallbackContext context = null;

            string result = service.PaymentCallback(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString), out context);


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

        /// <summary>
        /// 修改支付方式
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Auth(NeedAuth = true)]
        public ActionResult AjaxChangeOrderPayType()
        {
            int soSysNo = int.Parse(Request.Form["so"]);
            int paySysNo = int.Parse(Request.Form["pay"]);

            LoginUser user = UserMgr.ReadUserInfo();

            if (user == null || user.UserSysNo <= 0)
            {
                throw new BusinessException("请先登录！");
            }

            var order = CustomerFacade.GetCenterSODetailInfo(user.UserSysNo, soSysNo);

            if (order == null || order.SOItemList == null || order.SOItemList.Count == 0)
            {
                throw new BusinessException("订单不存在！");
            }

            if (order.CustomerSysNo != user.UserSysNo)
            {
                throw new BusinessException("不能修他人订单！");
            }

            if (order.Status != Enums.SOStatus.Original)
            {
                throw new BusinessException("订单不是待支付状态！");
            }

            if (order.Payment != null && order.Payment.IsPayWhenRecv == 1)
            {
                throw new BusinessException("货到付款订单不能修改在线支付方式！");
            }

            var netPayInfo = PaymentService.GetCenterDBNetpayBySOSysNo(order.SoSysNo);
            if (netPayInfo != null && netPayInfo.Status > (int)NetPayStatusType.Origin)
            {
                throw new BusinessException("订单已支付！");
            }

            PaymentService.UpdateOrderPayType(soSysNo, paySysNo);

            return Json("s", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 特定接口回调

        /// <summary>
        /// 东方支付退款回调
        /// </summary>
        [Auth(NeedAuth = false)]
        public void EasiPayRefundBack()
        {
            Response.Write((new PaymentService()).EasiPayRefundBack(Request.Form));
        }

        /// <summary>
        /// 东方支付订单申报回调
        /// </summary>
        [Auth(NeedAuth = false)]
        public void EasiPayDeclareBack()
        {
            Response.Write((new PaymentService()).EasiPaySODeclareBack(Request.Form));
        }

        /// <summary>
        /// 东方支付商品申报回调
        /// </summary>
        [Auth(NeedAuth = false)]
        public void EasiPayDeclareProductBack()
        {
            Response.Write((new PaymentService()).EasiPayDeclareProductBack(Request.Form));
        }

        /// <summary>
        /// 财付通退款回调
        /// </summary>
        [Auth(NeedAuth = false)]
        public void TenPayRefundBack()
        {
            Response.Write((new PaymentService()).TenPayRefundBack(Request.Form.Count > 0 ? Request.Form : Request.QueryString));
        }

        #endregion

        #region 支付宝接口回调

        /// <summary>
        /// 在线支付后台回调
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public void AliPayBgCallback(int PayTypeSysNo)
        {
            CallbackContext context = null;
            Response.Write((new PaymentService()).AliPayCallback(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString), out context));
        }

        /// <summary>
        /// 在线支付前台回调，处理数据
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <returns></returns>
        [Auth(NeedAuth = false)]
        public ActionResult AliPayShowCallback(int PayTypeSysNo)
        {
            var service = new PaymentService();

            service.OnlineShowPayCallbackPreCheck(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString));

            CallbackContext context = null;

            string result = service.AliPayCallback(PayTypeSysNo, (Request.Form.Count > 0 ? Request.Form : Request.QueryString), out context);


            return View("Complete", context);
        }

        #endregion
    }
}

