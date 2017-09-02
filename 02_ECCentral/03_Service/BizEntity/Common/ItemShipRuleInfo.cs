using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 商品配送规则信息
    /// </summary>
    public class ItemShipRuleInfo
    {
        /// <summary>
        /// 商户 0普通商户 1阿斯利康商户
        /// </summary>
        public int? CompanyCustomer { get; set; }

        /// <summary>
        /// 类型 必选 E|禁运 D
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int? ShipTypeSysNo { get; set; }

        /// <summary>
        /// 收货区域编号
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 状态 A 有效 | D 无效
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 商品范围 P商品 | C类别
        /// </summary>
        public string ItemRange { get; set; }
    }
}
