using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Member;
using ECommerce.Enums;
using ECommerce.Entity.Order;
using ECommerce.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using ECommerce.SOPipeline.Entity;

namespace ECommerce.SOPipeline.Impl
{
    public class MerchantCouponCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            CalculateDiscountAmount(order);

            Dictionary<int, List<OrderInfo>> merchantOrderList = new Dictionary<int, List<OrderInfo>>();
            
            #region 1、分拆商家订单
            
            if (order.SubOrderList != null)
            {
                foreach (var subOrder in order.SubOrderList)
                {
                    int merchantSysNo = Convert.ToInt32(subOrder.Key.Split('|')[0]);
                    if (!merchantOrderList.ContainsKey(merchantSysNo))
                        merchantOrderList[merchantSysNo] = new List<OrderInfo>();
                    merchantOrderList[merchantSysNo].Add(subOrder.Value);
                }
            }
            
            #endregion

            #region 2、计算商家优惠券

            List<CustomerCouponInfo> merchantCouponList = PromotionDA.GetMerchantCouponCodeList(order.Customer.SysNo);
            if (merchantCouponList != null)
            {
                foreach (var merchantOrder in merchantOrderList)
                {
                    List<CustomerCouponInfo> currMerchantCouponList = merchantCouponList.FindAll(m => m.MerchantSysNo == merchantOrder.Key);
                    if (currMerchantCouponList == null || currMerchantCouponList.Count == 0)
                        continue;
                    foreach (var coupon in currMerchantCouponList)
                    {
                        bool canUse = false;
                        #region 检查商品限制，是否可用
                        List<int> productSysNoList = new List<int>();
                        foreach (var itemOrder in merchantOrder.Value)
                        {
                            foreach (var itemGroup in itemOrder.OrderItemGroupList)
                            {
                                itemGroup.ProductItemList.ForEach(product =>
                                {
                                    productSysNoList.Add(product.ProductSysNo);
                                });
                            }
                        }
                        List<CouponSaleRules> couponSaleRulesList = PromotionDA.GetCouponSaleRulesList(coupon.CouponCode);
                        if (couponSaleRulesList == null || couponSaleRulesList.Count == 0)
                        {
                            //不存在排除或指定商品，全网通用
                            canUse = true;
                        }
                        else
                        {
                            foreach (int productSysNo in productSysNoList)
                            {
                                if (couponSaleRulesList.Exists(m => m.ProductSysNo == productSysNo))
                                {
                                    if (couponSaleRulesList[0].RelationType == "N")
                                    {
                                        //存在排除的商品，不可用
                                        canUse = false;
                                        break;
                                    }
                                    else
                                    {
                                        //存在指定的商品，可用
                                        canUse = true;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                        if (canUse)
                        {
                            if (CheckCouponCodeCanUse(coupon.CouponCode, order.Customer))
                            {
                                foreach (var itemOrder in merchantOrder.Value)
                                {
                                    if (itemOrder.MerchantCouponList == null)
                                        itemOrder.MerchantCouponList = new List<CustomerCouponInfo>();
                                    itemOrder.MerchantCouponList.Add(coupon);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region 3、商家订单使用优惠券

            foreach (var merchantOrder in merchantOrderList)
            {
                MerchantCoupon merchantCoupon = null;
                if(order.MerchantCouponCodeList != null)
                    merchantCoupon = order.MerchantCouponCodeList.Find(m => m.MerchantSysNo == merchantOrder.Key);
                if (merchantCoupon == null)
                    continue;
                string couponCode = merchantCoupon.CouponCode.ToUpper().Trim();

                #region 1、初次筛选该优惠券号码对于前用户是否能用
                CustomerCouponInfo customerCoupon = PromotionDA.GetCouponSaleRules(order.Customer.SysNo, couponCode);
                if (customerCoupon == null)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("您没有此优惠券{0}", order.LanguageCode), merchantCoupon.CouponName);
                    continue;
                }
                if (customerCoupon.IsExpired)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券{0}已过期", order.LanguageCode), merchantCoupon.CouponName);
                    continue;
                }
                if (customerCoupon.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= customerCoupon.CustomerMaxFrequency.Value)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("您已达到使用优惠券{0}的次数上限", order.LanguageCode), merchantCoupon.CouponName);
                    continue;
                }

                int totalUsedCount = 0;
                if (customerCoupon.CodeType.Trim().ToUpper() == "C")
                {
                    totalUsedCount = PromotionDA.GetCouponCodeTotalUsedCount(couponCode);
                }
                else
                {
                    totalUsedCount = PromotionDA.GetCouponTotalUsedCount(customerCoupon.CouponSysNo);
                }
                if (customerCoupon.WebsiteMaxFrequency.HasValue && totalUsedCount >= customerCoupon.WebsiteMaxFrequency)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券{0}的已达到全网使用次数上限", order.LanguageCode), merchantCoupon.CouponName);
                    continue;
                }
                #endregion

                #region 2、获取该优惠券号码对应的优惠券活动所有信息
                CouponInfo coupon = PromotionDA.GetComboInfoByCouponCode(couponCode);
                if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= coupon.SaleRulesEx.CustomerMaxFrequency.Value)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("您已达到使用此优惠券活动的次数上限", order.LanguageCode));
                    continue;
                }
                if (coupon.SaleRulesEx.MaxFrequency.HasValue && totalUsedCount >= coupon.SaleRulesEx.MaxFrequency.Value)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券活动已达到全网使用次数上限", order.LanguageCode));
                    continue;
                }


                if (coupon == null)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券{0}不存在", order.LanguageCode), merchantCoupon.CouponName);
                    continue;
                }
                #endregion

