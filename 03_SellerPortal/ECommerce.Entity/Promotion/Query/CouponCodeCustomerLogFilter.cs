using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    public class CouponCodeCustomerLogFilter : QueryFilter
    {

        public int? CouponSysNo { get; set; }

        public string CouponName { get; set; }

        public string CouponCode { get; set; }

        public int? CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        public DateTime? BeginUseDate { get; set; }

        public DateTime? EndUseDate { get; set; }

        public CouponCodeUsedStatus? CouponStatus { get; set; }

        public int? MerchantSysNo { get; set; }
    }
}
