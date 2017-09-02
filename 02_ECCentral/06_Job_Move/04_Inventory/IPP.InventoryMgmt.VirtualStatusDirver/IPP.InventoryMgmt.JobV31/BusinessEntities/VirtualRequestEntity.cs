using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    [Serializable]
    public class VirtualRequestEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }
        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }
        [DataMapping("VirtualQty", DbType.Int32)]
        public int VirtualQty { get; set; }
        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime CreateTime { get; set; }
        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNo { get; set; }
        [DataMapping("PMRequestNote", DbType.String)]
        public string PMRequestNote { get; set; }
        [DataMapping("AuditNote", DbType.String)]
        public string AuditNote { get; set; }
        [DataMapping("AuditTime", DbType.DateTime)]
        public DateTime? AuditTime { get; set; }
        [DataMapping("AuditUserSysNo", DbType.Int32)]
        public int? AuditUserSysNo { get; set; }
        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }
        [DataMapping("VirtualType", DbType.Int32)]
        public int? VirtualType { get; set; }
        [DataMapping("CompanyCode", DbType.AnsiStringFixedLength)]
        public string CompanyCode { get; set; }
        [DataMapping("LanguageCode", DbType.AnsiStringFixedLength)]
        public string LanguageCode { get; set; }
        [DataMapping("StoreCompanyCode", DbType.AnsiString)]
        public string StoreCompanyCode { get; set; }
        [DataMapping("StartTime", DbType.DateTime)]
        public DateTime? StartTime { get; set; }
        [DataMapping("EndTime", DbType.DateTime)]
        public DateTime? EndTime { get; set; }

    }

    public enum VirtualRequestStatusEntity
    {
        Origin = 0,
        Approved = 1,
        Launch = 2,
        Expire = 3,
        Closing = 4,
        Rejected = -1
    }


}
