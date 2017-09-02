using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// POS支付查询条件
    /// </summary>
    public class POSPayQueryFilter
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
        /// 订单系统编号,多个订单号之间用.隔开
        /// </summary>
        public string SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// POS终端号
        /// </summary>
        public string POSTerminalID
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间（从）
        /// </summary>
        public DateTime? OutDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间（到）
        /// </summary>
        public DateTime? OutDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// POS收款时间（从）
        /// </summary>
        public DateTime? PayedDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// POS收款时间（到）
        /// </summary>
        public DateTime? PayedDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? SOIncomeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// POS支付方式
        /// </summary>
        public POSPayType? POSPayType
        {
            get;
            set;
        }

        /// <summary>
        /// POS自动确认状态
        /// </summary>
        public AutoConfirmStatus? AutoConfirmStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 合单号
        /// </summary>
        public string CombineNumber
        {
            get;
            set;
        }
    }
}