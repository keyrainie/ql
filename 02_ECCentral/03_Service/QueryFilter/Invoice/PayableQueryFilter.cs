using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class PayableQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public int? SysNo
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }
        public PayableOrderType? OrderType
        {
            get;
            set;
        }
        public PayableStatus? PayStatus
        {
            get;
            set;
        }
        public PayableInvoiceStatus? InvoiceStatus
        {
            get;
            set;
        }

        public DateTime? OrderDateFrom
        {
            get;
            set;
        }
        public DateTime? OrderDateTo
        {
            get;
            set;
        }
        public DateTime? POETPDateFrom
        {
            get;
            set;
        }
        public DateTime? POETPDateTo
        {
            get;
            set;
        }
        public DateTime? POEGPDateFrom
        {
            get;
            set;
        }
        public DateTime? POEGPDateTo
        {
            get;
            set;
        }
        public DateTime? InDateFrom
        {
            get;
            set;
        }
        public DateTime? InDateTo
        {
            get;
            set;
        }

        public bool? IsOnlyNegativeOrder
        {
            get;
            set;
        }
        public bool? IsNotInStock
        {
            get;
            set;
        }
        public int? POBelongPMSysNo
        {
            get;
            set;
        }
        public PayItemStyle? PayStyle
        {
            get;
            set;
        }
        public ECCentral.BizEntity.PO.PurchaseOrderStatus? POStatus
        {
            get;
            set;
        }
        public ECCentral.BizEntity.PO.PurchaseOrderType? POType
        {
            get;
            set;
        }
        public int? PayPeriodType
        {
            get;
            set;
        }
        public ECCentral.BizEntity.PO.SettleStatus? VendorSettleStatus
        {
            get;
            set;
        }
        public bool? FinanceSettleOrderStatus
        {
            get;
            set;
        }
        public bool? BalanceOrderStatus
        {
            get;
            set;
        }

        public int? VendorSysNo
        {
            get;
            set;
        }
        public int? CurrencySysNo
        {
            get;
            set;
        }
        public int? CreatePMSysNo
        {
            get;
            set;
        }
        public int? StockSysNo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string ChannelID
        {
            get;
            set;
        }

        public int? PaySettleCompany
        {
            get;
            set;
        }

        public DateTime? InStockDateFrom
        {
            get;
            set;
        }

        public DateTime? InStockDateTo
        {
            get;
            set;
        }
    }
}