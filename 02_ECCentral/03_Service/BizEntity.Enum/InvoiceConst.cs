using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public static class InvoiceConst
    {
        public const decimal VendorTaxRate = 1.17M;

        public static class ResourceTitle
        {
            public const string NetPay = "Invoice.NetPay";
            public const string SOIncome = "Invoice.SOIncome";
            public const string SOIncomeRefund = "Invoice.SOIncomeRefund";
            public const string PostPay = "Invoice.PostPay";
            public const string PostIncome = "Invoice.PostIncome";
            public const string Payable = "Invoice.Payable";
            public const string PayItem = "Invoice.PayItem";
            public const string SubInvoice = "Invoice.SubInvoice";
            public const string BizInteract = "Invoice.BizInteract";
            public const string BatchAction = "Invoice.BatchAction";
            public const string Invoice = "Invoice.Invoice";
            public const string TrackingInfo = "Invoice.TrackingInfo";
            public const string POVendorInvoice = "Invoice.POVendorInvoice";
            public const string InvoiceInput = "Invoice.InvoiceInput";
            public const string BalanceRefund = "Invoice.BalanceRefund";
            public const string Finance = "Invoice.Finance";
            public const string InvoiceReport = "Invoice.InvoiceReport";
            public const string ProductShift = "Invoice.ProductShift";
        }

        public static class StringFormat
        {
            public const string CurrencyFormat = "￥#####0.00";
            public const string DecimalFormat = "#########0.00";
            public const string ExactCurrencyFormat = "￥#####0.000";
            public const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
            public const string ShortDateFormat = "yyyy-MM-dd";
        }
    }
}