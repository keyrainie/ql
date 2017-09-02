using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Order;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    public class PromotionProcessPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            foreach (var subOrder in order.SubOrderList.Values)
            {
                //创建订单捆绑销售规则
                var dicountDetailGroups = subOrder.DiscountDetailList.Where(x => x.DiscountType == 1 && x.DiscountActivityNo > 0)
                                           .GroupBy(
                                                        k => new { SaleRuleSysNo = k.DiscountActivityNo, SaleRuleName = k.DiscountActivityName },
                                                        v => v
                                                    );

                DTOInfo dtoInfo = null;
                StringBuilder note = new StringBuilder();
                decimal discount = 0m;
                foreach (var theGroup in dicountDetailGroups)
                {
                    dtoInfo = new DTOInfo();
                    discount = 0m;
                    note.Clear();
                    foreach (var item in theGroup)
                    {
                        discount += item.UnitDiscount * item.Quantity;
                        note.AppendFormat("{0},{1},{2};", item.Quantity, item.ProductSysNo, (-1) * item.UnitDiscount);
                    }
                    dtoInfo["SOSysNo"] = subOrder.ID;
                    dtoInfo["SaleRuleSysNo"] = theGroup.Key.SaleRuleSysNo;
                    dtoInfo["SaleRuleName"] = theGroup.Key.SaleRuleName;
                    dtoInfo["Discount"] = (-1m) * discount;
                    dtoInfo["Times"] = 1; //拆单的时候，套餐的折扣信息已经合并，Discount字段记录的即是总折扣
                    dtoInfo["Note"] = note.ToString();
                    PipelineDA.CreateSalesRuleInfo(dtoInfo);
                }

                //更新优惠券信息
                if (subOrder.CouponCodeSysNo.HasValue && subOrder.CouponCodeSysNo > 0)
                {
                    PipelineDA.CreateSONewPromotionLog(subOrder);
                }
                if (subOrder.MerchantCouponCodeSysNo.HasValue && subOrder.MerchantCouponCodeSysNo > 0)
                {
                    PipelineDA.CreateSONewMerchantPromotionLog(subOrder);
                }
            }
        }
    }
}
