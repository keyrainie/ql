using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class PO
    {
        [DataMapping("ApportionTime", DbType.DateTime)]
        public DateTime ApportionTime { get; set; }

        [DataMapping("ApportionUserSysNo", DbType.Int32)]
        public int ApportionUserSysNo { get; set; }

        [DataMapping("AuditTime", DbType.DateTime)]
        public DateTime AuditTime { get; set; }

        [DataMapping("AuditUserSysNo", DbType.Int32)]
        public int AuditUserSysNo { get; set; }

        [DataMapping("CarriageCost", DbType.Decimal)]
        public Decimal CarriageCost { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNo { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }

        [DataMapping("ExchangeRate", DbType.Decimal)]
        public Decimal ExchangeRate { get; set; }

        [DataMapping("InStockMemo", DbType.String)]
        public string InStockMemo { get; set; }

        [DataMapping("InTime", DbType.DateTime)]
        public DateTime InTime { get; set; }

        [DataMapping("InUserSysNo", DbType.Int32)]
        public int InUserSysNo { get; set; }

        [DataMapping("IsApportion", DbType.Int32)]
        public int IsApportion { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo { get; set; }

        [DataMapping("PayTypeName", DbType.String)]
        public string PayTypeName { get; set; }

        [DataMapping("PM_ReturnPointSysNo", DbType.Int32)]
        public int PM_ReturnPointSysNo { get; set; }

        [DataMapping("PMRequestMemo", DbType.String)]
        public string PMRequestMemo { get; set; }

        [DataMapping("PMSysNo", DbType.Int32)]
        public int PMSysNo { get; set; }

        [DataMapping("POID", DbType.String)]
        public string POID { get; set; }

        [DataMapping("POType", DbType.Int32)]
        public int POType { get; set; }

        [DataMapping("PurchaseStockSysno", DbType.Int32)]
        public int PurchaseStockSysno { get; set; }

        [DataMapping("ReturnPointC3SysNo", DbType.Int32)]
        public int ReturnPointC3SysNo { get; set; }

        [DataMapping("SettlementCompany", DbType.Int32)]
        public int SettlementCompany { get; set; }

        [DataMapping("SettlementCompanyName", DbType.String)]
        public string SettlementCompanyName { get; set; }

        [DataMapping("ShipTypeSysNo", DbType.Int32)]
        public int ShipTypeSysNo { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("TaxRate", DbType.Decimal)]
        public Decimal TaxRate { get; set; }

        [DataMapping("TLRequestMemo", DbType.String)]
        public string TLRequestMemo { get; set; }

        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal TotalAmt { get; set; }

        [DataMapping("UsingReturnPoint", DbType.Decimal)]
        public decimal UsingReturnPoint { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("VendorStatus", DbType.Int32)]
        public int VendorStatus { get; set; }

        [DataMapping("VendorStatusDes", DbType.String)]
        public string VendorStatusDes { get; set; }

        [DataMapping("CurrencySymbol", DbType.String)]
        public string CurrencySymbol { get; set; }

        [DataMapping("ComfirmTime", DbType.DateTime)]
        public DateTime ComfirmTime { get; set; }

        [DataMapping("ComfirmUserSysNo", DbType.Int32)]
        public int? ComfirmUserSysNo { get; set; }

        [DataMapping("CurrencyName", DbType.String)]
        public string CurrencyName { get; set; }

        [DataMapping("StockName", DbType.String)]
        public string StockName { get; set; }

        [DataMapping("PMName", DbType.String)]
        public string PMName { get; set; }

        [DataMapping("ShipTypeName", DbType.String)]
        public string ShipTypeName { get; set; }

        [DataMapping("LocalCurrencySymbol", DbType.String)]
        public string LocalCurrencySymbol { get; set; } //本地指定的货币类型

        [DataMapping("ARMCount", DbType.Int32)]
        public int ARMCount { get; set; }

        [DataMapping("ReceiveAddress", DbType.String)]
        public string ReceiveAddress { get; set; }

        [DataMapping("ReceiveContact", DbType.String)]
        public string ReceiveContact { get; set; }

        [DataMapping("ReceiveContactPhone", DbType.String)]
        public string ReceiveContactPhone { get; set; }

        [DataMapping("ETATime", DbType.DateTime)]
        public DateTime ETATime { get; set; }

        [DataMapping("CheckRequestMemo", DbType.String)]
        public string CheckRequestMemo { get; set; }

        [DataMapping("ETAHalfDay", DbType.String)]
        public string ETAHalfDay { get; set; }

        [DataMapping("RemnantReturnPoint", DbType.String)]
        public string RemnantReturnPoint { get; set; }

        [DataMapping("ComfirmTimeShow", DbType.String)]
        public string ComfirmTimeShow { get; set; }

        [DataMapping("StatusDes", DbType.String)]
        public string StatusDes { get; set; }

        [DataMapping("ReturnPointC2SysNo", DbType.String)]
        public string ReturnPointC2SysNo { get; set; }

        [DataMapping("ReturnPointC1SysNo", DbType.String)]
        public string ReturnPointC1SysNo { get; set; }

        [DataMapping("POTypeName", DbType.String)]
        public string POTypeName { get; set; }

        [DataMapping("TaxRateDes", DbType.String)]
        public string TaxRateDes { get; set; }

        [DataMapping("IsConsignName", DbType.String)]
        public string IsConsignName { get; set; }

        [DataMapping("ReturnPointName", DbType.String)]
        public string ReturnPointName { get; set; }

        [DataMapping("Category", DbType.Object)]
        public object Category { get; set; }

        [DataMapping("ITStockSysNo", DbType.Int32)]
        public int? ITStockSysNo { get; set; }

        [DataMapping("ShiftSysNo", DbType.Int32)]
        public int? ShiftSysNo { get; set; }
    }
}
