using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class SegmentQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        public KeywordsStatus? Status { get; set; }

        public DateTime? InDate { get; set; }

        public string InUser { get; set; }

        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

    }
}
