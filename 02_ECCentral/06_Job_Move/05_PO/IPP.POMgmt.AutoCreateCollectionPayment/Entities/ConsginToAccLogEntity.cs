using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateCollectionPayment.Entities
{
    internal sealed class ConsginToAccLogEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public ConsignToAccStatus Status { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("CreateCost", DbType.Decimal)]
        public decimal CreateCost { get; set; }

        [DataMapping("FoldCost", DbType.Decimal)]
        public decimal FoldCost { get; set; }

        [DataMapping("SettleCost", DbType.Decimal)]
        public decimal SettleCost { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("ConsignToAccType", DbType.Int32)]
        public int ConsignToAccType { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }

        [DataMapping("SettleType", DbType.String)]
        public string SettleType { get; set; }

        [DataMapping("RetailPrice", DbType.Decimal)]
        public decimal RetailPrice { get; set; }

        [DataMapping("SettlePercentage", DbType.Decimal)]
        public decimal? SettlePercentage { get; set; }

        [DataMapping("MinCommission", DbType.Decimal)]
        public decimal MinCommission { get; set; }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("PayPeriodType", DbType.Int32)]
        public int PayPeriodType { get; set; }

        [DataMapping("StockName", DbType.String)]
        public string StockName { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductType", DbType.Int32)]
        public int ProductType { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("TaxRate", DbType.Decimal)]
        public decimal TaxRate { get; set; }

        [DataMapping("LastPrice", DbType.Decimal)]
        public decimal? LastPrice { get; set; }
    }
}
