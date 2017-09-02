using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateVendorSettle.Entities
{
    public class VendorSettleEntity
    {
        [DataMapping("PMSysno", DbType.Int32)]
        public int? ReturnPointPM { get; set; }

        [DataMapping("ReturnPointC3SysNo", DbType.Int32)]
        public int? ReturnPointC3SysNo { get; set; }

        [DataMapping("PM_ReturnPointSysNo", DbType.Int32)]
        public int? PM_ReturnPointSysNo { get; set; }

        [DataMapping("UsingReturnPoint", DbType.Decimal)]
        public decimal UsingReturnPoint { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("SettleID", DbType.String)]
        public string SettleID { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int? VendorSysNo { get; set; }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int? StockSysNo { get; set; }

        [DataMapping("StockName", DbType.String)]
        public string StockName { get; set; }

        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal TotalAmt { get; set; }

        [DataMapping("Balance", DbType.Decimal)]
        public decimal Balance { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int? CurrencySysNo { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int? CreateUserSysNo { get; set; }

        [DataMapping("CreateUser", DbType.String)]
        public string CreateUser { get; set; }

        [DataMapping("AuditTime", DbType.DateTime)]
        public DateTime? AuditTime { get; set; }

        [DataMapping("AuditUserSysNo", DbType.Int32)]
        public int? AuditUserSysNo { get; set; }

        [DataMapping("AuditUser", DbType.String)]
        public string AuditUser { get; set; }

        [DataMapping("SettleTime", DbType.DateTime)]
        public DateTime? SettleTime { get; set; }

        [DataMapping("SettleUserSysNo", DbType.Int32)]
        public int? SettleUserSysNo { get; set; }

        [DataMapping("SettleUser", DbType.String)]
        public string SettleUser { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public SettleStatus? Status { get; set; }

        [DataMapping("SettleBalanceSysNo", DbType.Int32)]
        public int? SettleBalanceSysNo { get; set; }

        [DataMapping("TaxRate", DbType.Decimal)]
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// 是否代销
        /// </summary>
        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        /// <summary>
        /// 账期类型
        /// </summary>
        [DataMapping("PayPeriodType", DbType.Int32)]
        public int PayPeriodType { get; set; }

        /// <summary>
        /// 产出返点金额
        /// </summary>
        [DataMapping("ExpectGetPointAmt", DbType.Decimal)]
        public decimal? ExpectGetPointAmt { get; set; }

        [DataMapping("EIMSNo", DbType.Int32)]
        public int? EIMSNo { get; set; }

        public List<SettleItemEntity> SettleItems { get; set; }
    }
}
