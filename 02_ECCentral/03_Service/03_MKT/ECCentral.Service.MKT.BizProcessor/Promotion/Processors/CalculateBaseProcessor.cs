using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.BizEntity.MKT.Promotion.Calculate;

namespace ECCentral.Service.MKT.BizProcessor
{
    public abstract class CalculateBaseProcessor
    {
        private ICouponsDA _da = ObjectFactory<ICouponsDA>.Instance;

        #region 条件检查

        /******Check基本规则**************
         * 1. 如果条件为空或者条件列表未设置，则视为不需要满足这些条件;
         * 2. 目前版本定位主要有一条不满足，则加到Error List中，然后返回不再Check；如果今后有需要Check全部，则只需要把每个Error后的Return去掉即可
         * 3. 调用方根据Check返回的ErrorList来判断是否通过，如果ErrorList的Count为0，表示通过
         * ******************************/

        /// <summary>
        /// 检查订单条件是否满足
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual List<string> CheckOrderCodition(SOInfo soInfo, PSOrderCondition condition)
        {
            List<string> errorList = new List<string>();

            if (condition == null)
            {
                return errorList;
            }
            if (condition.OrderMinAmount.HasValue)
            {
                if (!soInfo.BaseInfo.SOAmount.HasValue)
                {
                    //throw new BizException("订单信息异常:订单总金额为空值！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_OrderAmountIsNull"));
                }

                if (condition.OrderMinAmount > soInfo.BaseInfo.SOAmount)
                {
                    //errorList.Add("参加本活动时，订单金额必须达到此金额。包括商品金额、运费、手续费等所有金额，扣除捆绑等折扣，不扣除积分、礼品卡及余额");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_OrderAmountNotEnough"));
                    return errorList;
                }
            }
            if (condition.PayTypeSysNoList != null && condition.PayTypeSysNoList.Count > 0)
            {
                if (!soInfo.BaseInfo.PayTypeSysNo.HasValue)
                {
                    //throw new BizException("订单信息异常:还没有设置支付信息！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_NeedSetPayInfo"));
                }

                if (!condition.PayTypeSysNoList.Contains(soInfo.BaseInfo.PayTypeSysNo.Value))
                {
                    //errorList.Add("该客户选择的支付方式不答合优惠券要求！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_PayTypeNotMatchCoupons"));
                    return errorList;
                }
            }
            if (condition.ShippingTypeSysNoList != null && condition.ShippingTypeSysNoList.Count > 0)
            {
                if (soInfo.ShippingInfo == null || !soInfo.ShippingInfo.ShipTypeSysNo.HasValue)
                {
                    //throw new BizException("订单信息异常:还没有设置配送信息！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_NeedSetShippingInfo"));
                }

                if (!condition.ShippingTypeSysNoList.Contains(soInfo.ShippingInfo.ShipTypeSysNo.Value))
                {
                    //errorList.Add("该客户选择的配送方式不答合优惠券要求！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_ShippingNotMatchCoupons"));
                    return errorList;
                }
            }
            return errorList;
        }

