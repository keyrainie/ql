using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.SO
{
    [Serializable]
    [DataContract]
    public class SOItemInfo : IIdentity
    {
        public SOItemInfo()
        {
            //ItemExtList = new List<ItemExtension>();
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
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName //BriefName
        { get; set; }

        /// <summary>
        /// 订单商品类型
        /// </summary>
        [DataMember]
        public SOProductType? ProductType { get; set; }

        private int _quantity;
        /// <summary>
        /// 购买数量
        /// </summary>
        [DataMember]
        public int? Quantity
        {
            get { return _quantity; }
            set { _quantity = value ?? 0; }
        }

        /// <summary>
        /// 配送数量
        /// </summary>
        [DataMember]
        public int? ShoppingQty { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        [DataMember]
        public int? AvailableQty { get; set; }

        /// <summary>
        /// Online库存
        /// </summary>
        [DataMember]
        public int? OnlineQty { get; set; }

        /// <summary>
        /// 三级类别编号
        /// </summary>
        [DataMember]
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int? BrandSysNo { get; set; }

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

        private decimal _promotionAmount;
        /// <summary>
        /// 除优惠券的促销活动折扣总额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? PromotionAmount //DiscountAmt
        {
            get { return _promotionAmount; }
            set { _promotionAmount = value ?? 0M; }
            //{
            //    return DiscountList == null ? 0 : DiscountList.Sum<SOItemPromotionInfo>(item =>
            //    {
            //        return item.PromotionType != PromotionType.Coupon ? item.DiscountAmount.Value : 0;
            //    });
            //}
        }
        private decimal _couponAverageDiscount;

        /// <summary>
        /// 优惠券平均折扣到每个商品上的金额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? CouponAverageDiscount //PromotionDiscount
        {
            get { return _couponAverageDiscount; }
            set { _couponAverageDiscount = value ?? 0M; }
        }

        /// <summary>
        /// 优惠券折扣总额(== CouponAverageDiscount * Quantity)，注意：在使用时请不要与数量(Quantity)相乘，此结果已经是此种商品的优惠券总折扣；
        /// 如果要用单个商品的优惠券折扣，请用CouponAverageDiscount
        /// </summary>
        public decimal CouponAmount
        {
            get
            {
                return Math.Round(CouponAverageDiscount.Value * Quantity.Value, 2);
            }
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

        private decimal _costPrice;
        /// <summary>
        /// 成本价格
        /// </summary>
        [DataMember]
        public decimal? CostPrice //Cost 
        {
            get { return _costPrice; }
            set { _costPrice = value ?? 0M; }
        }

        private decimal _noTaxCostPrice;
        /// <summary>
        /// 无税成本价格
        /// </summary>
        [DataMember]
        public decimal? NoTaxCostPrice //UnitCostWithoutTax
        {
            get { return _noTaxCostPrice; }
            set { _noTaxCostPrice = value ?? 0M; }
        }

        private decimal? _tariffAmount;
        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal? TariffAmount
        {
            get { return _tariffAmount; }
            set { _tariffAmount = value ?? 0M; }
        }

        //----------------------------------------------------
        //关税冗余字段
        private string _tariffcode;
        /// <summary>
        /// 税号
        /// </summary>
        [DataMember]
        public string Tariffcode
        {
            get { return _tariffcode; }
            set { _tariffcode = value; }
        }

        private decimal? _tariffRate;
        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public decimal? TariffRate
        {
            get { return _tariffRate; }
            set { _tariffRate = value ?? 0M; }
        }


        private string _entryRecord;
        /// <summary>
        /// 商品入境备案号
        /// </summary>
        [DataMember]
        public string EntryRecord
        {
            get { return _entryRecord; }
            set { _entryRecord = value; }
        }

        private StoreType? _storeType;
        /// <summary>
        /// 商品存储运输方式
        /// </summary>
        [DataMember]
        public StoreType? StoreType
        {
            get { return _storeType; }
            set { _storeType = value; }
        }

        private SOProductPriceType _priceType = SOProductPriceType.Normal;
        /// <summary>
        /// 商品使用的价格类型
        /// </summary>
        [DataMember]
        public SOProductPriceType? PriceType  //IsMemberPrice
        {
            get { return _priceType; }
            set { _priceType = value ?? SOProductPriceType.Normal; }
        }

        private int _gainAveragePoint;

        /// <summary>
        /// 购买一个商品所获得的积分值
        /// </summary>
        [DataMember]
        public int? GainAveragePoint //Point
        {
            get { return _gainAveragePoint; }
            set { _gainAveragePoint = value ?? 0; }
        }

        /// <summary>
        /// 购买此商品所获得的总积分值(== GainAveragePoint * Quantity),在使用时间请不要与Quantity相乘,此值已经是总的积分值
        /// 如果要用单个商品获得的积分，请用GainAveragePoint
        /// </summary>
        public int GainPoint
        {
            get
            {
                return GainAveragePoint.Value * Quantity.Value;
            }
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        private ProductPayType _payType;
        [DataMember]
        public ProductPayType? PayType
        {
            get { return _payType; }
            set { _payType = value ?? 0; }
        }

        /// <summary>
        /// 质保信息
        /// </summary>
        [DataMember]
        public string Warranty { get; set; }

        private int _weight;
        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public int? Weight
        {
            get { return _weight; }
            set { _weight = value ?? 0; }
        }

        private int _stockSysNo;

        /// <summary>
        /// 商品仓库编号
        /// </summary>
        [DataMember]
        public int? StockSysNo //WarehouseNumber
        {
            get { return _stockSysNo; }
            set
            {
                _stockSysNo = value ?? 0;
            }
        }

        /// <summary>
        /// 商品仓库名（数据显示）
        /// </summary>
        [DataMember]
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 该条为延保的时候, 对应的主商品productsysno.(多个不同的主商品对应同一个延保, 则是多条记录)
        /// </summary>
        [DataMember]
        public string MasterProductSysNo { get; set; }

        /// <summary>
        /// 大家电运单号
        /// </summary>
        [DataMember]
        public string SHDSysNo { get; set; }

        /// <summary>
        /// 主商品 对应的 赠送赠品基数 （如 基数为2  则表示一个主商品 赠送2个赠品 一次类推）
        /// </summary>
        [DataMember]
        public Int32? RuleQty { get; set; }

        /// <summary>
        /// 是否发货
        /// </summary>
        [DataMember]
        public bool? IsShippedOut { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMember]
        public DateTime? ShippedOutTime { get; set; }

        #region 团购商品相关信息
        /// <summary>
        /// 团购处理状态（后台使用）
        /// </summary>
        [DataMember]
        public SettlementStatus? SettlementStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 团购的编号
        /// </summary>
        [DataMember]
        public int? ReferenceSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单商品活动类型
        /// </summary>
        [DataMember]
        public SOProductActivityType? ActivityType
        {
            get;
            set;
        }

        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        [DataMember]
        public string CreateUserName { get; set; }
        /// <summary>
        ///  最后修改时间
        /// </summary>
        [DataMember]
        public DateTime? LastEditDate { get; set; }
        /// <summary>
        /// 最后修改人
        /// </summary>
        [DataMember]
        public string LastEditUserName { get; set; }
        #endregion

        #region 手动改价
        /// <summary>
        /// 手动改价原因
        /// </summary>
        [DataMember]
        public string AdjustPriceReason { get; set; }

        /// <summary>
        /// 价格补偿 数量（既：价格将要减掉 的差额）
        /// </summary>   
        [DataMember]
        public decimal AdjustPrice { get; set; }

        /// <summary>
        /// 当前价格(调价后的商品)
        /// </summary>
        [DataMember]
        public decimal Price_End { get; set; }
        #endregion

        /// <summary>
        /// 是否缺货
        /// </summary>
        [DataMember]
        public bool IsLessStock { get; set; }

        /// <summary>
        /// 是否待采购
        /// </summary>
        [DataMember]
        public bool IsWaitPO { get; set; }

        [DataMember]
        public List<ItemExtension> ItemExtList { get; set; }
        /// <summary>
        /// 泰隆优选库存模式
        /// </summary>
        [DataMember]
        public ProductInventoryType InventoryType { get; set; }

        /// <summary>
        /// 商品所属组的系统编号
        /// </summary>
        public int ProductGroupSysNo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ItemExtension
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public int SOSysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 团购活动编号（今后可以是任何活动）
        /// </summary>
        [DataMember]
        public int? ReferenceSysNo { get; set; }

        /// <summary>
        /// 活动类型
        /// G - 团购
        /// </summary>
        [DataMember]
        public SOProductActivityType? Type { get; set; }

        /// <summary>
        /// 是否处理标识（后台使用）
        /// S 处理成功
        /// F 处理失败
        /// P 团购失败
        /// </summary>
        [DataMember]
        public SettlementStatus? SettlementStatus { get; set; }

        [DataMember]
        public decimal OriginalCurrentPrice { get; set; }
    }




    /// <summary>
    /// 订单商品毛利分配表
    /// </summary>
    [Serializable]
    [DataContract]
    [XmlRoot(Namespace = "http://ECCentral/BizEntity/SO")]
    public class ItemGrossProfitInfo
    {
        [DataMember]
        public int SOSysNo { get; set; }

        [DataMember]
        public ItemGrossProfitActionType? ActionType { get; set; }

        [DataMember]
        public int ProductSysNo { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal DisCount { get; set; }

        [DataMember]
        public GrossProfitType? GrossProfitType { get; set; }

        [DataMember]
        public ECCentral.BizEntity.SO.ValidStatus? Status { get; set; }

    }
}
