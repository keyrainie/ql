using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;

namespace ECommerce.SOPipeline.Impl
{
    public class InventoryValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            List<OrderItem> allOrderItemList = GetAllOrderItemList(order);

            //step 1. 检查总库存
            if (!ValidateInventory(allOrderItemList, out errorMsg))
            {
                return false;
            }

            //step 2. 检查分仓结果，拆单之后才进行检查
            if (order.SubOrderList != null && order.SubOrderList.Count > 0)
            {
                if (!ValidateAllocateStock(allOrderItemList, out errorMsg))
                {
                    return false;
                }
            }

            errorMsg = null;
            return true;
        }

        /// <summary>
        /// 检查总库存
        /// </summary>
        /// <param name="allOrderItemList"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool ValidateInventory(List<OrderItem> allOrderItemList, out string errorMsg)
        {
            List<OrderItem> shortageOfStockItemList = new List<OrderItem>();

            if (allOrderItemList != null && allOrderItemList.Count > 0)
            {
                List<int> allOrderItemSysNoList = allOrderItemList.Select(x => x.ProductSysNo).ToList();

                List<ProductSalesInfo> allOrderItemSalesInfoList = PipelineDA.GetProductSalesInfoBySysNoList(allOrderItemSysNoList);

                if (allOrderItemSalesInfoList != null)
                {
                    foreach (var orderItem in allOrderItemList)
                    {
                        var salesInfo = allOrderItemSalesInfoList.Find(x => x.ProductSysNo == orderItem.ProductSysNo);
                        if (salesInfo == null)
                        {
                            shortageOfStockItemList.Add(orderItem);
                        }
                        else
                        {
                            if (salesInfo.OnlineQty < orderItem.UnitQuantity)
                            {
                                shortageOfStockItemList.Add(orderItem);
                            }
                        }
                    }
                }
                else
                {
                    shortageOfStockItemList.AddRange(allOrderItemList);
                }
            }

            StringBuilder msgBuilder = new StringBuilder();

            shortageOfStockItemList.Select(x => string.Format("商品【{0}】库存不足！", x.ProductName))
                                    .ToList()
                                    .ForEach(msg =>
                                    {
                                        msgBuilder.AppendLine(msg);
                                    });

            errorMsg = msgBuilder.ToString();

            if (!string.IsNullOrEmpty(errorMsg))
            {
                return false;
            }

            errorMsg = null;
            return true;

        }

        /// <summary>
        /// 检查分仓结果
        /// </summary>
        /// <param name="allOrderItemList"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool ValidateAllocateStock(List<OrderItem> allOrderItemList, out string errorMsg)
        {
            StringBuilder msgBuilder = new StringBuilder();

            allOrderItemList.Where(item => item.WarehouseNumber <= 0)
                            .Select(x => string.Format("商品【{0}】分配仓库操作失败，暂时无法为您配送！", x.ProductName))
                            .ToList()
                            .ForEach(msg =>
                            {
                                msgBuilder.AppendLine(msg);
                            });

            errorMsg = msgBuilder.ToString();

            if (!string.IsNullOrEmpty(errorMsg))
            {
                return false;
            }

            errorMsg = null;
            return true;
        }

        private List<OrderItem> GetAllOrderItemList(OrderInfo order)
        {
            List<OrderItem> allOrderItemList = new List<OrderItem>();

            foreach (var itemGroup in order.OrderItemGroupList)
            {
                if (itemGroup.ProductItemList != null)
                {
                    itemGroup.ProductItemList.ForEach(product =>
                    {
                        if (!allOrderItemList.Exists(item =>
                        {
                            if (item.ProductSysNo == product.ProductSysNo)
                            {
                                item.UnitQuantity += product.UnitQuantity * itemGroup.Quantity;
                                return true;
                            }
                            return false;
                        }))
                        {
                            product.UnitQuantity = product.UnitQuantity * itemGroup.Quantity;
                            allOrderItemList.Add(product);
                        }
                    });
                }
            }
            if (order.GiftItemList != null)
            {
                order.GiftItemList.ForEach(gift =>
                {
                    if (!allOrderItemList.Exists(item =>
                    {
                        if (item.ProductSysNo == gift.ProductSysNo)
                        {
                            item.UnitQuantity += gift.UnitQuantity * gift.ParentCount;
                            return true;
                        }
                        return false;
                    }))
                    {
                        gift.UnitQuantity = gift.UnitQuantity * gift.ParentCount;
                        allOrderItemList.Add(gift);
                    }
                });
            }
            if (order.AttachmentItemList != null)
            {
                order.AttachmentItemList.ForEach(attachment =>
                {
                    if (!allOrderItemList.Exists(item =>
                    {
                        if (item.ProductSysNo == attachment.ProductSysNo)
                        {
                            item.UnitQuantity += attachment.UnitQuantity * attachment.ParentCount;
                            return true;
                        }
                        return false;
                    }))
                    {
                        attachment.UnitQuantity = attachment.UnitQuantity * attachment.ParentCount;
                        allOrderItemList.Add(attachment);
                    }
                });
            }

            return allOrderItemList;
        }
    }
}
