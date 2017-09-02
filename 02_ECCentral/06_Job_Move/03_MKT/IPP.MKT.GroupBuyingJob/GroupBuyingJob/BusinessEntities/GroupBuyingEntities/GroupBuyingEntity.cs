using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using System.ComponentModel;
using Newegg.Oversea.Framework.Entity;

namespace IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities
{
    public class ProductGroupBuyingEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

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

        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        [DataMapping("IsByGroup", DbType.String)]
        public string IsByGroup { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; }

        [DataMapping("SuccessDate", DbType.DateTime)]
        public DateTime? SuccessDate { get; set; }

        [DataMapping("OriginalPrice", DbType.Decimal)]
        public decimal OriginalPrice { get; set; }

        public decimal BasicPrice { get; set; }

        [DataMapping("DealPrice", DbType.Decimal)]
        public decimal DealPrice { get; set; }

        [DataMapping("IsSettlement", DbType.String)]
        public string IsSettlement { get; set; }

        [DataMapping("GroupBuyingTypeSysNo", DbType.Int32)]
        public int GroupBuyingTypeSysNo { get; set; }

        [DataMapping("LimitOrderCount", DbType.Int32)]
        public int LimitOrderCount { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }

        [DataMapping("CurrentSellCount", DbType.Int32)]
        public int CurrentSellCount { get; set; }

        [DataMapping("Reasons", DbType.String)]
        public string Reasons { get; set; }

        [DataMapping("Priority", DbType.Int32)]
        public int Priority { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime EditDate { get; set; }

        [DataMapping("AuditUser", DbType.String)]
        public string AuditUser { get; set; }

        [DataMapping("AuditDate", DbType.DateTime)]
        public DateTime AuditDate { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("RequestSysNo", DbType.Int32)]
        public int RequestSysNo { get; set; }

        [DataMapping("ProductStatus", DbType.Int32)]
        public int ProductStatus { get; set; }
        

    }

    public class ProductGroupBuying_PriceEntity 
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductGroupBuyingSysNo", DbType.Int32)]
        public int ProductGroupBuyingSysNo { get; set; }

        [DataMapping("SellCount", DbType.Int32)]
        public int SellCount { get; set; }

        [DataMapping("GroupBuyingPrice", DbType.Decimal)]
        public decimal GroupBuyingPrice { get; set; }

        [DataMapping("RequestSysNo", DbType.Int32)]
        public int RequestSysNo { get; set; }

    }

    public class ProductGroupBuying_SnapShotPriceEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductGroupBuyingSysNo", DbType.Int32)]
        public int ProductGroupBuyingSysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("SnapShotPoint", DbType.Int32)]
        public int SnapShotPoint { get; set; }

        [DataMapping("SnapShotMaxPerOrder", DbType.Int32)]
        public int SnapShotMaxPerOrder { get; set; }

        [DataMapping("SnapShotCurrentPrice", DbType.Decimal)]
        public decimal SnapShotCurrentPrice { get; set; }

        [DataMapping("SnapShotCashRebate", DbType.Decimal)]
        public decimal SnapShotCashRebate { get; set; }

        [DataMapping("SnapshotBasicPrice", DbType.Decimal)]
        public decimal SnapshotBasicPrice { get; set; }

    }

    public class ProductPriceInfoEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductID", DbType.Int32)]
        public string ProductID { get; set; }

        [DataMapping("BasicPrice", DbType.String)]
        public decimal BasicPrice { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal CurrentPrice { get; set; }

        [DataMapping("UnitCost", DbType.Decimal)]
        public decimal UnitCost { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; }

        [DataMapping("CashRebate", DbType.Decimal)]
        public decimal CashRebate { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("OriginalPrice", DbType.Decimal)]
        public decimal OriginalPrice { get; set; }

    }

}
