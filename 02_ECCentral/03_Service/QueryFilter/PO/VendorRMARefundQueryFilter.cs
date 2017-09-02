using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class VendorRMARefundQueryFilter
    {
        public VendorRMARefundQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public string VendorRefundSysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public string RMARegisterSysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public VendorRefundStatus? Status { get; set; }

        public VendorRefundPayType? PayType { get; set; }

        public int? PMSysNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
