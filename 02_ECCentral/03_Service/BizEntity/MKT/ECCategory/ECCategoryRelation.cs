using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 类别关系对象
    /// </summary>
    public class ECCategoryRelation
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 前台分类系统编号
        /// </summary>
        public int ECCategorySysNo { get; set; }

        /// <summary>
        /// 父级系统编号
        /// </summary>
        public int? ParentSysNo { get; set; }
    }
}
