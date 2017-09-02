using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class ComputerParts
    {
        public int SysNo { get; set; }

        public string ComputerPartName { get; set; }

        public YNStatus IsMust { get; set; }

        public int Priority { get; set; }

        public string Note { get; set; }

        public string ProductFacet { get; set; }

        /// <summary>
        /// 组件商品的可选分类
        /// </summary>
        public List<ComputerPartsCategory> PartsCategories { get; set; }
    }
}
