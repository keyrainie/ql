using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 用户优惠券查询条件
    /// </summary>
    [Serializable]
    public class CustomerCouponCodeQueryInfo : QueryFilterBase
    {
        public CustomerCouponCodeQueryInfo()
        {
            this.PageInfo = new PageInfo();
        }

        /// <summary>
        /// 用户编号，必须传值
        /// </summary>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// 用户等级，必须传值
        /// </summary>
        public int CustomerRank { get; set; }
        /// <summary>
        /// 状态，可为空
        /// </summary>
        public string Status { get; set; }
    }
}
