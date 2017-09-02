using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ECommerce.SOPipeline.Impl
{
    public class LogPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            foreach (var kvs in order.SubOrderList)
            {
                var subOrder = kvs.Value;
                //记录订单支付日志
                PipelineDA.UpdateFinanceNetPayForPrepay(subOrder);

                //记录订单创建日志 OrderKey:查找拆单在某步发生的日志 
                subOrder.IPAddress = HttpContext.Current.Request.UserHostAddress;
                subOrder["Note"] = string.Format("OrderKey:{0}, Customer#:{1}, RecvSysNo:{2}, Items:({3}), RecvAddress:{4}, ShipType:{5}, PayType:{6}", kvs.Key, subOrder.Customer.SysNo, subOrder.Contact.AddressAreaID, subOrder["Note"], subOrder.Contact.AddressDetail, subOrder.ShipTypeID, subOrder.PayTypeID);
                //免运费日志
                if (subOrder.HasProperty("FreeShippingChargeLog"))
                {
                    subOrder["Note"] = string.Format("{0}, {1}", subOrder["Note"], subOrder["FreeShippingChargeLog"]);
                }
                PipelineDA.UpdateSalesOrderLog(subOrder, 0);

                subOrder["Note"] = "订单已提交";
                PipelineDA.UpdateSalesOrderLog(subOrder, 200);
            }
        }
    }
}