        /// <summary>
        /// 检查客户条件是否满足
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual List<string> CheckCustomerCondition(SOInfo soInfo, PSCustomerCondition condition)
        {
            List<string> errorList = new List<string>();
            if (condition == null)
            {
                return errorList;
            }
            CustomerInfo customerInfo = ExternalDomainBroker.GetCustomerInfo(soInfo.BaseInfo.CustomerSysNo.Value);
            if (customerInfo == null)
            {
                //throw new BizException("订单信息异常:找不到该客户的详细信息！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_NeedCustomerInfo"));
            }
            if (condition.NeedEmailVerification.HasValue && condition.NeedEmailVerification.Value)
            {
                if (customerInfo.BasicInfo.IsEmailConfirmed == null || !customerInfo.BasicInfo.IsEmailConfirmed.Value)
                {
                    //errorList.Add("该客户的Email未验证，不符合要求！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_ErrorEmail"));
                    return errorList;
                }
            }
            if (condition.NeedMobileVerification.HasValue && condition.NeedMobileVerification.Value)
            {
                if (customerInfo.BasicInfo.CellPhoneConfirmed == null || !customerInfo.BasicInfo.CellPhoneConfirmed.Value)
                {
                    //errorList.Add("该客户的手机未验证，不符合要求！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_ErrorPhone"));
                    return errorList;
                }
            }
            if (condition.InvalidForAmbassador.HasValue && condition.InvalidForAmbassador.Value)
            {
                if (customerInfo.AgentInfo != null)
                {
                    //errorList.Add("该活动泰隆优选大使不可参加！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_CanntBeJoin"));
                    return errorList;
                }
            }
            if (condition.RelAreas != null && condition.RelAreas.AreaList != null && condition.RelAreas.AreaList.Count > 0)
            {
                int districtSysNo = customerInfo.BasicInfo.DwellAreaSysNo.Value;
                AreaInfo area = ObjectFactory<ICommonBizInteract>.Instance.GetAreaInfo(districtSysNo);
                if (condition.RelAreas.AreaList.Find(f => f.SysNo == area.ProvinceSysNo) == null)
                {
                    //errorList.Add("该客户的所属地不在本活动的限定地区范围内！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_ErrorArea"));
                    return errorList;
                }
            }

            if (condition.RelCustomerRanks != null && condition.RelCustomerRanks.CustomerRankList != null && condition.RelCustomerRanks.CustomerRankList.Count > 0)
            {
                if (condition.RelCustomerRanks.CustomerRankList[0].SysNo.Value != -1)
                {
                    if (condition.RelCustomerRanks.CustomerRankList.Find(f => f.SysNo == (int)customerInfo.Rank.Value) == null)
                    {
                        //errorList.Add("该用户不在本活动的限定用户组范围内！");
                        errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_ErrorUser"));
                        return errorList;
                    }
                }
            }

            if (condition.RelCustomers != null && condition.RelCustomers.CustomerIDList != null && condition.RelCustomers.CustomerIDList.Count > 0)
            {
                if (condition.RelCustomers.CustomerIDList.Find(f => f.CustomerSysNo == customerInfo.SysNo) == null)
                {
                    //errorList.Add("该用户不属于本活动的绑定用户！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_UserOutOfArry"));
                    return errorList;
                }
            }

            return errorList;
        }

        /// <summary>
        /// 检查活动参与次数是否满足
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        protected virtual List<string> CheckActivityFrequencyCondition(PSActivityFrequencyCondition condition)
        {
            List<string> errorList = new List<string>();
            if (condition == null)
            {
                return errorList;
            }

            if (condition.CustomerMaxFrequency.HasValue && condition.CustomerUsedFrequency.HasValue)
            {
                if (condition.CustomerUsedFrequency.Value >= condition.CustomerMaxFrequency.Value)
                {
                    //errorList.Add("该客户已参加活动的次数已经达到单个顾客的活动次数上限！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_CustomerLimitError"));
                    return errorList;
                }
            }

            if (condition.MaxFrequency.HasValue && condition.UsedFrequency.HasValue)
            {
                if (condition.UsedFrequency.Value >= condition.MaxFrequency.Value)
                {
                    //errorList.Add("全网已参加活动的次数已经达到全网的活动次数上限！");
                    errorList.Add(ResouceManager.GetMessageString("MKT.Calculate", "Calculate_CustomerLimitErrorAllApp"));
                    return errorList;
                }
            }
            return errorList;
        }

