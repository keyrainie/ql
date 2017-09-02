using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 礼品卡退款日志记录
    /// </summary>
    public class GiftCardRMARedeemLog
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? TransactionNumber { get; set; }

        /// <summary>
        /// 礼品卡Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// 操作编号
        /// </summary>
        public int? ActionSysNo { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// 退款单系统编号
        /// </summary>
        public int? RefundSysNo { get; set; }
        /// <summary>
        /// 销售单系统编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        public int? CurrencySysNo { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }
        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 可用金额
        /// </summary>
        public decimal? AvailAmount { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? Type { get; set; }
    }
}
