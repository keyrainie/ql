using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 退款调整单信息
    /// </summary>
    public class RefundBalanceInfo : IIdentity,ICompany
    {
        public RefundBalanceInfo()
        {
            this.IncomeBankInfo = new SOIncomeRefundInfo();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司代码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 原始退款单系统编号
        /// </summary>
        public int? OriginalRefundSysNo { get; set; }
        /// <summary>
        /// 原始销售单系统编号
        /// </summary>
        public int? OriginalSOSysNo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RefundID { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? NewOrderSysNo { get; set; }
        /// <summary>
        /// 退款调整单类型
        /// </summary>
        public RefundBalanceType? BalanceOrderType { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 退款人系统编号
        /// </summary>
        public int? RefundUserSysNo { get; set; }
        /// <summary>
        /// 退现金金额
        /// </summary>
        public decimal? CashAmt { get; set; }
        /// <summary>
        /// 退礼品卡金额
        /// </summary>
        public decimal? GiftCardAmt { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public RefundBalanceStatus? Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType { get; set; }
        /// <summary>
        /// 退积分
        /// </summary>
        public int? PointAmt { get; set; }

        /// <summary>
        /// 银行信息
        /// </summary>
        public SOIncomeRefundInfo IncomeBankInfo { get; set; }
    }
}
