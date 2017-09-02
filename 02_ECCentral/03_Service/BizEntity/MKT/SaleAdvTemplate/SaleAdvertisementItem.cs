using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.MKT
{
    public class SaleAdvertisementItem
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        public int? SaleAdvSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 组内优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 分组优先级
        /// </summary>
        public int? GroupPriority { get; set; }

        /// <summary>
        /// 商品介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 推荐方式
        /// </summary>
        public RecommendType? RecommendType { get; set; }

        public string IconAddr { get; set; }

        /// <summary>
        /// 分组类名
        /// </summary>
        public string GroupName { get; set; }

        public int? GroupSysNo { get; set; }

        /// <summary>
        /// 在线数量
        /// </summary>
        public int? OnlineQty { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 京东价
        /// </summary>
        public decimal? JDPrice { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }
    }
}
