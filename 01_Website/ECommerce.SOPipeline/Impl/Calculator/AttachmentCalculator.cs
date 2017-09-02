using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Order;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 附件处理
    /// </summary>
    public class AttachmentCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            List<SOItemInfo> soItemList = InternalHelper.ConvertToSOItemList(order, false);
            List<int> soItemSysNoList = soItemList.Select(f => f.ProductSysNo).Distinct().ToList();
            List<OrderAttachment> unitAttachmentList = PromotionDA.GetAttachmentListByProductSysNoList(soItemSysNoList);

            order.AttachmentItemList = new List<OrderAttachment>();
            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                foreach (OrderProductItem item in itemGroup.ProductItemList)
                {
                    OrderAttachment unitAttachment = unitAttachmentList.Find(f => f.ParentProductSysNo == item.ProductSysNo);
                    if (unitAttachment != null)
                    {
                        unitAttachment = unitAttachment.Clone() as OrderAttachment;
                        unitAttachment.ParentCount = itemGroup.Quantity * item.UnitQuantity;
                        unitAttachment.ParentPackageNo = itemGroup.PackageNo;
                        order.AttachmentItemList.Add(unitAttachment);
                    }
                }
            }

        }
    }
}
