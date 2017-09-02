using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 优惠券作废消息体
    /// </summary>
    [Serializable]
    public class CouponVoidMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Coupon_Voided";
            }
        }

        /// <summary>
        /// 优惠券系统编号
        /// </summary>
        public int CouponSysNo { get; set; }

        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
