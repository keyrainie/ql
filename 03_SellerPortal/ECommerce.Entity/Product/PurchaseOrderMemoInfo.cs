using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderMemoInfo
    {
        /// <summary>
        /// 采购单备忘录
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        ///  入库备注
        /// </summary>
        public string InStockMemo { get; set; }

        /// <summary>
        /// PM申请理由
        /// </summary>
        public string PMRequestMemo { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        public string RefuseMemo { get; set; }
    }
}