        /// <summary>
        /// 检查商品是否满足，返回输出的符合条件的订单商品列表
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="condition">活动的商品条件</param> 
        /// <returns>符合条件的订单商品列表</returns>
        protected virtual List<SOItemInfo> CheckProductConditionAndGetActivityItemList(SOInfo soInfo, CondProduct condition)
        {
            //如果条件是整站
            ConditionBase<List<SOItemInfo>> condWhole = new ConditionBase<List<SOItemInfo>>();
            if (condition.Whole != null && condition.Whole.Data.HasValue && condition.Whole.Data.Value)
            {
                condWhole.AndOrTypeRelation = AndOrType.Or;
                condWhole.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    condWhole.Data.Add(item);
                }
            }

            //如果条件只限定商家
            int merchantId = 1;
            if (soInfo.BaseInfo.Merchant != null && soInfo.BaseInfo.Merchant.MerchantID.HasValue)
            {
                merchantId = soInfo.BaseInfo.Merchant.MerchantID.Value;
            }
            ConditionBase<List<SOItemInfo>> condOnlyVendor = new ConditionBase<List<SOItemInfo>>();
            if (condition.CondBrand == null && condition.CondBrandC3 == null && condition.CondC3 == null && condition.CondItem == null
                && condition.CondVendor != null && condition.CondVendor.Find(v => v.VendorSysNo == merchantId) != null)
            {
                condOnlyVendor.AndOrTypeRelation = AndOrType.Or;
                condOnlyVendor.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    condOnlyVendor.Data.Add(item);
                }
            }


