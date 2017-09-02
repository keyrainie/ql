using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 置顶商品
    /// </summary>
    public class TopItemInfo : IWebChannel
    {
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 产品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public int? CategoryType { get; set; }
        /// <summary>
        /// 类别编号 
        /// </summary>
        public int? CategorySysNo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 是否扩展生效
        /// </summary>
        public bool? IsExtend { get; set; }
    }
}
