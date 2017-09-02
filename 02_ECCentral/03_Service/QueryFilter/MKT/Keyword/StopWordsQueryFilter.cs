using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class StopWordsQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 公司信息
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADTStatus? Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
