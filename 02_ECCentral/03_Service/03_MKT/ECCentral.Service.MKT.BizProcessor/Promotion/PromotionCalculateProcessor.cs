using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;

namespace ECCentral.Service.MKT.BizProcessor 
{
    /// <summary>
    /// 为SO和RMA提供的促销引擎计算服务
    /// </summary>
    [VersionExport(typeof(PromotionCalculateProcessor))]
    public class PromotionCalculateProcessor 
    {
        
        /// <summary>
        /// 为SO提供计算当前订单能能享受的所有促销活动结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo)
        {
           return CalculateSOPromotion(soInfo, true);
        }

        /// <summary>
        /// 为SO提供计算当前订单能能享受的所有促销活动结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="isModifyCoupons"></param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, bool isModifyCoupons)
        {
            /* TODO:
             * （1）根据SOInfo中的Master和Items的信息，判断当前能够参加哪些促销活动并将相应的促销活动信息Load出来:团购、限时和赠品的活动不做处理
             * （2）按照促销活动相互间的优先、排重关系，确定能够使用哪些促销活动:先做Combo，然后得到订单中商品实际总金额；再做优惠券
             * （3）调用各促销活动中的计算方法，返回各促销活动的计算结果
             * （4) 汇总所有促销活动的计算结果
             * */
            SOInfo soInfoClone = SerializationUtility.DeepClone<SOInfo>(soInfo);
            List<SOPromotionInfo> promotionList = new List<SOPromotionInfo>();
            List<SOPromotionInfo> comboPromotionList = ObjectFactory<ComboProcessor>.Instance.CalculateSOPromotion(soInfoClone,promotionList);
            //得到Combo中所有的折扣
            decimal allComboTotalDiscount = comboPromotionList.Sum(f => f.DiscountAmount.Value);
            promotionList.AddRange(comboPromotionList);

            //应用销售立减
            soInfoClone = SerializationUtility.DeepClone<SOInfo>(soInfo);
            var saleDiscountPromoInfoList = ObjectFactory<SaleDiscountRuleProcessor>.Instance.CalculateSOPromotion(soInfoClone,promotionList);
            decimal totalSaleDiscountAmount = saleDiscountPromoInfoList.Sum(s => s.DiscountAmount.Value);
            promotionList.AddRange(saleDiscountPromoInfoList);

            //再次Clone,供Coupon计算使用
            soInfoClone = SerializationUtility.DeepClone<SOInfo>(soInfo);
            soInfoClone.BaseInfo.SOAmount = soInfoClone.BaseInfo.SOAmount.Value + allComboTotalDiscount + totalSaleDiscountAmount;
            List<SOPromotionInfo> couponPromotionList = ObjectFactory<CouponsProcessor>.Instance.CalculateSOPromotion(soInfoClone, isModifyCoupons);
            promotionList.AddRange(couponPromotionList);
         

            return promotionList;
        }
 
        /// <summary>
        /// 为RMA退货时，需要退款的促销引擎计算服务
        /// </summary>
        /// <param name="originSOSysNo"></param>
        /// <param name="rmaInfo"></param>
        /// <returns></returns>
        public virtual RMAPromotionResult CalculateRMAPromotion(int originSOSysNo, RMARegisterInfo rmaInfo)
        {
            throw new NotImplementedException();
        }
    }
}
