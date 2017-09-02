using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateVendorSettle.Entities
{
    internal sealed class SettleItemEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("SettleSysNo", DbType.Int32)]
        public int? SettleSysNo { get; set; }

        [DataMapping("Cost", DbType.Decimal)]
        public decimal Cost { get; set; }

        [DataMapping("POConsignToAccLogSysNo", DbType.Int32)]
        public int? POConsignToAccLogSysNo { get; set; }

        [DataMapping("CreateCost", DbType.Decimal)]
        public decimal CreateCost { get; set; }

        [DataMapping("FoldCost", DbType.Decimal)]
        public decimal FoldCost { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("Product", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int? ProductSysNo { get; set; }

        [DataMapping("Stock", DbType.String)]
        public string StockName { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int? StockSysNo { get; set; }

        [DataMapping("Vendor", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int? VendorSysNo { get; set; }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime { get; set; }

        [DataMapping("ConsignQty", DbType.Int32)]
        public int ConsignQty { get; set; }

        [DataMapping("OnLineQty", DbType.Int32)]
        public int OnLineQty { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public ConsignToAccStatus? ConsignToAccStatus { get; set; }

        [DataMapping("ConsignToAccType", DbType.Int32)]
        public int ConsignToAccType { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("SettleCost", DbType.Decimal)]
        public decimal SettleCost { get; set; }

        [DataMapping("AcquireReturnPointType", DbType.Int32)]
        public int? AcquireReturnPointType { get; set; }

        [DataMapping("AcquireReturnPoint", DbType.Decimal)]
        public decimal? AcquireReturnPoint { get; set; }

        /// <summary>
        /// 代销结算类型
        /// </summary>
        [DataMapping("SettleType", DbType.String)]
        public string SettleType { get; set; }

        /// <summary>
        /// 佣金百分比
        /// </summary>
        [DataMapping("SettlePercentage", DbType.Decimal)]
        public decimal? SettlePercentage { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [DataMapping("RetailPrice", DbType.Decimal)]
        public decimal RetailPrice { get; set; }

        /// <summary>
        /// 发放积分
        /// </summary>
        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        /// <summary>
        /// 最低佣金限额
        /// </summary>
        [DataMapping("MinCommission", DbType.Decimal)]
        public decimal MinCommission { get; set; }
    }
}
