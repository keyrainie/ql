using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Impl
{
    public class MarginProcessPersiter : IPersist
    {
        public void Persist(OrderInfo order)
        {
            foreach (var subOrder in order.SubOrderList.Values)
            {
                PipelineDA.CreateSOItemGrossProfit(subOrder);
            }
        }
    }
}
