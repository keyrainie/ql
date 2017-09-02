using System;

namespace ECCentral.BizEntity.ExternalSYS.CPS
{
    /// <summary>
    /// 佣金结算单
    /// </summary>
    public class CommissionSettlementInfo
    {
        /// <summary>
        /// 获取或设置单据编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 结算用户
        /// </summary>
        public int UserSysNo { get; set; }

        /// <summary>
        /// 结算开始时间
        /// </summary>
        public DateTime SettledBeginDate { get; set; }

        /// <summary>
        /// 结算结束时间
        /// </summary>
        public DateTime SettledEndDate { get; set; }

        /// <summary>
        /// 获取或设置结算状态
        /// </summary>
        public FinanceStatus Status { get; set; }

        /// <summary>
        /// 获取或设置结算金额
        /// </summary>
        public decimal? CommissionAmt { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUser { get; set; }
    }
}