                #region 3、详细检查该优惠券号码是否可用
                if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedEmailVerification) && coupon.SaleRulesEx.NeedEmailVerification.ToUpper().Trim() == "Y")
                {
                    if (order.Customer.IsEmailConfirmed != 1)
                    {
                        SetCouponError(ref order, LanguageHelper.GetText("此优惠券要求客户验证了电子邮箱才可使用！", order.LanguageCode));
                        continue;
                    }
                }
                if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedMobileVerification) && coupon.SaleRulesEx.NeedMobileVerification.ToUpper().Trim() == "Y")
                {
                    if (order.Customer.IsPhoneValided != 1)
                    {
                        SetCouponError(ref order, LanguageHelper.GetText("此优惠券要求客户验证了手机才可使用！", order.LanguageCode));
                        continue;
                    }
                }
                if (coupon.SaleRulesList.Count > 0 && coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelCustomerRank))
                {
                    //-1表示不限制
                    if (!coupon.SaleRulesList.Exists(f => f.CustomerRank == -1))
                    {
                        int customerRank = order.Customer.CustomerRank;
                        if (!coupon.SaleRulesList.Exists(f => f.CustomerRank == customerRank))
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("当前客户不满足此优惠券要求的客户等级！", order.LanguageCode));
                            continue;
                        }
                    }
                }
                if (coupon.SaleRulesList.Count > 0 && coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelArea))
                {
                    int areaID = order.Contact.AddressAreaID;
                    Area area = PromotionDA.GetAreaBySysNo(areaID);
                    int provinceId = area.ProvinceSysNo.Value;

                    if (!coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelArea && f.AreaSysNo == provinceId))
                    {
                        SetCouponError(ref order, LanguageHelper.GetText("当前客户不满足此优惠券要求的客户地区！", order.LanguageCode));
                        continue;
                    }
                }


                #endregion

                #region 4、计算该优惠券可以抵扣多少，填充到OrderInfo中
                if (coupon.DiscountRuleList != null && coupon.DiscountRuleList.Count > 0)
                {
                    //取得满足优惠券条件的商品总金额
                    decimal canCalculateAmount = GetCanCalculateAmount(merchantOrder.Value, coupon);
                    if (canCalculateAmount == 0)
                    {
                        SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的商品，无法抵扣", order.LanguageCode));
                        continue;
                    }

                    canCalculateAmount = canCalculateAmount - Math.Abs(order.TotalDiscountAmount);

                    if (canCalculateAmount < coupon.SaleRulesEx.OrderAmountLimit)
                    {
                        SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券要求的商品总金额下限{0}，无法抵扣", order.LanguageCode), coupon.SaleRulesEx.OrderAmountLimit);
                        continue;
                    }
                    
                    decimal totalPriceAmount = (merchantOrder.Value.Sum(m => m.TotalProductAmount) - merchantOrder.Value.Sum(m => m.TotalDiscountAmount));
                    
                    #region 处理订单 折扣金额模式 和 百分比模式
                    if (coupon.DiscountRuleList[0].DiscountType == CouponDiscountType.OrderAmountDiscount || coupon.DiscountRuleList[0].DiscountType == CouponDiscountType.OrderAmountPercentage)
                    {
                        //获取适合的折扣规则
                        Coupon_DiscountRules curDiscountRule = GetMatchDiscountRule(coupon, canCalculateAmount);
                        if (curDiscountRule == null)
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("订单总金额未达到优惠券要求的最小购买金额，无法抵扣", order.LanguageCode));
                            return;
                        }

                        decimal discount = 0m;
                        if (curDiscountRule.DiscountType == CouponDiscountType.OrderAmountDiscount)
                        {
                            discount = Math.Abs(Math.Round(curDiscountRule.Value, 2));
                        }
                        else
                        {
                            discount = Math.Abs(Math.Round(canCalculateAmount * curDiscountRule.Value, 2));
                        }



                        if (coupon.SaleRulesEx.OrderMaxDiscount.HasValue && coupon.SaleRulesEx.OrderMaxDiscount.Value > 0m)
                        {
                            if (discount > coupon.SaleRulesEx.OrderMaxDiscount.Value)
                            {
                                discount = coupon.SaleRulesEx.OrderMaxDiscount.Value;
                            }
                        }

                        foreach (var subOrder in merchantOrder.Value)
                        {
                            subOrder.MerchantCouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                            subOrder.MerchantCouponCode = customerCoupon.CouponCode;
                            subOrder.MerchantCouponSysNo = customerCoupon.CouponSysNo;
                            subOrder.MerchantCouponName = customerCoupon.CouponName;
                            subOrder.MerchantCouponAmount = discount * ((subOrder.TotalProductAmount - subOrder.TotalDiscountAmount) / totalPriceAmount);
                            subOrder.MerchantCouponErrorDesc = string.Empty;
                            foreach (var itemGroup in subOrder.OrderItemGroupList)
                            {
                                foreach (var item in itemGroup.ProductItemList)
                                {
                                    item["UnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m :(decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                    item["MerchantUnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 处理 一个商品直减模式
                    if (coupon.DiscountRuleList[0].DiscountType == CouponDiscountType.ProductPriceDiscount)
                    {
                        if (coupon.SaleRulesList == null || coupon.SaleRulesList.Count == 0)
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的指定商品，无法抵扣", order.LanguageCode));
                            return;
                        }
                        Coupon_SaleRules productSaleRule = coupon.SaleRulesList.Find(f => f.SaleRuleType == CouponSaleRuleType.RelProduct);


                        int discountProductSysNo = productSaleRule.ProductSysNo;

                        int discountProductCount = 0;
                        decimal productReduce = 0m;
                        decimal productPrice = 0.00m;
                        foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
                        {
                            OrderProductItem item = itemGroup.ProductItemList.Find(f => f.ProductSysNo == discountProductSysNo);
                            if (item != null)
                            {
                                discountProductCount += item.UnitQuantity * itemGroup.Quantity;

                                productPrice = item.UnitSalePrice;
                            }
                        }
                        if (discountProductCount == 0)
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的指定商品，无法抵扣", order.LanguageCode));
                            return;
                        }

                        if (discountProductCount > coupon.DiscountRuleList[0].Quantity)
                        {
                            discountProductCount = coupon.DiscountRuleList[0].Quantity;
                        }

                        productReduce = coupon.DiscountRuleList[0].Value;
                        if (coupon.DiscountRuleList[0].Value > productPrice)
                        {
                            productReduce = productPrice;
                        }

                        decimal discount = Math.Abs(Math.Round(productReduce * discountProductCount, 2));



                        if (coupon.SaleRulesEx.OrderMaxDiscount.HasValue && coupon.SaleRulesEx.OrderMaxDiscount.Value > 0m)
                        {
                            if (discount > coupon.SaleRulesEx.OrderMaxDiscount.Value)
                            {
                                discount = coupon.SaleRulesEx.OrderMaxDiscount.Value;
                            }
                        }
                        foreach (var subOrder in merchantOrder.Value)
                        {
                            subOrder.MerchantCouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                            subOrder.MerchantCouponCode = customerCoupon.CouponCode;
                            subOrder.MerchantCouponSysNo = customerCoupon.CouponSysNo;
                            subOrder.MerchantCouponName = customerCoupon.CouponName;
                            subOrder.MerchantCouponAmount = discount * ((subOrder.TotalProductAmount - subOrder.TotalDiscountAmount) / totalPriceAmount);
                            subOrder.MerchantCouponErrorDesc = string.Empty;

                            subOrder["MerchantCoupon_DiscountProductSysNo"] = discountProductSysNo;
                            subOrder["MerchantCoupon_DiscountProductCount"] = discountProductCount;
                            foreach (var itemGroup in subOrder.OrderItemGroupList)
                            {
                                foreach (var item in itemGroup.ProductItemList)
                                {
                                    item["UnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                    item["MerchantUnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 处理 一个商品最终售价模式
                    if (coupon.DiscountRuleList[0].DiscountType == CouponDiscountType.ProductPriceFinal)
                    {
                        if (coupon.SaleRulesList == null || coupon.SaleRulesList.Count == 0)
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的指定商品，无法抵扣", order.LanguageCode));
                            return;
                        }
                        Coupon_SaleRules productSaleRule = coupon.SaleRulesList.Find(f => f.SaleRuleType == CouponSaleRuleType.RelProduct);


                        int discountProductSysNo = productSaleRule.ProductSysNo;

                        decimal discountProductPrice = 0.00m;
                        int discountProductCount = 0;
                        foreach (var currOrder in merchantOrder.Value)
                        {
                            foreach (OrderItemGroup itemGroup in currOrder.OrderItemGroupList)
                            {
                                OrderProductItem item = itemGroup.ProductItemList.Find(f => f.ProductSysNo == discountProductSysNo);
                                if (item != null)
                                {
                                    discountProductCount += item.UnitQuantity * itemGroup.Quantity;
                                    discountProductPrice = item.UnitSalePrice;
                                }
                            }
                        }
                        if (discountProductCount == 0)
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的指定商品,无法抵扣", order.LanguageCode));
                            return;
                        }
                        if (discountProductCount > coupon.DiscountRuleList[0].Quantity)
                        {
                            discountProductCount = coupon.DiscountRuleList[0].Quantity;
                        }

                        decimal discount = 0m;
                        if (discountProductPrice > coupon.DiscountRuleList[0].Value)
                        {
                            discount = Math.Abs(Math.Round((discountProductPrice - coupon.DiscountRuleList[0].Value) * discountProductCount, 2));
                        }
                        else
                        {
                            SetCouponError(ref order, LanguageHelper.GetText("该优惠券的指定商品已经是优惠券设定的最低价格,无法抵扣", order.LanguageCode));
                            return;
                        }
                        if (coupon.SaleRulesEx.OrderMaxDiscount.HasValue && coupon.SaleRulesEx.OrderMaxDiscount.Value > 0m)
                        {
                            if (discount > coupon.SaleRulesEx.OrderMaxDiscount.Value)
                            {
                                discount = coupon.SaleRulesEx.OrderMaxDiscount.Value;
                            }
                        }
                        foreach (var subOrder in merchantOrder.Value)
                        {
                            subOrder.MerchantCouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                            subOrder.MerchantCouponCode = customerCoupon.CouponCode;
                            subOrder.MerchantCouponSysNo = customerCoupon.CouponSysNo;
                            subOrder.MerchantCouponName = customerCoupon.CouponName;
                            subOrder.MerchantCouponAmount = discount * ((subOrder.TotalProductAmount - subOrder.TotalDiscountAmount) / totalPriceAmount);
                            subOrder.MerchantCouponErrorDesc = string.Empty;

                            subOrder["MerchantCoupon_DiscountProductSysNo"] = discountProductSysNo;
                            subOrder["MerchantCoupon_DiscountProductCount"] = discountProductCount;
                            foreach (var itemGroup in subOrder.OrderItemGroupList)
                            {
                                foreach (var item in itemGroup.ProductItemList)
                                {
                                    item["UnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                    item["MerchantUnitCouponAmt"] = (discount * (item.UnitSalePrice - (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])))
                                                            / totalPriceAmount;
                                }
                            }
                        }
                    }
                    #endregion

                    foreach (var subOrder in merchantOrder.Value)
                    {
                        subOrder["MerchantCoupon_DiscountType"] = coupon.DiscountRuleList[0].DiscountType;
                    }

                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// 计算促销折扣金额，主要是进行金额分摊
        /// </summary>
        /// <param name="order"></param>
        private void CalculateDiscountAmount(OrderInfo order)
        {
            if (order.DiscountDetailList == null || order.DiscountDetailList.Count <= 0) return;

            decimal unitDiscountAmt = 0m;

            foreach (var kvs in order.SubOrderList)
            {
                //初始化子单上的折扣列表，通过计算，将折扣信息分摊到每个子单上 
                kvs.Value.DiscountDetailList = new List<OrderItemDiscountInfo>();

                //计算该item总计购买数量，需要遍历主商品列表
                if (kvs.Value.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in kvs.Value.OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            itemGroup.ProductItemList.ForEach(orderItem =>
                            {
                                //暂时还没有考虑整单减免的情况。

                                //从订单的折扣明细表找到当前item的折扣信息
                                var discountInfoList = order.DiscountDetailList.FindAll(x => x.ProductSysNo == orderItem.ProductSysNo);
                                //先考虑和主商品相关的促销折扣
                                if (discountInfoList != null)
                                {
                                    //计算item总数量
                                    int totalItemBuyQuantity = 0;
                                    //只考虑主商品折扣
                                    if (order.OrderItemGroupList != null)
                                    {
                                        //计算该主商品购买的总数量
                                        order.OrderItemGroupList.ForEach(g =>
                                        {
                                            totalItemBuyQuantity += g.ProductItemList.Where(x => x.ProductSysNo == orderItem.ProductSysNo)
                                                                                     .Sum(x => x.UnitQuantity * g.Quantity);
                                        });
                                    }

                                    //计算item总折扣
                                    decimal totalDiscountAmt = 0m;
                                    foreach (var discountInfo in discountInfoList)
                                    {
                                        //将折扣信息加入到子单中, 按照item平均折扣重新计算UnitDiscount，并将Quantity填为1 
                                        OrderItemDiscountInfo clonedDiscountInfo = (OrderItemDiscountInfo)discountInfo.Clone();
                                        clonedDiscountInfo.Quantity = 1;
                                        clonedDiscountInfo.UnitDiscount = (discountInfo.UnitDiscount * discountInfo.Quantity)
                                                                                                     * orderItem.UnitQuantity / totalItemBuyQuantity;
                                        kvs.Value.DiscountDetailList.Add(clonedDiscountInfo);

                                        //计算单个item的平均折扣，同一个商品A如果既存在与套餐C中，又单独购买过，那么discountInfo中记录的是套餐中该商品享受的折扣
                                        //而单独购买的A不享受折扣，这里需要计算的是单个item的平均折扣，计算的时候需要用item的总折扣除以item的总数量
                                        totalDiscountAmt += (discountInfo.UnitDiscount * discountInfo.Quantity);
                                    }

                                    //计算item平均折扣金额，折扣金额为正数
                                    if (totalItemBuyQuantity > 0)
                                    {
                                        unitDiscountAmt = totalDiscountAmt / totalItemBuyQuantity;
                                        orderItem["UnitDiscountAmt"] = unitDiscountAmt;
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取适合的折扣规则
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="canCalculateAmount"></param>
        /// <returns></returns>
        private Coupon_DiscountRules GetMatchDiscountRule(CouponInfo coupon, decimal canCalculateAmount)
        {
            Coupon_DiscountRules curDiscountRule = null;
            coupon.DiscountRuleList.Sort((x, y) => y.Amount.CompareTo(x.Amount));
            foreach (Coupon_DiscountRules discountRule in coupon.DiscountRuleList)
            {
                if (canCalculateAmount >= discountRule.Amount)
                {
                    curDiscountRule = discountRule;
                    break;
                }
            }
            return curDiscountRule;
        }

        /// <summary>
        /// 取得满足优惠券条件的商品总金额
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private decimal GetCanCalculateAmount(List<OrderInfo> orderList, CouponInfo coupon)
        {
            List<int> productSysNoList = new List<int>();
            foreach (var order in orderList)
            {
                foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
                {
                    foreach (OrderProductItem item in itemGroup.ProductItemList)
                    {
                        productSysNoList.Add(item.ProductSysNo);
                    }
                }
            }
            List<SimpleItemEntity> productList = PromotionDA.GetSimpleItemListBySysNumbers(productSysNoList);
            //couponProductList:满足条件的商品列表，使用OrderProductItem这个对象来记录，UnitSalePrice是这个商品的价格，UnitQuantity会作为这个商品的总数量
            List<OrderProductItem> couponProductList = new List<OrderProductItem>();
            foreach (var order in orderList)
            {
                foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
                {
                    foreach (OrderProductItem item in itemGroup.ProductItemList)
                    {
                        SimpleItemEntity product = productList.Find(f => f.ProductSysNo == item.ProductSysNo);
                        if (CheckIsCouponProduct(product, coupon))
                        {
                            OrderProductItem couponProduct = couponProductList.Find(f => f.ProductSysNo == product.ProductSysNo);
                            if (couponProduct == null)
                            {
                                couponProduct = new OrderProductItem();
                                couponProduct.ProductSysNo = item.ProductSysNo;
                                couponProduct.UnitSalePrice = item.UnitSalePrice;
                                couponProduct.UnitQuantity = item.UnitQuantity * itemGroup.Quantity;
                                couponProductList.Add(couponProduct);
                            }
                            else
                            {
                                couponProduct.UnitQuantity += item.UnitQuantity * itemGroup.Quantity;
                            }
                        }
                    }
                }
            }

            decimal totalAmount = 0.00m;
            foreach (OrderProductItem couponProduct in couponProductList)
            {
                totalAmount += couponProduct.UnitSalePrice * couponProduct.UnitQuantity;
            }

            return totalAmount;
        }


        /// <summary>
        /// 判断商品是否满足优惠券条件,true=满足条件
        /// </summary>
        /// <param name="product"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private bool CheckIsCouponProduct(SimpleItemEntity product, CouponInfo coupon)
        {
            //无商品限制
            if (coupon.RulesType.Trim().ToUpper() == "A")
            {
                return true;
            }

            //限定类别、品牌、商家
            if (coupon.RulesType.Trim().ToUpper() == "X")
            {
                //采用排除法：如果设置了某项条件，但是商品不满足该项条件，那么返回false；
                //如果没有设置某项条件，那么我们认为这项条件是不做任何限制的；
                //逐一Check，如果所有检查项都没有被排除，那么返回true
                List<Coupon_SaleRules> sellerList = coupon.SaleRulesList.FindAll(f => f.SaleRuleType == CouponSaleRuleType.RelSeller);
                if (sellerList != null && sellerList.Count > 0 && !sellerList.Exists(f => f.SellerSysNo == product.MerchantSysNo))
                {
                    return false;
                }

                List<Coupon_SaleRules> categoryList = coupon.SaleRulesList.FindAll(f => f.SaleRuleType == CouponSaleRuleType.RelCategory);
                if (categoryList != null && categoryList.Count > 0)
                {
                    if (categoryList[0].RelationType.Trim().ToUpper() == "Y")
                    {
                        if (!categoryList.Exists(f => f.C3SysNo == product.C3SysNo))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (categoryList.Exists(f => f.C3SysNo == product.C3SysNo))
                        {
                            return false;
                        }
                    }
                }

                List<Coupon_SaleRules> brandList = coupon.SaleRulesList.FindAll(f => f.SaleRuleType == CouponSaleRuleType.RelBrand);
                if (brandList != null && brandList.Count > 0)
                {
                    if (brandList[0].RelationType.Trim().ToUpper() == "Y")
                    {
                        if (!brandList.Exists(f => f.BrandSysNo == product.BrandSysNo))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (brandList.Exists(f => f.BrandSysNo == product.BrandSysNo))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            //限定商品
            if (coupon.RulesType.Trim().ToUpper() == "I")
            {
                if (coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelProduct
                                                 && f.ProductSysNo == product.ProductSysNo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置优惠券使用失败的描述等
        /// </summary>
        /// <param name="order"></param>
        /// <param name="errDesc"></param>
        /// <param name="param"></param>
        private void SetCouponError(ref OrderInfo order, string errDesc, params object[] param)
        {
            order.MerchantCouponSysNo = null;
            order.MerchantCouponCodeSysNo = null;
            order.MerchantCouponErrorDesc = string.Format(errDesc, param);
        }

        /// <summary>
        /// 检查优惠券是否可用
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        private bool CheckCouponCodeCanUse(string couponCode, CustomerInfo customer)
        {
            #region 1、初次筛选该优惠券号码对于前用户是否能用
            CustomerCouponInfo customerCoupon = PromotionDA.GetCouponSaleRules(customer.SysNo, couponCode);
            if (customerCoupon == null)
            {
                return false;
            }
            if (customerCoupon.IsExpired)
            {
                return false;
            }
            if (customerCoupon.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= customerCoupon.CustomerMaxFrequency.Value)
            {
                return false;
            }

            int totalUsedCount = 0;
            if (customerCoupon.CodeType.Trim().ToUpper() == "C")
            {
                totalUsedCount = PromotionDA.GetCouponCodeTotalUsedCount(couponCode);
            }
            else
            {
                totalUsedCount = PromotionDA.GetCouponTotalUsedCount(customerCoupon.CouponSysNo);
            }
            if (customerCoupon.WebsiteMaxFrequency.HasValue && totalUsedCount >= customerCoupon.WebsiteMaxFrequency)
            {
                return false;
            }
            #endregion

            #region 2、获取该优惠券号码对应的优惠券活动所有信息
            CouponInfo coupon = PromotionDA.GetComboInfoByCouponCode(couponCode);
            if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= coupon.SaleRulesEx.CustomerMaxFrequency.Value)
            {
                return false;
            }
            if (coupon.SaleRulesEx.MaxFrequency.HasValue && totalUsedCount >= coupon.SaleRulesEx.MaxFrequency.Value)
            {
                return false;
            }

            if (coupon == null)
            {
                return false;
            }
            #endregion

            #region 3、详细检查该优惠券号码是否可用
            if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedEmailVerification) && coupon.SaleRulesEx.NeedEmailVerification.ToUpper().Trim() == "Y")
            {
                if (customer.IsEmailConfirmed != 1)
                {
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedMobileVerification) && coupon.SaleRulesEx.NeedMobileVerification.ToUpper().Trim() == "Y")
            {
                if (customer.IsPhoneValided != 1)
                {
                    return false;
                }
            }
            if (coupon.SaleRulesList.Count > 0 && coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelCustomerRank))
            {
                //-1表示不限制
                if (!coupon.SaleRulesList.Exists(f => f.CustomerRank == -1))
                {
                    int customerRank = customer.CustomerRank;
                    if (!coupon.SaleRulesList.Exists(f => f.CustomerRank == customerRank))
                    {
                        return false;
                    }
                }
            }

            #endregion

            return true;
        }
    }
}
