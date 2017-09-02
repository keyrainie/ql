using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 商品库存信息实体
    /// </summary>
    public class ExperienceHallInventoryInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int? TotalQty { get; set; }

       /// <summary>
       /// 出库数量
       /// </summary>
        public int? OutStockQty { get; set; }
    }
}
