using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ProductPriceCompareQueryFilter
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

        public DateTime? CreateTimeFrom { get; set; }

        public DateTime? CreateTimeTo { get; set; }

        public int? ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public ProductPriceCompareStatus? Status { get; set; }

        public int? C3SysNo { get; set; }

        public int? C2SysNo { get; set; }

        public int? C1SysNo { get; set; }

        public int? PMSysNo { get; set; }
    }
}
