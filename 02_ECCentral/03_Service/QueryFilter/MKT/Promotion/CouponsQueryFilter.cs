using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class CouponsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public string ChannelID { get; set; }

        public int? CouponsSysNoFrom { get; set; }

        public int? CouponsSysNoTo { get; set; }

        public string CouponsName { get; set; }

        public string CouponCode { get; set; }

        public CouponsStatus? Status { get; set; }

        public string CreateUser { get; set; }

        public string AuditUser { get; set; }


        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? MerchantSysNo { get; set; }
    }
}
