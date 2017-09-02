using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.MobileService.Models.Cart;
using Nesoft.Utility;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.UI;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Core;
using Nesoft.ECWeb.Entity.Shopping;
using Nesoft.ECWeb.Facade.Payment;
using System.Text;
using Nesoft.ECWeb.MobileService.Models.Version;
using Nesoft.ECWeb.Entity.Payment;
using System.Collections.Specialized;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class OrderManager
    {
        /// <summary>
        /// 普通商品checkout
        /// </summary>
        /// <returns></returns>
        public static CheckOutResultModel Checkout(string proSysNos = null, string packSysNos = null)
        {
            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
            #region 得到新的shoppingCart
            ShoppingCart shoppingCartNew = ShoppingStorageManager.GetShoppingCartFromCreateNew();
            List<ShoppingItemGroup> ShoppingItem = new List<ShoppingItemGroup>();
            //套餐
            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                (m.PackageType.Equals(1) && m.PackageChecked));
            shoppingCartNew.ShoppingItemGroupList.AddRange(ShoppingItem);
            //单个商品
            ShoppingItem = shoppingCart.ShoppingItemGroupList.FindAll(m =>
                (m.PackageType.Equals(0) && m.ShoppingItemList[0].ProductChecked));
            shoppingCartNew.ShoppingItemGroupList.AddRange(ShoppingItem);
            #endregion
            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                throw new BusinessException("购物车不能为空");
            }
            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;

            LoginUser userInfo = UserMgr.ReadUserInfo();
            shoppingCartNew.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCartNew, userInfo.UserSysNo);

            checkOutResult.PackageTypeGroupList = packSysNos;
            checkOutResult.PackageTypeSingleList = proSysNos;

            CheckOutResultModel model = OrderMapping.MappingCheckOutResult(checkOutResult);

            ShoppingStorageManager.SaveShoppingCart(GetShoppingCart(checkOutResult));

            return model;
        }

        /// <summary>
        /// 不经过购物车直接购买商品，不持久化到cookie中
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="quantity">商品数量</param>
        /// <returns></returns>
        public static CheckOutResultModel DirectCheckout(List<DirectCheckoutReqModel> list)
        {
            if (list == null)
            {
                throw new BusinessException("无效的请求");
            }

            //多个商品编号用逗号（,）隔开，多个购买数量用逗号（,）隔开
            StringBuilder productBuild = new StringBuilder();
            StringBuilder quantityBuild = new StringBuilder();
            foreach (DirectCheckoutReqModel item in list)
            {
                productBuild.Append(item.ProductSysNo);
                productBuild.Append(",");
                quantityBuild.Append(item.Quantity);
                quantityBuild.Append(",");
            }

            string productSysNo = productBuild.ToString().TrimEnd(',');
            string quantity = quantityBuild.ToString().TrimEnd(',');

            ShoppingCart shoppingCart = ShoppingStorageManager.GetShoppingCartFromParam(productSysNo, quantity);

            if (ShoppingCartIsEmpty(shoppingCart))
            {
                throw new BusinessException("无效的请求");
            }

            var checkOutResult = ShoppingFacade.BuildCheckOut(null, shoppingCart, UserMgr.ReadUserInfo().UserSysNo);
            checkOutResult.ShoppingItemParam = productSysNo + "|" + quantity;

            CheckOutResultModel model = OrderMapping.MappingCheckOutResult(checkOutResult);

            return model;
        }

        public static CheckOutResultModel AjaxBuildCheckOut(CheckOutContext context)
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
                shoppingCartNew.ShoppingItemGroupList = shoppingCart.ShoppingItemGroupList.FindAll(m => m.PackageChecked);
            }

            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;

            var checkOutResult = ShoppingFacade.BuildCheckOut(context, shoppingCartNew, UserMgr.ReadUserInfo().UserSysNo);
            CheckOutResultModel model = OrderMapping.MappingCheckOutResult(checkOutResult);

            //重新保存一次购物车cookie,在购物流程中会对购物车中库存数量不足的赠品进行剔除
            //fixbug: 直接购物车下单不应该影响购物车Cookie
            if (String.IsNullOrWhiteSpace(context.ShoppingItemParam))
            {
                ShoppingStorageManager.SaveShoppingCart(GetShoppingCart(checkOutResult));
            }

            return model;
        }

        public static AjaxResult AjaxSubmitCheckout(CheckOutContext context)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Success = false;
            ajaxResult.Code = -1;
            if (context == null)
            {
                ajaxResult.Code = -2;
                ajaxResult.Message = "无效的请求";

                return ajaxResult;
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
                shoppingCartNew.ShoppingItemGroupList = shoppingCart.ShoppingItemGroupList.FindAll(m => m.PackageChecked);
            }

            if (ShoppingCartIsEmpty(shoppingCartNew))
            {
                ajaxResult.Code = -3;
                ajaxResult.Message = "空购物车";

                return ajaxResult;
            }
            shoppingCartNew.OrderDeleteGiftSysNo = shoppingCart.OrderDeleteGiftSysNo;
            shoppingCartNew.OrderSelectGiftSysNo = shoppingCart.OrderSelectGiftSysNo;
            CheckOutResult result = ShoppingFacade.SubmitCheckout(context, shoppingCartNew, UserMgr.ReadUserInfo().UserSysNo, HeaderHelper.GetClientType() == ClientType.IPhone ? SOSource.IPhone : SOSource.Android);
            if (result.ErrorMessages != null && result.ErrorMessages.Count > 0)
            {
                result.ErrorMessages.ForEach(item =>
                {
                    if(!string.IsNullOrWhiteSpace(item))
                        ajaxResult.Message += item;
                });
            }
            if (result.HasSucceed)
            {
                //取得订单编号列表
                List<int> soSysNoList = result.OrderProcessResult.ReturnData.SubOrderList.Select(subOrder => subOrder.Value.ID).ToList();
                ShoppingStorageManager.SaveLatestSO(soSysNoList);
                //团购订单数据不是来自cookie，不能清除cookie
                if (!result.OrderProcessResult.ReturnData.SubOrderList.Any(x => x.Value.SOType == (int)SOType.GroupBuy))
                {
                    //ShoppingStorageManager.RemoveShoppingCartCookie();
                    ShoppingCart leaveshoppingCart = ShoppingStorageManager.GetShoppingCartFromCreateNew();
                    leaveshoppingCart.ShoppingItemGroupList = shoppingCart.ShoppingItemGroupList.FindAll(m => !m.PackageChecked);
                    if (leaveshoppingCart.ShoppingItemGroupList.Count > 0)
                    {
                        foreach (var itemGroup in leaveshoppingCart.ShoppingItemGroupList)
                        {
                            if (itemGroup.PackageType.Equals(0))
                            {
                                foreach (ShoppingItem ProductItem in itemGroup.ShoppingItemList)
                                {
                                    itemGroup.PackageChecked = true;
                                    ProductItem.ProductChecked = true;
                                }
                            }
                            if (itemGroup.PackageType.Equals(1))
                            {
                                itemGroup.PackageChecked = true;
                            }
                        }
                    }
                    LoginUser userInfo = UserMgr.ReadUserInfo();
                    leaveshoppingCart.CustomerSysNo = userInfo == null ? 0 : userInfo.UserSysNo;
                    HttpContext.Current.Request.Cookies.Remove(ShoppingStorageManager.GetConfigCartName());
                    ShoppingStorageManager.SaveShoppingCart(leaveshoppingCart);


                }

                ajaxResult.Code = 0;
                ajaxResult.Success = true;
                ajaxResult.Data = result.OrderProcessResult.ReturnData.ShoppingCartID;
                //var cookie = HttpContext.Current.Request.Cookies.Get(ShoppingStorageManager.GetConfigCartName());
                //ShoppingCart shoppingCart222 = ShoppingStorageManager.GetShoppingCartFromCookieOrCreateNew();
                return ajaxResult;
            }
            

            return ajaxResult;
        }

        /// <summary>
        /// 根据购物车ID获取订单简单信息列表
        /// </summary>
        /// <param name="shoppingCartID">购物车ID</param>
        /// <returns></returns>
        public static AjaxResult GetOrderListByShoppingCartID(int shoppingCartID)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Data = ThankyouFacade.GetOrderListByShoppingCartID(shoppingCartID);
            ajaxResult.Success = true;
            ajaxResult.Code = 0;

            return ajaxResult;
        }

        /// <summary>
        /// 在线支付
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        public static AjaxResult OnlinePay(string SOSysNo)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Data = (new PaymentService()).Payment(SOSysNo);
            ajaxResult.Success = true;
            ajaxResult.Code = 0;

            return ajaxResult;
        }

        public static AjaxResult AliOnlinePay(string SOSysNo, NameValueCollection collection)
        {
            AjaxResult ajaxResult = new AjaxResult();
            PaymentService Pay = new PaymentService();
            string msg = string.Empty;
            int CheckNumble = Pay.AliPayCheck(SOSysNo);
            if (CheckNumble == 1000)
            {
                msg = "订单不存在！";
                ajaxResult.Message = msg;
            }
            else if (CheckNumble == 3000)
            {
                msg = "订单已支付！";
                ajaxResult.Message = msg;
            }
            else if (CheckNumble == 4000)
            {
                CallbackContext context = null;
                msg = (new PaymentService()).AliPayCallback(112, collection, out context);
                ajaxResult.Message = msg;
            }
            ajaxResult.Success = true;
            ajaxResult.Code = 0;

            return ajaxResult;
        }

       
        #region 私有方法

        /// <summary>
        /// 检查购物车是否为空 
        /// </summary>
        /// <param name="shoppingCart">购物车对象</param>
        /// <returns></returns>
        private static bool ShoppingCartIsEmpty(ShoppingCart shoppingCart)
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

        /// <summary>
        /// 根据CheckOutResult获取ShoppingCart
        /// </summary>
        /// <param name="checkOutResult"></param>
        /// <returns></returns>
        private static ShoppingCart GetShoppingCart(CheckOutResult checkOutResult)
        {
            ShoppingCart pipelineShoppingCart = (checkOutResult!=null&&checkOutResult.OrderProcessResult != null
             && checkOutResult.OrderProcessResult.ReturnData != null
             && checkOutResult.OrderProcessResult.ReturnData["pipelineShoppingCart"] != null)
             ? checkOutResult.OrderProcessResult.ReturnData["pipelineShoppingCart"] as ShoppingCart
             : ShoppingStorageManager.GetShoppingCartFromCookie();

            return pipelineShoppingCart;
        }

        #endregion
    }
}