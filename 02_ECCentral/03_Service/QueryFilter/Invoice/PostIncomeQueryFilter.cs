using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// PostIncome查询条件
    /// </summary>
    public class PostIncomeQueryFilter
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间（从）
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间（到）
        /// </summary>
        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款时间（从）
        /// </summary>
        public DateTime? IncomeDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 收款时间（到）
        /// </summary>
        public DateTime? IncomeDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 处理情况
        /// </summary>
        public PostIncomeHandleStatusUI? HandleStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 付款行
        /// </summary>
        public string PayBank
        {
            get;
            set;
        }

        /// <summary>
        /// 收款行
        /// </summary>
        public string IncomeBank
        {
            get;
            set;
        }

        /// <summary>
        /// 制单人
        /// </summary>
        public string CreateUser
        {
            get;
            set;
        }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayUser
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单号，多个订单号之间用逗号分隔
        /// </summary>
        public string ConfirmedSOSysNoList
        {
            get;
            set;
        }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
    }
}