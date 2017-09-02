using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class BalanceRefundQueryFilter
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
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间 从
        /// </summary>
        public DateTime? CreateTimeFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间 到
        /// </summary>
        public DateTime? CreateTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 退款类型，Portal端传来的类型只有“邮局退款”和“银行转账”
        /// </summary>
        public RefundPayType? RefundType
        {
            get;
            set;
        }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank
        {
            get;
            set;
        }

        /// <summary>
        /// 分行名称
        /// </summary>
        public string BranchBank
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardOwner
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        public BalanceRefundStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// CS审核时间 从
        /// </summary>
        public DateTime? CSAuditTimeFrom
        {
            get;
            set;
        }

        /// <summary>
        /// CS审核时间 到
        /// </summary>
        public DateTime? CSAuditTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 财务审核时间 从
        /// </summary>
        public DateTime? FinAuditTimeFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 财务审核时间 到
        /// </summary>
        public DateTime? FinAuditTimeTo
        {
            get;
            set;
        }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string ReferenceID
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string WebChannelID
        {
            get;
            set;
        }
    }
}