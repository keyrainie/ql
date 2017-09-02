using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class RepeatPromotionQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public Int32 ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public int C3SysNo { get; set; }
    }
}
