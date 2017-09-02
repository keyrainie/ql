using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 优惠券相关验证
    /// </summary>
    public class CouponCodeValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            if (order.CouponCodeSysNo.HasValue && order.CouponCodeSysNo > 0)
            {
                if (order.SubOrderList == null || order.SubOrderList.Count <= 0)
                {
                    if (order.SOType == (int)SOType.GroupBuy)
                    {
                        errorMsg = LanguageHelper.GetText("团购订单不能使用优惠券", order.LanguageCode);
                        return false;
                    }
                }
                else if (order.SubOrderList.Any(x => x.Value.SOType == (int)SOType.GroupBuy))
                {
                    errorMsg = LanguageHelper.GetText("团购订单不能使用优惠券", order.LanguageCode);
                    return false;
                }

                int quantity = PipelineDA.GetCouponCodeUseQuantity(order.CouponCodeSysNo.Value);
                if (quantity == 0)
                {
                    errorMsg = LanguageHelper.GetText("优惠券数量不足", order.LanguageCode);
                    return false;
                }

                quantity = PipelineDA.CouponSaleRulesExUserQuantity(order.CouponCodeSysNo.Value);
                if (quantity == 0)
                {
                    errorMsg = LanguageHelper.GetText("优惠券数量不足", order.LanguageCode);
                    return false;
                }
            }
            errorMsg = null;
            return true;
        }
    }
}
