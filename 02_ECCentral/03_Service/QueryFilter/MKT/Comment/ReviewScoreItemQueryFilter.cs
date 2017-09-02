using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class ReviewScoreItemQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 一级类别编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类别编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态    A：有效 D:无效
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
