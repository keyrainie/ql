using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品价格信息
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class ProductPriceInfo
    {
        /// <summary>
        /// 商品市场价
        /// </summary>
        [DataMember]
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 商家售价
        /// </summary>
        [DataMember]
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 商品返现
        /// </summary>
        [DataMember]
        public decimal? CashRebate { get; set; }

        /// <summary>
        /// 商品返点
        /// </summary>
        [DataMember]
        public int? Point { get; set; }

        /// <summary>
        /// 商品折扣金额
        /// </summary>
        [DataMember]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 商品成本
        /// </summary>
        [DataMember]
        public decimal UnitCost { get; set; }

        /// <summary>
        /// 商品去税成本
        /// </summary>
        [DataMember]
        public decimal UnitCostWithoutTax { get; set; }

        /// <summary>
        /// 商品正常采购价格
        /// </summary>
        [DataMember]
        public decimal VirtualPrice { get; set; }

        /// <summary>
        /// 批发价格
        /// </summary>
        [DataMember]
        public List<ProductWholeSalePriceInfo> ProductWholeSalePriceInfo { get; set; }

        /// <summary>
        /// 会员价格
        /// </summary>
        [DataMember]
        public List<ProductRankPriceInfo> ProductRankPrice { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        [DataMember]
        public ProductPayType PayType { get; set; }

        /// <summary>
        /// 每单最小订购数量
        /// </summary>
        [DataMember]
        public int MinCountPerOrder { get; set; }

        /// <summary>
        /// 每天最大订购数量
        /// </summary>
        [DataMember]
        public int MaxCountPerDay { get; set; }

        /// <summary>
        /// 最低佣金
        /// </summary>
        [DataMember]
        public decimal MinCommission { get; set; }

        /// <summary>
        /// 是否同步门店价格
        /// </summary>
        [DataMember]
        public IsSyncShopPrice IsSyncShopPrice { get; set; }
    }
}
