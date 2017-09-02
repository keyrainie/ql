using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class HotKeywordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        public int? EditUserSysNo { get; set; }

        public int? Priority { get; set; }

        public int? PageType { get; set; }

        public int? PageID { get; set; }

        /// <summary>
        /// 编辑时间开始于
        /// </summary>
        public DateTime? EditDateFrom { get; set; }

        public DateTime? EditDateTo { get; set; }

        /// <summary>
        /// 屏蔽时间开始
        /// </summary>
        public DateTime? InvalidDateFrom { get; set; }

        public DateTime? InvalidDateTo { get; set; }


        /// <summary>
        /// 状态  对应 IsOnlineShow
        /// </summary>
        public NYNStatus? IsOnlineShow { get; set; }

    }
}
