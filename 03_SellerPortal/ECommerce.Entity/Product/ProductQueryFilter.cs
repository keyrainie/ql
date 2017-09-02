using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    public class ProductQueryFilter : QueryFilter
    {
        /// <summary>
        /// 商家编号
        /// </summary>
        public string VendorSysNo { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 状态条件 0-等于 1-不等于
        /// </summary>
        public int StatusCondition { get; set; }
        /// <summary>
        /// 商品类别系统编号（当有多个系统编号时为SysNo1，SysNo2，...，SysNon）
        /// </summary>
        public string CategorySysNo { get; set; }
        /// <summary>
        /// 商品类别编号
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 创建开始时间
        /// </summary>
        public DateTime CreateTimeBegin { get; set; }
        /// <summary>
        /// 创建结束时间
        /// </summary>
        public DateTime CreateTimeEnd { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string ProductTradeType { get; set; }

        /// <summary>
        /// Gets or sets the upc code.
        /// </summary>
        /// <value>
        /// The upc code.
        /// </value>
        public string UPCCode { get; set; }

    }
}
