using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.SOPipeline;
using Nesoft.ECWeb.MobileService.Models.Cart;

namespace Nesoft.ECWeb.MobileService.Models.Order
{
    public class OrderInfoModel
    {
        /// <summary>
        /// cartOrOrder
        /// </summary>
        public string cartOrOrder { get; set; }

        /// <summary>
        /// 网站渠道ID
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int SOType { get; set; }

        /// <summary>
        /// 在网站下单的注册会员的信息
        /// </summary>
        public CustomerInfo Customer { get; set; }

        /// <summary>
        /// 订购人信息（实物类商品订单的就为收货人信息，服务类商品订单的就为领取服务的联系人信息）
        /// </summary>
        public ContactInfo Contact { get; set; }

        /// <summary>
        /// 所使用的支付方式
        /// </summary>
        public string PayTypeID { get; set; }

        /// <summary>
        /// 所使用的支付方式名称
        /// </summary>
        public string PayTypeName { get; set; }

        /// <summary>
        /// 所使用的快递方式
        /// </summary>
        public string ShipTypeID { get; set; }
        /// <summary>
        /// 所使用的快递方式 名称
        /// </summary>
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 所需要的发票收据信息
        /// </summary>
        public ReceiptInfo Receipt { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 使用积分抵扣的积分数量
        /// </summary>
        public int PointPay { get; set; }

        /// <summary>
        /// 积分抵扣的金额
        /// </summary>
        public decimal PointPayAmount { get; set; }

        /// <summary>
        /// 订单最大可使用积分
        /// </summary>
        public int MaxPointPay { get; set; }
        /// <summary>
        /// 如果使用积分失败，那么UsePointPayDesc中有对应的描述
        /// </summary>
        public string UsePointPayDesc { get; set; }

        /// <summary>
        /// 余额支付的金额
        /// </summary>
        public decimal BalancePayAmount { get; set; }

        /// <summary>
        /// 该订单需要支付的运费,通过Calcluator来赋值
        /// </summary>
        public decimal ShippingAmount { get; set; }

        /// <summary>
        /// 该订单需要支付的消费税,通过Calcluator来赋值
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// 该订单需要支付的消费税,通过Calcluator来赋值
        /// </summary>
        public string TaxAmountStr { get; set; }

        /// <summary>
        /// 该订单支付产生的手续费
        /// </summary>
        public decimal CommissionAmount { get; set; }

        /// <summary>
        /// 商品组
        /// </summary>
        public List<OrderItemGroupModel> OrderItemGroupList { get; set; }

        /// <summary>
        /// 订单赠品
        /// </summary>
        public List<OrderGiftItem> GiftItemList { get; set; }

        /// <summary>
        /// 订单附件
        /// </summary>
        public List<OrderAttachment> AttachmentItemList { get; set; }

        /// <summary>
        /// 该订单所有的商品折扣明细列表。每项折扣通过ProductSysNo来关联到那个商品上，如果折扣明细中ProductSysNo=0，那么则是整单满减这类
        /// </summary>
        public List<OrderItemDiscountInfo> DiscountDetailList { get; set; }

        /// <summary>
        /// 本次加入购物车商品优惠金额的小计
        /// </summary>
        public decimal TotalDiscountAmount { get; set; }

        /// <summary>
        /// 本次加入购物车商品奖励的账户余额的小计
        /// </summary>
        public decimal TotalRewardedBalance { get; set; }

        /// <summary>
        /// 本次加入购物车商品奖励的积分的小计
        /// </summary>
        public int TotalRewardedPoint { get; set; }

        /// <summary>
        /// 本次加入购物车商品减免的运费金额的小计
        /// </summary>
        public decimal TotalShipFeeDiscountAmt { get; set; }

        /// <summary>
        /// 整单重量
        /// </summary>
        public int TotalWeight { get; set; }

        /// <summary>
        /// 最大单品重量
        /// </summary>
        public int MaxWeight { get; set; }
        
        /// <summary>
        /// 所有的商品数量，包含了赠品和附件
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// 所有的被选中商品数量，包含了赠品和附件
        /// </summary>
        public int TotalProductCount
        {
            get
            {
                if (this.cartOrOrder == "Cart")
                {
                    //订单级别用户选择的赠品（包含非赠品池删除后保留的赠品和赠品池选择的赠品）
                    List<OrderGiftItem> orderGiftList = new List<OrderGiftItem>();
                    //订单级别赠品池赠品
                    List<OrderGiftItem> orderGiftPoolList = new List<OrderGiftItem>();
                    //订单级别赠品池赠品的所有活动
                    List<int> orderGiftPoolActivityNoList = new List<int>();
                    if (this.GiftItemList != null)
                    {
                        orderGiftList = this.GiftItemList.FindAll(m => m.ParentPackageNo.Equals(0)
                            && m.ParentProductSysNo.Equals(0)
                            && ((m.IsGiftPool && m.IsSelect) || !m.IsGiftPool));
                        orderGiftPoolList = this.GiftItemList.FindAll(m => m.ParentPackageNo.Equals(0)
                            && m.ParentProductSysNo.Equals(0)
                            && m.IsGiftPool);
                        orderGiftPoolActivityNoList = orderGiftPoolList.Select(m => m.ActivityNo).Distinct().ToList<int>();
                    }

                    //计算个数
                    int cartTotalItemCount = 0;
                    if (this.OrderItemGroupList != null)
                    {
                        foreach (var itemGroup in this.OrderItemGroupList)
                        {
                            if (itemGroup != null && itemGroup.ProductItemList != null && itemGroup.PackageChecked == true)
                            {
                                cartTotalItemCount += itemGroup.Quantity * itemGroup.ProductItemList.Sum(item => item.UnitQuantity);
                                // 单个商品购买
                                if (itemGroup.PackageType.Equals(0))
                                {
                                    foreach (var ProductItem in itemGroup.ProductItemList)
                                    {
                                        if (ProductItem.GiftList != null && ProductItem.GiftList.Count > 0 && ProductItem.ProductChecked == true)
                                        {
                                            cartTotalItemCount += ProductItem.GiftList.Sum(item => item.UnitQuantity * item.ParentCount);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (orderGiftList != null && orderGiftList.Count > 0)
                    {
                        foreach (var item in orderGiftList)
                        {
                            var itemGroupList = this.OrderItemGroupList.FindAll(m => m.MerchantSysNo == item.MerchantSysNo && m.PackageChecked);
                            if (itemGroupList.Count > 0 && itemGroupList != null)
                            {
                                cartTotalItemCount += item.UnitQuantity * item.ParentCount;
                            }
                        }
                    }
                    //foreach (var itemGroup in this.OrderItemGroupList)
                    //{
                    //    // 单个商品购买
                    //    if (itemGroup.PackageType.Equals(0))
                    //    {
                    //        foreach (var item in itemGroup.ProductItemList)
                    //        {
                    //            //赠品
                    //            List<OrderGiftItem> giftList = null;
                    //            if (this.GiftItemList != null && this.GiftItemList.Count > 0)
                    //            {
                    //                giftList = this.GiftItemList.FindAll(m
                    //                => m.ParentPackageNo == itemGroup.PackageNo
                    //                && m.ParentProductSysNo == item.ProductSysNo);
                    //            }
                    //            if (giftList == null)
                    //            {
                    //                giftList = new List<OrderGiftItem>();
                    //            }
                    //            if (giftList.Count > 0)
                    //            {
                    //                //非赠品池赠品
                    //                var normalGiftList = giftList.FindAll(m => !m.IsGiftPool);
                    //                foreach (var gift in normalGiftList)
                    //                {
                    //                    if (item.ProductChecked)
                    //                    {
                    //                        cartTotalItemCount += gift.UnitQuantity * gift.ParentCount;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    int totalItemCount = cartTotalItemCount;
                    return totalItemCount;
                }
                else
                {
                    int totalItemCount = 0;  
                    if (this.OrderItemGroupList != null)
                    {
                        foreach (var itemGroup in this.OrderItemGroupList)
                        {
                            if (itemGroup != null && itemGroup.ProductItemList != null)
                            {
                                totalItemCount += itemGroup.Quantity * itemGroup.ProductItemList.Sum(item => item.UnitQuantity);
                            }
                        }
                    }
                    return totalItemCount;
                }
            }
        }

        /// <summary>
        /// 所有的主商品数量（不包含赠品和附件）
        /// </summary>
        public int TotalMainProductCount
        {
            get
            {
                int totalMainProductCount = 0;
                if (this.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in this.OrderItemGroupList)
                    {
                        if (itemGroup != null && itemGroup.ProductItemList != null)
                        {
                            totalMainProductCount += itemGroup.Quantity * itemGroup.ProductItemList.Sum(item => item.UnitQuantity);
                        }
                    }
                }
                return totalMainProductCount;
            }
        }
        
        /// <summary>
        /// 优惠券代码编号
        /// </summary>
        public int? CouponCodeSysNo { get; set; }

        /// <summary>
        /// 优惠券代码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 优惠券活动编号
        /// </summary>
        public int? CouponSysNo { get; set; }

        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 本订单中优惠券可折扣金额
        /// </summary>
        public decimal CouponAmount { get; set; }

        /// <summary>
        /// 如果优惠券不满足使用规则，那么CouponCodeSysNo为空，并且CouponErrorDesc中有对应的描述
        /// </summary>
        public string CouponErrorDesc { get; set; }

        /// <summary>
        /// 优惠券限定的商品范围模式:A=所有商品，X=只限制类别和品牌，I=限制商品
        /// </summary>
        public string CoupongRulesType { get; set; }

        /// <summary>
        /// 满足使用优惠券条件的所有商品列表，使用OrderProductItem这个对象来记录：
        /// ProductSysNo是这个商品的编号，UnitSalePrice是这个商品的销售价格，UnitQuantity则作为在整个订单中的总数量
        /// </summary>
        public List<OrderProductItem> CouponProductList { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal SOAmount { get; set; }
        

        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal TotalProductAmount { get; set; }


        /// <summary>
        /// 需要现金支付的总金额
        /// </summary>
        public decimal CashPayAmount { get; set; }
        

        /// <summary>
        /// 当前用户选择的语言，主要用于Service端给出对应语言的提示
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public int OrderSource { get; set; }

        public int? CreateUserSysNo { get; set; }
        public string CreateUserName { get; set; }


        public List<KeyValuePair<string, OrderInfoModel>> SubOrderList { get; set; }

        /// <summary>
        /// 购物车编号
        /// </summary>
        public string ShoppingCartID { get; set; }




        /// <summary>
        /// 公共属性
        /// </summary>
        public string InUser { get; set; }
        public string InDate { get; set; }
        public string CompanyCode { get; set; }
        /// <summary>
        /// 创建订单IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 温馨提示
        /// </summary>
        public string WarmTips { get; set; }

        /// <summary>
        /// 配送方式描述
        /// </summary>
        public string ShipTypeDesc { get; set; }
    }
}