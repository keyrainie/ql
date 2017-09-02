using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// seo查询
    /// </summary>
    public class SEOQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string PageDescription { get; set; }

        /// <summary>
        /// 页面关键字
        /// </summary>
        public string PageKeywords { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ECCentral.BizEntity.MKT.ADStatus? Status { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }
    }
}
