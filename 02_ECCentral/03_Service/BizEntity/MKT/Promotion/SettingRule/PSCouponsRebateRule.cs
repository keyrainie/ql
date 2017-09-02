using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动赠送的礼券规则
    /// </summary>
    public class PSCouponsRebateRule
    {
        /// <summary>
        /// 赠送的礼券金额
        /// </summary>
        public decimal? CouponRebate { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        public string CouponCode { get; set; }

    }
}
