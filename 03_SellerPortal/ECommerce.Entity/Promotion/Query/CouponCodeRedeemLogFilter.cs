using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class CouponCodeRedeemLogFilter : QueryFilter
    {
        public int? CouponSysNo { get; set; }

        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券Code
        /// </summary>
        public string CouponCode { get; set; }

        public string CustomerID { get; set; }

        public string SOID { get; set; }

        public DateTime? BeginUseDate { get; set; }

        public DateTime? EndUseDate { get; set; }

        public CouponCodeUsedStatus? CouponCodeStatus { get; set; }

        public int? MerchantSysNo { get; set; }
    }
}
