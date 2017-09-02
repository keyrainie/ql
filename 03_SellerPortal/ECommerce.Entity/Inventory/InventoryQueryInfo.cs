using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Inventory
{
    public class InventoryQueryInfo
    {
        /// <summary>
        /// 商品SysNo
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 仓库SysNo
        /// </summary>
        public int StockSysNo { get; set; }
        /// <summary>
        /// 仓库名字
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 是否代销
        /// </summary>
        public bool IsConsign { get; set; }
        /// <summary>
        /// 商品名字
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 财务库存
        /// </summary>
        public int AccountQty { get; set; }
        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty { get; set; }
        /// <summary>
        /// 已分配库存
        /// </summary>
        public int AllocatedQty { get; set; }
        /// <summary>
        /// 单据数量
        /// </summary>
        public int OrderQty { get; set; }
        /// <summary>
        /// 虚拟库存
        /// </summary>
        public int VirtualQty { get; set; }
        /// <summary>
        /// 代销库存
        /// </summary>
        public int ConsignQty { get; set; }
        /// <summary>
        /// 采购在途库存
        /// </summary>
        public int PurchaseQty { get; set; }
        /// <summary>
        /// 移入在途数量
        /// </summary>
        public int ShiftInQty { get; set; }
        /// <summary>
        /// 移出在途数量
        /// </summary>
        public int ShiftOutQty { get; set; }
        /// <summary>
        /// 移仓在途库存
        /// </summary>
        public int ShiftQty { get; set; }
        public int CompanyCode { get; set; }

        /// <summary>
        /// 临时保留库存
        /// </summary>
        public int RetainQty { get; set; }
    }
}
