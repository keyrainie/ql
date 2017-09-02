using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT.Promotion
{
    /// <summary>
    /// 仓库和库存设置
    /// </summary>
    public class StockQtySetting
    {
        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? StockSysNo { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int? Qty { get; set; }
    }
}
