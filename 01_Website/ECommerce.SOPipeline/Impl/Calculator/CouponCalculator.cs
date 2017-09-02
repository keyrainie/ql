using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Member;
using ECommerce.Enums;
using ECommerce.Entity.Order;
using ECommerce.Entity;

namespace ECommerce.SOPipeline.Impl
{
    public class CouponCalculator : ICalculate
    {

        public void Calculate(ref OrderInfo order)
        {

            order.CouponErrorDesc = string.Empty;
            if (string.IsNullOrWhiteSpace(order.CouponCode))
            {
                return;
            }
            string couponCode = order.CouponCode.ToUpper().Trim();

            #region 1、初次筛选该优惠券号码对于前用户是否能用
            List<CustomerCouponInfo> customerCouponList = PromotionDA.GetCanUsingCouponCodeList(order.Customer.SysNo, order.Customer.CustomerRank);
            CustomerCouponInfo customerCoupon = customerCouponList.Find(f => f.CouponCode.ToUpper().Trim() == couponCode);
            if (customerCoupon == null)
            {
                SetCouponError(ref order, LanguageHelper.GetText("您没有此优惠券号码{0}", order.LanguageCode), couponCode);
                return;
            }
            if (customerCoupon.IsExpired)
            {
                SetCouponError(ref order, LanguageHelper.GetText("此优惠券号码{0}已过期", order.LanguageCode), couponCode);
                return;
            }
            if (customerCoupon.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= customerCoupon.CustomerMaxFrequency.Value)
            {
                SetCouponError(ref order, LanguageHelper.GetText("您已达到使用优惠券号码{0}的次数上限", order.LanguageCode), couponCode);
                return;
            }

            int totalUsedCount = 0;
            int CodeTotalUsedCount = 0;
            //用户参与活动的次数
            int CustomerUsedCount = 0;
            //if (customerCoupon.CodeType.Trim().ToUpper() == "C")
            //{
            CodeTotalUsedCount = PromotionDA.GetCouponCodeTotalUsedCount(couponCode);
            //}
            //else
            //{
            //这个是活动的已使用次数
            totalUsedCount = PromotionDA.GetCouponTotalUsedCount(customerCoupon.CouponSysNo);
            CustomerUsedCount = PromotionDA.GetCustomerCouponNumber(customerCoupon.CouponSysNo, order.Customer.SysNo);
            //}
            if (customerCoupon.WebsiteMaxFrequency.HasValue && CodeTotalUsedCount >= customerCoupon.WebsiteMaxFrequency)
            {
                SetCouponError(ref order, LanguageHelper.GetText("此优惠券号码{0}在全网使用次数已用尽，请使用其他优惠券号码。", order.LanguageCode), couponCode);
                return;
            }
            //if (customerCoupon.WebsiteMaxFrequency.HasValue && totalUsedCount >= customerCoupon.WebsiteMaxFrequency)
            //{
            //    SetCouponError(ref order, LanguageHelper.GetText("此优惠券号码{0}的已达到全网使用次数上限", order.LanguageCode), couponCode);
            //    return;
            //}
            #endregion

            #region 2、获取该优惠券号码对应的优惠券活动所有信息
            CouponInfo coupon = PromotionDA.GetComboInfoByCouponCode(order.CouponCode);
            var orderItem = order.OrderItemGroupList.FirstOrDefault();
            int merchantSysNo = orderItem == null ? 1 : orderItem.MerchantSysNo;
            string merchantName = orderItem == null ? "泰隆" : orderItem.MerchantName;
            if (coupon.MerchantSysNo != merchantSysNo && coupon.MerchantSysNo != 1)
            {
                SetCouponError(ref order, LanguageHelper.GetText(string.Format("该优惠券不是{0}优惠券", merchantName), order.LanguageCode));
                return;
            }
            if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && CustomerUsedCount >= coupon.SaleRulesEx.CustomerMaxFrequency.Value)
            //if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && customerCoupon.UsedCount >= coupon.SaleRulesEx.CustomerMaxFrequency.Value)
            {
                SetCouponError(ref order, LanguageHelper.GetText("您已达到使用此优惠券活动的次数上限", order.LanguageCode));
                return;
            }
            if (coupon.SaleRulesEx.MaxFrequency.HasValue && totalUsedCount >= coupon.SaleRulesEx.MaxFrequency.Value)
            {
                SetCouponError(ref order, LanguageHelper.GetText("此优惠券活动已达到全网使用次数上限", order.LanguageCode));
                return;
            }


            if (coupon == null)
            {
                SetCouponError(ref order, LanguageHelper.GetText("此优惠券{0}不存在", order.LanguageCode), couponCode);
                return;
            }
            #endregion

