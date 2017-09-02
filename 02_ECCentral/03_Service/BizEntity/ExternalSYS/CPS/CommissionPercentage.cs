namespace ECCentral.BizEntity.ExternalSYS.CPS
{
    /// <summary>
    /// 佣金信息
    /// </summary>
    public class CommissionPercentage
    {
        /// <summary>
        /// 一级类别
        /// </summary>
        public int C1SysNo { get; set; }

        /// <summary>
        /// 佣金比例
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public string IsDefault { get; set; }
    }
}
