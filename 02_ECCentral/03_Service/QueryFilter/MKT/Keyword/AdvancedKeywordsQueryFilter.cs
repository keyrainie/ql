using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;


namespace ECCentral.QueryFilter.MKT
{
    public class AdvancedKeywordsQueryFilter 
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADStatus? Status { get; set; }


        public string Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
    }
}
