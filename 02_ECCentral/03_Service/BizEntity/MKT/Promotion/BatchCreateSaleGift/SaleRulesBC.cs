using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 销售规则在品牌和类别上的限制
    /// </summary>
    public class SaleRulesBC
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 活动系统编号
        /// </summary>
        public int PromotionSysNo { get; set; }
        /// <summary>
        /// 1级类别系统编号
        /// </summary>
        public int? Category1SysNo { get; set; }
        /// <summary>
        /// 一级列表名称
        /// </summary>
        public string Category1Name { get; set; }
        /// <summary>
        /// 2级类别系统编号
        /// </summary>
        public int? Category2SysNo { get; set; }
        /// <summary>
        /// 二级类别名称
        /// </summary>
        public string Category2Name { get; set; }
        /// <summary>
        /// 3级类别系统编号
        /// </summary>
        public int? Category3SysNo { get; set; }
        /// <summary>
        /// 3级类别名称
        /// </summary>
        public string Category3Name { get; set; }
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandSysNo { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 组合类型
        /// </summary>
        public string ComboType { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 是否全局
        /// </summary>
        public string IsGlobal { get; set; }
        /// <summary>
        /// 是否复制创建
        /// </summary>
        public int IsCopy { get; set; }
    }
}
