using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class HotSaleCategoryQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        public string GroupName { get; set; }

        public int? Position { get; set; }

        public ADStatus? Status { get; set; }

        public int? PageType { get; set; }
    }
}
