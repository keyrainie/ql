using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    /// <summary>
    /// 优惠券发放历史查询
    /// </summary>
    public class CouponCustomerLogQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 领取开始时间
        /// </summary>
        public DateTime? QueryTimeFrom { get; set; }

        /// <summary>
        /// 领取结束时间
        /// </summary>
        public DateTime? QueryTimeTo { get; set; }

        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 最小优惠券编号
        /// </summary>
        public string CouponCodeFrom { get; set; }

        /// <summary>
        /// 最大优惠券编号 
        /// </summary>
        public string CouponCodeTo { get; set; }
    }
}
