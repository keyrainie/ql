using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.MobileService.Models.Order;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.Facade.Payment;
using Nesoft.ECWeb.Entity.Payment;

namespace Nesoft.ECWeb.MobileService.Controllers
{
    public class OrderController : BaseApiController
    {
        /// <summary>
        /// 普通商品checkout
        /// </summary>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpGet]
        public JsonResult Checkout(string proSysNos = null, string packSysNos = null)
        {
            if (proSysNos == "null") { proSysNos = null; }
            if (packSysNos == "null") { packSysNos = null; }
            CheckOutResultModel model = OrderManager.Checkout(proSysNos, packSysNos);
            return BulidJsonResult(model);

        }

        /// <summary>
        /// 不经过购物车直接购买商品，不持久化到cookie中
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="quantity">商品数量</param>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpPost]
        public JsonResult DirectCheckout(List<DirectCheckoutReqModel> list)
        {
            CheckOutResultModel model = OrderManager.DirectCheckout(list);

            return BulidJsonResult(model);
        }

        /// <summary>
        /// 构建CheckOut
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpPost]
        public JsonResult BuildCheckOut(CheckOutContext context)
        {
            CheckOutResultModel model = OrderManager.AjaxBuildCheckOut(context);

            return BulidJsonResult(model);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpPost]
        public JsonResult SubmitCheckout(CheckOutContext context)
        {
            return BulidJsonResult(OrderManager.AjaxSubmitCheckout(context));
        }

        /// <summary>
        /// 根据购物车ID获取订单简单信息列表
        /// </summary>
        /// <param name="shoppingCartID">购物车ID</param>
        /// <returns></returns>
        [RequireAuthorize]
        [HttpGet]
        public JsonResult GetOrderListByShoppingCartID(int shoppingCartID)
        {
            return BulidJsonResult(OrderManager.GetOrderListByShoppingCartID(shoppingCartID));
        }


        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        [RequireAuthorize]
        [HttpGet]
        public JsonResult OnlinePay(string SOSysNo)
        {
            //System.Threading.Thread.Sleep(2000);
            return BulidJsonResult(OrderManager.OnlinePay(SOSysNo));
        }

        /// <summary>
        /// 在线支付后台回调
        /// </summary>
        /// <param name="id">支付方式系统编号</param>
        /// <returns></returns>
        public void OnlinePayBgCallback(int id)
        {
            Nesoft.Utility.Logger.WriteLog("后台调用，是否存在日志", "Order", "OnlinePayBgCallback");
            CallbackContext context = null;
            Response.Write((new PaymentService()).PaymentCallback(id, Request.Form, out context));
        }

        /// <summary>
        /// 在线支付前台回调，处理数据
        /// </summary>
        /// <param name="id">支付方式系统编号</param>
        /// <returns></returns>
        public ActionResult OnlinePayShowCallback(int id)
        {
            Nesoft.Utility.Logger.WriteLog("前台调用，是否存在日志", "Order", "OnlinePayBgCallback");
            CallbackContext context = null;
            (new PaymentService()).PaymentCallback(id, Request.Form, out context);
            return View("Complete", context);
        }

        #region 私有方法

        private JsonResult BulidJsonResult(CheckOutResultModel model)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Data = model;
            if (model == null)
            {
                ajaxResult.Success = false;
                ajaxResult.Code = -1;
            }
            else
            {
                ajaxResult.Success = true;
                ajaxResult.Code = 0;
            }

            return BulidJsonResult(ajaxResult);
        }

        private JsonResult BulidJsonResult(AjaxResult ajaxResult)
        {
            JsonResult result = new JsonResult();
            result.Data = ajaxResult;

            return result;
        }

        #endregion
    }
}
