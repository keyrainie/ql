using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Shopping;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 分仓计算逻辑
    /// </summary>
    public class AllocateStockCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            WarehouseAllocateRequest request = new WarehouseAllocateRequest();
            SetWarehouseAllocateRequest(order, request);

            WarehouseAllocateResponse response = GetWarehouseAllocateResponse(request);
            if (response.AllocateResult > -1)
            {
                SetWarehouseAllocateResult(order, response.AllocateItemInventoryInfoList);
            }
        }

        private WarehouseAllocateResponse GetWarehouseAllocateResponse(WarehouseAllocateRequest request)
        {
            WarehouseAllocateResponse response = PipelineDA.AllocateWarehouse(request);

            if (response.AllocateItemInventoryInfoList != null)
            {
                List<AllocatedItemInventoryInfo> allocatedItemInventoryInfoResult = new List<AllocatedItemInventoryInfo>();

                foreach (AllocateItemInfo productInfo in request.ProductList)
                {
                    List<AllocatedItemInventoryInfo> thisItemAllocateInventoryList = response.AllocateItemInventoryInfoList.FindAll(item =>
                    {
                        return productInfo.ProductID == item.ProductID;
                    });

                    if (thisItemAllocateInventoryList != null && thisItemAllocateInventoryList.Count > 0)
                    {
                        //删除库存不足的仓库，不做并单发货，不存在前台自动移仓单
                        thisItemAllocateInventoryList.RemoveAll(item =>
                        {
                            return (item.StockAvailableQty + item.StockConsignQty + item.StockVirtualQty) < productInfo.Quantity;
                        });

                        //筛选出最佳出库仓库
                        if (thisItemAllocateInventoryList.Count > 0)
                        {
                            FilterAllocatedItemInventoryInfo(productInfo, thisItemAllocateInventoryList);

                            allocatedItemInventoryInfoResult.AddRange(thisItemAllocateInventoryList);
                        }
                    }
                }
                response.AllocateItemInventoryInfoList = allocatedItemInventoryInfoResult;

                if (response.AllocateItemInventoryInfoList.Count == 0)
                {
                    response.AllocateResult = -1;
                }
            }

            return response;
        }

        private void FilterAllocatedItemInventoryInfo(AllocateItemInfo productInfo, List<AllocatedItemInventoryInfo> productAllocateInventoryList)
        {
            if (productAllocateInventoryList == null || productAllocateInventoryList.Count <= 0) return;

            //规则1 优先从实库仓发货
            if (productAllocateInventoryList.Exists(item =>
            {
                return item.StockAvailableQty + item.StockConsignQty > 0;
            }))
            {
                productAllocateInventoryList.RemoveAll(item =>
                {
                    return item.StockAvailableQty + item.StockConsignQty <= 0;
                });
            }

            //规则2 优先从评分最高的仓库发货
            int maxScore = productAllocateInventoryList.Max(item => item.WareHouseScore);
            productAllocateInventoryList.RemoveAll(item =>
            {
                return item.WareHouseScore < maxScore;
            });

            //规则3 优先从库存充足的仓库发货
            if (productAllocateInventoryList.Exists(item =>
            {
                return item.StockAvailableQty + item.StockConsignQty + item.StockVirtualQty >= item.ShoppingQty;
            }))
            {
                productAllocateInventoryList.RemoveAll(item =>
                {
                    return item.StockAvailableQty + item.StockConsignQty + item.StockVirtualQty < item.ShoppingQty;
                });
            }

            //规则4 优先从仓库编号最小的仓库发货
            int minWarehouseNumber = productAllocateInventoryList.Min(item => item.WarehouseNumber);
            productAllocateInventoryList.RemoveAll(item =>
            {
                return item.WarehouseNumber > minWarehouseNumber;
            });
        }

        private void SetWarehouseAllocateRequest(OrderInfo orderInfo, WarehouseAllocateRequest request)
        {
            request.ShippingAreaID = orderInfo.Contact != null ? orderInfo.Contact.AddressAreaID : 0;
            request.ProductList = new List<AllocateItemInfo>();

            if (orderInfo.OrderItemGroupList != null)
            {
                foreach (OrderItemGroup itemGroup in orderInfo.OrderItemGroupList)
                {
                    if (itemGroup.ProductItemList != null)
                    {
                        itemGroup.ProductItemList.ForEach(masterItem =>
                        {
                            SetAllocationItemInfo(masterItem, SOItemType.ForSale, itemGroup.Quantity, request.ProductList);
                        });
                    }
                }
            }
            if (orderInfo.GiftItemList != null)
            {
                orderInfo.GiftItemList.ForEach(giftItem =>
                {
                    SetAllocationItemInfo(giftItem, (giftItem.SaleGiftType == SaleGiftType.Vendor ?
                                                                      SOItemType.Gift : SOItemType.ActivityGift), giftItem.ParentCount, request.ProductList);
                });
            }
            if (orderInfo.AttachmentItemList != null)
            {
                orderInfo.AttachmentItemList.ForEach(attachment =>
                {
                    SetAllocationItemInfo(attachment, SOItemType.HiddenGift, attachment.ParentCount, request.ProductList);
                });
            }
            //加够商品
            if (orderInfo.PlusPriceItemList != null)
            {
                orderInfo.PlusPriceItemList.ForEach(plusPriceItem =>
                {
                    SetAllocationItemInfo(plusPriceItem, (plusPriceItem.SaleGiftType == SaleGiftType.Vendor ?
                                                                      SOItemType.Gift : SOItemType.ActivityGift), plusPriceItem.ParentCount, request.ProductList);
                });
            }
        }

        private void SetAllocationItemInfo<T>(T item, SOItemType itemType, int itemGroupQuantity, List<AllocateItemInfo> productList)
            where T : OrderItem
        {
            if (productList == null)
            {
                productList = new List<AllocateItemInfo>();
            }

            if (!productList.Exists(subItem =>
            {
                if (subItem.ProductID == item.ProductSysNo)
                {
                    subItem.Quantity += item.UnitQuantity * itemGroupQuantity;
                    return true;
                }
                return false;
            }))
            {
                AllocateItemInfo allocateInfo = new AllocateItemInfo();
                allocateInfo.ProductID = item.ProductSysNo;
                allocateInfo.ProductCode = item.ProductID;
                allocateInfo.ProductName = item.ProductName;
                allocateInfo.ProductType = itemType;
                allocateInfo.Quantity = item.UnitQuantity * itemGroupQuantity;
                productList.Add(allocateInfo);
            }
        }

        private void SetWarehouseAllocateResult(OrderInfo orderInfo, List<AllocatedItemInventoryInfo> allocateItemInventoryInfoList)
        {
            //设置分仓结果
            if (orderInfo.OrderItemGroupList != null)
            {
                foreach (OrderItemGroup itemGroup in orderInfo.OrderItemGroupList)
                {
                    if (itemGroup.ProductItemList != null)
                    {
                        itemGroup.ProductItemList.ForEach(masterItem =>
                        {
                            AllocatedItemInventoryInfo allocatedInfo = allocateItemInventoryInfoList.Find(item => item.ProductID == masterItem.ProductSysNo);
                            SetItemStockInfo(masterItem, allocatedInfo);
                        });
                    }
                }

                if (orderInfo.GiftItemList != null)
                {
                    orderInfo.GiftItemList.ForEach(gift =>
                    {
                        AllocatedItemInventoryInfo allocatedInfo = allocateItemInventoryInfoList.Find(item => item.ProductID == gift.ProductSysNo);
                        SetItemStockInfo(gift, allocatedInfo);
                    });
                }

                if (orderInfo.AttachmentItemList != null)
                {
                    orderInfo.AttachmentItemList.ForEach(attachment =>
                    {
                        AllocatedItemInventoryInfo allocatedInfo = allocateItemInventoryInfoList.Find(item => item.ProductID == attachment.ProductSysNo);
                        SetItemStockInfo(attachment, allocatedInfo);
                    });
                }

                //加够商品
                if (orderInfo.PlusPriceItemList != null)
                {
                    orderInfo.PlusPriceItemList.ForEach(plusPrice =>
                    {
                        AllocatedItemInventoryInfo allocatedInfo = allocateItemInventoryInfoList.Find(item => item.ProductID == plusPrice.ProductSysNo);
                        SetItemStockInfo(plusPrice, allocatedInfo);
                    });
                }
            }
        }

        private void SetItemStockInfo<T>(T item, AllocatedItemInventoryInfo allocatedInfo)
            where T : OrderItem
        {
            //设置分仓成功商品的出库仓库，没有分仓成功的商品最后会全部集中到一个子单中
            if (allocatedInfo != null)
            {
                item.WarehouseNumber = allocatedInfo.WarehouseNumber;
                item.WarehouseName = allocatedInfo.WarehouseName;
                item.WarehouseCountryCode = allocatedInfo.WarehouseCountryCode;
            }
        }
    }
}
