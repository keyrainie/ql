using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class KeyWordsForProductQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        public ADStatus? Status { get; set; }

        /// <summary>
        /// ProductID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        public string CompanyCode { get; set; }
    }
}
