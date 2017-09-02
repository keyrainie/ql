using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderReceivedInfo
    {
        /// <summary>
        /// 批次号
        /// </summary>
        public int BatchNumber { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 采购单系统编号
        /// </summary>
        public string POSysNo { get; set; }

        /// <summary>
        /// 本次入库数量
        /// </summary>
        public int ReceivedQuantity { get; set; }

        /// <summary>
        /// 采购数量  
        /// </summary>
        public int PurchaseQty { get; set; }

        /// <summary>
        /// 剩余待入库数量
        /// </summary>
        public int WaitInQty { get; set; }

        /// <summary>
        /// 总入库数量
        /// </summary>
        public int TotalReceQty { get; set; }

        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime ReceivedDate { get; set; }
    }
}
