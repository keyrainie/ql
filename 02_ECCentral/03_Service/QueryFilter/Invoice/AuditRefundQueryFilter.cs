using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 退款审核查询条件
    /// </summary>
    public class AuditRefundQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public RefundStatus? AuditStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间（从）
        /// </summary>
        public DateTime? CreateTimeFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间（到）
        /// </summary>
        public DateTime? CreateTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间（从）
        /// </summary>
        public DateTime? AuditTimeFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间（到）
        /// </summary>
        public DateTime? AuditTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单号
        /// </summary>
        public string RMANumber
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单状态
        /// </summary>
        public RMARefundStatus? RMAStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 订单号，多个订单号之间用逗号隔开
        /// </summary>
        public string OrderNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型（除去SO）
        /// </summary>
        public RefundOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 退款原因SysNo
        /// </summary>
        public string RMAReasonCode
        {
            get;
            set;
        }

        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否物流拒收
        /// </summary>
        public bool? ShipRejected
        {
            get;
            set;
        }

        /// <summary>
        /// 涉及现金
        /// </summary>
        public bool CashRelated
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态（收款状态吧？）
        /// </summary>
        public SOIncomeStatus? RefundStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额操作符号
        /// </summary>
        public OperationSignType OperationType
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        public WLTRefundStatus? WLTRefundStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public string PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需要开增票
        /// </summary>
        public bool? IsVAT
        {
            get;
            set;
        }

        /// <summary>
        /// 分仓系统编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付平台
        /// </summary>
        public string PartnerName
        {
            get;
            set;
        }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string OutOrderNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
    }
}