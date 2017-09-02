using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class CouponCodeQueryFilter : QueryFilter
    {
        public int? CouponSysNo { get; set; }

        /// <summary>
        /// 优惠券Code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateTo { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        public int? MerchantSysNo { get; set; }
    }
}
