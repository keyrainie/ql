using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ECommerce.Entity.Member;
using ECommerce.Entity.Shipping;
using ECommerce.Entity.Shopping;
using ECommerce.Enums;
using ECommerce.Facade.GroupBuying;
using ECommerce.Facade.Member;
using ECommerce.Facade.Shipping;
using ECommerce.Facade.Shopping;
using ECommerce.SOPipeline;
using ECommerce.WebFramework;
using ECommerce.Utility;
using ECommerce.Entity.Promotion;

namespace ECommerce.UI.Controllers
{
    public class ShoppingPurchaseController : SSLControllerBase
    {
        #region Checkout

        /// <summary>
        /// 普通商品checkout
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            #region 得到新的shoppingCart
            ShoppingCart shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            string PackageTypeSingleList = Request.QueryString["PackageTypeSingle"];
            string PackageTypeGroupList = Request.QueryString["PackageTypeGroup"];
            ShoppingItemGroup ShoppingItem = new ShoppingItemGroup();
            if (!string.IsNullOrEmpty(PackageTypeGroupList))
            {
                string[] array = PackageTypeGroupList.Split(',');
                foreach (var item in array)
                {
                    int sysNo = 0;
                    if (int.TryParse(item, out sysNo))
                    {
                        ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                            (m.PackageType.Equals(1) && m.PackageNo.Equals(sysNo)))[0];
                        shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);
                    }
                }
            }

            if (!string.IsNullOrEmpty(PackageTypeSingleList))
            {
                string[] array = PackageTypeSingleList.Split(',');
                foreach (var item in array)
                {
                    int sysNo = 0;
                    if (int.TryParse(item, out sysNo))
                    {
                        ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                            (m.PackageType.Equals(0) && m.ShoppingItemList[0].ProductSysNo.Equals(sysNo)))[0];
                        shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);
                        
                    }
                }
            }
            #endregion
            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                return Redirect(PageHelper.BuildUrl("ShoppingCartRoute"));
            }
            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;

            LoginUser userInfo = UserMgr.ReadUserInfo();
            shoppingCartNew.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCartNew, CurrUser.UserSysNo);
            checkOutResult.PackageTypeGroupList = PackageTypeGroupList;
            checkOutResult.PackageTypeSingleList = PackageTypeSingleList;

            int bankAccountPoint = 0;
            bankAccountPoint = CookieHelper.GetCookie<int>("BankAccountPoint");//获取网银用户积分
            checkOutResult.Customer.BankAccountPoint = bankAccountPoint;
            ViewBag.IsBankingAccount = bankAccountPoint > 0;//仅当网银积分>0方才显示该区域

            //重新保存一次购物车cookie,在购物流程中会对购物车中库存数量不足的赠品进行剔除
            ShoppingCart pipelineShoppingCart = (checkOutResult.OrderProcessResult != null
                && checkOutResult.OrderProcessResult.ReturnData != null
                && checkOutResult.OrderProcessResult.ReturnData["shoppingCartNew"] != null)
                ? checkOutResult.OrderProcessResult.ReturnData["shoppingCartNew"] as ShoppingCart
                : ShoppingStorageManager.GetShoppingCartFromCookie();

            ShoppingStorageManager.SaveShoppingCart(pipelineShoppingCart);

            checkOutResult = CalcGroupBuyTag(checkOutResult);

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
                throw new BusinessException("无效的请求");
            }
            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCart, CurrUser.UserSysNo);
            checkOutResult.ShoppingItemParam = productSysNo + "|" + quantity;

            checkOutResult = CalcGroupBuyTag(checkOutResult);

            return View("Checkout", checkOutResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxBuildCheckOut(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            ShoppingCart shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            //优先从购买商品参数来构建购物车对象
            if (!String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                String[] shoppingItemParams = context.ShoppingItemParam.Split(new char[] { '|' });
                if (shoppingItemParams.Length == 2)
                {
                    shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromParam(shoppingItemParams[0], shoppingItemParams[1]);
                }
            }
            //其次从cookie中来构建购物车对象
            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
                ShoppingItemGroup ShoppingItem = new ShoppingItemGroup();
                if (!string.IsNullOrEmpty(context.PackageTypeGroupList))
                {
                    string[] array = context.PackageTypeGroupList.Split(',');
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                                (m.PackageType.Equals(1) && m.PackageNo.Equals(sysNo)))[0];
                            shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(context.PackageTypeSingleList))
                {
                    string[] array = context.PackageTypeSingleList.Split(',');
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                                (m.PackageType.Equals(0) && m.ShoppingItemList[0].ProductSysNo.Equals(sysNo)))[0];
                            shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);

                        }
                    }
                }
            }

            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;

            var checkOutResult = ShoppingFacade.BuildCheckOut(context, shoppingCartNew, CurrUser.UserSysNo);

            //重新保存一次购物车cookie,在购物流程中会对购物车中库存数量不足的赠品进行剔除
            //fixbug: 直接购物车下单不应该影响购物车Cookie
            if (String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                ShoppingCart pipelineShoppingCart = (checkOutResult.OrderProcessResult != null
                    && checkOutResult.OrderProcessResult.ReturnData != null
                    && checkOutResult.OrderProcessResult.ReturnData["shoppingCartNew"] != null)
                    ? checkOutResult.OrderProcessResult.ReturnData["shoppingCartNew"] as ShoppingCart
                    : ShoppingStorageManager.GetShoppingCartFromCookie();


                //Key添加CustomerSysNo
                LoginUser userInfo = UserMgr.ReadUserInfo();
                pipelineShoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
                ShoppingStorageManager.SaveShoppingCart(pipelineShoppingCart);
            }

            checkOutResult = CalcGroupBuyTag(checkOutResult);

            return PartialView("_CheckOutEditPanel", checkOutResult);
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
            ShoppingCart shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            //优先从购买商品参数来构建购物车对象
            if (!String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                String[] shoppingItemParams = context.ShoppingItemParam.Split(new char[] { '|' });
                if (shoppingItemParams.Length == 2)
                {
                    shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromParam(shoppingItemParams[0], shoppingItemParams[1]);
                }
            }
            //其次从cookie中来构建购物车对象
            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
                ShoppingItemGroup ShoppingItem = new ShoppingItemGroup();
                if (!string.IsNullOrEmpty(context.PackageTypeGroupList))
                {
                    string[] array = context.PackageTypeGroupList.Split(',');
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                                (m.PackageType.Equals(1) && m.PackageNo.Equals(sysNo)))[0];
                            shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(context.PackageTypeSingleList))
                {
                    string[] array = context.PackageTypeSingleList.Split(',');
                    foreach (var item in array)
                    {
                        int sysNo = 0;
                        if (int.TryParse(item, out sysNo))
                        {
                            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                                (m.PackageType.Equals(0) && m.ShoppingItemList[0].ProductSysNo.Equals(sysNo)))[0];
                            shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);

                        }
                    }
                }
            }

            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                return Json(new { url = PageHelper.BuildUrl("ShoppingCartRoute") });
            }
            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;
            CheckOutResult result = ShoppingFacade.SubmitCheckout(context, shoppingCartNew, CurrUser.UserSysNo, SOSource.None);
            if (result.HasSucceed)
            {
                //取得订单编号列表
                List<int> soSysNoList = result.OrderProcessResult.ReturnData.SubOrderList.Select(subOrder => subOrder.Value.ID).ToList();
                ShoppingStorageManager.SaveLatestSO(soSysNoList);
                //团购订单数据不是来自cookie，不能清除cookie
                if (!result.OrderProcessResult.ReturnData.SubOrderList.Any(x => x.Value.SOType == (int)SOType.GroupBuy || x.Value.SOType == (int)SOType.VirualGroupBuy))
                {
                    //ShoppingStorageManager.RemoveShoppingCartCookie();
                    //移除mini购物车
                    //ShoppingStorageManager.RemoveShoppingCartMiniCookie();

                    //删除套餐
                    if (!string.IsNullOrEmpty(context.PackageTypeGroupList))
                    {
                        string[] array = context.PackageTypeGroupList.Split(',');
                        foreach (var item in array)
                        {
                            int sysNo = 0;
                            if (int.TryParse(item, out sysNo))
                            {
                                shoppingCart.ShoppingItemGroupList =
                                    shoppingCart.ShoppingItemGroupList.FindAll(m
                                        => (m.PackageType.Equals(1) && !m.PackageNo.Equals(sysNo))
                                        || m.PackageType.Equals(0));
                            }
                        }
                    }
                    //删除单个商品
                    if (!string.IsNullOrEmpty(context.PackageTypeSingleList))
                    {
                        string[] array = context.PackageTypeSingleList.Split(',');
                        foreach (var item in array)
                        {
                            int sysNo = 0;
                            if (int.TryParse(item, out sysNo))
                            {
                                shoppingCart.ShoppingItemGroupList =
                                    shoppingCart.ShoppingItemGroupList.FindAll(m
                                        => (m.PackageType.Equals(0) && !m.ShoppingItemList[0].ProductSysNo.Equals(sysNo))
                                        || m.PackageType.Equals(1));

                            }
                        }
                    }
                    //用于计算会员价：
                    LoginUser userInfo = UserMgr.ReadUserInfo();
                    shoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
                    OrderPipelineProcessResult shoppingResult = ShoppingFacade.BuildShoppingCart(shoppingCart);
                    ShoppingCart pipelineShoppingCart = (shoppingResult.ReturnData != null
                        && shoppingResult.ReturnData["ShoppingCart"] != null)
                        ? shoppingResult.ReturnData["ShoppingCart"] as ShoppingCart
                        : new ShoppingCart();
                    shoppingCart.OrderSelectGiftSysNo = pipelineShoppingCart.OrderSelectGiftSysNo;
                    shoppingCart.OrderDeleteGiftSysNo = pipelineShoppingCart.OrderDeleteGiftSysNo;
                    ShoppingStorageManager.SaveShoppingCart(shoppingCart);
                    //将迷你购物车加入Cookie
                    ShoppingStorageManager.SaveShoppingCartMini(ShoppingFacade.BuildMiniShoppingCartFromPipeline(shoppingResult));
                    
                }

                return Json(new { url = PageHelper.BuildUrl("Thankyou", result.OrderProcessResult.ReturnData.ShoppingCartID) });
            }

            return Json(new { errors = result.ErrorMessages });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxSubmitShippingAddress()
        {
            ShippingContactInfo shippingAddressInfo = new ShippingContactInfo()
            {
                IsDefault = true
            };
            if (TryUpdateModel(shippingAddressInfo))
            {
                int receiveAreaIdD;
                if (int.TryParse(Request["District"], out receiveAreaIdD) && receiveAreaIdD > 0)
                {
                    shippingAddressInfo.ReceiveAreaSysNo = receiveAreaIdD;
                    //checkout页面“收货人”和“联系人”写成同一个值
                    shippingAddressInfo.ReceiveContact = shippingAddressInfo.ReceiveName;
                   shippingAddressInfo= CustomerShippingAddresssFacade.EditCustomerContactInfo(shippingAddressInfo, CurrUser.UserSysNo);
                    return Json(shippingAddressInfo);
                }
                throw new BusinessException("收货区域不能为空");
            }
            throw new BusinessException("无效的请求");
        }

        [HttpPost]
        public ActionResult AjaxEditShippingAddress()
        {
            if (Request["cmd"] == null)
            {
                throw new BusinessException("无效的请求");
            }

            int contactAddressSysNo;
            string command = Request["cmd"].Trim().ToUpper();
            switch (command)
            {
                case "GET":

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
                        return PartialView("_ShippingAddressEditPanel", customerShippingAddressInfo);
                    }

                case "DEL":

                    if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
                    {
                        throw new BusinessException("无效的请求");
                    }
                    else
                    {
                        CustomerShippingAddresssFacade.DeleteCustomerContactInfo(contactAddressSysNo, CurrUser.UserSysNo);
                        return Json(new { sysno = contactAddressSysNo });
                    }

                case "DEFAULT":
                    if (Request["id"] == null || !int.TryParse(Request["id"], out contactAddressSysNo))
                    {
                        throw new BusinessException("无效的请求");
                    }
                    else
                    {
                        CustomerShippingAddresssFacade.SetCustomerContactAsDefault(contactAddressSysNo, CurrUser.UserSysNo);
                        return Content("y");
                    }

                default:
                    break;
            }

            return Json("");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxGetShippingAddressList(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }

            CheckOutResult result = new CheckOutResult();
            result.ShippingAddressList = CustomerShippingAddresssFacade.GetCustomerShippingAddressList(CurrUser.UserSysNo);
            if (result.ShippingAddressList != null)
            {
                result.SelShippingAddress = result.ShippingAddressList.Find(x => x.SysNo == context.ShippingAddressID);
                if (result.SelShippingAddress == null)
                {
                    result.SelShippingAddress = result.ShippingAddressList.Find(x => x.IsDefault);
                }
            }
            return PartialView("_ShippingAddressPanel", result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxGetPayAndShipType(CheckOutContext context)
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            ShoppingCart shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }
            ShoppingItemGroup ShoppingItem = new ShoppingItemGroup();
            if (!string.IsNullOrEmpty(context.PackageTypeGroupList))
            {
                string[] array = context.PackageTypeGroupList.Split(',');
                foreach (var item in array)
                {
                    int sysNo = 0;
                    if (int.TryParse(item, out sysNo))
                    {
                        ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                            (m.PackageType.Equals(1) && m.PackageNo.Equals(sysNo)))[0];
                        shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);
                    }
                }
            }

            if (!string.IsNullOrEmpty(context.PackageTypeSingleList))
            {
                string[] array = context.PackageTypeSingleList.Split(',');
                foreach (var item in array)
                {
                    int sysNo = 0;
                    if (int.TryParse(item, out sysNo))
                    {
                        ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                            (m.PackageType.Equals(0) && m.ShoppingItemList[0].ProductSysNo.Equals(sysNo)))[0];
                        shoppingCartNew.ShoppingItemGroupList.Add(ShoppingItem);

                    }
                }
            }
            CheckOutResult result = ShoppingFacade.GetPayAndShipTypeList(context, CurrUser.UserSysNo, shoppingCartNew);
            return PartialView("_PayAndShipTypePanel", result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxEditCustomerAuthentication(CustomerAuthenticationInfo model)
        {
            if (model == null)
            {
                throw new BusinessException("无效的请求");
            }
            var customer = CustomerFacade.GetCustomerInfo(CurrUser.UserSysNo);
            if (customer != null)
            {
                model.CustomerSysNo = customer.SysNo;
                var info = CustomerFacade.SaveCustomerAuthenticationInfo(model);
                return Json(info);
            }
            else
            {
                throw new BusinessException("此用户不存在");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ContentResult AjaxCheckIDCardNumber(string param, string name)
        {
            if (name == "IDCardNumber")
            {
                DateTime birthday;
                string errorMsg;
                if (Request.QueryString["d"] == null ||
                    !DateTime.TryParse(Request.QueryString["d"], out birthday))
                {
                    return Content("出生日期与身份证不一致");
                }
                bool checkResult = CustomerFacade.CheckIDCard(param, birthday, out errorMsg);
                if (!checkResult)
                {
                    return Content(errorMsg);
                }
                return Content("y");
            }
            return Content("y");
        }

        [HttpPost]
        public ActionResult AjaxGetShipTypeList(string paymentcateid, string shipaddrsysno)
        {
            
            PaymentCategory paymentCate;
            int shipAddrSysNo;
            if (!Enum.TryParse(paymentcateid, out paymentCate))
            {
                throw new BusinessException("无效的请求");
            }
            if (!int.TryParse(shipaddrsysno, out shipAddrSysNo))
            {
                throw new BusinessException("无效的请求");
            }

            var shippingAddress = CustomerShippingAddresssFacade.GetCustomerShippingAddress(shipAddrSysNo, CurrUser.UserSysNo);
            if (shippingAddress == null)
            {
                return PartialView("_ShipTypePanel", new List<ShipTypeInfo>(0));
            }
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            CheckOutResult result = ShoppingFacade.GetPayAndShipTypeList(null, CurrUser.UserSysNo, shoppingCart);
            var shipTypeList = result.ShipTypeList;
            //var shipTypeList = ShipTypeFacade.GetSupportedShipTypeList(shippingAddress.ReceiveAreaSysNo, paymentCate);
            return PartialView("_ShipTypePanel", shipTypeList);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxEditPayAndShipType(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }
            PaymentCategory paymentCate;
            int shipTypeID;
            if (!Enum.TryParse(context.PaymentCategoryID, out paymentCate)||
                !int.TryParse(context.ShipTypeID, out shipTypeID))
            {
                throw new BusinessException("无效的请求");
            }
            CustomerShippingAddresssFacade.UpdateCustomerContactInfo(context.ShippingAddressID,
                (int)paymentCate, shipTypeID, CurrUser.UserSysNo);
            return Content("y");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxGetCustomerInvoice(CheckOutContext context)
        {
            if (context == null)
            {
                throw new BusinessException("无效的请求");
            }
            var custInvoiceInfo = CustomerFacade.GetCustomerInvoiceInfo(CurrUser.UserSysNo);
            if (custInvoiceInfo == null)
            {
                custInvoiceInfo = new CustomerInvoiceInfo();
            }
            custInvoiceInfo.NeedInvoice = (context.NeedInvoice == 1);
            return PartialView("_CustomerInvoicePanel", custInvoiceInfo);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxUpdateCustomerInvoice(string invoiceTitle)
        {
            CustomerInvoiceInfo customerInvoiceInfo = new CustomerInvoiceInfo()
            {
                CustomerSysNo = CurrUser.UserSysNo,
                InvoiceTitle = invoiceTitle
            };
            CustomerFacade.UpdateCustomerInvoice(customerInvoiceInfo);
            return Json(customerInvoiceInfo);
        }

        /// <summary>
        /// 检查购物车是否为空 
        /// </summary>
        /// <param name="shoppingCart">购物车对象</param>
        /// <returns></returns>
        private bool ShoppingCartIsEmpty(ShoppingCart shoppingCart)
        {
            if (string.IsNullOrEmpty(shoppingCart.ChannelID)
                || shoppingCart.ShoppingItemGroupList == null
                || shoppingCart.ShoppingItemGroupList.Count == 0
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList == null
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList.Count == 0)
            {
                return true;
            }
            return false;
        }

        private CheckOutResult CalcGroupBuyTag(CheckOutResult checkout)
        {
            if (checkout.OrderProcessResult != null && checkout.OrderProcessResult.ReturnData != null
                && checkout.OrderProcessResult.ReturnData.OrderItemGroupList != null)
            {
                var order = checkout.OrderProcessResult.ReturnData;
                checkout.IsGroupBuyOrder = order.OrderItemGroupList.Exists((x =>
                {

                    if (x.ProductItemList != null)
                    {
                        return x.ProductItemList.Exists(y => y.SpecialActivityType == 1 || y.SpecialActivityType == 3);
                    }
                    return false;
                }));
            }
            return checkout;
        }

        #endregion

        #region Tankyou

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

        public ActionResult ThankyouVirualGroupBuy(string orderSysNo)
        {
            int nOrderSysNo = 0;
            if (!int.TryParse(orderSysNo, out nOrderSysNo))
            {
                TempData["ErrorMessage"] = "请输入正确的订单编号！";
                return View("Error");
            }
            return View(GroupBuyingFacade.GetGroupBuyingPayGetTicketInfo(nOrderSysNo));
        }

        #endregion

        public ActionResult GetCouponPopContent(int MerchantSysNo)
        {
            CouponContentInfo Model = new CouponContentInfo();
            //优惠卷
            LoginUser user = UserMgr.ReadUserInfo();
            List<CustomerCouponInfo> CustomerCouponList = new List<CustomerCouponInfo>();
            if (user != null)
            {
                CustomerCouponList = ShoppingFacade.GetCustomerPlatformCouponCode(user.UserSysNo, MerchantSysNo);
            }
            if (user != null)
            {
                Model.UserSysNo = user.UserSysNo;
                Model.MerchantSysNo = MerchantSysNo;
                Model.customerCouponCodeList = CustomerCouponList;
            }
            PartialViewResult view = PartialView("~/Views/ShoppingPurchase/_PlatformCouponList.cshtml", Model);
            return view;
        }
    }
}
