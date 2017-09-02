using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class ProductShiftDetailEntity
    {
        public string OrderID { get; set; }
     
        public string OrderType { get; set; }
     
        public string CustomerID { get; set; }
     
        public string CustomerName { get; set; }
     
        public string TaxNumber { get; set; }
     
        public string Contact { get; set; }
     
        public string Account { get; set; }
     
        public string ProductID { get; set; }
     
        public string ProductName { get; set; }
     
        public string ProductModel { get; set; }
     
        public string Unit { get; set; }
     
        public int? Quantity { get; set; }
     
        public decimal? PriceWithoutTax { get; set; }
     
        public decimal? Tax { get; set; }
     
        public int? Discount { get; set; }
     
        public decimal? Price { get; set; }
     
        public string Notes { get; set; }
     
        public int? InvoiceType { get; set; }
     
        public string RelatedOrderID { get; set; }
     
        public string PayType { get; set; }
     
        public int? IsSplit { get; set; }
     
        public DateTime OutTime { get; set; }
     
        public int? Status { get; set; }
     
        public string OrginComputerNo { get; set; }
     
        public int? WarehouseNumber { get; set; }
     
        public int? SysNo { get; set; }
     
        public int? StockSysNoB { get; set; }
     
        public int? StItemSysNo { get; set; }
        ///////////////CRL17402////////////////////     
        public decimal? UnitCost { get; set; }

        public int? ProductSysNo { get; set; }

        public string InCompany { get; set; }
        public string OutCompany { get; set; }

        public decimal? AtTotalAmt { get; set; }
    }
}
