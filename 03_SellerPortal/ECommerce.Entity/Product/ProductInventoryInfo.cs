using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class ProductInventoryInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 财务库存
        /// </summary>
        public int AccountQty { get; set; }

        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty { get; set; }

        /// <summary>
        /// 占用库存
        /// </summary>
        public int AllocatedQty { get; set; }

        /// <summary>
        /// 被订购数量
        /// </summary>
        public int OrderQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        public int ConsignQty { get; set; }

       /// <summary>
        /// 虚库数量
       /// </summary>
        public int VirtualQty { get; set; }

        /// <summary>
        /// 线上库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 待入库数量
        /// </summary>
        /// </summary>
        public int PurchaseQty { get; set; }

        /// <summary>
        /// 移仓在途数量
        /// </summary>
        public int ShiftQty { get; set; }

        /// <summary>
        /// 未激活或已失效库存
        /// </summary>
        public int UnActivatyCount { get; set; }
    }
}
