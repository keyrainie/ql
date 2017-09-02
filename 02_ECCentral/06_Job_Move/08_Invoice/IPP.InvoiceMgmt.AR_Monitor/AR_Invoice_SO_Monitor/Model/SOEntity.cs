using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.Model
{
    class SOEntity
    {
        public int SysNo { get; set; }
        public decimal CashPay { get; set; }
        public decimal PayPrice { get; set; }
        public decimal ShipPrice { get; set; }
        public decimal PremiumAmt { get; set; }
        public decimal DiscountAmt { get; set; }
        public int PayType { get; set; }
        public int IsPayWhenRecv { get; set; }
        public decimal GiftCardPay { get; set; }
        public int? SOType { get; set; }
        public string StockType { get; set; }
        public string InvoiceType { get; set; }
    }
}
