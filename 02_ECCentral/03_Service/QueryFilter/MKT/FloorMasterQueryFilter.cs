using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class FloorMasterQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public PageCodeType? PageType { get; set; }

        /// <summary>
        /// 页面代码
        /// </summary>
        public string PageCode { get; set; }
    }
}
