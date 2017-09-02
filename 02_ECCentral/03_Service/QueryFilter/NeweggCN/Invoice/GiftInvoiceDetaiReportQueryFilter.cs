using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class GiftInvoiceDetaiReportQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public DateTime? OutDateFrom
        {
            get;
            set;
        }

        public DateTime? OutDateTo
        {
            get;
            set;
        }

        public DateTime? InvoiceDateFrom
        {
            get;
            set;
        }

        public DateTime? InvoiceDateTo
        {
            get;
            set;
        }

        public int? StockSysNo
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public string InvoiceNumber
        {
            get;
            set;
        }

        public string CustomerName
        {
            get;
            set;
        }

        public string OrderType
        {
            get;
            set;
        }

        public ECCentral.BizEntity.SO.SOType? SOType
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}