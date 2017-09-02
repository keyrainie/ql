using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.IM
{
    public class ProductLineFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? C1SysNo { get; set; }

        public int? C2SysNo { get; set; }

        public int? BrandSysNo { get; set; }

        public int PMUserSysNo { get; set; }

        /// <summary>
        /// 是否查询空二级类
        /// </summary>
        public bool? IsSearchEmptyCategory { get; set; }

        public string CompanyCode { get; set; }

        public PMRangeType? PMRangeType { get; set; }
    }
}
