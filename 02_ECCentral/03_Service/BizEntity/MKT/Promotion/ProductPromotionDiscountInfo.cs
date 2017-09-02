namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 商品促销折扣信息
    /// </summary>
    public class ProductPromotionDiscountInfo
    {
        /// <summary>
        /// 促销类型
        /// </summary>
        public PromotionType PromotionType { get; set; }

        /// <summary>
        /// 促销编号
        /// </summary>
        public int ReferenceSysNo { get; set; }

        /// <summary>
        /// 抵扣金额
        /// </summary>
        public decimal Discount { get; set; }
    }
}
