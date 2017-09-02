using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Order;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Product;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 本Calculator中，允许套餐和赠品的多重优惠，即比如买A赠A1。
    /// 然后有个套餐A+B ，套餐已经享受了50元的优惠（A -30，B -20)，此时A还能赠送A1。
    /// 增加了处理用户的赠品选择
    /// </summary>
    public class SaleGiftCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            GetAllSaleGift(ref order);

            #region 给赠品赋税率，运输方式等
            List<int> giftSysNoList = order.GiftItemList.Select(x => x.ProductSysNo).Distinct().ToList<int>();
            List<ProductEntryInfo> allProductEntryInfoList = PipelineDA.GetProductEntryInfo(giftSysNoList);
            foreach (OrderGiftItem giftItem in order.GiftItemList)
            {
                ProductEntryInfo entryInfo = allProductEntryInfoList.Find(f => f.ProductSysNo == giftItem.ProductSysNo);
                if (entryInfo != null)
                {
                    giftItem["ProductStoreType"] = (int)entryInfo.ProductStoreType;
                    //giftItem["TariffRate"] = entryInfo.TariffRate;
                }
                else
                {
                    giftItem["ProductStoreType"] = (int)ProductStoreType.Narmal;
                   // giftItem["TariffRate"] = 0;
                }

            }
            #endregion

            //加价购和赠品都属于赠品，都按赠品获取，之后按销售价格区分拆开
            order.PlusPriceItemList = order.GiftItemList.FindAll(m => m.UnitSalePrice > 0m);
            order.GiftItemList = order.GiftItemList.FindAll(m => m.UnitSalePrice == 0m);

            PrcessorCustomerSelectGift(ref order);
            PrcessorCustomerSelectPlusPrice(ref order);
        }

        #region GetAllSaleGift
        private void GetAllSaleGift(ref OrderInfo order)
        {
            if (order.GiftItemList == null)
            {
                order.GiftItemList = new List<OrderGiftItem>();
            }
            //1.基于性能考虑，合并订单中的所有主商品
            List<SOItemInfo> soItemList = InternalHelper.ConvertToSOItemList(order, false);
            List<SaleGiftInfo> saleGiftList = new List<SaleGiftInfo>();

            //2.得到订单中所有可能可以使用赠品活动，并进行活动排重（单品，厂商--可以直接用，同时购买和满赠--可能可以用，需要在后面处理）。
            foreach (SOItemInfo soItem in soItemList)
            {
                List<SaleGiftInfo> productSaleGiftList = PromotionDA.GetSaleGiftListByProductSysNo(soItem.ProductSysNo);
                if (productSaleGiftList != null && productSaleGiftList.Count > 0)
                {
                    foreach (SaleGiftInfo saleGift in productSaleGiftList)
                    {
                        if (!saleGiftList.Exists(f => f.SysNo == saleGift.SysNo))
                        {
                            saleGiftList.Add(saleGift);
                        }
                    }
                }
            }

            //3.遍历所有可能可以使用的赠品活动，再进一步根据order中的每一组商品，结合赠品活动类型进行处理
            if (saleGiftList.Count > 0)
            {
                foreach (SaleGiftInfo saleGift in saleGiftList)
                {
                    switch (saleGift.SaleGiftType)
                    {
                        case ECommerce.Enums.SaleGiftType.Single:
                            #region 单品处理
                            ProcessSingleGift(ref order, saleGift);
                            #endregion
                            break;
                        case ECommerce.Enums.SaleGiftType.Vendor:
                            #region 厂商赠品处理
                            ProcessSingleGift(ref order, saleGift);
                            #endregion
                            break;
                    }
                }
                //同时购买处理
                ProcessMultiGift(ref order, saleGiftList);

                //满赠处理
                ProcessFullGift(ref order, saleGiftList);
            }
        }

        //针对单品买赠这种情况：单品、厂商赠品
        private void ProcessSingleGift(ref OrderInfo order, SaleGiftInfo saleGift)
        {
            List<GiftSaleRule> saleRuleList = saleGift.GiftSaleRuleList;
            //轮询处理每个商品是否满足单品买赠这类活动
            foreach (OrderItemGroup orderItemGroup in order.OrderItemGroupList)
            {
                foreach (OrderProductItem orderProductItem in orderItemGroup.ProductItemList)
                {
                    if (saleRuleList.Exists(f => f.RelProductSysNo == orderProductItem.ProductSysNo))
                    {
                        foreach (GiftItem giftItem in saleGift.GiftItemList)
                        {
                            OrderGiftItem orderGiftItem = new OrderGiftItem()
                            {
                                ActivityNo = saleGift.SysNo.Value,
                                ActivityName = saleGift.PromotionName,
                                IsGiftPool = saleGift.IsGiftPool,
                                IsSelect = false,
                                ParentCount = orderProductItem.UnitQuantity * orderItemGroup.Quantity,
                                ParentPackageNo = orderItemGroup.PackageNo,
                                ParentProductSysNo = orderProductItem.ProductSysNo,
                                ProductID = giftItem.ProductID,
                                ProductName = giftItem.ProductName,
                                DefaultImage = giftItem.DefaultImage,
                                ProductSysNo = giftItem.ProductSysNo,
                                SaleGiftType = saleGift.SaleGiftType,
                                UnitQuantity = saleGift.IsGiftPool ? 1 : giftItem.UnitQuantity,
                                PoolLimitCount = saleGift.IsGiftPool && saleGift.ItemGiftCount.HasValue && saleGift.ItemGiftCount.Value > 0 ? saleGift.ItemGiftCount.Value : 1,
                                UnitSalePrice = giftItem.PlusPrice,
                                UnitCostPrice = giftItem.UnitCost,
                                MerchantSysNo = giftItem.MerchantSysNo,
                                Weight=giftItem.Weight,
                                UnitRewardedPoint=giftItem.UnitRewardedPoint,
                            };
                            orderGiftItem["Warranty"] = giftItem.Warranty;
                            order.GiftItemList.Add(orderGiftItem);
                        }
                    }
                }
            }

        }

        //处理同时购买
        private void ProcessMultiGift(ref OrderInfo order, List<SaleGiftInfo> saleGiftList)
        {
            List<SOItemInfo> soItemList = InternalHelper.ConvertToSOItemList(order, false);
            foreach (SaleGiftInfo saleGift in saleGiftList)
            {
                if (saleGift.SaleGiftType != ECommerce.Enums.SaleGiftType.Multiple)
                {
                    continue;
                }

                //先判断订单是否满足这个同时购买的活动
                bool isCludeMulti = true;
                int maxMultiCount = 0;

                foreach (GiftSaleRule saleruleCheck in saleGift.GiftSaleRuleList)
                {
                    SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == saleruleCheck.RelProductSysNo);
                    if (soItem == null)
                    {
                        isCludeMulti = false;
                        break;
                    }
                    int qty = soItem.Quantity / saleruleCheck.RelMinQty;
                    //不满足条件，直接跳出
                    if (qty <= 0)
                    {
                        isCludeMulti = false;
                        break;
                    }
                    if (maxMultiCount == 0)
                    {
                        maxMultiCount = qty;
                    }
                    if (qty < maxMultiCount)
                    {
                        maxMultiCount = qty;
                    }
                }


                //如果满足同时购买
                if (isCludeMulti && maxMultiCount>0)
                {
                    //先在order中加上赠品
                    foreach (GiftItem giftItem in saleGift.GiftItemList)
                    {
                        OrderGiftItem orderGiftItem = new OrderGiftItem()
                        {
                            ActivityNo = saleGift.SysNo.Value,
                            ActivityName = saleGift.PromotionName,
                            IsGiftPool = saleGift.IsGiftPool,
                            IsSelect = false,
                            ParentCount = maxMultiCount,//giftItem.UnitQuantity * 
                            ParentPackageNo = 0,
                            ParentProductSysNo = 0,
                            ProductID = giftItem.ProductID,
                            ProductName = giftItem.ProductName,
                            DefaultImage = giftItem.DefaultImage,
                            ProductSysNo = giftItem.ProductSysNo,
                            SaleGiftType = saleGift.SaleGiftType,
                            UnitQuantity = saleGift.IsGiftPool ? 1 : giftItem.UnitQuantity,
                            PoolLimitCount = saleGift.IsGiftPool && saleGift.ItemGiftCount.HasValue && saleGift.ItemGiftCount.Value > 0 ? saleGift.ItemGiftCount.Value : 1,
                            UnitSalePrice = giftItem.PlusPrice,
                            UnitCostPrice = giftItem.UnitCost,
                            MerchantSysNo = giftItem.MerchantSysNo,
                            Weight = giftItem.Weight,
                            UnitRewardedPoint = giftItem.UnitRewardedPoint,
                        };
                        orderGiftItem["Warranty"] = giftItem.Warranty;
                        order.GiftItemList.Add(orderGiftItem);
                    }
                    //移除order中符合当前同时购买的主商品或减数量，这样下一个同时购买的活动就不会重复使用满足上一个同时购买活动的主商品了
                    foreach (GiftSaleRule saleRule in saleGift.GiftSaleRuleList)
                    {
                        SOItemInfo soItem = soItemList.Find(f => f.ProductSysNo == saleRule.RelProductSysNo);
                        soItem.Quantity = soItem.Quantity - maxMultiCount * saleRule.RelMinQty;
                        if (soItem.Quantity == 0)
                        {
                            soItemList.Remove(soItem);
                        }
                    }
                }
            }
        }

        //处理满赠:当前满赠是按照重复享受满赠活动的方式
        private void ProcessFullGift(ref OrderInfo order, List<SaleGiftInfo> saleGiftList)
        {
            foreach (SaleGiftInfo saleGift in saleGiftList)
            {
                if (saleGift.SaleGiftType != ECommerce.Enums.SaleGiftType.Full)
                {
                    continue;
                }

                //订单少商品金额大于门槛金额，才能满足
                if (order.TotalProductAmount >= saleGift.AmountLimit)
                {                    
                    foreach (GiftItem giftItem in saleGift.GiftItemList)
                    {
                        OrderGiftItem orderGiftItem = new OrderGiftItem()
                        {
                            ActivityNo = saleGift.SysNo.Value,
                            ActivityName = saleGift.PromotionName,
                            IsGiftPool = saleGift.IsGiftPool,
                            IsSelect = false,
                            ParentCount = 1,
                            ParentPackageNo = 0,
                            ParentProductSysNo = 0,
                            ProductID = giftItem.ProductID,
                            ProductName = giftItem.ProductName,
                            DefaultImage = giftItem.DefaultImage,
                            ProductSysNo = giftItem.ProductSysNo,
                            SaleGiftType = saleGift.SaleGiftType,
                            UnitQuantity = saleGift.IsGiftPool ? 1 : giftItem.UnitQuantity,
                            PoolLimitCount = saleGift.IsGiftPool && saleGift.ItemGiftCount.HasValue && saleGift.ItemGiftCount.Value > 0 ? saleGift.ItemGiftCount.Value : 1,
                            UnitSalePrice = giftItem.PlusPrice,
                            UnitCostPrice = giftItem.UnitCost,
                            MerchantSysNo = giftItem.MerchantSysNo,
                            Weight = giftItem.Weight,
                            UnitRewardedPoint = giftItem.UnitRewardedPoint,
                        };
                        orderGiftItem["Warranty"] = giftItem.Warranty;
                        order.GiftItemList.Add(orderGiftItem);
                    }
                }
            }
        }
        #endregion

        #region 处理用户选择的赠品
        private void PrcessorCustomerSelectGift(ref OrderInfo order)
        {
            if (!(order["ShoppingCart"] != null && order["ShoppingCart"] is ShoppingCart))
            {
                return;
            }

            ShoppingCart shoppingCart = order["ShoppingCart"] as ShoppingCart;

            //处理用户删除/选择的优惠促销

            List<OrderGiftItem> newGiftItemList = new List<OrderGiftItem>();
            List<OrderGiftItem> userGiftItemList = new List<OrderGiftItem>();

            #region 订单级别用户删除/选择的赠品
            //根据促销计算保留的订单级别的赠品
            List<ShoppingOrderGift> shoppingLastSelectGift = new List<ShoppingOrderGift>();
            List<ShoppingOrderGift> shoppingLastDeleteGift = new List<ShoppingOrderGift>();

            //订单级别选择的赠品
            foreach (var giftItem in shoppingCart.OrderSelectGiftSysNo)
            {
                if (order.GiftItemList.Exists(m => m.ActivityNo == giftItem.ActivityNo
                            && m.ParentPackageNo == 0
                            && m.ParentProductSysNo == 0
                            && m.IsGiftPool == true
                            && m.ProductSysNo == giftItem.GiftSysNo))
                {
                    userGiftItemList.Add(new OrderGiftItem()
                    {
                        ParentPackageNo = 0,
                        ParentProductSysNo = 0,
                        ActivityNo = giftItem.ActivityNo,
                        ProductSysNo = giftItem.GiftSysNo,
                        IsGiftPool = true,
                        IsSelect = true
                    });
                    shoppingLastSelectGift.Add(giftItem);
                }
            }
            //订单级别删除的赠品
            foreach (var giftItem in shoppingCart.OrderDeleteGiftSysNo)
            {
                if (order.GiftItemList.Exists(m => m.ActivityNo == giftItem.ActivityNo
                            && m.ParentPackageNo == 0
                            && m.ParentProductSysNo == 0
                            && m.IsGiftPool == false
                            && m.ProductSysNo == giftItem.GiftSysNo))
                {
                    userGiftItemList.Add(new OrderGiftItem()
                    {
                        ParentPackageNo = 0,
                        ParentProductSysNo = 0,
                        ActivityNo = giftItem.ActivityNo,
                        ProductSysNo = giftItem.GiftSysNo,
                        IsGiftPool = false
                    });
                    shoppingLastDeleteGift.Add(giftItem);
                }
            }
            shoppingCart.OrderSelectGiftSysNo = shoppingLastSelectGift;
            shoppingCart.OrderDeleteGiftSysNo = shoppingLastDeleteGift;
            #endregion

            #region 商品上用户删除/选择的赠品
            foreach (var itemGroup in shoppingCart.ShoppingItemGroupList)
            {
                foreach (var item in itemGroup.ShoppingItemList)
                {
                    //商品选择的赠品
                    foreach (var giftItem in item.SelectGiftSysNo)
                    {
                        userGiftItemList.Add(new OrderGiftItem()
                        {
                            ParentPackageNo = itemGroup.PackageNo,
                            ParentProductSysNo = item.ProductSysNo,
                            ActivityNo = giftItem.ActivityNo,
                            ProductSysNo = giftItem.GiftSysNo,
                            IsGiftPool = true,
                            IsSelect = true
                        });
                    }
                    //商品删除的赠品
                    foreach (var giftItem in item.DeleteGiftSysNo)
                    {
                        userGiftItemList.Add(new OrderGiftItem()
                        {
                            ParentPackageNo = itemGroup.PackageNo,
                            ParentProductSysNo = item.ProductSysNo,
                            ActivityNo = giftItem.ActivityNo,
                            ProductSysNo = giftItem.GiftSysNo,
                            IsGiftPool = false
                        });
                    }
                }
            }
            #endregion

            #region 删除赠品
            if (order.GiftItemList != null && order.GiftItemList.Count > 0)
            {
                foreach (OrderGiftItem giftItem in order.GiftItemList)
                {
                    if (giftItem.IsGiftPool)
                    {
                        //是赠品池赠品，若用户选择，则标识为用户已选择，不删除
                        if (userGiftItemList.Exists(m => m.ActivityNo == giftItem.ActivityNo
                            && m.ParentPackageNo == giftItem.ParentPackageNo
                            && m.ParentProductSysNo == giftItem.ParentProductSysNo
                            && m.ProductSysNo == giftItem.ProductSysNo
                            && m.IsGiftPool == giftItem.IsGiftPool))
                        {
                            giftItem.IsSelect = true;
                        }
                        newGiftItemList.Add(giftItem);
                    }
                    else
                    {
                        //非赠品池赠品，若用户删除，则删除该赠品
                        if (!userGiftItemList.Exists(m => m.ActivityNo == giftItem.ActivityNo
                            && m.ParentPackageNo == giftItem.ParentPackageNo
                            && m.ParentProductSysNo == giftItem.ParentProductSysNo
                            && m.ProductSysNo == giftItem.ProductSysNo
                            && m.IsGiftPool == giftItem.IsGiftPool))
                        {
                            newGiftItemList.Add(giftItem);
                        }
                    }
                }
            }
            #endregion

            order.GiftItemList = newGiftItemList;
        }
        #endregion

        #region 处理用户加够的商品
        private void PrcessorCustomerSelectPlusPrice(ref OrderInfo order)
        {
            if (!(order["ShoppingCart"] != null && order["ShoppingCart"] is ShoppingCart))
            {
                return;
            }

            ShoppingCart shoppingCart = order["ShoppingCart"] as ShoppingCart;

            foreach (var giftItem in order.PlusPriceItemList)
            {
                giftItem.IsSelect = false;
                if (shoppingCart.PlusPriceProductSelectList.Exists(m => m == giftItem.ProductSysNo))
                    giftItem.IsSelect = true;
            }
        }
        #endregion
    }
}
