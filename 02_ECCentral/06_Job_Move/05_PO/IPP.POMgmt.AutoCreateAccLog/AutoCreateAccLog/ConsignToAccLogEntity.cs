using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoCreateAccLog
{
    public class ConsignToAccLogEntity
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("StockSysNo", DbType.String)]
        public string StockSysNo { get; set; }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        [DataMapping("CreateCost", DbType.Decimal)]
        public decimal CreateCost { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("FoldCost", DbType.Decimal)]
        public decimal? FoldCost { get; set; }

        [DataMapping("SettleCost", DbType.Decimal)]
        public decimal? SettleCost { get; set; }

        [DataMapping("ConsignToAccType", DbType.Int32)]
        public int ConsignToAccType { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("SettleType", DbType.String)]
        public string SettleType { get; set; }

        [DataMapping("SettlePercentage", DbType.Decimal)]
        public decimal? SettlePercentage { get; set; }

        [DataMapping("RetailPrice", DbType.Decimal)]
        public decimal RetailPrice { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }
    }
}
