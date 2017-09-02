using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class HelpCenterQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 标识类型，比如New,Hot
        /// </summary>
        public FeatureType? Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 所属类型系统编号
        /// </summary>
        public int? CategorySysNo { get; set; }
    }
}
