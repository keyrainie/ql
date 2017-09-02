using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单Log信息
    /// </summary>
    public class PurchaseOrderLogInfo
    {

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int VendorSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 采购单系统编号
        /// </summary>
        public int POSysNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int BatchNumber { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int WarehouseNumber { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int ReceivedQuantity { get; set; }

        /// <summary>
        /// 总入库数量
        /// </summary>
        public int TotalReceQty { get; set; }

        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// 入库用户
        /// </summary>
        public string ReceivedUser { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 本次入库金额
        /// </summary>
        public decimal TotalAmt { get; set; }

        /// <summary>
        /// 本次使用EIMS金额
        /// </summary>
        public decimal EIMSAmt { get; set; }

        /// <summary>
        /// 本次入库金额
        /// </summary>
        public decimal SumTotalAmt { get; set; }

        /// <summary>
        /// 本次使用EIMS金额
        /// </summary>
        public decimal SumEIMSAmt { get; set; }
    }
}
