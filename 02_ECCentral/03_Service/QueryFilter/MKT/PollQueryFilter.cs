using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class PollQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        public string PollName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 用户自定义
        /// </summary>
        public YNStatus? UserDefined { get; set; }

        public int? PageType { get; set; }

        public int? PageID { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
