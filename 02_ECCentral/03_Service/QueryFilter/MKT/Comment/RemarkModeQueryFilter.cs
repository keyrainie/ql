using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class RemarkModeQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        //public int? ProductCategory { get; set; }

        public int? Category1SysNo { get; set; }

        public int? Category2SysNo { get; set; }

        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 三级类别名称
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 所有三级类别
        /// </summary>
        public ECCentral.BizEntity.MKT.RemarksType RemarkType { get; set; }

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
