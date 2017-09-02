using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 优惠券提交审核消息体
    /// </summary>
    [Serializable]
    public class CouponSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Coupon_Submited";
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
