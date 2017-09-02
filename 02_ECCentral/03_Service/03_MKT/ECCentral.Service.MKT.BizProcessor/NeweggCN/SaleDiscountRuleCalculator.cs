using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor
{
    public class SaleDiscountRuleCalculator
    {
        public static readonly SaleDiscountRuleCalculator Instance = new SaleDiscountRuleCalculator();
        private SaleDiscountRuleCalculator()
        {
        }

        public List<SOPromotionInfo> CalcSaleDiscountRule(List<SOItemInfo> itemList, int soSysNo, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            var allValidRule = ObjectFactory<ISaleDiscountRuleDA>.Instance.GetAllValid();
            List<SOPromotionInfo> promoInfoList = new List<SOPromotionInfo>();

            //递归应用
            CalcRecursive(allValidRule, itemList, soSysNo, alreadyApplyPromoList, ref promoInfoList);

            return promoInfoList;
        }

        private void CalcRecursive(List<SaleDiscountRule> allValidRule, List<SOItemInfo> itemList, int soSysNo, List<SOPromotionInfo> alreadyApplyPromoList, ref List<SOPromotionInfo> promoInfoList)
        {
            if (itemList.Count == 0)
            {
                return;
            }

            List<SOPromotionInfo> allPromoList = new List<SOPromotionInfo>();
            //加上其它已有优惠信息
            allPromoList.AddRange(alreadyApplyPromoList);
            //加上本类促销的优惠信息
            allPromoList.AddRange(promoInfoList);

            List<SOPromotionInfo> tempList = new List<SOPromotionInfo>();
            foreach (var rule in allValidRule)
            {
                SOPromotionInfo promotionInfo = ApplyingRule(rule, itemList, soSysNo, allPromoList);
                if (promotionInfo != null)
                {
                    tempList.Add(promotionInfo);
                }
            }
            if (tempList.Count > 0)
            {
                //找出最优惠的活动
                var promoSortList = tempList.OrderBy(p => p.DiscountAmount);
                SOPromotionInfo bestPromotionInfo = promoSortList.First();
                //将最优的活动添加到应用列表中
                promoInfoList.Add(bestPromotionInfo);

                //移除已参与应用的商品
                foreach (var promoItem in bestPromotionInfo.SOPromotionDetails)
                {
                    itemList.RemoveAll(item => item.ProductSysNo == promoItem.MasterProductSysNo);
                }
                //移除已参与的活动
                allValidRule.RemoveAll(rule => rule.SysNo == bestPromotionInfo.PromotionSysNo);
            }
            else
            {
                return;
            }

            //继续递归应用
            CalcRecursive(allValidRule, itemList, soSysNo, alreadyApplyPromoList, ref promoInfoList);
        }

        private SOPromotionInfo ApplyingRule(SaleDiscountRule rule, List<SOItemInfo> itemList, int soSysNo, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            List<SOPromotionDetailInfo> promoDetailList = new List<SOPromotionDetailInfo>();
            decimal totalAmt = 0;
            int totalQty = 0;
            List<SOItemInfo> matchedItemList = new List<SOItemInfo>(itemList.Count);
            foreach (var item in itemList)
            {
                //判断三级类别，品牌，商品ID条件满足
                bool isItemMatch = IsCategoryBrandMatch(rule, item);
                if (isItemMatch)
                {
                    //加入到匹配商品列表
                    matchedItemList.Add(item);
                    if (!rule.IsSingle)
                    {
                        //计算商品已有折扣
                        decimal discountAmt = GetItemDiscount(item, alreadyApplyPromoList);

                        //非单品标记时，计算总金额和总数量
                        totalAmt += item.Quantity.Value * item.Price.Value - discountAmt;
                        totalQty += item.Quantity.Value;
                    }
                }
            }
            //应用规则
            if (rule.IsSingle)
            {
                foreach (var item in matchedItemList)
                {
                    //计算商品已有折扣
                    decimal discountAmt = GetItemDiscount(item, alreadyApplyPromoList);
                    int matchedTimes = IsSingleMatch(rule, item, discountAmt);
                    if (matchedTimes > 0)
                    {
                        var promoDetail = CalcSingleDiscount(rule, item, matchedTimes);
                        promoDetailList.Add(promoDetail);
                    }
                }
            }
            else
            {
                //非单品，根据规则类型进一步判断
                if (totalAmt > 0 && totalQty > 0)
                {
                    int matchedTimes = 0;
                    if (rule.RuleType == SaleDiscountRuleType.AmountRule)
                    {
                        matchedTimes = CalcTimesByAmt(rule, totalAmt);
                    }
                    else
                    {
                        matchedTimes = CalcTimesByQty(rule, totalQty);
                    }
                    if (matchedTimes > 0)
                    {
                        var detailList = CalcNotSingleDiscount(rule, matchedItemList, totalAmt, totalQty, matchedTimes, alreadyApplyPromoList);
                        promoDetailList.AddRange(detailList);
                    }
                }
            }

            SOPromotionInfo promotionInfo = null;
            if (promoDetailList.Count > 0)
            {
                //生成SOPromotionInfo信息
                promotionInfo = new SOPromotionInfo();
                promotionInfo.PromotionType = SOPromotionType.SaleDiscountRule;
                promotionInfo.PromotionSysNo = rule.SysNo;
                promotionInfo.PromotionName = rule.ActivityName;
                promotionInfo.DiscountAmount = -1 * promoDetailList.Sum(item => item.DiscountAmount.Value);
                promotionInfo.GainPoint = 0;
                promotionInfo.Priority = 0;
                promotionInfo.Time = 1;
                promotionInfo.SOSysNo = soSysNo;
                promotionInfo.VendorSysNo = rule.VendorSysNo ?? 0;
                promotionInfo.PromoRuleData = SerializationUtility.XmlSerialize(rule);
                promotionInfo.Discount = promotionInfo.DiscountAmount;

                promotionInfo.SOPromotionDetails = promoDetailList;
            }

            return promotionInfo;
        }

        //判断单品规则是否满足
        private int IsSingleMatch(SaleDiscountRule data, SOItemInfo soItem, decimal discountAmt)
        {
            int matchedTimes = 0;
            if (data.RuleType == SaleDiscountRuleType.AmountRule)
            {
                decimal itemAmt = soItem.Quantity.Value * soItem.Price.Value - discountAmt;
                matchedTimes = CalcTimesByAmt(data, itemAmt);
            }
            else
            {
                matchedTimes = CalcTimesByQty(data, soItem.Quantity.Value);
            }

            return matchedTimes;
        }

        //判断三级类别，品牌，商品ID条件满足
        private bool IsCategoryBrandMatch(SaleDiscountRule data, SOItemInfo soItem)
        {
            bool isItemMatch = true;
            //满足的条件数
            int matchedConditionCount = 3;
            if (data.C3SysNo > 0 && soItem.C3SysNo > 0)
            {
                isItemMatch = isItemMatch && (data.C3SysNo == soItem.C3SysNo);
            }
            else
            {
                matchedConditionCount--;
            }
            if (data.BrandSysNo > 0 && soItem.BrandSysNo > 0)
            {
                isItemMatch = isItemMatch && (data.BrandSysNo == soItem.BrandSysNo);
            }
            else
            {
                matchedConditionCount--;
            }
            if (data.ProductSysNo > 0 && data.ProductGroupSysNo > 0)
            {
                int soItemProductGroupSysNo = 0;
                //需要注意组商品概念
                isItemMatch = isItemMatch && (data.ProductSysNo == soItem.ProductSysNo || data.ProductGroupSysNo == soItemProductGroupSysNo);
            }
            else
            {
                matchedConditionCount--;
            }

            return isItemMatch && matchedConditionCount > 0;
        }

        private SOPromotionDetailInfo CalcSingleDiscount(SaleDiscountRule data, SOItemInfo item, int matchedTimes)
        {
            decimal saleDiscountAmount = data.DiscountAmount * matchedTimes;

            return GeneratePromoDetailInfo(item.ProductSysNo.Value, item.Quantity.Value, saleDiscountAmount);
        }

        /// <summary>
        /// 计算非单品标记
        /// </summary>
        /// <param name="data">规则</param>
        /// <param name="matchedItemList">商品列表</param>
        /// <param name="totalItemPrice">商品总额</param>
        /// <param name="totalItemQty">商品总数量</param>
        private List<SOPromotionDetailInfo> CalcNotSingleDiscount(SaleDiscountRule data, List<SOItemInfo> matchedItemList, decimal totalItemPrice, int totalItemQty, int matchedTimes, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            //商品折扣总值
            decimal itemTotalDiscount = data.DiscountAmount * matchedTimes;
            //已分摊的折扣
            decimal itemDistributeDiscount = 0;

            //按商品金额比例分摊
            int index = 1;
            List<SOPromotionDetailInfo> promoDetailList = new List<SOPromotionDetailInfo>();
            foreach (var item in matchedItemList)
            {
                //计算商品已有折扣
                decimal discountAmt = GetItemDiscount(item, alreadyApplyPromoList);

                bool isLast = (index == matchedItemList.Count);
                decimal saleDiscountAmount = 0;
                if (isLast)
                {
                    //最后一个用总的折扣-已分摊的折扣
                    saleDiscountAmount = itemTotalDiscount - itemDistributeDiscount;
                }
                else
                {
                    saleDiscountAmount = (item.Price.Value * item.Quantity.Value - discountAmt) / totalItemPrice * itemTotalDiscount;
                    //保留两位小数，确保分摊无误差
                    saleDiscountAmount = Math.Round(saleDiscountAmount, 2);
                    itemDistributeDiscount += saleDiscountAmount;
                }
                index++;
                var detail = GeneratePromoDetailInfo(item.ProductSysNo.Value, item.Quantity.Value, saleDiscountAmount);
                promoDetailList.Add(detail);
            }

            return promoDetailList;
        }

        private decimal GetItemDiscount(SOItemInfo item, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            //计算商品已有折扣
            decimal discountAmt = 0;
            foreach (var p in alreadyApplyPromoList)
            {
                foreach (var d in p.SOPromotionDetails)
                {
                    if (d.MasterProductSysNo == item.ProductSysNo)
                    {
                        if (d.DiscountAmount.HasValue)
                        {
                            discountAmt += d.DiscountAmount.Value;
                        }
                    }
                }
            }

            return discountAmt;
        }

        private int CalcTimesByAmt(SaleDiscountRule data, decimal itemAmt)
        {
            if (itemAmt >= 0 && itemAmt >= data.MinAmt)
            {
                if (data.MinAmt <= 0)
                {
                    return 1;
                }
                //判断循环标记
                if (data.IsCycle)
                {
                    if (itemAmt > data.MaxAmt)
                    {
                        return (int)(data.MaxAmt / data.MinAmt);
                    }
                    return (int)(itemAmt / data.MinAmt);
                }
                else
                {
                    return 1;
                }
            }

            //一次也不满足
            return 0;
        }

        private int CalcTimesByQty(SaleDiscountRule data, int itemQty)
        {
            if (itemQty >= 0 && itemQty >= data.MinQty)
            {
                if (data.MinQty <= 0)
                {
                    return 1;
                }
                //判断循环标记
                if (data.IsCycle)
                {
                    if (itemQty > data.MaxQty)
                    {
                        return data.MaxQty / data.MinQty;
                    }

                    return itemQty / data.MinQty;
                }
                else
                {
                    return 1;
                }
            }

            //一次也不满足
            return 0;
        }

        private SOPromotionDetailInfo GeneratePromoDetailInfo(int productSysNo, int qty, decimal discountAmount)
        {
            SOPromotionDetailInfo promotionDetail = new SOPromotionDetailInfo();
            promotionDetail.DiscountAmount = Math.Abs(discountAmount);
            promotionDetail.GainPoint = 0;
            promotionDetail.MasterProductQuantity = qty;
            promotionDetail.MasterProductSysNo = productSysNo;

            return promotionDetail;
        }
    }
}
