using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 商品价格折扣规则
    /// </summary>
    public class PSPriceDiscountRule
    {
        /// <summary>
        /// 渠道商品的系统编码
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 折扣方式:商品直减金额,商品最终售价,商品价格折扣百分比
        /// </summary>
        public PSDiscountTypeForProductPrice? DiscountType { get; set; }

        /// <summary>
        /// 价格折扣所设置的数值
        /// </summary>
        public decimal? DiscountValue { get; set; }       

        ///<summary>
        /// 门槛数量，最少要达到多少的数量
        /// </summary>
        public int? MinQty { get; set; }

        /// <summary>
        /// 最大数量，最多可以有多少数量享受折扣
        /// </summary>
        public int? MaxQty { get; set; }
    }
}
