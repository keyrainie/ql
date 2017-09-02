using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class ReconciliationQueryFilter
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
        /// 单据ID
        /// </summary>
        public string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付流水单号
        /// </summary>
        public string SerialNo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间起
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间起
        /// </summary>
        public DateTime? ConfirmDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间止
        /// </summary>
        public DateTime? ConfirmDateTo
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
    }
}
