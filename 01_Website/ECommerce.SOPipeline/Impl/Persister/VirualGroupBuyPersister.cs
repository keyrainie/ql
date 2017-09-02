using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.SOPipeline.Impl
{
    public class VirualGroupBuyPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            int soSysNo = PipelineDA.GenerateSOSysNo();
            foreach (var subOrder in order.SubOrderList.Values)
            {
                if (subOrder.OrderItemGroupList.Count > 0
                    && subOrder.OrderItemGroupList[0].ProductItemList.Count > 0)
                {
                    int quantity = subOrder.OrderItemGroupList[0].Quantity * subOrder.OrderItemGroupList[0].ProductItemList[0].UnitQuantity;
                    while (quantity > 0)
                    {
                        subOrder.ID = soSysNo;
                        PipelineDA.CreateGroupBuyingTicket(subOrder);
                        quantity--;
                    }
                }
            }
        }
    }
}
