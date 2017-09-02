using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Order;

namespace ECommerce.SOPipeline.Impl
{
    public class InternalHelper
    {

        /// <summary>
        /// 将3层结构OrderInfo中的所有商品转换为平面结构的SOItem列表
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="isIncludeGiftAndAttachment">是否要包含赠品和附件</param>
        /// <param name="isIncludePlusPriceProduct">是否要包含加购商品</param>
        /// <returns></returns>
        internal static List<SOItemInfo> ConvertToSOItemList(OrderInfo orderInfo, bool isIncludeGiftAndAttachment, bool isIncludePlusPriceProduct = false)
        {
            List<SOItemInfo> soItemList = new List<SOItemInfo>();
            //合并Shopping商品组中的商品
            foreach (OrderItemGroup orderItemGroup in orderInfo.OrderItemGroupList)
            {
                foreach (OrderProductItem orderProductItem in orderItemGroup.ProductItemList)
                {
                    SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == orderProductItem.ProductSysNo);
                    if (soItem == null)
                    {
                        soItem = new SOItemInfo();
                        soItem.ProductName = orderProductItem.ProductName;
                        soItem.ProductSysNo = orderProductItem.ProductSysNo;
                        soItem.ProductID = orderProductItem.ProductID;
                        soItem.Quantity = orderProductItem.UnitQuantity * orderItemGroup.Quantity;      
                        soItemList.Add(soItem);
                    }
                    else
                    {
                        soItem.Quantity += orderProductItem.UnitQuantity * orderItemGroup.Quantity;
                    }
                }
            }

            if (isIncludeGiftAndAttachment)
            {
                //合并订单中的赠品
                if (orderInfo.GiftItemList != null)
                {
                    foreach (OrderGiftItem item in orderInfo.GiftItemList)
                    {
                        if (item.ProductSysNo > 0 && item.ParentCount > 0 && item.UnitQuantity > 0)
                        {
                            SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == item.ProductSysNo);
                            if (soItem == null)
                            {
                                soItem = new SOItemInfo();
                                soItem.ProductName = item.ProductName;
                                soItem.ProductSysNo = item.ProductSysNo;
                                soItem.ProductID = item.ProductID;
                                soItem.Quantity = item.UnitQuantity * item.ParentCount;
                                soItemList.Add(soItem);
                            }
                            else
                            {
                                soItem.Quantity += item.UnitQuantity * item.ParentCount;
                            }
                        }
                    }
                }
                //合并订单中的赠品
                if (orderInfo.GiftItemList != null)
                {
                    foreach (OrderAttachment item in orderInfo.AttachmentItemList)
                    {
                        if (item.ProductSysNo > 0 && item.ParentCount > 0 && item.UnitQuantity > 0)
                        {
                            SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == item.ProductSysNo);
                            if (soItem == null)
                            {
                                soItem = new SOItemInfo();
                                soItem.ProductName = item.ProductName;
                                soItem.ProductSysNo = item.ProductSysNo;
                                soItem.ProductID = item.ProductID;
                                soItem.Quantity = item.UnitQuantity * item.ParentCount;
                                soItemList.Add(soItem);
                            }
                            else
                            {
                                soItem.Quantity += item.UnitQuantity * item.ParentCount;
                            }
                        }
                    }
                }

            }

            if (isIncludePlusPriceProduct)
            {
                //合并订单中的加购商品
                if (orderInfo.PlusPriceItemList != null)
                {
                    foreach (OrderGiftItem item in orderInfo.PlusPriceItemList)
                    {
                        if (item.ProductSysNo > 0 && item.UnitQuantity > 0)
                        {
                            SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == item.ProductSysNo);
                            if (soItem == null)
                            {
                                soItem = new SOItemInfo();
                                soItem.ProductName = item.ProductName;
                                soItem.ProductSysNo = item.ProductSysNo;
                                soItem.ProductID = item.ProductID;
                                soItem.Quantity = item.UnitQuantity * item.ParentCount;
                                soItemList.Add(soItem);
                            }
                            else
                            {
                                soItem.Quantity += item.UnitQuantity * item.ParentCount;
                            }
                        }
                    }
                }
            }

            return soItemList;
        }
    }
}
