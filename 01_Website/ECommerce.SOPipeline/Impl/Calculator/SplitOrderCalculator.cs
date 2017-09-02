using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Common;
using ECommerce.Entity;
using ECommerce.Entity.Product;
using ECommerce.Entity.Shopping;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 拆单计算逻辑
    /// </summary>
    public class SplitOrderCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo orderInfo)
        {
            Dictionary<string, OrderInfo> subOrderInfoList = new Dictionary<string, OrderInfo>();

            #region 【 将加够商品合并至主商品 】
            
            foreach (var plusPriceItem in orderInfo.PlusPriceItemList)
            {
                if (plusPriceItem.IsSelect)
                    orderInfo.OrderItemGroupList.Add(PlusPriceConvertToProduct(plusPriceItem, orderInfo.Customer.SysNo));
            }

            #endregion

            #region 【 先处理主商品 】

            List<OrderProductItem> masterItemList = new List<OrderProductItem>();

            if (orderInfo.OrderItemGroupList != null)
            {
                foreach (var itemGroup in orderInfo.OrderItemGroupList)
                {
                    if (itemGroup.ProductItemList != null)
                    {
                        itemGroup.ProductItemList.ForEach(masterItem =>
                        {
                            //将主商品拆分到最细粒度，即一个商品一个OrderProductItem
                            for (int i = 0; i < masterItem.UnitQuantity * itemGroup.Quantity; i++)
                            {
                                OrderProductItem cloneItem = (OrderProductItem)masterItem.Clone();
                                cloneItem.UnitQuantity = 1;
                                //设置商品上的套餐号和套餐类型，很别扭,先这样吧，下个项目再修改 at 【2014-08-08 13:25:43】 
                                cloneItem["PackageNo"] = itemGroup.PackageNo;
                                cloneItem["PackageType"] = itemGroup.PackageType;
                                masterItemList.Add(cloneItem);
                            }
                        });
                    }
                }
            }

            //先初步按照 供应商+仓库+存储运输方式 拆分
            Dictionary<string, List<OrderProductItem>> dicMasterItemPreSplitResult = GetMainItemSplitResult(masterItemList);

            Dictionary<string, List<OrderProductItem>> dicSubOrderMasterItemResult = new Dictionary<string, List<OrderProductItem>>();

            //取得每单最大限制金额
            Dictionary<string, decimal> maxPerOrderAmountConfig = GetMaxPerOrderAmountConfig();

            foreach (var kvs in dicMasterItemPreSplitResult)
            {
                kvs.Value.Sort((item1, item2) =>
                {
                    return (item1.TotalSalePrice).CompareTo(item2.TotalSalePrice);
                });

                //单个订单金额拆分规则
                string warehouseCountryCode = string.IsNullOrEmpty(kvs.Value[0].WarehouseCountryCode) ? string.Empty :
                                                                                                         kvs.Value[0].WarehouseCountryCode;

                decimal maxPerOrderAmount = decimal.MaxValue;
                if (maxPerOrderAmountConfig.ContainsKey(warehouseCountryCode.ToUpper().Trim()))
                {
                    maxPerOrderAmount = maxPerOrderAmountConfig[warehouseCountryCode.ToUpper().Trim()];
                }

                decimal shoppingItemPriceSum = 0;
                string thisKey = string.Empty;
                List<OrderProductItem> thisGroupMasterItemList = new List<OrderProductItem>();

                //根据每单最大购买金额进行一次拆单
                foreach (OrderProductItem masterItem in kvs.Value)
                {
                    decimal currentItemAmtSum = masterItem.TotalSalePrice;
                    //判断当前订单金额是否大于出库地区每单最大金额
                    if (shoppingItemPriceSum + currentItemAmtSum > maxPerOrderAmount)
                    {
                        //如果大于出库地区每单最大金额，并且主商品分组为空的话，那么当前单商品金额就已经超出
                        //最大购买金额，单商品自成一单
                        if (thisGroupMasterItemList.Count <= 0)
                        {
                            shoppingItemPriceSum = masterItem.TotalSalePrice;
                            thisGroupMasterItemList.Add(masterItem);

                            //根据订单金额重新计算一次key值
                            thisKey = GetSubOrderKey(kvs.Key, shoppingItemPriceSum, dicSubOrderMasterItemResult);
                            dicSubOrderMasterItemResult[thisKey] = thisGroupMasterItemList;

                            shoppingItemPriceSum = currentItemAmtSum;
                            thisGroupMasterItemList = new List<OrderProductItem>();
                        }
                        else
                        {
                            //主商品分组合并组成一个子单，清空之前的主商品分组，并且将当前主商品加入到分组中
                            //根据订单金额重新计算一次key值
                            thisKey = GetSubOrderKey(kvs.Key, shoppingItemPriceSum, dicSubOrderMasterItemResult);
                            dicSubOrderMasterItemResult[thisKey] = thisGroupMasterItemList;

                            shoppingItemPriceSum = currentItemAmtSum;
                            thisGroupMasterItemList = new List<OrderProductItem>() { masterItem };
                        }
                    }
                    else
                    {
                        //金额没有超过最大购买金额，将当前主商品加入到分组中
                        shoppingItemPriceSum += masterItem.TotalSalePrice;
                        thisGroupMasterItemList.Add(masterItem);
                    }
                }
                //处理剩下的主商品分组，这些主商品组成一个子单
                if (thisGroupMasterItemList.Count > 0)
                {
                    thisKey = GetSubOrderKey(kvs.Key, shoppingItemPriceSum, dicSubOrderMasterItemResult);
                    dicSubOrderMasterItemResult[thisKey] = thisGroupMasterItemList;
                }
            }

            foreach (var kvs in dicSubOrderMasterItemResult)
            {
                OrderInfo subOrderInfo = (OrderInfo)orderInfo.Clone();
                //设置订单的默认出库仓
                subOrderInfo["WarehouseNumber"] = kvs.Value.FirstOrDefault().WarehouseNumber;
                subOrderInfo["WarehouseName"] = kvs.Value.FirstOrDefault().WarehouseName;
                subOrderInfo["WarehouseCountryCode"] = kvs.Value.FirstOrDefault().WarehouseCountryCode;
                subOrderInfo["MerchantSysNo"] = kvs.Value.FirstOrDefault().MerchantSysNo;
                subOrderInfo["StoreType"] = kvs.Value.FirstOrDefault()["ProductStoreType"];

                subOrderInfo.OrderItemGroupList = new List<OrderItemGroup>();
                subOrderInfo.GiftItemList = new List<OrderGiftItem>();
                subOrderInfo.AttachmentItemList = new List<OrderAttachment>();

                OrderItemGroup itemGroup = new OrderItemGroup();
                //商品在拆单后，购买行为上的分组失去意义，设置组上的购买数量为1，商品的上UnitQuantity及时真实的商品购买数量
                itemGroup.Quantity = 1;
                itemGroup.ProductItemList = new List<OrderProductItem>();

                kvs.Value.ForEach(masterItem =>
                {
                    //itemGroup.ProductItemList.Add(masterItem);
                    //由于前面将主商品拆分到最细，这里需要将主商品合并
                    if (!itemGroup.ProductItemList.Exists(x =>
                    {
                        if (x.ProductSysNo == masterItem.ProductSysNo)
                        {
                            x.UnitQuantity += masterItem.UnitQuantity;
                            return true;
                        }
                        return false;
                    }))
                    {
                        itemGroup.ProductItemList.Add(masterItem);
                    }
                });
                subOrderInfo.OrderItemGroupList.Add(itemGroup);

                subOrderInfoList.Add(kvs.Key, subOrderInfo);
            }

            #endregion

            #region 【 处理赠品和附件 】

            List<OrderItem> giftAndAccessoryList = new List<OrderItem>();

            if (orderInfo.GiftItemList != null)
            {
                orderInfo.GiftItemList.ForEach(gift =>
                {
                    if (!giftAndAccessoryList.Exists(g =>
                    {
                        OrderGiftItem giftItem = (OrderGiftItem)g;
                        //赠品合并增加一个赠品活动编号维度,客户端考虑对相同商品编号的赠品合并
                        if (giftItem.ProductSysNo == gift.ProductSysNo &&
                            giftItem.ActivityNo == gift.ActivityNo)
                        {
                            g.UnitQuantity += gift.UnitQuantity * gift.ParentCount;
                            return true;
                        }
                        return false;
                    }))
                    {
                        OrderGiftItem cloneGift = (OrderGiftItem)gift.Clone();
                        cloneGift.UnitQuantity = gift.UnitQuantity * gift.ParentCount;
                        //合并后赠品的组数量失去意义，设置赠品的组购买数量为1
                        cloneGift.ParentCount = 1;
                        giftAndAccessoryList.Add(cloneGift);
                    }
                });
            }

            if (orderInfo.AttachmentItemList != null)
            {
                orderInfo.AttachmentItemList.ForEach(attachment =>
                {
                    if (!giftAndAccessoryList.Exists(a =>
                    {
                        if (a.ProductSysNo == attachment.ProductSysNo)
                        {
                            a.UnitQuantity += attachment.UnitQuantity * attachment.ParentCount;
                            return true;
                        }
                        return false;
                    }))
                    {
                        OrderAttachment cloneAttachment = (OrderAttachment)attachment.Clone();
                        cloneAttachment.UnitQuantity = attachment.UnitQuantity * attachment.ParentCount;
                        //合并后附件的组数量失去意义，设置附件的组购买数量为1
                        cloneAttachment.ParentCount = 1;
                        giftAndAccessoryList.Add(cloneAttachment);
                    }

                });
            }

            Dictionary<string, List<OrderItem>> dicGiftAndAccessorySplitResult = GetItemSplitResult(giftAndAccessoryList);

            //赠品和附件不会影响订单金额，将按照商家+仓库+存储运输方式拆好的单和前面主商品拆好的单进行合并
            foreach (var kvs in dicGiftAndAccessorySplitResult)
            {
                OrderInfo theSubOrderInfo = null;
                //赠品（附件）在新的子单中
                bool giftOrAccessoryInNewSubOrder = false;

                foreach (string thisKey in subOrderInfoList.Keys)
                {
                    //thisKey:vendorSysno|warehouseNumber|storeType|amt-1....n
                    string prefixKey = thisKey.Substring(0, thisKey.LastIndexOf('|'));
                    prefixKey = prefixKey.Substring(0, prefixKey.LastIndexOf('|'));

                    //kvs.Key: vendorSysno|warehouseNumber|storeType
                    string newKey = kvs.Key.Substring(0, kvs.Key.LastIndexOf('|'));
                    if (newKey.ToUpper().Trim() == prefixKey.ToUpper().Trim())
                    {
                        theSubOrderInfo = subOrderInfoList[thisKey];
                        break;
                    }
                }

                if (theSubOrderInfo == null)
                {
                    giftOrAccessoryInNewSubOrder = true;
                    theSubOrderInfo = (OrderInfo)orderInfo.Clone();
                    //设置订单的默认出库仓
                    theSubOrderInfo["WarehouseNumber"] = kvs.Value.FirstOrDefault().WarehouseNumber;
                    theSubOrderInfo["WarehouseName"] = kvs.Value.FirstOrDefault().WarehouseName;
                    theSubOrderInfo["WarehouseCountryCode"] = kvs.Value.FirstOrDefault().WarehouseCountryCode;
                    theSubOrderInfo["MerchantSysNo"] = kvs.Value.FirstOrDefault().MerchantSysNo;
                    theSubOrderInfo["StoreType"] = kvs.Value.FirstOrDefault()["ProductStoreType"];

                    theSubOrderInfo.OrderItemGroupList = new List<OrderItemGroup>();
                    theSubOrderInfo.GiftItemList = new List<OrderGiftItem>();
                    theSubOrderInfo.AttachmentItemList = new List<OrderAttachment>();

                    subOrderInfoList.Add(kvs.Key, theSubOrderInfo);
                }

                foreach (var orderItem in kvs.Value)
                {
                    if (orderItem is OrderGiftItem)
                    {
                        OrderGiftItem giftItem = orderItem as OrderGiftItem;
                        //Modified by PoseidonTong at [2014-08-11 12:33:01]
                        //赠品在和主商品同仓的前提下，优先和关联的主商品拆在同一个订单
                        if (!giftOrAccessoryInNewSubOrder && giftItem.ParentProductSysNo > 0)
                        {
                            var giftMasterProductSubOrderInfos = subOrderInfoList.Select(_ => _.Value)
                                            .Where(x => x.OrderItemGroupList != null &&
                                                        x.OrderItemGroupList.Exists(y =>
                                                                                        y.ProductItemList != null &&
                                                                                        y.ProductItemList.Exists(p => p.ProductSysNo == giftItem.ParentProductSysNo)
                                                                                    ));

                            if (giftMasterProductSubOrderInfos.Count() > 0)
                            {
                                //取满足条件的第一个子单，考虑同一个主商品由于金额
                                //限制拆分成了多个子单的情况，这里将赠品放到第一个主单中
                                giftMasterProductSubOrderInfos.First().GiftItemList.Add(giftItem);
                                continue;
                            }
                        }
                        theSubOrderInfo.GiftItemList.Add(giftItem);
                    }
                    else if (orderItem is OrderAttachment)
                    {
                        OrderAttachment attrItem = orderItem as OrderAttachment;
                        //Modified by PoseidonTong at [2014-08-11 12:33:01]
                        //附件在和主商品同仓的前提下，优先和关联的主商品拆在同一个订单
                        if (!giftOrAccessoryInNewSubOrder && attrItem.ParentProductSysNo > 0)
                        {
                            var attMasterProductSubOrderInfos = subOrderInfoList.Select(_ => _.Value)
                                            .Where(x => x.OrderItemGroupList != null &&
                                                        x.OrderItemGroupList.Exists(y =>
                                                                                        y.ProductItemList != null &&
                                                                                        y.ProductItemList.Exists(p => p.ProductSysNo == attrItem.ParentProductSysNo)
                                                                                    ));

                            if (attMasterProductSubOrderInfos.Count() > 0)
                            {
                                //取满足条件的第一个子单，考虑同一个主商品由于金额
                                //限制拆分成了多个子单的情况，这里将附件放到第一个主单中
                                attMasterProductSubOrderInfos.First().AttachmentItemList.Add(attrItem);
                                continue;
                            }
                        }
                        theSubOrderInfo.AttachmentItemList.Add(attrItem);
                    }
                }
            }

            #endregion

            #region 【 设置子单类型和商家模式 】

            foreach (var kvs in subOrderInfoList)
            {
                OrderInfo subOrderInfo = kvs.Value;
                #region 设置商家模式
                ECommerce.Entity.Product.VendorInfo vendorInfo = PipelineDA.GetVendorInfo((int)subOrderInfo["MerchantSysNo"]);
                subOrderInfo["ShippingType"] = vendorInfo.ShippingType;
                subOrderInfo["StockType"] = vendorInfo.StockType;
                subOrderInfo["InvoiceType"] = vendorInfo.InvoiceType;
                subOrderInfo["SellerType"] = vendorInfo.SellerType;
                subOrderInfo["VendorName"] = vendorInfo.VendorName;
                #endregion

                #region 设置子单类型
                subOrderInfo.SOType = (int)SOType.Normal;
                ////团购商品不能和普通商品混合下单，所以只要订单中有团购商品，那么订单一定就是团购订单
                //if (subOrderInfo.OrderItemGroupList != null)
                //{
                //    if (subOrderInfo.OrderItemGroupList.Exists(itemGroup =>
                //    {
                //        if (itemGroup.ProductItemList != null)
                //        {
                //            return itemGroup.ProductItemList.Exists(product =>
                //            {
                //                if (product.SpecialActivityType == 1 || product.SpecialActivityType == 3)
                //                {
                //                    //记录下团购编号
                //                    subOrderInfo["ReferenceSysNo"] = product.SpecialActivitySysNo;
                //                    return true;
                //                }
                //                return false;
                //            });
                //        }
                //        return false;
                //    }))
                //    {
                //        subOrderInfo.SOType = (int)SOType.GroupBuy;
                //    }
                //    else
                //    {
                //        subOrderInfo.SOType = (int)SOType.Normal;
                //    }
                //}
                //else
                //{
                //    subOrderInfo.SOType = (int)SOType.Normal;
                //}
                #endregion
            }

            #endregion

            orderInfo.SubOrderList = subOrderInfoList;
        }

        private string GetSubOrderKey<T>(string orignalKey, decimal itemAmtSum, Dictionary<string, List<T>> orignalItemSplitResult)
        {
            int index = 0;
            string key = string.Format("{0}|{1}", orignalKey, (int)itemAmtSum);
            foreach (var kvs in orignalItemSplitResult)
            {
                //kvs.Key    vendorSysno|warehouseNumber|storeType|amt-1....n
                string[] keyParts = kvs.Key.Split('-');
                string keyPrefix = keyParts[0];
                if (keyPrefix.ToUpper() == key.ToUpper())
                {
                    if (keyParts.Length > 1)
                    {
                        index = Math.Max(index, Convert.ToInt32(keyParts[1]) + 1);
                    }
                    else
                    {
                        index = 1;
                    }
                }
            }
            if (index > 0)
            {
                key = string.Format("{0}-{1}", key, index);
            }

            if (!orignalItemSplitResult.ContainsKey(key))
            {
                orignalItemSplitResult.Add(key, new List<T>());
            }

            return key;
        }

        /// <summary>
        /// 主商品拆单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private Dictionary<string, List<T>> GetMainItemSplitResult<T>(List<T> itemList)
            where T : OrderItem
        {
            Dictionary<string, List<T>> dicItemSplitResult = new Dictionary<string, List<T>>();

            if (itemList != null)
            {
                itemList.ForEach(item =>
                {
                    //string key = string.Format("{0}|{1}|{2}", item.MerchantSysNo
                    //                                                , item.WarehouseNumber
                    //                                                , (int)item["ProductStoreType"]);

                    //本期不按照存储运输方式拆单
                    string key = string.Format("{0}|{1}|{2}", item.MerchantSysNo
                                                , item.WarehouseNumber
                                                , 0);

                    if (!dicItemSplitResult.ContainsKey(key))
                    {
                        dicItemSplitResult.Add(key, new List<T>());
                    }
                    dicItemSplitResult[key].Add(item);
                });
            }

            return dicItemSplitResult;
        }

        /// <summary>
        /// 赠品附件拆单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private Dictionary<string, List<T>> GetItemSplitResult<T>(List<T> itemList)
            where T : OrderItem
        {
            Dictionary<string, List<T>> dicItemSplitResult = new Dictionary<string, List<T>>();

            if (itemList != null)
            {
                itemList.ForEach(item =>
                {
                    //string key = string.Format("{0}|{1}|{2}", item.MerchantSysNo
                    //                                                , item.WarehouseNumber
                    //                                                , (int)item["ProductStoreType"]);
                    //本期不按照存储运输方式拆单
                    string key = string.Format("{0}|{1}|{2}", item.MerchantSysNo
                                                                    , item.WarehouseNumber
                                                                    , 0);

                    if (!dicItemSplitResult.ContainsKey(key))
                    {
                        dicItemSplitResult.Add(key, new List<T>());
                    }

                    dicItemSplitResult[key].Add(item);
                });
            }

            return dicItemSplitResult;
        }

        /// <summary>
        /// 取得每单最大限制金额，默认指定HK的最大限制金额为800，JP的最大限制金额为1000
        /// 可以通过web.config的配置项“MaxPerOrderAmount”进行改写
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, decimal> GetMaxPerOrderAmountConfig()
        {
            List<string> allStockCountryCode = PipelineDA.GetAllStockCountryCode();
            Dictionary<string, decimal> config = allStockCountryCode.ToDictionary(k => k, v => decimal.MaxValue);

            string maxPerOrderAmountConfig = ConstValue.MaxPerOrderAmount;
            if (!string.IsNullOrWhiteSpace(maxPerOrderAmountConfig))
            {
                string[] parts = maxPerOrderAmountConfig.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i = i + 2)
                {
                    string key = parts[i];
                    decimal maxPerOrderAmt;
                    if (config.ContainsKey(key) && decimal.TryParse(parts[i + 1], out maxPerOrderAmt))
                    {
                        config[key] = maxPerOrderAmt;
                    }
                }
            }
            return config;
        }

        /// <summary>
        /// 加够商品转换为主商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="customerSysNo">用户编号</param>
        /// <returns></returns>
        private OrderItemGroup PlusPriceConvertToProduct(OrderGiftItem plusPrice, int customerSysNo)
        {
            OrderItemGroup result = new OrderItemGroup();
            OrderProductItem productItem = new OrderProductItem();
            productItem.ProductSysNo = plusPrice.ProductSysNo;

            //商品基础信息
            ProductBasicInfo basicInfo = PipelineDA.GetProductBasicInfoBySysNo(productItem.ProductSysNo);
            //商品备案信息
            ProductEntryInfo entryInfo = PipelineDA.GetProductEntryInfoBySysNo(productItem.ProductSysNo);
            basicInfo.ProductEntryInfo = entryInfo;
            productItem["ProductStoreType"] = (int)entryInfo.ProductStoreType;
            //商品备货时间                    
            productItem["LeadTimeDays"] = basicInfo.ProductEntryInfo.LeadTimeDays;

            //商品销售信息
            ProductSalesInfo salesInfo = PipelineDA.GetProductSalesInfoBySysNo(productItem.ProductSysNo);
            productItem.ProductID = basicInfo.Code;
            productItem.ProductName = basicInfo.ProductName;
            productItem.Weight = basicInfo.Weight;
            productItem["ProductTitle"] = basicInfo.ProductTitle;
            productItem.DefaultImage = basicInfo.DefaultImage;
            productItem.UnitMarketPrice = salesInfo.MarketPrice;
            productItem.UnitCostPrice = salesInfo.UnitCostPrice;
            productItem.UnitSalePrice = plusPrice.UnitSalePrice;
            productItem.UnitRewardedPoint = 0;
            //productItem["TariffRate"] = basicInfo.ProductEntryInfo.TariffRate;
            productItem.TotalInventory = salesInfo.OnlineQty;
            productItem.MerchantSysNo = basicInfo.VendorSysno;
            productItem.MerchantName = basicInfo.VendorInfo.VendorName;
            result.MerchantSysNo = basicInfo.VendorSysno;
            result.MerchantName = basicInfo.VendorInfo.VendorName;
            productItem["ProductStatus"] = (int)basicInfo.ProductStatus;
            productItem["Warranty"] = basicInfo.Warranty;
            productItem.WarehouseNumber = plusPrice.WarehouseNumber;
            productItem.WarehouseName = plusPrice.WarehouseName;
            productItem.WarehouseCountryCode = plusPrice.WarehouseCountryCode;            

            salesInfo.MinCountPerOrder = salesInfo.OnlineQty < productItem.UnitQuantity ? salesInfo.OnlineQty : productItem.UnitQuantity;
            salesInfo.MaxCountPerOrder = salesInfo.OnlineQty < productItem.UnitQuantity ? salesInfo.OnlineQty : productItem.UnitQuantity;

            productItem["MinCountPerOrder"] = productItem.UnitQuantity;
            productItem["MaxCountPerOrder"] = productItem.UnitQuantity;

            result.MinCountPerSO = productItem.UnitQuantity;
            result.MaxCountPerSO = productItem.UnitQuantity;
            result.MerchantSysNo = productItem.MerchantSysNo;
            result.MerchantName = productItem.MerchantName;
            result.PackageNo = 0;
            result.PackageType = 0;
            result.Quantity = 1;

            //商品分组属性
            List<ProductSplitGroupProperty> splitGroupProperty = PipelineDA.GetProductSplitGroupPropertyList(productItem.ProductSysNo);
            if (splitGroupProperty != null && splitGroupProperty.Count > 0)
            {
                productItem.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                foreach (ProductSplitGroupProperty property in splitGroupProperty)
                {
                    productItem.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(property.PropertyDescription, property.ValueDescription));
                }
            }
            result.ProductItemList = new List<OrderProductItem>();
            result.ProductItemList.Add(productItem);

            return result;
        }
    }
}
