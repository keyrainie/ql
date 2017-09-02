using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class DefaultKeywordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? PageType { get; set; }

        public int? PageID { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        public ADStatus? Status { get; set; }

        public string CompanyCode { get; set; }

        public DateTime? BeginDateFrom { get; set; }

        public DateTime? BeginDateTo { get; set; }

        public DateTime? EndDateFrom { get; set; }

        public DateTime? EndDateTo { get; set; }
    }
}
