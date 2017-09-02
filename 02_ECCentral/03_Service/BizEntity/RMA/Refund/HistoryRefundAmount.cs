using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 历史退款统计
    /// </summary>
    public class HistoryRefundAmount
    {
        /// <summary>
        /// 现金退款总和
        /// </summary>
        public decimal TotalCashAmt { get; set; }
        /// <summary>
        /// 礼品卡退款总和
        /// </summary>
        public decimal TotalGiftCardAmt { get; set; }
        /// <summary>
        /// 转积分退款总和
        /// </summary>
        public int TotalPointAmt { get; set; }
        /// <summary>
        /// 补偿运费退款总和
        /// </summary>
        public decimal TotalShipPriceAmt { get; set; }
        /// <summary>
        /// 退款总和
        /// </summary>
        public decimal TotalRoBalanceAmt { get; set; }
    }
}
