using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class ConsignAdjustQueryFilter
    {
        public ConsignAdjustQueryFilter()
        {
            PageInfo = new PagingInfo { PageIndex = 0, PageSize = 25 };
        }   

        public PagingInfo PageInfo { get; set; }

        public int? VendorSysNo { get; set; }

        public int? PMSysNo { get; set; }

        public ConsignAdjustStatus? Status { get; set; }

        public DateTime? SettleRange { get; set; }

        //public int? SettleSysno { get; set; }
    }
}