            //如果品牌方面有限制
            ConditionBase<List<SOItemInfo>> condOnlyBrand = new ConditionBase<List<SOItemInfo>>();
            if (condition.CondBrand != null && condition.CondBrand.Data != null && condition.CondBrand.Data.Count > 0)
            {
                condOnlyBrand.AndOrTypeRelation = condition.CondBrand.AndOrTypeRelation.Value;
                condOnlyBrand.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    if (condition.CondBrand.Data.Contains(item.BrandSysNo.Value))
                    {
                        condOnlyBrand.Data.Add(item);
                    }
                }
            }

            //如果类别方面有限制 
            ConditionBase<List<SOItemInfo>> condOnlyC3 = new ConditionBase<List<SOItemInfo>>();
            if (condition.CondC3 != null && condition.CondC3.Data != null && condition.CondC3.Data.Count > 0)
            {
                condOnlyC3.AndOrTypeRelation = condition.CondC3.AndOrTypeRelation.Value;
                condOnlyC3.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    if (condition.CondC3.Data.Contains(item.C3SysNo.Value))
                    {
                        condOnlyC3.Data.Add(item);
                    }
                }
            }
            //如果品牌、类别同时限制
            ConditionBase<List<SOItemInfo>> condBrandC3 = new ConditionBase<List<SOItemInfo>>();
            if (condition.CondBrandC3 != null && condition.CondBrandC3.Data != null && condition.CondBrandC3.Data.Count > 0)
            {
                condBrandC3.AndOrTypeRelation = condition.CondBrandC3.AndOrTypeRelation.Value;
                condBrandC3.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    foreach (int[] arry in condition.CondBrandC3.Data)
                    {
                        if (arry.Length != 2) continue;
                        int brandno = arry[0];
                        int c3no = arry[1];
                        if (item.BrandSysNo.Value == brandno && item.C3SysNo.Value == c3no)
                        {
                            condBrandC3.Data.Add(item);
                        }
                    }
                }
            }

            //如果存在指定商品条件 
            ConditionBase<List<SOItemInfo>> condProduct = new ConditionBase<List<SOItemInfo>>();
            if (condition.CondItem != null && condition.CondItem.Data != null && condition.CondItem.Data.Count > 0)
            {
                condProduct.AndOrTypeRelation = condition.CondItem.AndOrTypeRelation.Value;
                condProduct.Data = new List<SOItemInfo>();
                foreach (SOItemInfo item in soInfo.Items)
                {
                    if (condition.CondItem.Data.Contains(item.ProductSysNo.Value))
                    {
                        condProduct.Data.Add(item);
                    }
                }
            }

            /**************************构造树************************/
            List<ConditionBase<List<SOItemInfo>>> totalList = new List<ConditionBase<List<SOItemInfo>>>();

            if (condWhole.Data != null) totalList.Add(condWhole);
            if (condOnlyBrand.Data != null) totalList.Add(condOnlyBrand);
            if (condOnlyC3.Data != null) totalList.Add(condOnlyC3);
            if (condBrandC3.Data != null) totalList.Add(condBrandC3);
            if (condProduct.Data != null) totalList.Add(condProduct);
            if (condOnlyVendor.Data != null) totalList.Add(condOnlyVendor);
            ConditionBase<List<SOItemInfo>> totalCondition = new ConditionBase<List<SOItemInfo>>();
            BuildTree(ref totalCondition, totalList);

            List<SOItemInfo> canPromotionSOItemList = new List<SOItemInfo>();
            ParseTree(totalCondition, ref canPromotionSOItemList);



            //排除非主商品的商品，如赠品，附件等等
            RemoveNotSaleMasterProduct(canPromotionSOItemList);

            return canPromotionSOItemList;
        }


        /// <summary>
        /// 排除非主商品的商品，如：赠品、附件等
        /// </summary>
        /// <param name="promotionSOItemList"></param>
        public virtual void RemoveNotSaleMasterProduct(List<SOItemInfo> promotionSOItemList)
        {
            //排除非主商品的商品，如赠品，附件等等
            List<SOItemInfo> needRemovedList = new List<SOItemInfo>();
            foreach (SOItemInfo item in promotionSOItemList)
            {
                if (item.ProductType.Value != SOProductType.Product)
                {
                    needRemovedList.Add(item);
                }
            }
            needRemovedList.ForEach(f => promotionSOItemList.Remove(f));
        }

        #endregion

        #region 促销活动的优惠规则处理
        /// <summary>
        /// 金额方面的折扣处理：目前价格方面的折扣与总金额方面的折扣不可并存
        /// </summary>
        /// <param name="promotionInfo"></param>
        /// <param name="soInfo"></param>
        /// <param name="canPromotionSOItemList"></param>
        protected void CalcAmountRule(SOPromotionInfo promotionInfo, SOInfo soInfo,
            List<SOItemInfo> canPromotionSOItemList, CouponsInfo couponsInfo)
        {
            PSOrderAmountDiscountRule ruleAmount = couponsInfo.OrderAmountDiscountRule;
            List<PSPriceDiscountRule> rulePriceList = couponsInfo.PriceDiscountRule;
            decimal sumAllItemPriceAmount = 0.00m;          //能够参与活动的商品总金额，所有商品原价乘以数量的总金额
            decimal sumAllOrderItemAmount = 0.00m;          //所有订单商品的商品总金额,用于计算优惠券平摊金额
            decimal saleRulePromotionAmount = 0.00m;          //除去优惠券后,其它优惠金额总额
            decimal soItemAmount = soInfo.BaseInfo.SOAmount.Value; //订单商品实际总金额，已经扣去了Combo等折扣
            decimal? orderMaxDiscount = null;   //本活动中设置的每单折扣上限
            decimal calcDiscountAmount = 0.00m; //计算出来的折扣

            foreach (SOItemInfo item in canPromotionSOItemList)
            {
                sumAllItemPriceAmount += Math.Round(item.OriginalPrice.Value * ((decimal)item.Quantity.Value), 2);
            }

            foreach (var item in soInfo.Items)
            {
                if (item.ProductType.Value == SOProductType.Product || item.ProductType.Value == SOProductType.ExtendWarranty)
                {
                    sumAllOrderItemAmount += item.OriginalPrice.Value * ((decimal)item.Quantity.Value);
                    saleRulePromotionAmount += item.PromotionAmount ?? 0.00M;
                }
            }

            #region 计算总金额方面的折扣
            if (ruleAmount != null && ruleAmount.OrderAmountDiscountRank != null && ruleAmount.OrderAmountDiscountRank.Count > 0)
            {
                calcDiscountAmount = 0.00m;

                orderMaxDiscount = ruleAmount.OrderMaxDiscount;
                if (ruleAmount.OrderAmountDiscountRank != null && ruleAmount.OrderAmountDiscountRank.Count > 0)
                {
                    //首先确认取折扣信息时,根据限定金额Amount倒序
                    var ruleAmountDiscountRankNew = from p in ruleAmount.OrderAmountDiscountRank
                                                    orderby p.OrderMinAmount descending
                                                    select p;
                    foreach (OrderAmountDiscountRank rank in ruleAmountDiscountRankNew)
                    {
                        decimal minAmount = rank.OrderMinAmount.HasValue ? rank.OrderMinAmount.Value : 0.00m;

                        if (couponsInfo.ProductRangeType == CouponsProductRangeType.AllProducts && sumAllItemPriceAmount < minAmount)
                        {
                            //普通订单商品（SOItemInfo.ProductType==SOProductType.Product的商品）的总金额与限定金额值比较，跳过不满足条件价格条件。
                            continue;
                        }
                        if (soItemAmount >= minAmount)
                        {
                            if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountPercentage)
                            {
                                calcDiscountAmount = Math.Round(soItemAmount * rank.DiscountValue.Value, 2);
                            }
                            if (rank.DiscountType.Value == PSDiscountTypeForOrderAmount.OrderAmountDiscount)
                            {
                                calcDiscountAmount = Math.Round(rank.DiscountValue.Value, 2);
                            }
                            break;
                        }
                    }
                }
            }
            #endregion

            #region 计算商品价格方面的折扣
            if (rulePriceList != null && rulePriceList.Count > 0)
            {
                calcDiscountAmount = 0.00m;

                List<PSPriceDiscountRule> discountList = rulePriceList.FindAll(f => f.DiscountType == PSDiscountTypeForProductPrice.ProductPriceDiscount);
                List<PSPriceDiscountRule> finalList = rulePriceList.FindAll(f => f.DiscountType == PSDiscountTypeForProductPrice.ProductPriceFinal);
                //如果商品价格是直接折扣
                if (discountList != null && discountList.Count > 0)
                {
                    var discountListNew = from p in discountList
                                          orderby p.MinQty descending
                                          select p;
                    foreach (SOItemInfo item in canPromotionSOItemList)
                    {
                        foreach (PSPriceDiscountRule prule in discountListNew)
                        {
                            int minQty = prule.MinQty.HasValue ? prule.MinQty.Value : 1;
                            if (item.Quantity >= minQty)
                            {
                                calcDiscountAmount += Math.Round(prule.DiscountValue.Value * item.Quantity.Value, 2);
                                break;
                            }
                        }
                    }
                }

                //如果是最终售价
                if (finalList != null && finalList.Count > 0)
                {
                    var finalListNew = from p in finalList
                                       orderby p.MinQty descending
                                       select p;
                    foreach (SOItemInfo item in canPromotionSOItemList)
                    {
                        foreach (PSPriceDiscountRule prule in finalListNew)
                        {
                            int minQty = prule.MinQty.HasValue ? prule.MinQty.Value : 1;
                            if (item.Quantity >= minQty)
                            {
                                //当单个商品 最终售价大于商品售价时
                                if (prule.DiscountValue.Value >= item.Price.Value)
                                {
                                    calcDiscountAmount += 0.00m;
                                }
                                else
                                {
                                    calcDiscountAmount += Math.Round((item.Price.Value - prule.DiscountValue.Value) * item.Quantity.Value, 2);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 得到真正最终可以折扣的金额
            decimal canDiscountAmount = calcDiscountAmount;
            if (orderMaxDiscount.HasValue)
            {
                if (calcDiscountAmount <= orderMaxDiscount.Value)
                {
                    canDiscountAmount = calcDiscountAmount;
                }
                else
                {
                    canDiscountAmount = Math.Round(orderMaxDiscount.Value, 2);
                }
            }

            promotionInfo.DiscountAmount = Math.Abs(canDiscountAmount);
            #endregion

            #region 开始分摊折扣金额
            if (promotionInfo.DiscountAmount == 0.00m)
            {
                return;
            }
            //最后一个Item要特殊处理，要用总折扣减去前边所有item的折扣合
            decimal allocatedDiscount = 0.00m;
            decimal actualAllItemPriceAmount = sumAllOrderItemAmount + saleRulePromotionAmount;
            if (actualAllItemPriceAmount != 0)
            {
                for (int i = 0; i < promotionInfo.SOPromotionDetails.Count; i++)
                {
                    SOPromotionDetailInfo detail = promotionInfo.SOPromotionDetails[i];
                    SOItemInfo item = soInfo.Items.Find(f => f.ProductSysNo == detail.MasterProductSysNo);
                    //除去套餐优惠金额
                    decimal pTotal = item.OriginalPrice.Value * item.Quantity.Value + item.PromotionAmount ?? 0;
                    if (i < promotionInfo.SOPromotionDetails.Count - 1)
                    {
                        //decimal currentDiscount=decimal.Floor((canDiscountAmount * (pTotal / sumAllItemPriceAmount)) * 100) / 100;
                        decimal currentDiscount = canDiscountAmount * (pTotal / actualAllItemPriceAmount); //只保留小数点后两位,Bug:89610
                        currentDiscount = Math.Abs(currentDiscount);
                        currentDiscount = Math.Min(currentDiscount, pTotal);
                        detail.DiscountAmount = currentDiscount;
                        allocatedDiscount += currentDiscount;
                    }
                    else
                    {
                        detail.DiscountAmount = Math.Abs(canDiscountAmount - allocatedDiscount);
                        detail.DiscountAmount = Math.Min(detail.DiscountAmount.Value, pTotal);
                    }
                }
            }
            #endregion
        }


        /// <summary>
        /// 积分方面的赠送处理
        /// </summary>
        /// <param name="promotionInfo"></param>
        /// <param name="soInfo"></param>
        /// <param name="canPromotionSOItemList"></param>
        /// <param name="rule"></param>
        protected void CalcPointRule(SOPromotionInfo promotionInfo, SOInfo soInfo,
            List<SOItemInfo> canPromotionSOItemList, PSPointScoreRule rule)
        {
            if (!rule.PointScore.HasValue) return;

            int canSendPoint = rule.PointScore.Value;

            //开始分摊积分
            decimal sumAmount = 0.00m;          //能够参与活动的商品总金额  
            foreach (SOItemInfo item in canPromotionSOItemList)
            {
                sumAmount += Math.Round(item.OriginalPrice.Value * ((decimal)item.Quantity.Value), 2);
            }

            int allocatedPoint = 0;
            for (int i = 0; i < promotionInfo.SOPromotionDetails.Count; i++)
            {
                SOPromotionDetailInfo detail = promotionInfo.SOPromotionDetails[i];
                if (i < promotionInfo.SOPromotionDetails.Count - 1)
                {
                    SOItemInfo item = soInfo.Items.Find(f => f.ProductSysNo == detail.MasterProductSysNo);
                    int currentPoint = Convert.ToInt32(canSendPoint * (item.OriginalPrice.Value * item.Quantity.Value / sumAmount));
                    detail.GainPoint = currentPoint;
                    allocatedPoint += currentPoint;
                }
                else
                {
                    detail.GainPoint = canSendPoint - allocatedPoint;
                }
            }
        }

        /// <summary>
        /// 赠品方面的赠送处理
        /// </summary>
        /// <param name="promotionInfo"></param>
        /// <param name="soInfo"></param>
        /// <param name="canPromotionSOItemList"></param>
        /// <param name="rule"></param>
        protected void CalcGiftItemRule(SOPromotionInfo promotionInfo, SOInfo soInfo,
            List<SOItemInfo> canPromotionSOItemList, PSGiftItemRule rule)
        {
            //如果是只有1个主商品进行赠品，那么赠品可以全部绑定在这个这主商品下
            //如果是满足多个主商品才进行赠品，那么赠品无法绑定到各主商品下
            if (promotionInfo.SOPromotionDetails.Count == 1)
            {
                SOPromotionDetailInfo detail = promotionInfo.SOPromotionDetails[0];
                int masterProductSysNo = rule.MasterProductSysNoList[0].Value;
                // (detail.MasterProductSysNo.Value == masterProductSysNo)
                //{
                //detail.GiftList = new List<SOPromotionInfo.GiftInfo>();
                //rule.GiftItemSysNoList.ForEach(f => detail.GiftList.Add(
                //    new SOPromotionInfo.GiftInfo()
                //    {
                //        ProductSysNo = f.GiftItemSysNo.Value,
                //        Quantity = f.GiftItemCount.HasValue ? f.GiftItemCount.Value : 1
                //    }));

                //}
            }

            rule.GiftItemSysNoList.ForEach(f => promotionInfo.GiftList.Add(
                            new SOPromotionInfo.GiftInfo()
                            {
                                ProductSysNo = f.GiftItemSysNo.Value,
                                Quantity = f.GiftItemCount.HasValue ? f.GiftItemCount.Value : 1
                            }));

        }

        /// <summary>
        /// 优惠券方面的赠送处理
        /// </summary>
        /// <param name="promotionInfo"></param>
        /// <param name="soInfo"></param>
        /// <param name="canPromotionSOItemList"></param>
        /// <param name="rule"></param>
        protected void CalcCouponCodeRule(SOPromotionInfo promotionInfo, SOInfo soInfo,
            List<SOItemInfo> canPromotionSOItemList, PSCouponsRebateRule rule)
        {
            //如果是只有1个主商品进行赠送优惠券，那么赠送的优惠券可以全部绑定在这个这主商品下
            //如果是满足多个主商品才进行赠送优惠券，那么赠送的优惠券无法绑定到各主商品下
            if (promotionInfo.SOPromotionDetails.Count == 1)
            {
                if (!string.IsNullOrEmpty(rule.CouponCode))
                {
                    SOPromotionDetailInfo detail = promotionInfo.SOPromotionDetails[0];
                    //detail.CouponCodeList.Add(rule.CouponCode);
                }
            }
            if (!string.IsNullOrEmpty(rule.CouponCode))
            {
                promotionInfo.CouponCodeList.Add(rule.CouponCode);
            }
        }

        #endregion

        #region 商品条件的二叉树数据结构处理
        /******************************************************
         * 算法原理：
         * （1）按照树的深度顺序：非->与->或，构造二叉树的各节点,构造完后，再解析结果
         * （2）解析时采用后序遍历、从右到左的深度查找模式；
         *      先处理Or逻辑，这样得到最大结果；
         *      再处理And逻辑，这样得到条件交集结果；
         *      最后处理Not逻辑，排除剩下的不能被包含的结果
         * ****************************************************/
        /// <summary>
        /// 构造二叉树,每个节点都是一个ConditionBase的类型
        /// </summary>
        /// <param name="condition">根节点Condition</param>
        /// <param name="list">各条件得到商品结果的列表</param>
        /// <returns></returns>
        public ConditionBase<List<SOItemInfo>> BuildTree(ref ConditionBase<List<SOItemInfo>> condition, List<ConditionBase<List<SOItemInfo>>> list)
        {
            //先将条件列表项按照：非->与->或  进行排序
            List<ConditionBase<List<SOItemInfo>>> listSort = new List<ConditionBase<List<SOItemInfo>>>();
            foreach (ConditionBase<List<SOItemInfo>> cond in list.FindAll(f => f.AndOrTypeRelation == AndOrType.Not))
            {
                listSort.Add(cond);
            }
            foreach (ConditionBase<List<SOItemInfo>> cond in list.FindAll(f => f.AndOrTypeRelation == AndOrType.And))
            {
                listSort.Add(cond);
            }
            foreach (ConditionBase<List<SOItemInfo>> cond in list.FindAll(f => f.AndOrTypeRelation == AndOrType.Or))
            {
                listSort.Add(cond);
            }
            BuildTreeNode(condition, listSort, 0);
            return condition;
        }

        /// <summary>
        /// 遍历（非->与->或）排序后的列表，构造二叉树的各节点,构造完后，树的深度顺序将为：非->与->或
        /// 注意：数据节点始终作为左节点，右节点始终作为干节点来构造
        /// </summary>
        /// <param name="rootNode">根节点Condition</param>
        /// <param name="list">按照（非->与->或）进行排序后的列表</param>
        /// <param name="num">列表序号</param>
        protected void BuildTreeNode(ConditionBase<List<SOItemInfo>> rootNode, List<ConditionBase<List<SOItemInfo>>> list, int num)
        {
            ConditionBase<List<SOItemInfo>> leftNode = list[num];
            rootNode.LeftChild = leftNode;
            rootNode.AndOrTypeRelation = leftNode.AndOrTypeRelation;
            num++;
            if (num < list.Count)
            {
                rootNode.RightChild = new ConditionBase<List<SOItemInfo>>();
                BuildTreeNode(rootNode.RightChild, list, num);
            }
        }

        /// <summary>
        /// 解析二叉树,根据树节点的各项条件得到结果
        /// 注意：与构造二叉树时一致，采用后序遍历法，并且按照从右到左的深度查找模式；
        /// 右节点，即干节点，用来存放处理后的结果数据，然后与自己的左子节点数据进行逻辑运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="list"></param>
        public void ParseTree(ConditionBase<List<SOItemInfo>> condition, ref List<SOItemInfo> list)
        {
            if (condition != null)
            {
                ParseTree(condition.RightChild, ref list);
                ParseTree(condition.LeftChild, ref list);

                if (condition.Data == null && condition.LeftChild != null)
                {
                    condition.Data = list;
                    switch (condition.AndOrTypeRelation.Value)
                    {
                        case AndOrType.Or:
                            list = MathGetUnion(list, condition.LeftChild.Data);
                            break;
                        case AndOrType.And:
                            list = MathGetIntersection(list, condition.LeftChild.Data);
                            break;
                        case AndOrType.Not:
                            list = MathGetAfterExclude(list, condition.LeftChild.Data);
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Not 非集
        /// </summary>
        /// <param name="col1">基础集合</param>
        /// <param name="excludeCol">需要排除的集合</param>
        /// <returns></returns>
        protected List<SOItemInfo> MathGetAfterExclude(List<SOItemInfo> col1, List<SOItemInfo> excludeCol)
        {
            List<SOItemInfo> needRemoved = new List<SOItemInfo>();
            foreach (SOItemInfo item in col1)
            {
                if (excludeCol.Find(f => f.ProductSysNo == item.ProductSysNo) != null)
                {
                    needRemoved.Add(item);
                }
            }
            needRemoved.ForEach(f => col1.Remove(f));

            return col1;
        }

        /// <summary>
        /// And交集处理
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        protected List<SOItemInfo> MathGetIntersection(List<SOItemInfo> col1, List<SOItemInfo> col2)
        {
            List<SOItemInfo> list = new List<SOItemInfo>();
            foreach (SOItemInfo item in col1)
            {
                if (col2.Find(f => f.ProductSysNo == item.ProductSysNo) != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Or并集处理
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        protected List<SOItemInfo> MathGetUnion(List<SOItemInfo> col1, List<SOItemInfo> col2)
        {
            List<SOItemInfo> list = col1;
            foreach (SOItemInfo item in col2)
            {
                if (list.Find(f => f.ProductSysNo == item.ProductSysNo) == null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        #endregion


    }


}
