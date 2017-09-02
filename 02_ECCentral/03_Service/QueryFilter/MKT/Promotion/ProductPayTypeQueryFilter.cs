using System;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT.Promotion
{
    public class ProductPayTypeQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public int? PayTypeSysNo { get; set; }

        /// <summary>
        /// 开始时间从
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 开始时间到
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        /// <summary>
        /// 结束时间从
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// 结束时间到
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }
}
