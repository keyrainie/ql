using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 广告商查询
    /// </summary>
    public class AdvertiserQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 监测代码
        /// </summary>
        public string MonitorCode { get; set; }

        /// <summary>
        /// Cookie有效期【天】
        /// </summary>
        public int? CookiePeriod { get; set; }

        /// <summary>
        /// 广告商名称
        /// </summary>
        public string AdvertiserName { get; set; }

        /// <summary>
        /// 广告商状态
        /// </summary>
        public ADStatus? Status { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
