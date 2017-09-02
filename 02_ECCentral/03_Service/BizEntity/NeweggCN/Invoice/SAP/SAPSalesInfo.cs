using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.NeweggCN.Invoice.SAP
{
    public class SAPSalesInfo
    {
        public DateTime? ConfirmTime { get; set; }

        public string ConfirmUserName { get; set; }

        public DateTime? CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public int? CustomerSysNo { get; set; }

        public decimal? IncomeAmt { get; set; }

        public decimal? InvoiceAmt { get; set; }

        public string InvoiceNumber { get; set; }

        public string IsOutStock { get; set; }

        public decimal? OrderAmt { get; set; }

        public string OrderID { get; set; }

        public string OrderStatus { get; set; }

        public string OrderSysNo { get; set; }

        public string OrderType { get; set; }

        public int? OrderTypeSysNo { get; set; }

        public DateTime? OutStockTime { get; set; }

        public string OutStockUserName { get; set; }

        public decimal? PayPrice { get; set; }

        public int? PayType { get; set; }

        public decimal? Premium { get; set; }

        public decimal? PrepayAmt { get; set; }

        public string ReferenceID { get; set; }

        public string RefundPayType { get; set; }

        public int? RefundPayTypeSysNo { get; set; }

        public decimal? ReturnCash { get; set; }

        public int? ReturnPoint { get; set; }

        public decimal? ShipCost { get; set; }

        public decimal? ShipPrice { get; set; }

        public decimal? ShipTotal { get; set; }

        public string SOIncomeStyle { get; set; }

        public int? SOStatus { get; set; }

        public int? SOSysNo { get; set; }

        public string SalesSysNo { get; set; }

        public decimal? ToleranceAmt { get; set; }

        public string StockName { get; set; }

        public string StockID { get; set; }
    }
}
