using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    public class CustomerAuthenticationPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            foreach (var subOrder in order.SubOrderList.Values)
            {
                //订单创建实名认证信息
                PipelineDA.CreateSOAuthentication(subOrder);
            }
        }
    }
}
