using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class CouponQueryFilter : QueryFilter
    {
        public int? SysNo { get; set; }

        public int? MerchantSysNo { get; set; }

        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券Code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 活动状态  初始态、就绪、运行、作废、终止、完成
        /// </summary>
        public CouponStatus? Status { get; set; }

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
    }

    
}
