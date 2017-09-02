using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 类别管理
    /// </summary>
    public class ECCategory : IIdentity, IWebChannel
    {
        /// <summary>
        /// 层级关系系统编号
        /// </summary>
        public int RSysNo { get; set; }

        /// <summary>
        /// R父类别编号
        /// </summary>
        public int? RParentSysNo { get; set; }

        /// <summary>
        /// 父类别编号
        /// </summary>
        public int? ParentSysNo { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父类别集合
        /// </summary>
        public List<ECCategory> ParentList { get; set; }

        /// <summary>
        /// 子类别集合
        /// </summary>
        public List<ECCategory> ChildrenList { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 前台三级分类对应的后台三级分类编号，只有前台三级分类有此属性
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 是否在前台父级分类页面是否显示
        /// </summary>
        public YNStatus IsParentCategoryShow { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 促销状态，比如New,Hot
        /// </summary>
        public FeatureType? PromotionStatus { get; set; }

        /// <summary>
        /// 级别(类别类型),1级，2级，3级
        /// </summary>
        public ECCategoryLevel Level { get; set; }
    }
}
