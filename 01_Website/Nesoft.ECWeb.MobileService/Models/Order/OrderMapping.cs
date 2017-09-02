using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.Utility;
using Nesoft.ECWeb.Facade.Shopping;
using Nesoft.ECWeb.MobileService.Models.Cart;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Enums;
using System.Configuration;
using Nesoft.ECWeb.MobileService.Models.App;
using Nesoft.ECWeb.MobileService.AppCode;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class OrderMapping
    {
        public static OrderInfoModel MappingOrderInfo(OrderInfo info, String cartOrOrder)
        {
            if (cartOrOrder == "Cart")
            {
                #region 购物车
                OrderInfoModel model = null;
                if (info != null)
                {
                    model = new OrderInfoModel()
                    {
                        cartOrOrder = cartOrOrder,
                        ChannelID = info.ChannelID,
                        ID = info.ID,
                        SOType = info.SOType,
                        Customer = info.Customer,
                        Contact = info.Contact,
                        PayTypeID = info.PayTypeID,
                        PayTypeName = info.PayTypeName,
                        ShipTypeID = info.ShipTypeID,
                        Receipt = info.Receipt,
                        Memo = info.Memo,
                        PointPay = info.PointPay,
                        PointPayAmount = info.PointPayAmount,
                        MaxPointPay = info.MaxPointPay,
                        UsePointPayDesc = info.UsePointPayDesc,
                        BalancePayAmount = info.BalancePayAmount,
                        ShippingAmount = info.ShippingAmount,
                        CommissionAmount = info.CommissionAmount,
                        DiscountDetailList = info.DiscountDetailList,
                        TotalDiscountAmount = info.TotalDiscountAmount,
                        TotalRewardedBalance = info.TotalRewardedBalance,
                        TotalRewardedPoint = info.TotalRewardedPoint,
                        TotalShipFeeDiscountAmt = info.TotalShipFeeDiscountAmt,
                        TotalWeight = info.TotalWeight,
                        MaxWeight = info.MaxWeight,
                        TotalItemCount = info.TotalItemCount,
                        CouponCodeSysNo = info.CouponCodeSysNo,
                        CouponCode = info.CouponCode,
                        CouponSysNo = info.CouponSysNo,
                        CouponName = info.CouponName,
                        CouponAmount = info.CouponAmount,
                        CouponErrorDesc = info.CouponErrorDesc,
                        CoupongRulesType = info.CoupongRulesType,
                        CouponProductList = info.CouponProductList,
                        SOAmount = info.SOAmount,
                        CashPayAmount = info.CashPayAmount,
                        LanguageCode = info.LanguageCode,
                        OrderSource = info.OrderSource,
                        CreateUserSysNo = info.CreateUserSysNo,
                        CreateUserName = info.CreateUserName,
                        ShoppingCartID = info.ShoppingCartID,
                        InUser = info.InUser,
                        InDate = info.InDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        CompanyCode = info.CompanyCode,
                        IPAddress = info.IPAddress,
                        WarmTips = info.WarmTips,
                        ShipTypeDesc = info["ShipTypeDesc"] == null ? "" : info["ShipTypeDesc"].ToString()
                    };

                    model.OrderItemGroupList = GetCartItemGroupList(info);
                    model.TotalProductAmount = model.OrderItemGroupList != null ? model.OrderItemGroupList.FindAll(a => a.PackageChecked).Sum(x => (x.ProductItemList.Sum(y => y.UnitSalePrice * x.Quantity * y.UnitQuantity))) : 0.00m;



                    #region 计算优惠金额
                    decimal totalDiscountAmount = 0;
                    foreach (var itemGroup in model.OrderItemGroupList)
                    {
                        // 单个商品购买
                        if (itemGroup.PackageType.Equals(0))
                        {
                            foreach (var item in itemGroup.ProductItemList)
                            {
                                //折扣
                                decimal totalUnitDiscount = 0m;
                                List<OrderItemDiscountInfo> discountList = null;
                                if (model.DiscountDetailList != null && model.DiscountDetailList.Count > 0)
                                {
                                    discountList = model.DiscountDetailList.FindAll(m
                                    => m.PackageNo == itemGroup.PackageNo
                                    && m.ProductSysNo == item.ProductSysNo);
                                    totalUnitDiscount = discountList.Sum(m => m.UnitDiscount);
                                }
                                if (item.ProductChecked)
                                {
                                    totalDiscountAmount += totalUnitDiscount * (itemGroup.Quantity * item.UnitQuantity);
                                }
                            }
                        }
                        else if (itemGroup.PackageType.Equals(1))// 套餐购买
                        {
                            foreach (var item in itemGroup.ProductItemList)
                            {
                                //折扣
                                decimal totalUnitDiscount = 0m;
                                List<OrderItemDiscountInfo> discountList = null;
                                if (model.DiscountDetailList != null && model.DiscountDetailList.Count > 0)
                                {
                                    discountList = model.DiscountDetailList.FindAll(m
                                    => m.PackageNo == itemGroup.PackageNo
                                    && m.ProductSysNo == item.ProductSysNo);
                                    totalUnitDiscount = discountList.Sum(m => m.UnitDiscount);
                                }
                                if (itemGroup.PackageChecked)
                                {
                                    totalDiscountAmount += totalUnitDiscount * (itemGroup.Quantity * item.UnitQuantity);
                                }
                            }
                        }
                    }
                    model.TotalDiscountAmount = totalDiscountAmount;

                    #endregion

                    //model.TaxAmount = model.OrderItemGroupList.Sum(x => x.ProductItemList.Sum(y => y.UnitQuantity * x.Quantity * y.UnitTaxFee));
                    model.TaxAmount = 0;
                    if (model.TaxAmount > 50)
                    {
                        model.TaxAmountStr = model.TaxAmount.ToString("F2");
                    }
                    else
                    {
                        model.TaxAmountStr = model.TaxAmount.ToString("F2") + "(免)";
                    }
                    decimal shipAmount = model.ShippingAmount - model.TotalShipFeeDiscountAmt;
                    //如果各单个商品减免的运费总和 > 运费设置，那么相当于不收取运费。
                    if (shipAmount < 0)
                    {
                        shipAmount = 0.00m;
                    }

                    model.SOAmount = (model.TotalProductAmount + (model.TaxAmount > 50 ? model.TaxAmount : 0) + shipAmount + model.CommissionAmount - model.TotalDiscountAmount);
                    model.CashPayAmount = (model.SOAmount - model.PointPayAmount - model.BalancePayAmount - model.CouponAmount);

                    if (info.SubOrderList != null && info.SubOrderList.Count > 0)
                    {

                        if (model.SubOrderList == null)
                        {
                            model.TaxAmount = 0;
                            model.SOAmount = 0;
                            model.CashPayAmount = 0;
                            model.SubOrderList = new List<KeyValuePair<string, OrderInfoModel>>();
                        }
                        foreach (var item in info.SubOrderList)
                        {
                            OrderInfoModel subModel = MappingOrderInfo(item.Value, "Order");
                            //decimal taxFee = subModel.OrderItemGroupList.Sum(x => x.ProductItemList.Sum(y => y.UnitQuantity * x.Quantity * y.UnitTaxFee));
                            decimal taxFee = 0;
                            model.TaxAmount += taxFee > 50 ? taxFee : 0;

                            decimal subShipAmount = subModel.ShippingAmount - subModel.TotalShipFeeDiscountAmt;
                            //如果各单个商品减免的运费总和 > 运费设置，那么相当于不收取运费。
                            if (subShipAmount < 0)
                            {
                                subShipAmount = 0.00m;
                            }
                            decimal subSOAmount = (subModel.TotalProductAmount + (taxFee > 50 ? taxFee : 0) + subShipAmount + subModel.CommissionAmount - subModel.TotalDiscountAmount);
                            model.SOAmount += subSOAmount;
                            decimal subCashPayAmount = (subSOAmount - subModel.PointPayAmount - subModel.BalancePayAmount - subModel.CouponAmount);
                            model.CashPayAmount += subCashPayAmount;

                            subModel.GiftItemList = item.Value != null ? MergeGift(item.Value.GiftItemList) : null;
                            subModel.AttachmentItemList = item.Value != null ? item.Value.AttachmentItemList : null;

                            model.SubOrderList.Add(new KeyValuePair<string, OrderInfoModel>(item.Key, subModel));
                        }
                    }
                    else
                    {
                        model.GiftItemList = GetGiftItem(info, 0, 0, 0);
                        model.AttachmentItemList = GetAttachment(info, 0, 0);
                    }

                    model.SOAmount = Math.Round(model.SOAmount, 2, MidpointRounding.AwayFromZero);
                    model.CashPayAmount = Math.Round(model.CashPayAmount, 2, MidpointRounding.AwayFromZero);
                }
                return model;
                #endregion
            }
            else
            {
                #region 订单核算
                OrderInfoModel model = null;
                if (info != null)
                {
                    model = new OrderInfoModel()
                    {
                        cartOrOrder = cartOrOrder,
                        ChannelID = info.ChannelID,
                        ID = info.ID,
                        SOType = info.SOType,
                        Customer = info.Customer,
                        Contact = info.Contact,
                        PayTypeID = info.PayTypeID,
                        PayTypeName = info.PayTypeName,
                        ShipTypeID = info.ShipTypeID,
                        
                        Receipt = info.Receipt,
                        Memo = info.Memo,
                        PointPay = info.PointPay,
                        PointPayAmount = info.PointPayAmount,
                        MaxPointPay = info.MaxPointPay,
                        UsePointPayDesc = info.UsePointPayDesc,
                        BalancePayAmount = info.BalancePayAmount,
                        ShippingAmount = info.ShippingAmount,
                        CommissionAmount = info.CommissionAmount,
                        DiscountDetailList = info.DiscountDetailList,
                        TotalDiscountAmount = info.TotalDiscountAmount,
                        TotalRewardedBalance = info.TotalRewardedBalance,
                        TotalRewardedPoint = info.TotalRewardedPoint,
                        TotalShipFeeDiscountAmt = info.TotalShipFeeDiscountAmt,
                        TotalWeight = info.TotalWeight,
                        MaxWeight = info.MaxWeight,
                        TotalItemCount = info.TotalItemCount,
                        CouponCodeSysNo = info.CouponCodeSysNo,
                        CouponCode = info.CouponCode,
                        CouponSysNo = info.CouponSysNo,
                        CouponName = info.CouponName,
                        CouponAmount = info.CouponAmount,
                        CouponErrorDesc = info.CouponErrorDesc,
                        CoupongRulesType = info.CoupongRulesType,
                        CouponProductList = info.CouponProductList,
                        SOAmount = info.SOAmount,
                        CashPayAmount = info.CashPayAmount,
                        LanguageCode = info.LanguageCode,
                        OrderSource = info.OrderSource,
                        CreateUserSysNo = info.CreateUserSysNo,
                        CreateUserName = info.CreateUserName,
                        ShoppingCartID = info.ShoppingCartID,
                        InUser = info.InUser,
                        InDate = info.InDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        CompanyCode = info.CompanyCode,
                        IPAddress = info.IPAddress,
                        WarmTips = info.WarmTips,
                        ShipTypeDesc = info["ShipTypeDesc"] == null ? "" : info["ShipTypeDesc"].ToString()
                    };

                    model.OrderItemGroupList = GetCartItemGroupList(info);
                    model.TotalProductAmount = model.OrderItemGroupList != null ? model.OrderItemGroupList.Sum(x => (x.ProductItemList.Sum(y => y.UnitSalePrice * x.Quantity * y.UnitQuantity))) : 0.00m;
                    //model.TaxAmount = model.OrderItemGroupList.Sum(x => x.ProductItemList.Sum(y => y.UnitQuantity * x.Quantity * y.UnitTaxFee));
                    model.TaxAmount = 0;
                    if (model.TaxAmount > 50)
                    {
                        model.TaxAmountStr = model.TaxAmount.ToString("F2");
                    }
                    else
                    {
                        model.TaxAmountStr = model.TaxAmount.ToString("F2") + "(免)";
                    }
                    decimal shipAmount = model.ShippingAmount - model.TotalShipFeeDiscountAmt;
                    //如果各单个商品减免的运费总和 > 运费设置，那么相当于不收取运费。
                    if (shipAmount < 0)
                    {
                        shipAmount = 0.00m;
                    }

                    model.SOAmount = (model.TotalProductAmount + (model.TaxAmount > 50 ? model.TaxAmount : 0) + shipAmount + model.CommissionAmount - model.TotalDiscountAmount);
                    model.CashPayAmount = (model.SOAmount - model.PointPayAmount - model.BalancePayAmount - model.CouponAmount);

                    if (info.SubOrderList != null && info.SubOrderList.Count > 0)
                    {

                        if (model.SubOrderList == null)
                        {
                            model.TaxAmount = 0;
                            model.SOAmount = 0;
                            model.CashPayAmount = 0;
                            model.SubOrderList = new List<KeyValuePair<string, OrderInfoModel>>();
                        }
                        foreach (var item in info.SubOrderList)
                        {
                            OrderInfoModel subModel = MappingOrderInfo(item.Value, "Order");
                            //decimal taxFee = subModel.OrderItemGroupList.Sum(x => x.ProductItemList.Sum(y => y.UnitQuantity * x.Quantity * y.UnitTaxFee));
                            decimal taxFee = 0;
                            model.TaxAmount += taxFee > 50 ? taxFee : 0;

                            decimal subShipAmount = subModel.ShippingAmount - subModel.TotalShipFeeDiscountAmt;
                            //如果各单个商品减免的运费总和 > 运费设置，那么相当于不收取运费。
                            if (subShipAmount < 0)
                            {
                                subShipAmount = 0.00m;
                            }
                            decimal subSOAmount = (subModel.TotalProductAmount + (taxFee > 50 ? taxFee : 0) + subShipAmount + subModel.CommissionAmount - subModel.TotalDiscountAmount);
                            model.SOAmount += subSOAmount;
                            decimal subCashPayAmount = (subSOAmount - subModel.PointPayAmount - subModel.BalancePayAmount - subModel.CouponAmount);
                            model.CashPayAmount += subCashPayAmount;

                            subModel.GiftItemList = item.Value != null ? MergeGift(item.Value.GiftItemList) : null;
                            subModel.AttachmentItemList = item.Value != null ? item.Value.AttachmentItemList : null;

                            model.SubOrderList.Add(new KeyValuePair<string, OrderInfoModel>(item.Key, subModel));
                        }
                    }
                    else
                    {
                        model.GiftItemList = GetGiftItem(info, 0, 0, 0);
                        model.AttachmentItemList = GetAttachment(info, 0, 0);
                    }

                    model.SOAmount = Math.Round(model.SOAmount, 2, MidpointRounding.AwayFromZero);
                    model.CashPayAmount = Math.Round(model.CashPayAmount, 2, MidpointRounding.AwayFromZero);
                }
                #endregion
                return model;
            }
            
        }

        public static CheckOutResultModel MappingCheckOutResult(CheckOutResult result)
        {
            CheckOutResultModel model = EntityConverter<CheckOutResult, CheckOutResultModel>.Convert(result,"Item");
            if (model != null)
            {
                //model.PayTypeList = (model.PayTypeList != null && model.PayTypeList.Count > 0) ? model.PayTypeList.FindAll(item => item.PayTypeID == 111) : null;
                if (result != null && result.OrderProcessResult != null)
                {
                    model.ReturnData = MappingOrderInfo(result.OrderProcessResult.ReturnData, "Order");
                    model.ReturnData.ShipTypeName = (model.ShipTypeList != null && model.ShipTypeList.Count > 0) ? model.ShipTypeList.FindAll(item => item.ShipTypeID == model.ReturnData.ShipTypeID).ElementAt(0).ShipTypeName : null;
                }
            }

            return model;
        }

        /// <summary>
        ///  获取商品组列表
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static List<OrderItemGroupModel> GetCartItemGroupList(OrderInfo info)
        {
            List<OrderItemGroupModel> list = new List<OrderItemGroupModel>();
            if (info != null && info.OrderItemGroupList != null && info.OrderItemGroupList.Count>0)
            {
                foreach (OrderItemGroup item in info.OrderItemGroupList)
                {
                    OrderItemGroupModel groupModel = new OrderItemGroupModel()
                    {
                        MerchantSysNo = item.MerchantSysNo,
                        MerchantName = item.MerchantName,
                        ProductItemList = GetCartItemList(info, item),
                        Quantity = item.Quantity,
                        MinCountPerSO = item.MinCountPerSO,
                        MaxCountPerSO = item.MaxCountPerSO,
                        PackageType = item.PackageType,
                        PackageNo = item.PackageNo,
                        TotalSalePrice = item.TotalSalePrice,
                        TotalQuantity = item.Quantity * (item.ProductItemList.Sum(x => x.UnitQuantity)),
                        PackageChecked = item.PackageChecked
                    };
                    groupModel.PackagePrice = groupModel.ProductItemList.Sum(price => price.TotalSalePrice);

                    list.Add(groupModel);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="info"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private static List<OrderProductItemModel> GetCartItemList(OrderInfo info, OrderItemGroup group)
        {
            List<OrderProductItemModel> list = new List<OrderProductItemModel>();
            ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Middle);
            group.ProductItemList.ForEach(item =>
            {
                OrderProductItemModel model = new OrderProductItemModel()
                {
                    ProductSysNo = item.ProductSysNo,
                    ProductID = item.ProductID,
                    ProductName = item["ProductTitle"] == null ? "" : item["ProductTitle"].ToString(),
                    SplitGroupPropertyDescList = item.SplitGroupPropertyDescList,
                    DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage),
                    ConsumptionDate = item.ConsumptionDate,
                    UnitQuantity = item.UnitQuantity,
                    UnitMarketPrice = item.UnitMarketPrice,
                    UnitCostPrice = item.UnitCostPrice,
                    UnitSalePrice = item.UnitSalePrice,
                    Weight = item.Weight,
                    TotalInventory = item.TotalInventory,
                    WarehouseNumber = item.WarehouseNumber,
                    WarehouseName = item.WarehouseName,
                    WarehouseCountryCode = item.WarehouseCountryCode,
                    MerchantSysNo = item.MerchantSysNo,
                    UnitRewardedPoint = item.UnitRewardedPoint,
                    TotalRewardedPoint = item.TotalRewardedPoint,
                    SpecialActivityType = item.SpecialActivityType,
                    SpecialActivitySysNo = item.SpecialActivitySysNo,
                    Quantity = group.Quantity * item.UnitQuantity,
                    AttachmentList = GetAttachment(info, GetPackageNo(group.PackageNo, item), item.ProductSysNo),
                    GiftList = GetGiftItem(info, GetPackageNo(group.PackageNo, item), item.ProductSysNo, 2),
                    ProductChecked = item.ProductChecked

                };
                decimal discountPrice = item["UnitDiscountAmt"] == null ? GetDiscountPrice(info, GetPackageNo(group.PackageNo, item), item.ProductSysNo) : (decimal)item["UnitDiscountAmt"];//折扣金额
                model.UnitPrice = item.UnitSalePrice - discountPrice;
               // model.UnitTaxFee = decimal.Round((decimal)item["TariffRate"] * (model.UnitPrice - (item["UnitCouponAmt"] == null ? 0m : (decimal)item["UnitCouponAmt"])), 2, MidpointRounding.AwayFromZero);//- (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"])折扣金额
                model.UnitTaxFee = (item["UnitDiscountAmt"] == null ? 0m : (decimal)item["UnitDiscountAmt"]);//折扣金额
                model.TotalSalePrice = (item.UnitSalePrice - discountPrice + model.UnitTaxFee) * model.Quantity;
                //计算是否已加入收藏夹
                model.IsWished = false;
                Nesoft.ECWeb.UI.LoginUser currUser = Nesoft.ECWeb.UI.UserMgr.ReadUserInfo();
                if (currUser != null && currUser.UserSysNo > 0)
                {
                    model.IsWished = ProductFacade.IsProductWished(item.ProductSysNo, currUser.UserSysNo);
                }

                list.Add(model);
            });
           
            return list;
        }

        /// <summary>
        /// 获取赠品
        /// </summary>
        /// <param name="info"></param>
        /// <param name="packageNo">套餐编号</param>
        /// <param name="ParentProductSysNo"></param>
        /// <param name="isGiftPool">0获取全部，1赠品池 2非赠品池</param>
        /// <returns></returns>
        private static List<OrderGiftItem> GetGiftItem(OrderInfo info, int packageNo, int parentProductSysNo, int isGiftPool)
        {
            List<OrderGiftItem> list = new List<OrderGiftItem>();
            if (info.GiftItemList != null && info.GiftItemList.Count > 0)
            {
                MobileAppConfig config = AppSettings.GetCachedConfig();
                ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Middle);
                string path = config.MobileAppServiceHost;
                foreach (OrderGiftItem item in info.GiftItemList)
                {
                    if (string.IsNullOrEmpty(item.DefaultImage))
                    {
                        item.DefaultImage = path.TrimEnd('/').TrimEnd('\\') + "/Resources/Images/shoppingcart" + (item.ProductSysNo % 4 + 1) + ".jpg";
                    }
                    else
                    {
                        item.DefaultImage = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);
                    }
                    
                    if (isGiftPool == 0 && item.ParentProductSysNo == parentProductSysNo && item.ParentPackageNo == packageNo)
                    {
                        list.Add(item);
                    }
                    else if (isGiftPool == 1 && item.ParentProductSysNo == parentProductSysNo && item.ParentPackageNo == packageNo && item.IsGiftPool == true)
                    {
                        list.Add(item);
                    }
                    else if (isGiftPool == 2 && item.ParentProductSysNo == parentProductSysNo && item.ParentPackageNo == packageNo && item.IsGiftPool == false)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="packageNo">套餐编号</param>
        /// <param name="ParentProductSysNo"></param>
        /// <returns></returns>
        private static List<OrderAttachment> GetAttachment(OrderInfo info,int packageNo, int ParentProductSysNo)
        {
            List<OrderAttachment> list = new List<OrderAttachment>();
            if (info.AttachmentItemList != null && info.AttachmentItemList.Count > 0)
            {
                list.AddRange(info.AttachmentItemList.FindAll(item => item.ParentProductSysNo == ParentProductSysNo && item.ParentPackageNo == packageNo));
            }

            return list;
        }

        /// <summary>
        /// 获取折扣单价
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ParentProductSysNo"></param>
        /// <returns></returns>
        private static decimal GetDiscountPrice(OrderInfo info,int packageNo, int ParentProductSysNo)
        {
            if (info.DiscountDetailList != null && info.DiscountDetailList.Count > 0)
            {
                return info.DiscountDetailList.FindAll(item => item.ProductSysNo == ParentProductSysNo&&item.PackageNo==packageNo).Sum(x => x.UnitDiscount);
            }

            return 0m;
        }

        /// <summary>
        /// 获取packageNo
        /// </summary>
        /// <param name="packageNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static int GetPackageNo(int packageNo, OrderProductItem item)
        {
            if (packageNo == 0)
            {
                return item["PackageNo"] != null ? (int)item["PackageNo"] : 0;
            }

            return packageNo;
        }

        /// <summary>
        /// 合并checkout赠品
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<OrderGiftItem> MergeGift(List<OrderGiftItem> list)
        {
            if (list != null && list.Count > 0)
            {
                Dictionary<int, OrderGiftItem> dic = new Dictionary<int, OrderGiftItem>();
                foreach (OrderGiftItem item in list)
                {
                    if (dic.ContainsKey(item.ProductSysNo))
                    {
                        dic[item.ProductSysNo].UnitQuantity += item.UnitQuantity;
                    }
                    else
                    {
                        dic.Add(item.ProductSysNo, item);
                    }
                }

                return dic.Values.ToList();
            }

            return list;
        }
    }
}