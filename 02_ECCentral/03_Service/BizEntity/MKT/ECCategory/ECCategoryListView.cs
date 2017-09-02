using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 类别集合
    /// </summary>
    public class ECCategoryListReulst
    {
        /// <summary>
        /// 一级类别集合
        /// </summary>
        public List<ECCategory> Category1List { get; set; }

        /// <summary>
        /// 二级类别集合
        /// </summary>
        public List<ECCategory> Category2List { get; set; }

        /// <summary>
        /// 三级类别集合
        /// </summary>
        public List<ECCategory> Category3List { get; set; }
    }
}
