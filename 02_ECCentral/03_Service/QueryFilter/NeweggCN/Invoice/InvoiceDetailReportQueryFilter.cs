using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class InvoiceDetailReportQueryFilter
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
        /// 出库时间 从
        /// </summary>
        public DateTime? OutDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间 到
        /// </summary>
        public DateTime? OutDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 开票日期 从
        /// </summary>
        public DateTime? InvoiceDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 开票日期 到
        /// </summary>
        public DateTime? InvoiceDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据号
        /// </summary>
        public string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string OrderType
        {
            get;
            set;
        }
    }
}