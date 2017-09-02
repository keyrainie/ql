using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT.Promotion
{
    public class CouponCodeCustomerLogFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? CouponBeginNo { get; set; }

        public int? CouponEndNo { get; set; }

        public string CouponName { get; set; }

        public string CouponCode { get; set; }

        public int? CustomerSysNo { get; set; }

        public string CustomerID { get; set; }

        public DateTime? BeginUseDate { get; set; }

        public DateTime? EndUseDate { get; set; }

        public CouponCodeUsedStatus? CouponStatus { get; set; }
    }
}
