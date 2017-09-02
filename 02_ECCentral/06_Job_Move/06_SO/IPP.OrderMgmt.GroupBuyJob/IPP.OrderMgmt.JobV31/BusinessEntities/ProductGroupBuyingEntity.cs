using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    [Serializable]
    public class ProductGroupBuyingEntity : EntityBase
    {
       

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("GroupBuyingTitle", DbType.String)]
        public string GroupBuyingTitle { get; set; }

        [DataMapping("GroupBuyingDesc", DbType.String)]
        public string GroupBuyingDesc { get; set; }

        [DataMapping("GroupBuyingPicUrl", DbType.String)]
        public string GroupBuyingPicUrl { get; set; }

        [DataMapping("GroupBuyingSmallPicUrl", DbType.String)]
        public string GroupBuyingSmallPicUrl { get; set; }

        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime BeginDate { get; set; }

        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime EndDate { get; set; }

        [DataMapping("IsByGroup", DbType.AnsiStringFixedLength)]
        public string IsByGroup { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; }

        [DataMapping("SuccessDate", DbType.DateTime)]
        public DateTime? SuccessDate { get; set; }

        [DataMapping("OriginalPrice", DbType.Decimal)]
        public decimal? OriginalPrice { get; set; }

        [DataMapping("DealPrice", DbType.Decimal)]
        public decimal? DealPrice { get; set; }

        [DataMapping("CurrentSellCount", DbType.Int32)]
        public int? CurrentSellCount { get; set; }

        [DataMapping("IsSettlement", DbType.AnsiStringFixedLength)]
        public string IsSettlement { get; set; }

        [DataMapping("GroupBuyingTypeSysNo", DbType.Int32)]
        public int GroupBuyingTypeSysNo { get; set; }

        [DataMapping("LimitOrderCount", DbType.Int32)]
        public int LimitOrderCount { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }

        [DataMapping("Status", DbType.AnsiStringFixedLength)]
        public string Status { get; set; }

        [DataMapping("Reasons", DbType.String)]
        public string Reasons { get; set; }

        [DataMapping("Priority", DbType.Int32)]
        public int Priority { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("AuditDate", DbType.DateTime)]
        public DateTime? AuditDate { get; set; }

        [DataMapping("AuditUser", DbType.String)]
        public string AuditUser { get; set; }

        public int? LowerLimitSellCount { get; set; }

    }
}
