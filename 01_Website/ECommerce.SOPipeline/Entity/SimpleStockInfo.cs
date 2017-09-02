using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 简单的仓库实体对象
    /// </summary>
    public class SimpleStockInfo
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string StockID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 仓库类型 （0：直邮 1：自贸 2：其它）
        /// </summary>
        public int StockType { get; set; }
        /// <summary>
        /// 仓库所属商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }
    }
}
