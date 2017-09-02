using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 团购中的商品信息，用于更新团购订单 业务
    /// </summary>
    [Serializable]
    public class ProductGroupBuyInfo
    {
        public int SysNo { get; set; }

        public int ProductSysNo { get; set; }

        public int MaxPerOrder { get; set; }
    }
}
