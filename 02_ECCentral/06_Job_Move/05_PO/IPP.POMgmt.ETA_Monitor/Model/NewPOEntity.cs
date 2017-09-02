using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.POmgmt.ETA.Model
{
    public class NewPOEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("POID", DbType.String)]
        public string POID { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int? VendorSysNo { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int? StockSysNo { get; set; }

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int? ShipTypeSysNo { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int? PayTypeSysNo { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int? CurrencySysNo { get; set; }

        [DataMapping("ExchangeRate", DbType.Decimal)]
        public decimal? ExchangeRate { get; set; }

        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal? TotalAmt { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int? CreateUserSysNo { get; set; }

        [DataMapping("AuditTime", DbType.DateTime)]
        public DateTime? AuditTime { get; set; }

        [DataMapping("AuditUserSysNo", DbType.Int32)]
        public int? AuditUserSysNo { get; set; }

        [DataMapping("InTime", DbType.DateTime)]
        public DateTime? InTime { get; set; }

        [DataMapping("InUserSysNo", DbType.Int32)]
        public int? InUserSysNo { get; set; }

        [DataMapping("IsApportion", DbType.Int32)]
        public int? IsApportion { get; set; }

        [DataMapping("ApportionTime", DbType.DateTime)]
        public DateTime? ApportionTime { get; set; }

        [DataMapping("ApportionUserSysNo", DbType.Int32)]
        public int? ApportionUserSysNo { get; set; }

        [DataMapping("ETP", DbType.DateTime)]
        public DateTime? ETP { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }


        [DataMapping("InStockMemo", DbType.String)]
        public string InStockMemo { get; set; }

        [DataMapping("CarriageCost", DbType.Decimal)]
        public decimal? CarriageCost { get; set; }

        [DataMapping("PM_ReturnPointSysNo", DbType.Int32)]
        public int? PM_ReturnPointSysNo { get; set; }

        [DataMapping("UsingReturnPoint", DbType.Decimal)]
        public decimal? UsingReturnPoint { get; set; }

        [DataMapping("ReturnPointC3SysNo", DbType.Int32)]
        public int? ReturnPointC3SysNo { get; set; }

        [DataMapping("TaxRate", DbType.Decimal)]
        public decimal? TaxRate { get; set; }

        [DataMapping("PurchaseStockSysno", DbType.Int32)]
        public int? PurchaseStockSysno { get; set; }

        [DataMapping("PMRequestMemo", DbType.String)]
        public string PMRequestMemo { get; set; }

        [DataMapping("TLRequestMemo", DbType.String)]
        public string TLRequestMemo { get; set; }

        [DataMapping("PMSysNo", DbType.Int32)]
        public int? PMSysNo { get; set; }

        [DataMapping("SettlementCompany", DbType.Int32)]
        public int? SettlementCompany { get; set; }

        [DataMapping("WHReceiptSN", DbType.String)]
        public string WHReceiptSN { get; set; }

        [DataMapping("ExecptStatus", DbType.String)]
        public string ExecptStatus { get; set; }

        [DataMapping("ComfirmUserSysNo", DbType.Int32)]
        public int? ComfirmUserSysNo { get; set; }

        [DataMapping("ComfirmTime", DbType.DateTime)]
        public DateTime? ComfirmTime { get; set; }

        public List<int> Privilege { get; set; }

        [DataMapping("ETATime", DbType.DateTime)]
        public DateTime? ETATime { get; set; }
        [DataMapping("ETAHalfDay", DbType.String)]
        public string ETAHalfDay { get; set; }
        [DataMapping("AbandonTime", DbType.String)]
        public DateTime? AbandonTime { get; set; }
        /// <summary>
        /// 存放一些附加信息
        /// </summary>
        public string AppendMessage { get; set; }

        [DataMapping("Rank", DbType.String)]
        public string Rank { get; set; }

        [DataMapping("RefuseMemo", DbType.String)]
        public string RefuseMemo { get; set; }

        [DataMapping("TPStatus", DbType.String)]
        public string TPStatus { get; set; }
        /// <summary>
        /// crl17688新增 prince
        /// 2010.11.18
        /// </summary>
        [DataMapping("CheckResult", DbType.String)]
        public string CheckResult { get; set; }
    }
}
