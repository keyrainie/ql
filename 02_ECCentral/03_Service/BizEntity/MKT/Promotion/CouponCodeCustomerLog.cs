using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 优惠卷获取记录
    /// </summary>
    public class CouponCodeCustomerLog
    {        
        /// <summary>
        /// 优惠卷金额
        /// </summary>
        public int CouponAmount { get; set; }
       
        /// <summary>
        /// 优惠卷编码
        /// </summary>
        public string CouponCode { get; set; }
        
        /// <summary>
        /// 优惠卷系统编号
        /// </summary>
        public int CouponCodeSysNo { get; set; }

        /// <summary>
        /// 优惠活动系统编号
        /// </summary>
        public int CouponSysNo { get; set; }

        /// <summary>
        /// 获得优惠卷的订单编号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 使用了此优惠卷的订单编号
        /// </summary>
        public int? UsedOrderSysNo { get; set; }
        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }
    }
}
