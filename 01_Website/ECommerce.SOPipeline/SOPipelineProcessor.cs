using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using System.Transactions;
using ECommerce.Entity.Product;
using ECommerce.Enums;
using System.Web;
using ECommerce.Entity.Shipping;
using ECommerce.SOPipeline.Impl;

namespace ECommerce.SOPipeline
{
    public class SOPipelineProcessor
    {
        #region 根据购物车Cookie构造订单信息

        public static OrderPipelineProcessResult BuildShoppingCart(ShoppingCart shoppingCart)
        {
            #region 1.Check
            OrderPipelineProcessResult checkResult = _BuildShoppingCartBaseCheck(shoppingCart);
            if (!checkResult.HasSucceed)
            {
                return checkResult;
            }
            #endregion

            #region 2.转换为orderinfo
            OrderInfo orderInfo = Convert2OrderInfo(shoppingCart);
            #endregion

            #region 3.订单和促销计算
            OrderPipeline pipeline = OrderPipeline.Create("BuildShoppingCart");
            OrderPipelineProcessResult result = pipeline.Process(orderInfo);
            #endregion

            return result;
        }
        private static OrderPipelineProcessResult _BuildShoppingCartBaseCheck(ShoppingCart shoppingCart)
        {
            OrderPipelineProcessResult result = new OrderPipelineProcessResult();
            if(shoppingCart==null)
            {
                result.HasSucceed = false;
                return result;
            }
                
            if (string.IsNullOrEmpty(shoppingCart.ChannelID)
                || shoppingCart.ShoppingItemGroupList == null
                || shoppingCart.ShoppingItemGroupList.Count == 0
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList == null
                || shoppingCart.ShoppingItemGroupList[0].ShoppingItemList.Count == 0)
            {
                result.HasSucceed = false;
            }
            else
            {
                result.HasSucceed = true;
            }
            return result;
        }

        #endregion

        public static OrderPipelineProcessResult BuildCheckOut(OrderInfo orderInfo)
        {
            OrderPipeline pipeline = OrderPipeline.Create("BuildCheckOut");
            OrderPipelineProcessResult result =  null;
            try
            {
                result = pipeline.Process(orderInfo);
            }
            catch (BusinessException bizEx)
            {
                if (result == null)
                {
                    result = new OrderPipelineProcessResult();
                }
                result.HasSucceed = false;
                result.ErrorMessages.Add(bizEx.Message.ToString());
            }
            catch (Exception ex)
            {
                if (result == null)
                {
                    result = new OrderPipelineProcessResult();
                }
                ECommerce.Utility.Logger.WriteLog(ex.ToString(), "SOPipeline");
                result.HasSucceed = false;
                result.ErrorMessages.Add("系统繁忙，请稍后再试");
            }
            return result;
        }

        public static OrderPipelineProcessResult CreateSO(OrderInfo orderInfo)
        {
            OrderPipelineProcessResult order = BuildCheckOut(orderInfo);

            if (order.HasSucceed)
            {
                OrderPipeline pipeline = OrderPipeline.Create("CreateSO");
                OrderPipelineProcessResult result = null;
                try
                {
                    result = pipeline.Process(order.ReturnData);
                }
                catch (BusinessException bizEx)
                {
                    if (result == null)
                    {
                        result = new OrderPipelineProcessResult();
                    }
                    result.HasSucceed = false;
                    result.ErrorMessages.Add(bizEx.Message.ToString());
                }
                catch(Exception ex)
                {
                    if (result == null)
                    {
                        result = new OrderPipelineProcessResult();
                    }
                    ECommerce.Utility.Logger.WriteLog(ex.ToString(), "SOPipeline");
                    result.HasSucceed = false;
                    result.ErrorMessages.Add("系统繁忙，请稍后再试");
                }
                return result;
            }
            return order;
        }

        /// <summary>
        /// 将购物车对象转换成OrderInfo
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public static OrderInfo Convert2OrderInfo(ShoppingCart shoppingCart)
        {
            OrderInfo orderInfo = new OrderInfo();

            orderInfo.Customer = new CustomerInfo();
            orderInfo.Customer.SysNo = shoppingCart.CustomerSysNo;
            orderInfo.Receipt = new ReceiptInfo();
            orderInfo.GiftItemList = new List<OrderGiftItem>();
            orderInfo.AttachmentItemList = new List<OrderAttachment>();
            orderInfo.DiscountDetailList = new List<OrderItemDiscountInfo>();
            
            orderInfo.LanguageCode = shoppingCart.LanguageCode;
            orderInfo.ChannelID = shoppingCart.ChannelID;
            orderInfo.OrderItemGroupList = new List<OrderItemGroup>();
            foreach (ShoppingItemGroup itemGroup in shoppingCart.ShoppingItemGroupList)
            {
                

                OrderItemGroup orderItemGroup = new OrderItemGroup();
                if (itemGroup.PackageChecked)
                {
                    orderItemGroup.PackageChecked = true;
                }
                orderItemGroup.PackageType = itemGroup.PackageType;
                orderItemGroup.PackageNo = itemGroup.PackageNo;
                orderItemGroup.Quantity = itemGroup.Quantity;
                orderItemGroup.ProductItemList = new List<OrderProductItem>();
                foreach (ShoppingItem item in itemGroup.ShoppingItemList)
                {
                    OrderProductItem orderProductItem = new OrderProductItem();
                    if (item.ProductChecked)
                    {
                        orderProductItem.ProductChecked = true;
                    }
                    orderProductItem.ProductSysNo = item.ProductSysNo;
                    orderProductItem.UnitQuantity = item.UnitQuantity;
                    orderItemGroup.ProductItemList.Add(orderProductItem);
                }
                orderInfo.OrderItemGroupList.Add(orderItemGroup);
            }
            orderInfo["ShoppingCart"] = shoppingCart;

            return orderInfo;
        }

        /// <summary>
        /// 根据配送地区编号和支付方式编号取得所有支持的配送方式列表
        /// </summary>
        /// <param name="addressAreaID">配送地区编号</param>
        /// <param name="paymentCategory">支付类别</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> GetSupportedShipTypeList(int addressAreaID, PaymentCategory? paymentCategory)
        {
            return PipelineDA.GetSupportedShipTypeList(addressAreaID, paymentCategory);
        }

        /// <summary>
        /// 获取商家仓库的所有配送方式
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> Checkout_GetStockShippingType(int sellerSysNo)
        {
            return PipelineDA.Checkout_GetStockShippingType(sellerSysNo);
        }
    }
}
