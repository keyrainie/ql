using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 设置礼品卡订单类型
    /// </summary>
    public class SetGiftCardSOType : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            foreach (var kvs in order.SubOrderList)
            {
                kvs.Value.SOType = (int)SOType.PhysicalCard;
            }
        }
    }
}
