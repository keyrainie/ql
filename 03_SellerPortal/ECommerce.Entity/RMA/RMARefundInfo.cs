using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.RMA
{
    public class RMARefundInfo : EntityBase
    {
        public int? SysNo { get; set; }

        public string RefundID { get; set; }

        public RMARefundStatus? Status { get; set; }

        public SOIncomeStatus? SOIncomeStatus { get; set; }

        public int? SOSysNo { get; set; }

        public int? CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        public RefundPayType? RefundPayType { get; set; }

        /// <summary>
        /// 初算退款金额
        /// </summary>
        public decimal? OrgCashAmt { get; set; }
        /// <summary>
        /// 实际退款金额
        /// </summary>
        public decimal? CashAmt { get; set; }

        /// <summary>
        /// 初算应退积分
        /// </summary>
        public int? OrgPointAmt { get; set; }

        /// <summary>
        /// 最终应退积分
        /// </summary>
        public int? PointPay { get; set; }

        /// <summary>
        /// 初算应退礼品卡金额
        /// </summary>
        public decimal? OrgGiftCardAmt { get; set; }

        /// <summary>
        /// 最终应退礼品卡金额
        /// </summary>
        public decimal? GiftCardAmt { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string CardOwnerName { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNumber { get; set; }


        public RefundOrderType OrderType
        {
            get { return RefundOrderType.RO; }
        }

        public int? AuditUserSysNo { get; set; }

        public string AuditUserName { get; set; }

        public DateTime? AuditDate { get; set; }

        public int? RefundUserSysNo { get; set; }

        public string RefundUserName { get; set; }

        public DateTime? RefundDate { get; set; }

        /// <summary>
        /// 从账户扣积分
        /// </summary>
        public int? DeductPointFromAccount { get; set; }

        /// <summary>
        /// 本次退款扣积分
        /// </summary>
        public decimal? DeductPointFromCurrentCash { get; set; }


        /// <summary>
        /// 积分支付比例
        /// </summary>
        public decimal? SOCashPointRate { get; set; }

        public List<RMARefundItemInfo> RefundItems { get; set; }
    }
}
