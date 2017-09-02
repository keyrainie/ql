using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOPriceMasterInfo : IIdentity
    {
        public SOPriceMasterInfo()
        {
            Items = new List<SOPriceItemInfo>();
        }
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public int? StockSysNo //WareHouseNumber
        {
            get;
            set;
        }
        private decimal _soAmount;
        /// <summary>
        /// 订单金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? SOAmount//SoAmt
        {
            get { return _soAmount; }
            set { _soAmount = value ?? 0M; }
        }
        private decimal _cashPay;
        /// <summary>
        /// 现金支付(>=0)
        /// </summary>
        [DataMember]
        public decimal? CashPay
        {
            get { return _cashPay; }
            set { _cashPay = value ?? 0M; }
        }
        private decimal _premiumAmount;
        /// <summary>
        /// 保价费(>=0)
        /// </summary>
        [DataMember]
        public decimal? PremiumAmount//PremiumAmt
        {
            get { return _premiumAmount; }
            set { _premiumAmount = value ?? 0M; }
        }
        private decimal _shipPrice;
        /// <summary>
        /// 运费(>=0)
        /// </summary>
        [DataMember]
        public decimal? ShipPrice//ShippingCharge
        {
            get { return _shipPrice; }
            set { _shipPrice = value ?? 0M; }
        }
        private decimal _payPrice;
        /// <summary>
        /// 手续费(>=0)
        /// </summary>
        [DataMember]
        public decimal? PayPrice//PayPrice
        {
            get { return _payPrice; }
            set { _payPrice = value ?? 0M; }
        }
        private decimal _pointPayAmount;
        /// <summary>
        /// 积分支付金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? PointPayAmount//-PointPay
        {
            get { return _pointPayAmount; }
            set { _pointPayAmount = value ?? 0M; }
        }

        private decimal _prepayAmount;
        /// <summary>
        /// 余额支付金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? PrepayAmount//-PrepayAmt
        {
            get { return _prepayAmount; }
            set { _prepayAmount = value ?? 0M; }
        }

        private decimal _couponAmount;
        /// <summary>
        /// 优惠券折扣金额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? CouponAmount//Promotion
        {
            get { return _couponAmount; }
            set { _couponAmount = value ?? 0M; }
        }
        private decimal _promotionAmount;
        /// <summary>
        /// 促销拆扣(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? PromotionAmount//Discount
        {
            get { return _promotionAmount; }
            set { _promotionAmount = value ?? 0M; }
        }
        private decimal _invoiceAmount;
        /// <summary>
        /// 发票金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? InvoiceAmount//InvoiceAmt
        {
            get { return _invoiceAmount; }
            set { _invoiceAmount = value ?? 0M; }
        }

        private decimal _receivableAmount;
        /// <summary>
        /// 应收款(>=0)
        /// </summary>
        [DataMember]
        public decimal? ReceivableAmount//ReceivableAmt
        {
            get { return _receivableAmount; }
            set { _receivableAmount = value ?? 0M; }
        }

        private int _gainPoint;
        /// <summary>
        /// 获得的积分(>=0)
        /// </summary>
        [DataMember]
        public int? GainPoint//PointAmt
        {
            get { return _gainPoint; }
            set { _gainPoint = value ?? 0; }
        }
        private decimal _giftCardPay;
        /// <summary>
        /// 礼品上支付(>=0)
        /// </summary>
        [DataMember]
        public decimal? GiftCardPay//-GiftCardPay
        {
            get { return _giftCardPay; }
            set { _giftCardPay = value ?? 0M; }
        }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public SOPriceStatus? Status//Status
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 价格商品明细
        /// </summary>
        [DataMember]
        public List<SOPriceItemInfo> Items
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 订单价格详细信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOPriceItemInfo
    {
        /// <summary>
        /// 订单价格主编号
        /// </summary>
        [DataMember]
        public int? MasterSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 主商品编号
        /// </summary>
        [DataMember]
        public string MasterProductSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public SOProductType? ProductType
        {
            get;
            set;
        }
        private int _quantity;
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int? Quantity
        {
            get { return _quantity; }
            set { _quantity = value ?? 0; }
        }
        private decimal _price;
        /// <summary>
        /// 当前价格
        /// </summary>
        [DataMember]
        public decimal? Price
        {
            get { return _price; }
            set { _price = value ?? 0M; }
        }
        private decimal _originalPrice;
        /// <summary>
        /// 原始价格
        /// </summary>
        [DataMember]
        public decimal? OriginalPrice
        {
            get { return _originalPrice; }
            set { _originalPrice = value ?? 0M; }
        }
        private decimal _cashPay;
        /// <summary>
        /// 现金支付金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? CashPay
        {
            get { return _cashPay; }
            set { _cashPay = value ?? 0M; }
        }
        private decimal _premiumAmount;
        /// <summary>
        /// 保价费(>=0)
        /// </summary>
        [DataMember]
        public decimal? PremiumAmount// PremiumAmt
        {
            get { return _premiumAmount; }
            set { _premiumAmount = value ?? 0M; }
        }
        private decimal _shipPrice;
        /// <summary>
        /// 运费(>=0)
        /// </summary>
        [DataMember]
        public decimal? ShipPrice //ShippingCharge
        {
            get { return _shipPrice; }
            set { _shipPrice = value ?? 0M; }
        }
        private decimal _payPrice;
        /// <summary>
        /// 手续费(>=0)
        /// </summary>
        [DataMember]
        public decimal? PayPrice
        {
            get { return _payPrice; }
            set { _payPrice = value ?? 0M; }
        }
        private decimal _pointPayAmount;
        /// <summary>
        /// 积分支付(>=0)
        /// </summary>
        [DataMember]
        public decimal? PointPayAmount// -PointPay
        {
            get { return _pointPayAmount; }
            set { _pointPayAmount = value ?? 0M; }
        }
        private decimal _prepayAmount;
        /// <summary>
        /// 余额支付(>=0)
        /// </summary>
        [DataMember]
        public decimal? PrepayAmount//-PrepayAmt
        {
            get { return _prepayAmount; }
            set { _prepayAmount = value ?? 0M; }
        }
        private decimal _couponAmount;
        /// <summary>
        /// 优惠券折扣金额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? CouponAmount //Promotion
        {
            get { return _couponAmount; }
            set { _couponAmount = value ?? 0M; }
        }

        private decimal _promotionAmount;
        /// <summary>
        /// 促销折扣金额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? PromotionAmount //Discount
        {
            get { return _promotionAmount; }
            set { _promotionAmount = value ?? 0M; }
        }
        private decimal _extendPrice;
        /// <summary>
        /// (>=0)
        /// </summary>
        [DataMember]
        public decimal? ExtendPrice
        {
            get { return _extendPrice; }
            set { _extendPrice = value ?? 0M; }
        }
        private int _gainPoint;
        /// <summary>
        /// 获取积分(>=0)
        /// </summary>
        [DataMember]
        public int? GainPoint//Point
        {
            get { return _gainPoint; }
            set { _gainPoint = value ?? 0; }
        }
        private decimal _giftCardPay;
        /// <summary>
        /// 礼品上支付(>=0)
        /// </summary>
        [DataMember]
        public decimal? GiftCardPay //-GiftCardPay
        {
            get { return _giftCardPay; }
            set { _giftCardPay = value ?? 0M; }
        }
        private decimal _invoiceAmount;
        /// <summary>
        /// 发票金额(>=0)
        /// </summary>
        [DataMember]
        public decimal? InvoiceAmount // InvoiceAmt
        {
            get { return _invoiceAmount; }
            set { _invoiceAmount = value ?? 0M; }
        }

    }

}