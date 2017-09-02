using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 前台订单产生的优惠卷信息
    /// </summary>
    public class PromotionCode_Customer_Log
    {
        /// <summary>
        /// 用户订单编号
        /// </summary>
        public int? UsedOrderSysNo { get; set; }
        /// <summary>
        /// 优惠券系统编号
        /// </summary>
        public int? CouponCodeSysNo { get; set; }
        /// <summary>
        /// 优惠券编号
        /// </summary>
        public string PromotionCode { get; set; }
        /// <summary>
        /// 优惠券额度类型（折扣or金额）
        /// </summary>
        public int? PromotionAmountType { get; set; }
        /// <summary>
        /// 优惠券系统编号
        /// </summary>
        public int? PromotionSysNo { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo { get; set; }


    }
}
