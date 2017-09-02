using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class CategoryKeywordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商品一级类编号
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 商品二级类编号
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 商品三级类编号
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
