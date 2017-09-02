using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    public partial class ProductPriceInfo
    {
        /// <summary>
        /// 是否使用AlipayVIP价格
        /// </summary>
        [DataMember]
        public IsUseAlipayVipPrice? IsUseAlipayVipPrice { get; set; }

        /// <summary>
        /// AlipayVIP价格
        /// </summary>
        [DataMember]
        public decimal? AlipayVipPrice { get; set; }

        [DataMember]
        public int IsWholeSale { get; set; }

        [DataMember]
        public int IsExistRankPrice { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        [DataMember]
        public decimal Margin { get; set; }

        /// <summary>
        /// 含优惠券、赠品、随心配毛利率
        /// </summary>
        [DataMember]
        public decimal NewMargin { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        [DataMember]
        public decimal MarginAmount { get; set; }

        /// <summary>
        ///  含优惠券、赠品、随心配毛利
        /// </summary>
        [DataMember]
        public decimal NewMarginAmount { get; set; }
    }
}
