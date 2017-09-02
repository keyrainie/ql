using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 针对团购和抢购的处理
    /// </summary>
    public class SpecialActivityCalculator:ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            foreach (OrderItemGroup orderItemGroup in order.OrderItemGroupList)
            {
                foreach (OrderProductItem orderItem in orderItemGroup.ProductItemList)
                {
                    //团购
                    int groupBuySysNo = PromotionDA.ProductIsGroupBuying(orderItem.ProductSysNo);
                    if (groupBuySysNo > 0)
                    {
                        if (PromotionDA.ProductIsVirualGroupBuying(orderItem.ProductSysNo) > 0)
                        {
                            orderItem.SpecialActivityType = 3;
                            orderItem.SpecialActivitySysNo = groupBuySysNo;
                        }
                        else
                        {
                            orderItem.SpecialActivityType = 1;
                            orderItem.SpecialActivitySysNo = groupBuySysNo;
                        }
                        break;
                    }
                    //限时和秒杀
                    CountdownInfo countdown = PromotionDA.GetProductCountdownByProductSysNo(orderItem.ProductSysNo);
                    if (countdown != null)
                    {
                        orderItem.SpecialActivityType = 2;
                        orderItem.SpecialActivitySysNo =  countdown.SysNo.Value;
                        break;
                    }
                }
            }
            if (order.SubOrderList != null)
            {
                foreach (var subOrder in order.SubOrderList)
                {
                    if (subOrder.Value.SOType == (int)SOType.VirualGroupBuy)
                    {
                        subOrder.Value.ShipTypeID = "101";
                    }
                }
            }
        }
    }
}
