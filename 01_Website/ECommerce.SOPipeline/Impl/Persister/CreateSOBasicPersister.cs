using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Order;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 订单信息持久化
    /// </summary>
    public class CreateSOBasicPersister : IPersist
    {
        public void Persist(OrderInfo order)
        {
            //更新平台优惠券使用次数
            if (order.CouponCodeSysNo.HasValue && order.CouponCodeSysNo > 0)
            {
                PipelineDA.UpdateCouponCodeQuantity(order.CouponCodeSysNo.Value);
                PipelineDA.UpdateSaleRulesExQuantity(order.CouponCodeSysNo.Value);
            }
            //更新店铺优惠券使用次数
            if (order.SubOrderList != null && order.SubOrderList.Count > 0)
            {
                List<int> merchantCouponCodeSysNoList = new List<int>();
                foreach (var subOrder in order.SubOrderList.Values)
                {
                    if (subOrder.MerchantCouponCodeSysNo.HasValue && subOrder.MerchantCouponCodeSysNo > 0
                        && !merchantCouponCodeSysNoList.Exists(m => m == subOrder.MerchantCouponCodeSysNo.Value))
                    {
                        merchantCouponCodeSysNoList.Add(subOrder.MerchantCouponCodeSysNo.Value);
                    }
                }
                foreach (int couponCodeSysNo in merchantCouponCodeSysNoList)
                {
                    PipelineDA.UpdateCouponCodeQuantity(couponCodeSysNo);
                    PipelineDA.UpdateSaleRulesExQuantity(couponCodeSysNo);
                }
            }

            //生成购物车编号
            order.ShoppingCartID = PipelineDA.GenerateSOSysNo().ToString();

            foreach (var subOrder in order.SubOrderList.Values)
            {
                subOrder.ID = PipelineDA.GenerateSOSysNo();
                subOrder.ShoppingCartID = order.ShoppingCartID;
                subOrder.InDate = DateTime.Now;
                subOrder["Note"] = string.Empty;//订单日志
                subOrder["IsUsePrepay"] = (subOrder.BalancePayAmount > 0m) ? 1 : 0;

                PipelineDA.CreateShoppingCart(subOrder);//Create SO ShoppingCart
                PipelineDA.CreateSOMaster(subOrder);//Create SOMaster

                    #region SO Item
                    List<OrderItem> subOrderItemList = new List<OrderItem>();
                    subOrder.OrderItemGroupList.ForEach(orderGroup =>
                    {
                        orderGroup.ProductItemList.ForEach(proItem =>
                        {
                            
                            proItem["SpecialActivityType"] = proItem.SpecialActivityType;
                            proItem["SOSysNo"] = subOrder.ID;
                            proItem["GiftSysNo"] = null;
                            proItem["ProductType"] = (int)SOItemType.ForSale;
                            proItem["MasterProductCode"] = null;
                            proItem["Quantity"] = proItem.UnitQuantity;
                            proItem["Point"] = proItem.UnitRewardedPoint;
                            proItem["DiscountAmt"] = (decimal)proItem["UnitDiscountAmt"];
                            //总优惠券均摊金额，正数
                            proItem["PromotionDiscount"] = (decimal)proItem["UnitCouponAmt"];
                            //平台优惠券均摊金额，正数
                            proItem["PlatPromotionDiscount"] = (decimal)proItem["UnitCouponAmt"] - (proItem["MerchantUnitCouponAmt"] == null ? 0m : (decimal)proItem["MerchantUnitCouponAmt"]);
                            proItem["Price"] = proItem.UnitSalePrice - (decimal)proItem["UnitCouponAmt"];//-(decimal)proItem["UnitDiscountAmt"];
                            subOrderItemList.Add(proItem);
                        });
                    });

                    subOrder.AttachmentItemList.ForEach(proItem =>
                    {
                        proItem["SpecialActivityType"] = 0;
                        proItem["SOSysNo"] = subOrder.ID;
                        proItem["GiftSysNo"] = null;
                        proItem["ProductType"] = (int)SOItemType.HiddenGift;
                        proItem["MasterProductCode"] = null;
                        proItem["Quantity"] = proItem.UnitQuantity;
                        proItem["Price"] = 0;
                        proItem["Point"] = proItem.UnitRewardedPoint;
                        proItem["DiscountAmt"] = decimal.Zero;
                        proItem["PromotionDiscount"] = 0;
                        subOrderItemList.Add(proItem);
                    });

                    subOrder.GiftItemList.ForEach(proItem =>
                    {
                        proItem["SpecialActivityType"] = 0;
                        proItem["SOSysNo"] = subOrder.ID;
                        proItem["GiftSysNo"] = proItem.ActivityNo;
                        proItem["ProductType"] = (int)SOItemType.Gift;
                        proItem["MasterProductCode"] = null;
                        proItem["Quantity"] = proItem.UnitQuantity;
                        proItem["Price"] = 0;
                        proItem["Point"] = proItem.UnitRewardedPoint;
                        proItem["DiscountAmt"] = decimal.Zero;
                        proItem["PromotionDiscount"] = 0;
                        if (proItem.SaleGiftType == SaleGiftType.Vendor)
                        {
                            proItem["ProductType"] = (int)SOItemType.Gift;
                        }
                        else
                        {
                            proItem["ProductType"] = SOItemType.ActivityGift;//新蛋赠品(除厂商赠品外以前统一为新的赠品)
                        }
                        subOrderItemList.Add(proItem);
                    });

                    //平台
                    if (subOrder.CouponCodeSysNo.HasValue && subOrder.CouponCodeSysNo > 0)
                    {
                        OrderProductItem item = new OrderProductItem();
                        item["SOSysNo"] = subOrder.ID;
                        item.ProductSysNo = subOrder.CouponCodeSysNo.Value;
                        item.ProductName = string.Format("【平台优惠券】{0}({1})", subOrder.CouponName, subOrder.CouponCode);
                        item["GiftSysNo"] = null;
                        item["ProductType"] = (int)SOItemType.Promotion; //优惠券
                        item["MasterProductCode"] = null;
                        item.Weight = 0;
                        item["Quantity"] = 1;
                        item["ProductType"] = SOItemType.Promotion;
                        item["Price"] = decimal.Zero;
                        item.UnitCostPrice = decimal.Zero;
                        item["Point"] = 0;
                        item["DiscountAmt"] = decimal.Zero;
                        item["UnitDiscountAmt"] = 0m;
                        item["PromotionDiscount"] = 0;
                        item.UnitSalePrice = (-1) * (subOrder.CouponAmount - subOrder.MerchantCouponAmount);
                        item.WarehouseNumber = 0;
                        item.WarehouseName = string.Empty;

                        //item["TariffCode"] = null;//关税代码
                        // item["TariffRate"] = null;//单个关税
                        // item["EntryRecord"] = null;//报关编号
                        item["ProductStoreType"] = null;//储存运输方式

                        subOrderItemList.Add(item);
                    }
                    //商家
                    if (subOrder.MerchantCouponCodeSysNo.HasValue && subOrder.MerchantCouponCodeSysNo > 0)
                    {
                        OrderProductItem item = new OrderProductItem();
                        item["SOSysNo"] = subOrder.ID;
                        item.ProductSysNo = subOrder.MerchantCouponCodeSysNo.Value;
                        item.ProductName = string.Format("【商家优惠券】{0}({1})", subOrder.MerchantCouponName, subOrder.MerchantCouponCode);
                        item["GiftSysNo"] = null;
                        item["ProductType"] = (int)SOItemType.Promotion; //优惠券
                        item["MasterProductCode"] = null;
                        item.Weight = 0;
                        item["Quantity"] = 1;
                        item["ProductType"] = SOItemType.Promotion;
                        item["Price"] = decimal.Zero;
                        item.UnitCostPrice = decimal.Zero;
                        item["Point"] = 0;
                        item["DiscountAmt"] = decimal.Zero;
                        item["UnitDiscountAmt"] = 0m;
                        item["PromotionDiscount"] = 0;
                        item.UnitSalePrice = (-1) * subOrder.MerchantCouponAmount;
                        item.WarehouseNumber = 0;
                        item.WarehouseName = string.Empty;

                        //item["TariffCode"] = null;//关税代码
                        //item["TariffRate"] = null;//单个关税
                        //item["EntryRecord"] = null;//报关编号
                        item["ProductStoreType"] = null;//储存运输方式

                        subOrderItemList.Add(item);
                    }
                    #endregion

                    subOrderItemList.ForEach(item =>
                    {
                        //货币、当时汇率
                        item["CurrencySysNo"] = order["CurrencySysNo"];
                        item["ExchangeRate"] = order["ExchangeRate"];
                        
                        var productType = (int)item["ProductType"];
                        if (productType == 0 || productType == 1 || productType == 2 || productType == 5 || productType == 6)
                        {
                            var GiftSysNo = item["GiftSysNo"];//赠品
                            var SpecialActivityType = (int)item["SpecialActivityType"];// 1=团购商品；2=抢购商品；3-虚拟团购商品
                            var DiscountAmt = (decimal)item["DiscountAmt"];//捆绑的折扣
                            if (ConstValue.PaymentInventory)
                            {
                                #region
                                ////赠品
                                //if (GiftSysNo != null)
                                //{
                                //    //create soitem
                                //    PipelineDA.CreateSOItem(item);
                                //    //so item log
                                //    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                //}
                                ////团购
                                //else if (SpecialActivityType != 0 && SpecialActivityType == 1)
                                //{
                                //    //create soitem
                                //    PipelineDA.CreateSOItem(item);
                                //    //so item log
                                //    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                //}
                                #endregion
                                //限时促销
                                if (SpecialActivityType != 0 && SpecialActivityType == 2 && PipelineDA.CheckCountDownByProductSysNo(item.ProductSysNo))
                                {
                                    //create soitem
                                    PipelineDA.CreateSOItem(item);
                                    //so item log
                                    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                }
                                else
                                {
                                    //扣减订单商品库存
                                    int inventoryType = PipelineDA.GetInventoryType(item);
                                    var rowCount = PipelineDA.UpdateInventory(item);
                                    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                    {
                                        ECommerce.Utility.Logger.WriteLog("inventory: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                        throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                    }
                                    rowCount = PipelineDA.UpdateInventoryStock(item);
                                    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                    {
                                        ECommerce.Utility.Logger.WriteLog("inventory_stock: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                        throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                    }

                                    //create soitem
                                    PipelineDA.CreateSOItem(item);

                                    //so item log
                                    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                }
                                #region 所有促销活动都开启支付完成后扣减库存
                                ////正常品
                                //if (GiftSysNo == null && SpecialActivityType == 0 && !PipelineDA.CheckGiftSaleListByProductSysNo(item.ProductSysNo) && DiscountAmt == 0)
                                //{
                                //    //扣减订单商品库存
                                //    int inventoryType = PipelineDA.GetInventoryType(item);
                                //    var rowCount = PipelineDA.UpdateInventory(item);
                                //    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                //    {
                                //        ECommerce.Utility.Logger.WriteLog("inventory: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                //        throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                //    }
                                //    rowCount = PipelineDA.UpdateInventoryStock(item);
                                //    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                //    {
                                //        ECommerce.Utility.Logger.WriteLog("inventory_stock: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                //        throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                //    }

                                //    //create soitem
                                //    PipelineDA.CreateSOItem(item);

                                //    //so item log
                                //    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                //}
                                ////赠品，团购，限时促销，捆绑商品
                                //else
                                //{
                                //    //create soitem
                                //    PipelineDA.CreateSOItem(item);

                                //    //so item log
                                //    subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                                //}
                                #endregion
                            }
                            else
                            {
                                //扣减订单商品库存
                                int inventoryType = PipelineDA.GetInventoryType(item);
                                var rowCount = PipelineDA.UpdateInventory(item);
                                if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                {
                                    ECommerce.Utility.Logger.WriteLog("inventory: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                    throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                }
                                rowCount = PipelineDA.UpdateInventoryStock(item);
                                if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                {
                                    ECommerce.Utility.Logger.WriteLog("inventory_stock: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                    throw new BusinessException(string.Format("商品【{0}】库存不足", item.ProductName));
                                }

                                //create soitem
                                PipelineDA.CreateSOItem(item);

                                //so item log
                                subOrder["Note"] += string.Format("SysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                            }
                        }
                        else if (productType == 3)
                        {
                            //create soitem
                            PipelineDA.CreateSOItem(item);

                            subOrder["Note"] += string.Format("PromotionSysNo:{0}, Qty:{1}, Price:{2};", item.ProductSysNo, item["Quantity"], item["Price"]);
                        }
                    });
                
            }
        }
    }
}
