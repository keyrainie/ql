using System.Collections.Generic;

namespace ECCentral.BizEntity.ExternalSYS.CPS
{
    /// <summary>
    /// 佣金结算单单据
    /// </summary>
    public class CommissionSettlementItemInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 获取或设置SO或RMA对应的结算单
        /// </summary>
        public int? CommissionSettlementSysNo { get; set; }

        /// <summary>
        /// 获取或设置结算Item 对应的单据编号
        /// </summary>
        public int OrderSysNo { get; set; }

        /// <summary>
        /// 获取或设置结算Item状态【U：未结算 S：已结算 P：已支付】
        /// </summary>
        public FinanceStatus Status { get; set; }

        /// <summary>
        /// 获取或设置结算Item类型 SO or RMA
        /// </summary>
        public CPSOrderType Type { get; set; }

        /// <summary>
        /// 获取或设置佣金金额
        /// </summary>
        public decimal CommissionAmt { get; set; }

        /// <summary>
        /// 佣金结算单明细订单明细计算信息列表
        /// </summary>
        public List<CommissionSettlementItemSOItemCalculateInfo> CommissionSettlementItemSOItemCalculateInfoList { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUser { get; set; }
    }
}
