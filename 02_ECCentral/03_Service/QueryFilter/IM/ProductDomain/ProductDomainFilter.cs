using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductDomainFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? ProductDomainSysNo { get; set; }

        public string ProductDomainName { get; set; }

        public int? Category1SysNo { get; set; }

        public int? Category2SysNo { get; set; }

        public int? BrandSysNo { get; set; }

        public int? ProductDomainLeaderUserSysNo { get; set; }

        public int? PMSysNo { get; set; }

        /// <summary>
        /// 是否查询空二级类
        /// </summary>
        public bool? IsSearchEmptyCategory { get; set; }

        /// <summary>
        /// 是否集合样式显示
        /// </summary>
        public bool? AsAggregateStyle { get; set; }

        public string CompanyCode { get; set; }
    }
}
