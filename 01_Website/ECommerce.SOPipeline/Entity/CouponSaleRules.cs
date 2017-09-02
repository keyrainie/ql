using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline.Entity
{
    /// <summary>
    /// 优惠券规则
    /// </summary>
    public class CouponSaleRules
    {
        /// <summary>
        /// 优惠券编号
        /// </summary>
        public int CouponSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 限制类型：N.排除，Y.指定
        /// </summary>
        public string RelationType { get; set; }
    }
}