            #region 3、详细检查该优惠券号码是否可用
            if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedEmailVerification) && coupon.SaleRulesEx.NeedEmailVerification.ToUpper().Trim() == "Y")
            {
                if (order.Customer.IsEmailConfirmed != 1)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券要求客户验证了电子邮箱才可使用！", order.LanguageCode));
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(coupon.SaleRulesEx.NeedMobileVerification) && coupon.SaleRulesEx.NeedMobileVerification.ToUpper().Trim() == "Y")
            {
                if (order.Customer.IsPhoneValided != 1)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("此优惠券要求客户验证了手机才可使用！", order.LanguageCode));
                    return;
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
                        return;
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
                    return;
                }
            }


            #endregion

            #region 4、计算该优惠券可以抵扣多少，填充到OrderInfo中
            if (coupon.DiscountRuleList != null && coupon.DiscountRuleList.Count > 0)
            {
                //取得满足优惠券条件的商品总金额
                decimal canCalculateAmount = GetCanCalculateAmount(ref order, coupon);
                if (canCalculateAmount == 0)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券的商品，无法抵扣", order.LanguageCode));
                    return;
                }

                canCalculateAmount = canCalculateAmount - Math.Abs(order.TotalDiscountAmount);

                if (canCalculateAmount < coupon.SaleRulesEx.OrderAmountLimit)
                {
                    SetCouponError(ref order, LanguageHelper.GetText("没有满足该优惠券要求的商品总金额下限{0}，无法抵扣", order.LanguageCode), coupon.SaleRulesEx.OrderAmountLimit);
                    return;
                }


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

                    order.CouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                    order.CouponSysNo = customerCoupon.CouponSysNo;
                    order.CouponName = customerCoupon.CouponName;
                    order.CouponAmount = discount;
                    order.CouponErrorDesc = string.Empty;
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
                    order.CouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                    order.CouponSysNo = customerCoupon.CouponSysNo;
                    order.CouponName = customerCoupon.CouponName;
                    order.CouponAmount = discount;
                    order.CouponErrorDesc = string.Empty;

                    order["Coupon_DiscountProductSysNo"] = discountProductSysNo;
                    order["Coupon_DiscountProductCount"] = discountProductCount;
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
                    foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
                    {
                        OrderProductItem item = itemGroup.ProductItemList.Find(f => f.ProductSysNo == discountProductSysNo);
                        if (item != null)
                        {
                            discountProductCount += item.UnitQuantity * itemGroup.Quantity;
                            discountProductPrice = item.UnitSalePrice;
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
                    order.CouponCodeSysNo = customerCoupon.CouponCodeSysNo;
                    order.CouponSysNo = customerCoupon.CouponSysNo;
                    order.CouponName = customerCoupon.CouponName;
                    order.CouponAmount = discount;
                    order.CouponErrorDesc = string.Empty;

                    order["Coupon_DiscountProductSysNo"] = discountProductSysNo;
                    order["Coupon_DiscountProductCount"] = discountProductCount;
                }
                #endregion

                order["Coupon_DiscountType"] = coupon.DiscountRuleList[0].DiscountType;

            }

            #endregion

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
        /// <param name="order"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private decimal GetCanCalculateAmount(ref OrderInfo order, CouponInfo coupon)
        {
            List<SOItemInfo> soItemList = InternalHelper.ConvertToSOItemList(order, false, true);
            List<int> productSysNoList = new List<int>();
            soItemList.ForEach(f => productSysNoList.Add(f.ProductSysNo));

            //couponProductList:满足条件的商品列表，使用OrderProductItem这个对象来记录，UnitSalePrice是这个商品的价格，UnitQuantity会作为这个商品的总数量
            List<OrderProductItem> couponProductList = new List<OrderProductItem>();

            List<SimpleItemEntity> productList = PromotionDA.GetSimpleItemListBySysNumbers(productSysNoList);
            //主商品
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
            //加购商品
            if (order.PlusPriceItemList != null)
            {
                foreach (OrderGiftItem item in order.PlusPriceItemList)
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
                            couponProduct.UnitQuantity = item.UnitQuantity;
                            couponProductList.Add(couponProduct);
                        }
                        else
                        {
                            couponProduct.UnitQuantity += item.UnitQuantity;
                        }
                    }
                }
            }

            decimal totalAmount = 0.00m;
            foreach (OrderProductItem couponProduct in couponProductList)
            {
                totalAmount += couponProduct.UnitSalePrice * couponProduct.UnitQuantity;
            }

            order.CouponProductList = couponProductList;

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
                                             && f.RelationType == "N"
                                             && f.ProductSysNo == product.ProductSysNo))
                {
                    return false;
                }
                if (coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelProduct
                                             && f.RelationType == "N"
                                             && f.ProductSysNo != product.ProductSysNo))
                {
                    return true;
                }
                if (coupon.SaleRulesList.Exists(f => f.SaleRuleType == CouponSaleRuleType.RelProduct
                                             && f.RelationType == "Y"
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
            order.CouponSysNo = null;
            order.CouponCodeSysNo = null;
            order.CouponErrorDesc = string.Format(errDesc, param);
        }
        /// <summary>
        /// 获取用户平台优惠券
        /// </summary>
        /// 一张优惠券是否能被用户使用的条件
        /// 1：优惠券为面向所有用户的通用型优惠券
        /// 2: 优惠券为指定当前用户可使用通用型优惠券
        /// 3: 优惠券本身在有效期内
        /// 4：通用型优惠券所属活动在运行期间
        /// 5: 指向当前用户的投放型优惠券
        /// 6: 优惠券使用次数未尽
        /// 7：用户使用该优惠券次数未尽
        /// 8：活动全网次数未尽
        /// 9：用户参与活动次数未尽
        /// John.E.Kang 2015.11.18修改
        /// <param name="customerSysNo">用户ID</param>
        /// <param name="merchantSysNo">平台ID</param>
        /// <returns>用户可用的当前平台优惠券列表</returns>
        public static List<CustomerCouponInfo> GetCustomerPlatformCouponCode(int customerSysNo, int merchantSysNo)
        {
            //用户可用的优惠券列表
            List<CustomerCouponInfo> merchantCouponList = PromotionDA.GetMerchantCouponCodeList(customerSysNo);
            //用户当前平台可用优惠券
            List<CustomerCouponInfo> currMerchantCouponList = merchantCouponList.FindAll(m => m.MerchantSysNo == merchantSysNo || m.MerchantSysNo == 1);

            List<string> needRemoves = new List<string>();
            foreach (CustomerCouponInfo cc in currMerchantCouponList)
            {

                CouponInfo coupon = PromotionDA.GetComboInfoByCouponCode(cc.CouponCode);
                //不属于某一优惠券活动的优惠券吗无效
                if (coupon == null)
                {
                    needRemoves.Add(cc.CouponCode);
                    continue;
                }
                //获取优惠券在全网使用次数
                int totalUsedCount = 0;
                totalUsedCount = PromotionDA.GetCouponCodeTotalUsedCount(cc.CouponCode);

                int CouponTotalUsedCount = 0;
                CouponTotalUsedCount = PromotionDA.GetCouponTotalUsedCount(cc.CouponSysNo);
                //if (cc.CodeType.Trim().ToUpper() == "C")
                //{
                //    totalUsedCount = PromotionDA.GetCouponCodeTotalUsedCount(cc.CouponCode);
                //}
                //else
                //{
                //    totalUsedCount = PromotionDA.GetCouponTotalUsedCount(cc.CouponSysNo);
                //}
                //获取当前用户使用该优惠券的次数
                int customerUsedCount = PromotionDA.GetCustomerTotalUsedCount(cc.CouponCode, customerSysNo);
                //获取当前用户参与活动次数
                int customerJoinCouponCount = PromotionDA.GetCustomerCouponNumber(coupon.SysNo, customerSysNo);

                if (cc.CustomerMaxFrequency.HasValue && customerUsedCount >= cc.CustomerMaxFrequency.Value)
                {
                    needRemoves.Add(cc.CouponCode);
                    continue;
                }
                if (cc.WebsiteMaxFrequency.HasValue && totalUsedCount >= cc.WebsiteMaxFrequency.Value)
                {
                    needRemoves.Add(cc.CouponCode);
                    continue;
                }

                if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && customerJoinCouponCount >= coupon.SaleRulesEx.CustomerMaxFrequency.Value)
                {
                    needRemoves.Add(cc.CouponCode);
                    continue;
                }

                if (coupon.SaleRulesEx.MaxFrequency.HasValue && CouponTotalUsedCount >= coupon.SaleRulesEx.MaxFrequency.Value)
                {
                    needRemoves.Add(cc.CouponCode);
                    continue;
                }

                //过滤掉全网次数已用尽的优惠券活动相关的优惠券
            }

            needRemoves.ForEach(f => currMerchantCouponList.RemoveAll(c => c.CouponCode.Trim().ToUpper() == f.ToUpper().Trim()));

            return currMerchantCouponList;
        }
    }
}
