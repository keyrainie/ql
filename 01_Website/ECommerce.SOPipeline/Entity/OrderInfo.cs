using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Member;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 订单主信息，三层结构：OrderInfo > OrderItemGroup > OrderProductItem
    /// 其中OrderItemGroup对应的是Customer的一次加入购物车的购买行为
    /// </summary>
    public class OrderInfo : ExtensibleObject
    {
        
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
        /// 支付类别
        /// </summary>
        public int PaymentCategory { get; set; }

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
        /// 所需要的发票收据信息
        /// </summary>
        public ReceiptInfo Receipt { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 订单备注列表
        /// </summary>
        public List<MerchantOrderMemo> MerchantMemoList { get; set; }

        /// <summary>
        /// 使用积分抵扣的积分数量
        /// </summary>
        public int PointPay { get; set; }

        /// <summary>
        /// 积分抵扣的金额
        /// </summary>
        public decimal PointPayAmount { get; set; }

        /// <summary>
        /// 使用网银积分抵扣的积分数量
        /// </summary>
        public int BankPointPay { get; set; }

        /// <summary>
        /// 网银积分抵扣的金额
        /// </summary>
        public decimal BankPointPayAmount { get; set; }

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
        /// 该订单支付产生的手续费
        /// </summary>
        public decimal CommissionAmount { get; set; }

        /// <summary>
        /// 礼品卡支付的金额 
        /// </summary>
        public decimal GiftCardPayAmount { get; set; }

        /// <summary>
        /// 客户购物时，每次加入购物车操作的购物列表
        /// </summary>
        public List<OrderItemGroup> OrderItemGroupList { get; set; }

        /// <summary>
        /// 该订单所有加价购的商品列表
        /// </summary>
        public List<OrderGiftItem> PlusPriceItemList { get; set; }

        /// <summary>
        /// 该订单所有的赠品列表。每个赠品通过ParentProductSysNo来识别关联到哪个主商品上，如果ParentProductSysNo=0则是整单满赠
        /// </summary>
        public List<OrderGiftItem> GiftItemList { get; set; }

        public List<OrderAttachment> AttachmentItemList { get; set; }


        /// <summary>
        /// 该订单所有的商品折扣明细列表。每项折扣通过ProductSysNo来关联到那个商品上，如果折扣明细中ProductSysNo=0，那么则是整单满减这类
        /// </summary>
        public List<OrderItemDiscountInfo> DiscountDetailList { get; set; }

        /// <summary>
        /// 本次加入购物车商品优惠金额的小计
        /// </summary>
        public decimal TotalDiscountAmount { get { return (DiscountDetailList == null || DiscountDetailList.Count <= 0) ? 0 : DiscountDetailList.Sum(x => x.UnitDiscount * x.Quantity); } }

        /// <summary>
        /// 本次加入购物车商品奖励的账户余额的小计
        /// </summary>
        public decimal TotalRewardedBalance { get { return (DiscountDetailList == null || DiscountDetailList.Count <= 0) ? 0 : DiscountDetailList.Sum(x => x.UnitRewardedBalance * x.Quantity); } }

        /// <summary>
        /// 本次加入购物车商品奖励的积分的小计
        /// </summary>
        public int TotalRewardedPoint
        {
            get
            {
                int totalRewardedPoint = 0;
                totalRewardedPoint += (DiscountDetailList == null || DiscountDetailList.Count <= 0) ? 0 : DiscountDetailList.Sum(x => x.UnitRewardedPoint * x.Quantity);

                if (this.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            totalRewardedPoint += itemGroup.ProductItemList.Sum(item => item.TotalRewardedPoint * itemGroup.Quantity);
                        }
                    }
                }
                if (this.GiftItemList != null)
                {
                    totalRewardedPoint += this.GiftItemList.Sum(item => item.TotalRewardedPoint);
                }
                if (this.AttachmentItemList != null)
                {
                    totalRewardedPoint += this.AttachmentItemList.Sum(item => item.TotalRewardedPoint);
                }

                return totalRewardedPoint;
            }

        }

        /// <summary>
        /// 本次加入购物车商品减免的运费金额的小计
        /// </summary>
        public decimal TotalShipFeeDiscountAmt { get { return (DiscountDetailList == null || DiscountDetailList.Count <= 0) ? 0 : DiscountDetailList.Sum(x => x.UnitShipFeeDiscountAmt * x.Quantity); } }


        /// <summary>
        /// 整单重量
        /// </summary>
        public int TotalWeight
        {
            get
            {
                int soTotalWeight = 0;
                if (this.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            soTotalWeight += itemGroup.ProductItemList.Sum(item => item.Weight.GetValueOrDefault() * item.UnitQuantity * itemGroup.Quantity);
                        }
                    }
                }
                if (this.GiftItemList != null)
                {
                    soTotalWeight += this.GiftItemList.Sum(item => item.Weight.GetValueOrDefault() * item.UnitQuantity);
                }
                if (this.AttachmentItemList != null)
                {
                    soTotalWeight += this.AttachmentItemList.Sum(item => item.Weight.GetValueOrDefault() * item.UnitQuantity);
                }
                return soTotalWeight;
            }
        }

        /// <summary>
        /// 最大单品重量
        /// </summary>
        public int MaxWeight
        {
            get
            {
                int soMaxWeight = 0;
                if (this.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null && itemGroup.ProductItemList.Count > 0)
                        {
                            if (itemGroup.ProductItemList.Max(item => item.Weight.GetValueOrDefault()) >= soMaxWeight)
                            {
                                soMaxWeight = itemGroup.ProductItemList.Max(item => item.Weight.GetValueOrDefault());
                            }
                        }
                    }
                }
                if (this.GiftItemList != null && this.GiftItemList.Count > 0)
                {
                    if (this.GiftItemList.Max(item => item.Weight.GetValueOrDefault()) >= soMaxWeight)
                    {
                        soMaxWeight = this.GiftItemList.Max(item => item.Weight.GetValueOrDefault());
                    }
                }
                if (this.AttachmentItemList != null && this.AttachmentItemList.Count > 0)
                {
                    if (this.AttachmentItemList.Max(item => item.Weight.GetValueOrDefault()) >= soMaxWeight)
                    {
                        soMaxWeight = this.AttachmentItemList.Max(item => item.Weight.GetValueOrDefault());
                    }
                }
                return soMaxWeight;
            }
        }

        /// <summary>
        /// 所有的商品数量，包含了赠品和附件
        /// </summary>
        public int TotalItemCount
        {
            get
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
                if (this.GiftItemList != null)
                {
                    totalItemCount += this.GiftItemList.Sum(item => item.UnitQuantity * item.ParentCount);
                }
                if (this.AttachmentItemList != null)
                {
                    totalItemCount += this.AttachmentItemList.Sum(item => item.UnitQuantity * item.ParentCount);
                }

                return totalItemCount;
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

        #region 店铺优惠券

        /// <summary>
        /// 店铺优惠券列表
        /// </summary>
        public List<CustomerCouponInfo> MerchantCouponList { get; set; }
        /// <summary>
        /// 选择的店铺优惠券
        /// </summary>
        public List<MerchantCoupon> MerchantCouponCodeList { get; set; }
        /// <summary>
        /// 店铺优惠券代码编号
        /// </summary>
        public int? MerchantCouponCodeSysNo { get; set; }

        /// <summary>
        /// 店铺优惠券代码
        /// </summary>
        public string MerchantCouponCode { get; set; }

        /// <summary>
        /// 店铺优惠券活动编号
        /// </summary>
        public int? MerchantCouponSysNo { get; set; }

        /// <summary>
        /// 店铺优惠券活动名称
        /// </summary>
        public string MerchantCouponName { get; set; }

        /// <summary>
        /// 本订单中店铺优惠券可折扣金额
        /// </summary>
        public decimal MerchantCouponAmount { get; set; }

        /// <summary>
        /// 如果店铺优惠券不满足使用规则，那么MerchantCouponCodeSysNo为空，并且MerchantCouponErrorDesc中有对应的描述
        /// </summary>
        public string MerchantCouponErrorDesc { get; set; }

        #endregion

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
        /// 使用的礼品卡列表
        /// </summary>
        public List<GiftCardInfo> GiftCardList { get; set; }

        /// <summary>
        /// 绑定的礼品卡列表
        /// </summary>
        public List<GiftCardInfo> BindingGiftCardList { get; set; }

        /// <summary>
        /// 如果礼品卡不满足使用规则，那么GiftCardErrorDesc中有对应的描述
        /// </summary>
        public string GiftCardErrorDesc { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal SOAmount
        {
            get
            {
                decimal itemTotalAmount = 0m;
                if (OrderItemGroupList != null)
                {
                    itemTotalAmount = OrderItemGroupList.Sum(x => x.TotalSalePrice);
                }

                decimal shipAmount = ShippingAmount - TotalShipFeeDiscountAmt;
                //如果各单个商品减免的运费总和 > 运费设置，那么相当于不收取运费。
                if (shipAmount < 0)
                {
                    shipAmount = 0.00m;
                }
                itemTotalAmount += shipAmount;
                itemTotalAmount += TaxAmount;
                itemTotalAmount += CommissionAmount;
                itemTotalAmount -= TotalDiscountAmount;
                itemTotalAmount = Math.Round(itemTotalAmount, 2, MidpointRounding.AwayFromZero);
                return itemTotalAmount;
            }
        }

        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal TotalProductAmount
        {
            get
            {
                if (OrderItemGroupList != null)
                {
                    return OrderItemGroupList.Sum(x => x.TotalSalePrice);
                }
                return 0m;
            }
        }

        /// <summary>
        /// 需要现金支付的总金额
        /// </summary>
        public decimal CashPayAmount
        {
            get
            {
                decimal amount = SOAmount;
                amount = amount - PointPayAmount - BalancePayAmount - CouponAmount - GiftCardPayAmount;
                amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
                return amount;
            }
        }



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


        public Dictionary<string, OrderInfo> SubOrderList { get; set; }

        /// <summary>
        /// 购物车编号
        /// </summary>
        public string ShoppingCartID { get; set; }




        /// <summary>
        /// 公共属性
        /// </summary>
        public string InUser { get; set; }
        public DateTime InDate { get; set; }
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
        /// 虚拟团购订单使用的电话号码
        /// </summary>
        public string VirualGroupBuyOrderTel { get; set; }

        public override ExtensibleObject CloneObject()
        {
            OrderInfo cloneOrderInfo = new OrderInfo()
            {
                ChannelID = this.ChannelID,
                LanguageCode = this.LanguageCode,
                Contact = this.Contact == null ? null : (ContactInfo)this.Contact.Clone(),
                Customer = this.Customer == null ? null : (CustomerInfo)this.Customer.Clone(),
                ID = this.ID,
                Memo = this.Memo,
                MerchantMemoList = this.MerchantMemoList,
                PayTypeID = this.PayTypeID,
                ShipTypeID = this.ShipTypeID,
                PointPay = this.PointPay,
                ShippingAmount = this.ShippingAmount,
                TaxAmount = this.TaxAmount,
                CommissionAmount = this.CommissionAmount,
                GiftCardPayAmount = this.GiftCardPayAmount,
                GiftCardList = this.GiftCardList == null ? null : this.GiftCardList.ConvertAll<GiftCardInfo>(x => x == null ? null : (GiftCardInfo)x.Clone()),
                Receipt = this.Receipt == null ? null : (ReceiptInfo)this.Receipt.Clone(),
                OrderItemGroupList = this.OrderItemGroupList == null ? null : this.OrderItemGroupList.ConvertAll<OrderItemGroup>(x => x == null ? null : (OrderItemGroup)x.Clone()),
                GiftItemList = this.GiftItemList == null ? null : this.GiftItemList.ConvertAll<OrderGiftItem>(x => x == null ? null : (OrderGiftItem)x.Clone()),

                ShoppingCartID = this.ShoppingCartID,
                MerchantCouponList = this.MerchantCouponList,
                MerchantCouponCodeList = this.MerchantCouponCodeList,
                MerchantCouponAmount = this.MerchantCouponAmount,
                MerchantCouponCode = this.MerchantCouponCode,
                MerchantCouponCodeSysNo = this.MerchantCouponCodeSysNo,
                MerchantCouponName = this.MerchantCouponName,
                MerchantCouponSysNo = this.MerchantCouponSysNo,
                CouponCodeSysNo = this.CouponCodeSysNo,
                CouponCode = this.CouponCode,
                CouponName = this.CouponName,
                CouponAmount = this.CouponAmount,
                InUser = this.InUser,
                InDate = this.InDate,
                CompanyCode = this.CompanyCode,
                DiscountDetailList = this.DiscountDetailList == null ? null : this.DiscountDetailList.ConvertAll<OrderItemDiscountInfo>(x => x == null ? null : (OrderItemDiscountInfo)x.Clone()),
                SOType = this.SOType,
                OrderSource = this.OrderSource,
                BalancePayAmount = this.BalancePayAmount,
                CreateUserName = this.CreateUserName,
                CreateUserSysNo = this.CreateUserSysNo,
                PointPayAmount = this.PointPayAmount,
                AttachmentItemList = this.AttachmentItemList == null ? null : this.AttachmentItemList.ConvertAll<OrderAttachment>(x => x == null ? null : (OrderAttachment)x.Clone()),
                PayTypeName = this.PayTypeName,
                IPAddress = this.IPAddress,
                WarmTips = this.WarmTips,
                VirualGroupBuyOrderTel = this.VirualGroupBuyOrderTel
            };

            if (this.SubOrderList != null)
            {
                cloneOrderInfo.SubOrderList = new Dictionary<string, OrderInfo>();
                foreach (var kvs in this.SubOrderList)
                {
                    cloneOrderInfo.SubOrderList.Add(kvs.Key, (OrderInfo)kvs.Value.Clone());
                }
            }

            return cloneOrderInfo;
        }
    }

    public class MerchantOrderMemo
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
    }

    public class MerchantCoupon
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 优惠券
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }
    }

    public class DTOInfo : ExtensibleObject
    {
        public override ExtensibleObject CloneObject()
        {
            return new DTOInfo();
        }
    }
}
