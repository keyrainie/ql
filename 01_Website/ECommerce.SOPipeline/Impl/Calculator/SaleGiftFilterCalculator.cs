using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    public class SaleGiftFilterCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            #region 赠品
            if (order.GiftItemList != null)
            {
                //删除掉用户没有选择的赠品池赠品
                order.GiftItemList.RemoveAll(g => g.IsGiftPool && !g.IsSelect);

                //团购订单移除厂商赠品之外的所有赠品
                if (IsGroupBuyOrder(order))
                {
                    order.GiftItemList.RemoveAll(g => g.SaleGiftType != SaleGiftType.Vendor);
                }

                //移除库存不足的赠品
                var allSaleGiftList = order.GiftItemList;
                List<OrderGiftItem> removedGiftItemList = new List<OrderGiftItem>();

                allSaleGiftList.RemoveAll(gift =>
                {
                    if (gift.WarehouseNumber <= 0)
                    {
                        removedGiftItemList.Add(gift);
                        return true;
                    }
                    return false;
                });

                if (removedGiftItemList.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    ShoppingCart shoppingCartInfo = null;
                    if ((order["ShoppingCart"] != null && order["ShoppingCart"] is ShoppingCart))
                    {
                        shoppingCartInfo = order["ShoppingCart"] as ShoppingCart;
                    }

                    sb.AppendLine(string.Format("很抱歉，以下赠品已经全部赠完！"));
                    int index = 0;
                    List<int> removedGiftItemSysnoList = new List<int>();
                    removedGiftItemList.ForEach(gift =>
                    {
                        //排除重复的赠品编号
                        if (!removedGiftItemSysnoList.Any(sysno => sysno == gift.ProductSysNo))
                        {
                            sb.AppendLine(string.Format("{0}.【{1}】", ++index, gift.ProductName));
                            removedGiftItemSysnoList.Add(gift.ProductSysNo);
                        }
                        //应对如下情况的修改：进入checkout页面时赠品库存充足，刷新时由于赠品库存不足
                        //而移除掉赠品，这时候cookie中并没有记录下移除赠品的信息，导致提交订单时再次
                        //判断赠品数量，移除库存不足的赠品，并记录下移除赠品的信息。
                        //带来的问题是，用户在checkout页面并没有看到某个赠品，却在thankyou页面和订单
                        //详情页看到赠品由于数量不足而被移除的提示信息。
                        if (shoppingCartInfo != null)
                        {
                            RemoveGiftFromShoppingCart(gift, shoppingCartInfo);
                        }
                    });
                    order.WarmTips = sb.ToString();
                }
            }
            #endregion

            #region 加购商品
            if (order.PlusPriceItemList != null)
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

        private void RemoveGiftFromShoppingCart(OrderGiftItem gift, ShoppingCart shoppingCartInfo)
        {
            if (shoppingCartInfo != null)
            {
                //订单级别的
                if (gift.ParentPackageNo == 0 && gift.ParentProductSysNo == 0)
                {
                    if (gift.IsGiftPool)
                    {
                        //赠品池
                        shoppingCartInfo.OrderSelectGiftSysNo.RemoveAll(m => m.GiftSysNo == gift.ProductSysNo);
                    }
                    else
                    {
                        shoppingCartInfo.OrderDeleteGiftSysNo.Add(new ShoppingOrderGift()
                        {
                            ActivityNo = gift.ActivityNo,
                            GiftSysNo = gift.ProductSysNo
                        });
                    }
                }
                else
                {
                    foreach (var itemGroup in shoppingCartInfo.ShoppingItemGroupList)
                    {
                        foreach (var item in itemGroup.ShoppingItemList)
                        {
                            if (gift.IsGiftPool)
                            {
                                //赠品池
                                item.SelectGiftSysNo.RemoveAll(m => m.GiftSysNo == gift.ProductSysNo);
                            }
                            else
                            {
                                item.DeleteGiftSysNo.Add(new ShoppingOrderGift()
                                {
                                    ActivityNo = gift.ActivityNo,
                                    GiftSysNo = gift.ProductSysNo
                                });
                            }
                        }
                    }
                }
            }
        }

        private bool IsGroupBuyOrder(OrderInfo order)
        {
            if (order.OrderItemGroupList != null)
            {
                return order.OrderItemGroupList.Exists((x =>
                {

                    if (x.ProductItemList != null)
                    {
                        return x.ProductItemList.Exists(y => y.SpecialActivityType == 1);
                    }
                    return false;
                }));
            }
            return false;
        }
    }
}
