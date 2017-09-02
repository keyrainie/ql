namespace ECCentral.BizEntity.ExternalSYS.CPS
{
    /// <summary>
    /// 佣金结算单明细订单明细计算信息
    /// </summary>
    public class CommissionSettlementItemSOItemCalculateInfo
    {
        /// <summary>
        /// 订单商品价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 订单商品数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 订单商品佣金比例
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
