using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.BizEntity.MKT.Promotion.Calculate
{
    public class CondProduct
    {
        /// <summary>
        /// 全网商品设置
        /// </summary>
        public ConditionBase<bool?> Whole { get; set; }

        /// <summary>
        /// 商品分类范围设置信息
        /// </summary>
        public ConditionBase<List<int>> CondC3 { get; set; }

        /// <summary>
        /// 品牌范围设置信息
        /// </summary>
        public ConditionBase<List<int>> CondBrand { get; set; }

        /// <summary>
        /// 品牌、分类综合条件
        /// </summary>
        public ConditionBase<List<int[]>> CondBrandC3 { get; set; }

        /// <summary>
        /// 商品范围设置信息
        /// </summary>
        public ConditionBase<List<int>> CondItem { get; set; }

        /// <summary>
        /// 限定商家
        /// </summary>
        public List<RelVendor> CondVendor { get; set; }
    }

    
}
