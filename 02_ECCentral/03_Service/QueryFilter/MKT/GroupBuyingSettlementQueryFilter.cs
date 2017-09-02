using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.MKT
{
    public class GroupBuyingSettlementQueryFilter
    {
        public GroupBuyingSettlementQueryFilter()
        {
            PagingInfo = new PagingInfo { PageIndex = 0, PageSize = 10 };
        }

        public int? SysNo { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public DateTime? SettleDateDateFrom { get; set; }
        public DateTime? SettleDateDateTo { get; set; }
        public SettlementBillStatus? Status { get; set; }       
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }

        public PayItemStatus? PayItemStatus { get; set; }
    }
}
