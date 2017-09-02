using ECommerce.Entity.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class CouponContentInfo
    {
        public CouponContentInfo()
        {
            customerCouponCodeList = new List<CustomerCouponInfo>();
            couponList = new List<CouponInfo>();
        }
        public int UserSysNo { get; set; }
        public int MerchantSysNo { get; set; }
        public List<CustomerCouponInfo> customerCouponCodeList { get; set; }
        public List<CouponInfo> couponList { get; set; }
    }
}
