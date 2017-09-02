using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 包含采购单商品信息的采购单信息
    /// </summary>
    public class PurchaseOrderDetailInfo
    {
        /// <summary>
        /// 采购单系统编号
        /// </summary>
        public int? PONumber { get; set; }

        /// <summary>
        /// 中转仓库系统编号
        /// </summary>
        public int? ITStockSysNo { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int VendorNumber { get; set; }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? WarehouseNumber { get; set; }

        /// <summary>
        /// 采购单类型
        /// </summary>
        public string POType { get; set; }

        /// <summary>
        /// 采购单备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ItemNumber { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? PurchaseQty { get; set; }

        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// 采购商品批次信息
        /// </summary>
        public string BatchInfo { get; set; }
    }
}
