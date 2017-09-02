using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class ERPSPKCInfo
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// ERP商品ID
        /// </summary>
        public int? ERPSPID { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int? KCSL { get; set; }
        /// <summary>
        /// 网上销售数量
        /// </summary>
        public int? WSXSSL { get; set; }
    }
}
