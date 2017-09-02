using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 类别父对象
    /// </summary>
    public class ECCategoryParentView
    {
        /// <summary>
        /// 父类别集合对象
        /// </summary>
        public List<ECCategory> ParentCategoryList { get; set; }

        /// <summary>
        /// 父类别编号集合
        /// </summary>
        public List<int> CurrentParentSysNoList { get; set; }
    }
}
