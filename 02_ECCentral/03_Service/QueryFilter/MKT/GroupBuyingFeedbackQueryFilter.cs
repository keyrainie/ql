using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.MKT
{
    public class GroupBuyingFeedbackQueryFilter
    {
        public GroupBuyingFeedbackQueryFilter()
        {
            PagingInfo = new PagingInfo { PageIndex = 0, PageSize = 10 };
        }

        public int? FeedbackType { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public GroupBuyingFeedbackStatus? Status { get; set; }
        public DateTime? ReadDateFrom { get; set; }
        public DateTime? ReadDateTo { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CompanyCode { get; set; }
    }
}
