using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.MKT
{
    public class GroupBuyingTicketQueryFilter
    {
        public GroupBuyingTicketQueryFilter()
        {
            PagingInfo = new PagingInfo { PageIndex = 0, PageSize = 10 };
        }

        public string TicketID { get; set; }
        public string UsedStoreName { get; set; }
        public string GroupBuyingTitle { get; set; }        
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }        
        public DateTime? UsedDateFrom { get; set; }
        public DateTime? UsedDateTo { get; set; }
        public GroupBuyingTicketStatus? Status { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }
    }
}
