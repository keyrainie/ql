using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Shipping;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Shipping;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.WebFramework;
using Nesoft.Utility;

namespace Nesoft.ECWeb.M.Controllers
{
    public class ShoppingPurchaseController : SSLControllerBase
    {
        #region Actions
        /// <summary>
        /// 结算页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();

            if (ShoppingCartIsEmpty(shoppingCart))
            {
                return Redirect(PageHelper.BuildUrl("ShoppingCart"));
            }

            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCart, CurrUser.UserSysNo);

            //重新保存一次购物车cookie,在购物流程中会对购物车中库存数量不足的赠品进行剔除
            ShoppingCart pipelineShoppingCart = (checkOutResult.OrderProcessResult != null
                && checkOutResult.OrderProcessResult.ReturnData != null
                && checkOutResult.OrderProcessResult.ReturnData["ShoppingCart"] != null)
                ? checkOutResult.OrderProcessResult.ReturnData["ShoppingCart"] as ShoppingCart
                : ShoppingStorageManager.GetShoppingCartFromCookie();
            //Key添加CustomerSysNo
            LoginUser userInfo = UserManager.ReadUserInfo();
            shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            ShoppingStorageManager.SaveShoppingCart(pipelineShoppingCart);

            return View(checkOutResult);
        }

        /// <summary>
        /// 不经过购物车直接购买商品，不持久化到cookie中
        /// 多个商品编号用逗号（,）隔开，多个购买数量用逗号（,）隔开
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="quantity">商品数量</param>
        /// <returns></returns>
        public ActionResult DirectCheckout(string productSysNo, string quantity)
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromParam(productSysNo, quantity);

            if (ShoppingCartIsEmpty(shoppingCart))
            {
                return Redirect(PageHelper.BuildUrl("ShoppingCart"));
            }

            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCart, CurrUser.UserSysNo);
            checkOutResult.ShoppingItemParam = productSysNo + "|" + quantity;

            return View("Checkout", checkOutResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AjaxApplyCoupon(CheckOutContext context)
        {
            CheckOutResult checkOutResult = BuildCheckout(context);
            var result = new
            {
                couponCode = checkOutResult.ApplyCouponCode,
                couponName = checkOutResult.ApplyCouponName,
                success = checkOutResult.HasSucceed && string.IsNullOrWhiteSpace(checkOutResult.ApplyedCouponDesc),
                message = !checkOutResult.HasSucceed ? checkOutResult.ErrorMessages[0] : checkOutResult.ApplyedCouponDesc
            };
            return Json(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxBuildCheckOut(CheckOutContext context)
        {
            CheckOutResult checkOutResult = BuildCheckout(context);
            return PartialView("_CheckoutPanel", checkOutResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AjaxSubmitCheckout(CheckOutContext context)
        {
            if (context == null)
            {
                return Json(BuildAjaxErrorObject("无效的请求"));
            }

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            //优先从购买商品参数来构建购物车对象
            if (!String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                String[] shoppingItemParams = context.ShoppingItemParam.Split(new char[] { '|' });
                if (shoppingItemParams.Length == 2)
                {
                    shoppingCart = ShoppingStorageManager.GetShoppingCartFromParam(shoppingItemParams[0], shoppingItemParams[1]);
                }
            }
            //其次从cookie中来构建购物车对象
            if (ShoppingCartIsEmpty(shoppingCart))
            {
                shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            }

            if (ShoppingCartIsEmpty(shoppingCart))
            {
                return Json(new { url = PageHelper.BuildUrl("ShoppingCart") });
            }

            CheckOutResult result = ShoppingFacade.SubmitCheckout(context, shoppingCart, CurrUser.UserSysNo, SOSource.Wechat);
            if (result.HasSucceed)
            {
                //取得订单编号列表
                List<int> soSysNoList = result.OrderProcessResult.ReturnData.SubOrderList.Select(subOrder => subOrder.Value.ID).ToList();
                ShoppingStorageManager.SaveLatestSO(soSysNoList);
                //团购订单数据不是来自cookie，不能清除cookie
                if (!result.OrderProcessResult.ReturnData.SubOrderList.Any(x => x.Value.SOType == (int)SOType.GroupBuy))
                {
                    ShoppingStorageManager.RemoveShoppingCartCookie();
                }
                return Json(new { url = PageHelper.BuildUrl("Thankyou", result.OrderProcessResult.ReturnData.ShoppingCartID) });
            }

            return Json(new { error = true, messages = result.ErrorMessages });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxSubmitShippingAddress(FormCollection form)
        {
            ShippingContactInfo shippingAddressInfo = new ShippingContactInfo();
            if (TryUpdateModel(shippingAddressInfo))
            {
                int receiveAreaIdD;
                if (int.TryParse(Request["District"], out receiveAreaIdD) && receiveAreaIdD > 0)
                {
                    shippingAddressInfo.ReceiveAreaSysNo = receiveAreaIdD;
                    //checkout页面“收货人”和“联系人”写成同一个值
                    shippingAddressInfo.ReceiveContact = shippingAddressInfo.ReceiveName;
                    CustomerShippingAddresssFacade.EditCustomerContactInfo(shippingAddressInfo, CurrUser.UserSysNo);
                    return Json(shippingAddressInfo);
                }
                return Json(BuildAjaxErrorObject("收货区域不能为空"));
            }
            return Json(BuildAjaxErrorObject("无效的请求"));
        }

        [HttpPost]
        public ActionResult AjaxGetShippingAddress()
        {
            int contactAddressSysNo;
            if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
            {
                throw new BusinessException("无效的请求");
            }
            else
            {
                ShippingContactInfo customerShippingAddressInfo = CustomerShippingAddresssFacade.GetCustomerShippingAddress(contactAddressSysNo, CurrUser.UserSysNo);
                if (customerShippingAddressInfo == null)
                {
                    throw new BusinessException("无效的收货地址");
                }
                return PartialView("_ReceiveAddressEdit", customerShippingAddressInfo);
            }
        }

        /// <summary>
        /// 支付、完成订单
        /// </summary>
        /// <param name="cartID">购物车编号</param>
        /// <returns></returns>
        public ActionResult Thankyou(string cartID)
        {
            int shoppingCartID = 0;
            if (!int.TryParse(cartID, out shoppingCartID))
            {
                TempData["ErrorMessage"] = "请输入正确的购物车编号！";
                return View("Error");
            }
            return View(shoppingCartID);
        }

        #endregion

        #region Helper

        private CheckOutResult BuildCheckout(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            //优先从购买商品参数来构建购物车对象
            if (!String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                String[] shoppingItemParams = context.ShoppingItemParam.Split(new char[] { '|' });
                if (shoppingItemParams.Length == 2)
                {
                    shoppingCart = ShoppingStorageManager.GetShoppingCartFromParam(shoppingItemParams[0], shoppingItemParams[1]);
                }
            }
            //其次从cookie中来构建购物车对象
            if (ShoppingCartIsEmpty(shoppingCart))
            {
                shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            }

            var checkOutResult = ShoppingFacade.BuildCheckOut(context, shoppingCart, CurrUser.UserSysNo);

            //重新保存一次购物车cookie,在购物流程中会对购物车中库存数量不足的赠品进行剔除
            //fixbug: 直接购物车下单不应该影响购物车Cookie
            if (String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                ShoppingCart pipelineShoppingCart = (checkOutResult.OrderProcessResult != null
                    && checkOutResult.OrderProcessResult.ReturnData != null
                    && checkOutResult.OrderProcessResult.ReturnData["ShoppingCart"] != null)
                    ? checkOutResult.OrderProcessResult.ReturnData["ShoppingCart"] as ShoppingCart
                    : ShoppingStorageManager.GetShoppingCartFromCookie();

                //Key添加CustomerSysNo
                LoginUser userInfo = UserManager.ReadUserInfo();
                shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
                ShoppingStorageManager.SaveShoppingCart(pipelineShoppingCart);
            }

            return checkOutResult;
        }

        /// <summary>
        /// 检查购物车是否为空 
        /// </summary>
        /// <param name="shoppingCart">购物车对象</param>
        /// <returns></returns>
        private bool ShoppingCartIsEmpty(ShoppingCart shoppingCart)
        {
            if (shoppingCart == null || string.IsNullOrEmpty(shoppingCart.ChannelID)
                || shoppingCart.ShoppingItemGroupList == null
                || shoppingCart.ShoppingItemGroupList.Count == 0
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList == null
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList.Count == 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
