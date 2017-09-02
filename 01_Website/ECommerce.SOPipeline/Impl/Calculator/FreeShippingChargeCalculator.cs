using System;
using System.Collections.Generic;
using System.Linq;
using ECommerce.Entity.Shipping;
using ECommerce.Utility;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 免运费规则计算器
    /// </summary>
    public class FreeShippingChargeCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            if (order == null || order.SubOrderList == null)
            {
                return;
            }

            FreeShippingConfig config = FreeShippingConfig.GetConfig();
            if (config == null || config.Rules == null)
            {
                return;
            }

            List<FreeShippingItemConfig> rules = config.Rules;

            //过滤不在当前日期范围内的免运费规则
            rules.RemoveAll(rule => DateTime.Today < DateTime.Parse(rule.StartDate) || DateTime.Today > DateTime.Parse(rule.EndDate));

            if (rules.Count <= 0)
            {
                return;
            }

            //开始循环子单计算运费，需要排除前面步骤分仓失败的子单
            foreach (var kvs in order.SubOrderList.Where(x => !string.IsNullOrEmpty(x.Value.ShipTypeID)))
            {
                int MerchantSysNo = (int)kvs.Value["MerchantSysNo"];
                List<FreeShippingItemConfig> VendorRules = rules.FindAll(x => x.SellerSysNo == MerchantSysNo);
                if (VendorRules.Count > 0)
                {
                    this.InnerCalculate(kvs, VendorRules);
                }
            }

            //重新计算总单的运费金额
            order.ShippingAmount = order.SubOrderList.Sum(x => x.Value.ShippingAmount);
        }

        private void InnerCalculate(KeyValuePair<string, OrderInfo> preSubOrderKVS, List<FreeShippingItemConfig> rules)
        {
            bool isMatchFreeShippingCharge = false;
            ECommerce.Entity.Area contactAddressInfo = null;

            OrderInfo preSubOrderInfo = preSubOrderKVS.Value;
            OrderInfo clonedPreSubOrderInfo = (OrderInfo)preSubOrderInfo.Clone();
            //原运费
            decimal ShippingPrice = preSubOrderInfo.ShippingAmount;
            //有效的免运费规则
            string matchedRule = null;
            foreach (var rule in rules)
            {
                // 1. 检查是否满足金额条件限制
                isMatchFreeShippingCharge = OrderAmtMatchedContext.GetContext(rule).IsMatched(preSubOrderInfo, rule);

                #region 2. 检查是否满足支付类型条件限制(旧逻辑现在不用)
                /*
                if (isMatchFreeShippingCharge && !string.IsNullOrWhiteSpace(rule.PayTypeSettingValue))
                {
                    isMatchFreeShippingCharge = false;

                    string[] setValues = rule.PayTypeSettingValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < setValues.Length; i++)
                    {
                        if (setValues[i].Trim() == preSubOrderInfo.PayTypeID)
                        {
                            isMatchFreeShippingCharge = true;
                            break;
                        }
                    }
                }*/
                #endregion

                // 3. 检查是否满足配送区域条件限制
                if (isMatchFreeShippingCharge && !string.IsNullOrWhiteSpace(rule.ShipAreaSettingValue))
                {
                    isMatchFreeShippingCharge = false;
                    contactAddressInfo = ECommerce.DataAccess.Common.CommonDA.GetAreaBySysNo(preSubOrderInfo.Contact.AddressAreaID);
                    if (contactAddressInfo != null && contactAddressInfo.ProvinceSysNo != null)
                    {
                        string[] setValues = rule.ShipAreaSettingValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < setValues.Length; i++)
                        {
                            if (setValues[i].Trim() == contactAddressInfo.ProvinceSysNo.ToString())
                            {
                                isMatchFreeShippingCharge = true;
                                break;
                            }
                        }
                    }
                }

                //商品免运费数量
                int ProductFreeNumber = 0;
                #region 排除免运费的商品
                if (isMatchFreeShippingCharge && rule.ProductSettingValue != null && rule.ProductSettingValue.Count > 0)
                {
                    isMatchFreeShippingCharge = false;
                    if (clonedPreSubOrderInfo.OrderItemGroupList != null)
                    {
                        foreach (var itemGroup in clonedPreSubOrderInfo.OrderItemGroupList)
                        {
                            if (itemGroup.ProductItemList != null)
                            {
                                for (int i = itemGroup.ProductItemList.Count - 1; i >= 0; i--)
                                {
                                    if (rule.ProductSettingValue.Exists(productSysNo => itemGroup.ProductItemList[i].ProductSysNo == productSysNo))
                                    {
                                        isMatchFreeShippingCharge = true;
                                        itemGroup.ProductItemList.RemoveAt(i);
                                        ProductFreeNumber++;
                                    }
                                }
                            }
                        }
                    }
                    if (clonedPreSubOrderInfo.GiftItemList != null)
                    {
                        for (int i = clonedPreSubOrderInfo.GiftItemList.Count - 1; i >= 0; i--)
                        {
                            if (rule.ProductSettingValue.Exists(productSysNo => clonedPreSubOrderInfo.GiftItemList[i].ProductSysNo == productSysNo))
                            {
                                clonedPreSubOrderInfo.GiftItemList.RemoveAt(i);
                                ProductFreeNumber++;
                            }
                        }
                    }
                    if (clonedPreSubOrderInfo.AttachmentItemList != null)
                    {
                        for (int i = clonedPreSubOrderInfo.AttachmentItemList.Count - 1; i >= 0; i--)
                        {
                            if (rule.ProductSettingValue.Exists(productSysNo => clonedPreSubOrderInfo.AttachmentItemList[i].ProductSysNo == productSysNo))
                            {
                                clonedPreSubOrderInfo.AttachmentItemList.RemoveAt(i);
                                ProductFreeNumber++;
                            }
                        }
                    }
                }
                #endregion
                if (ProductFreeNumber > 0)
                {
                    matchedRule += rule.ToXmlString() + ";";
                }
                /*
                //对同时满足门槛金额限制、支付类型限制、配送区域限制的子单重新计算运费(旧逻辑现在不用)
                if (isMatchFreeShippingCharge)
                {
                    CalcShippingAmount(preSubOrderKVS, rule);
                    //break;   //旧逻辑
                }
                */
            }


            //排除免运费商品后，子单中还剩下部分商品，计算这部分商品的运费作为本单运费
            //fixbug: 在没有收货地址的情况下，如果订单满足免运费条件，此时ShippingFeeQueryInfo对象的AreaId=0，计算运费的sp进入某个特定分支后会报错
            if (clonedPreSubOrderInfo.TotalItemCount > 0 && preSubOrderInfo.Contact.AddressAreaID > 0)
            //if (ProductFreeNumber <= 0 && preSubOrderInfo.Contact.AddressAreaID > 0 && clonedPreSubOrderInfo.TotalItemCount > 0)
            {
                List<ShippingFeeQueryInfo> qryList = new List<ShippingFeeQueryInfo>();
                ShippingFeeQueryInfo qry = new ShippingFeeQueryInfo();
                qry.TransID = preSubOrderKVS.Key;
                qry.SoAmount = clonedPreSubOrderInfo.TotalProductAmount;
                qry.SoTotalWeight = clonedPreSubOrderInfo.TotalWeight;
                qry.SOSingleMaxWeight = clonedPreSubOrderInfo.MaxWeight;
                qry.AreaId = preSubOrderInfo.Contact.AddressAreaID;
                qry.CustomerSysNo = preSubOrderInfo.Customer.SysNo;
                qry.IsUseDiscount = 0;
                qry.SubShipTypeList = preSubOrderInfo.ShipTypeID.ToString();
                qry.SellType = Convert.ToInt32(preSubOrderInfo["SellerType"]);
                qry.MerchantSysNo = Convert.ToInt32(preSubOrderKVS.Key.Split('|')[0]);
                qry.ShipTypeId = 0;
                qryList.Add(qry);

                List<ShippingInfo> shipFeeList = PipelineDA.GetAllShippingFee(qryList);

                ShippingInfo curShippingInfo = shipFeeList.Find(x => x.TransID == preSubOrderKVS.Key && x.ShippingTypeID.ToString() == preSubOrderInfo.ShipTypeID);
                if (curShippingInfo != null)
                {
                    preSubOrderInfo.ShippingAmount = curShippingInfo.ShippingPrice; //+curShippingInfo.ShippingPackageFee; 
                }
            }
            else
            {
                //排除免运费商品后，子单中商品数量为0，免本单运费
                preSubOrderInfo.ShippingAmount = 0m;
            }
            if (!string.IsNullOrWhiteSpace(matchedRule))
            {
                preSubOrderInfo["FreeShippingChargeLog"] = string.Format("符合免运费规则：{0}\r\n，原运费为:{1:F2}元，减免后运费为:{2:F2}元",
        matchedRule.ToXmlString(), ShippingPrice, preSubOrderInfo.ShippingAmount);
            }
        }

        private void CalcShippingAmount(KeyValuePair<string, OrderInfo> preSubOrderKVS, FreeShippingItemConfig matchedRule)
        {
            OrderInfo preSubOrderInfo = preSubOrderKVS.Value;
            OrderInfo clonedPreSubOrderInfo = (OrderInfo)preSubOrderInfo.Clone();
            //主商品免运费数量
            int ProductFreeNumber = 0;
            //全网模式，免本单运费
            if (matchedRule.IsGlobal)
            {
                preSubOrderInfo.ShippingAmount = 0m;
            }
            else
            {
                //非全网模式，排除免运费的商品，计算剩下商品的运费
                if (matchedRule.ProductSettingValue != null && matchedRule.ProductSettingValue.Count > 0)
                {
                    #region 排除免运费的商品

                    if (clonedPreSubOrderInfo.OrderItemGroupList != null)
                    {
                        foreach (var itemGroup in clonedPreSubOrderInfo.OrderItemGroupList)
                        {
                            if (itemGroup.ProductItemList != null)
                            {
                                for (int i = itemGroup.ProductItemList.Count - 1; i >= 0; i--)
                                {
                                    if (matchedRule.ProductSettingValue.Exists(productSysNo => itemGroup.ProductItemList[i].ProductSysNo == productSysNo))
                                    {
                                        itemGroup.ProductItemList.RemoveAt(i);
                                        ProductFreeNumber++;
                                    }
                                }
                            }
                        }
                    }
                    if (clonedPreSubOrderInfo.GiftItemList != null)
                    {
                        for (int i = clonedPreSubOrderInfo.GiftItemList.Count - 1; i >= 0; i--)
                        {
                            if (matchedRule.ProductSettingValue.Exists(productSysNo => clonedPreSubOrderInfo.GiftItemList[i].ProductSysNo == productSysNo))
                            {
                                clonedPreSubOrderInfo.GiftItemList.RemoveAt(i);
                            }
                        }
                    }
                    if (clonedPreSubOrderInfo.AttachmentItemList != null)
                    {
                        for (int i = clonedPreSubOrderInfo.AttachmentItemList.Count - 1; i >= 0; i--)
                        {
                            if (matchedRule.ProductSettingValue.Exists(productSysNo => clonedPreSubOrderInfo.AttachmentItemList[i].ProductSysNo == productSysNo))
                            {
                                clonedPreSubOrderInfo.AttachmentItemList.RemoveAt(i);
                            }
                        }
                    }
                    #endregion

                    //排除免运费商品后，子单中还剩下部分商品，计算这部分商品的运费作为本单运费
                    //fixbug: 在没有收货地址的情况下，如果订单满足免运费条件，此时ShippingFeeQueryInfo对象的AreaId=0，计算运费的sp进入某个特定分支后会报错
                    if (clonedPreSubOrderInfo.TotalItemCount > 0
                        && preSubOrderInfo.Contact.AddressAreaID > 0)
                    //if (ProductFreeNumber <= 0 && preSubOrderInfo.Contact.AddressAreaID > 0 && clonedPreSubOrderInfo.TotalItemCount > 0)
                    {
                        List<ShippingFeeQueryInfo> qryList = new List<ShippingFeeQueryInfo>();
                        ShippingFeeQueryInfo qry = new ShippingFeeQueryInfo();
                        qry.TransID = preSubOrderKVS.Key;
                        qry.SoAmount = clonedPreSubOrderInfo.TotalProductAmount;
                        qry.SoTotalWeight = clonedPreSubOrderInfo.TotalWeight;
                        qry.SOSingleMaxWeight = clonedPreSubOrderInfo.MaxWeight;
                        qry.AreaId = preSubOrderInfo.Contact.AddressAreaID;
                        qry.CustomerSysNo = preSubOrderInfo.Customer.SysNo;
                        qry.IsUseDiscount = 0;
                        qry.SubShipTypeList = preSubOrderInfo.ShipTypeID.ToString();
                        qry.SellType = Convert.ToInt32(preSubOrderInfo["SellerType"]);
                        qry.MerchantSysNo = Convert.ToInt32(preSubOrderKVS.Key.Split('|')[0]);
                        qry.ShipTypeId = 0;
                        qryList.Add(qry);

                        List<ShippingInfo> shipFeeList = PipelineDA.GetAllShippingFee(qryList);

                        ShippingInfo curShippingInfo = shipFeeList.Find(x => x.TransID == preSubOrderKVS.Key && x.ShippingTypeID.ToString() == preSubOrderInfo.ShipTypeID);
                        if (curShippingInfo != null)
                        {
                            preSubOrderInfo.ShippingAmount = curShippingInfo.ShippingPrice; //+curShippingInfo.ShippingPackageFee; 
                        }
                    }
                    else
                    {
                        //排除免运费商品后，子单中商品数量为0，免本单运费
                        preSubOrderInfo.ShippingAmount = 0m;
                    }
                }
            }
            preSubOrderInfo["FreeShippingChargeLog"] = string.Format("符合免运费规则：{0}\r\n，原运费为:{1:F2}元，减免后运费为:{2:F2}元",
                matchedRule.ToXmlString(), clonedPreSubOrderInfo.ShippingAmount, preSubOrderInfo.ShippingAmount);
        }
    }

    internal class OrderAmtMatchedContext
    {
        private IOrderAmtMatchedStrategy _strategy;
        private OrderAmtMatchedContext(IOrderAmtMatchedStrategy strategy)
        {
            _strategy = strategy;
        }

        private static OrderAmtMatchedContext ProductAmtMatchedContext
                                    = new OrderAmtMatchedContext(new ProductAmtMatchedStrategy());

        private static OrderAmtMatchedContext ProductReduceDisAmtMatchedContext
                                    = new OrderAmtMatchedContext(new ProductReduceDisAmtMatchedStrategy());

        private static OrderAmtMatchedContext ProductAddTariffAmtMatchedContext
                                    = new OrderAmtMatchedContext(new ProductAddTariffAmtMatchedStrategy());

        private static OrderAmtMatchedContext ProductAddTariffReduceDisAmtMatchedContext
                                    = new OrderAmtMatchedContext(new ProductAddTariffReduceDisAmtMatchedStrategy());

        private static OrderAmtMatchedContext ShippingAmtMatchedContext
                                    = new OrderAmtMatchedContext(new ShippingAmtMatchedStrategy());

        internal static OrderAmtMatchedContext GetContext(FreeShippingItemConfig rule)
        {
            switch (rule.AmountSettingType.Trim())
            {
                case "1": return ProductAmtMatchedContext;
                case "2": return ProductReduceDisAmtMatchedContext;
                case "3": return ProductAddTariffAmtMatchedContext;
                case "4": return ProductAddTariffReduceDisAmtMatchedContext;
                case "5": return ShippingAmtMatchedContext;
                default: throw new ArgumentException(string.Format("无效的免运费规则类型:【{0}】", rule.AmountSettingType));
            }
        }

        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return _strategy.IsMatched(preOrderInfo, rule);
        }
    }

    internal interface IOrderAmtMatchedStrategy
    {
        bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule);
    }

    /// <summary>
    /// 判断商品总金额是否满足免运费条件
    /// </summary>
    internal class ProductAmtMatchedStrategy : IOrderAmtMatchedStrategy
    {
        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return preOrderInfo.TotalProductAmount >= decimal.Parse(rule.AmountSettingValue);
        }
    }

    /// <summary>
    /// 判断商品总金额 – 折扣 – 优惠券抵用是否满足免运费条件
    /// </summary>
    internal class ProductReduceDisAmtMatchedStrategy : IOrderAmtMatchedStrategy
    {
        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return (preOrderInfo.TotalProductAmount
                - preOrderInfo.TotalDiscountAmount
                - preOrderInfo.CouponAmount) >= decimal.Parse(rule.AmountSettingValue);
        }
    }

    /// <summary>
    /// 判断商品总金额+关税总金额是否满足免运费条件
    /// </summary>
    internal class ProductAddTariffAmtMatchedStrategy : IOrderAmtMatchedStrategy
    {
        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return (preOrderInfo.TotalProductAmount + preOrderInfo.TaxAmount) >= decimal.Parse(rule.AmountSettingValue);
        }
    }

    /// <summary>
    /// 判断(商品总金额 – 折扣 – 优惠券抵用)+关税总金额 是否满足免运费条件
    /// </summary>
    internal class ProductAddTariffReduceDisAmtMatchedStrategy : IOrderAmtMatchedStrategy
    {
        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return (preOrderInfo.TotalProductAmount
                + preOrderInfo.TaxAmount
                - preOrderInfo.TotalDiscountAmount
                - preOrderInfo.CouponAmount) >= decimal.Parse(rule.AmountSettingValue);
        }
    }

    /// <summary>
    /// 判断总运费是否满足免运费条件
    /// </summary>
    internal class ShippingAmtMatchedStrategy : IOrderAmtMatchedStrategy
    {
        public bool IsMatched(OrderInfo preOrderInfo, FreeShippingItemConfig rule)
        {
            return preOrderInfo.ShippingAmount >= decimal.Parse(rule.AmountSettingValue);
        }
    }
}