using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 按商家填写订单备注
    /// </summary>
    public class OrderMemoCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            if (order.SubOrderList != null)
            {
                foreach (var subOrder in order.SubOrderList)
                {
                    int merchantSysNo = 0;
                    if (subOrder.Value != null
                        && subOrder.Value.OrderItemGroupList != null
                        && subOrder.Value.OrderItemGroupList.Count > 0
                        && subOrder.Value.OrderItemGroupList[0].ProductItemList != null
                        && subOrder.Value.OrderItemGroupList[0].ProductItemList.Count > 0)
                    {
                        merchantSysNo = subOrder.Value.OrderItemGroupList[0].ProductItemList[0].MerchantSysNo;
                    }
                    //subOrder.Value.Memo = "";
                    if (subOrder.Value.MerchantMemoList != null)
                    {
                        MerchantOrderMemo merchantMemo = subOrder.Value.MerchantMemoList.Find(m => m.MerchantSysNo == merchantSysNo);
                        if (subOrder.Value.MerchantMemoList.Exists(m => m.MerchantSysNo == merchantSysNo))
                        {
                            subOrder.Value.Memo = subOrder.Value.MerchantMemoList.Find(m => m.MerchantSysNo == merchantSysNo).Memo;
                        }
                    }
                }
            }
        }
    }
}
