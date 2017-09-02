using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT.Promotion
{
    public class CouponCodeRedeemLogFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? CouponBeginNo { get; set; }

        public int? CouponEndNo { get; set; }

        public string CouponName { get; set; }

        public string CouponCode { get; set; }

        public string CustomerID { get; set; }

        public CouponCodeUsedStatus? CouponCodeStatus { get; set; }

        public string SOID { get; set; }

        public DateTime? BeginUseDate { get; set; }

        public DateTime? EndUseDate { get; set; }

    }
}
