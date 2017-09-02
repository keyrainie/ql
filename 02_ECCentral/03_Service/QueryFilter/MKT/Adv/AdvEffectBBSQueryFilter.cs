using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// BBS推广报表
    /// </summary>
    public class AdvEffectBBSQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDateTo { get; set; }

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
