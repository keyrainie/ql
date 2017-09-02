using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 子类别
    /// </summary>
    public class ECCategoryChildView
    {
        /// <summary>
        /// 子类别集合
        /// </summary>
        public List<ECCategory> ChildCategoryList { get; set; }

        /// <summary>
        /// 当前子类别编号集合
        /// </summary>
        public List<int> CurrentChildSysNoList { get; set; }
    }
}
